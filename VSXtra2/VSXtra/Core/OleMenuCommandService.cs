// ================================================================================================
// OleMenuCommandService.cs
//
// This source code is created by using the source code provided with the VS 2010 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2010.07.06, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Linq;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Constants = Microsoft.VisualStudio.OLE.Interop.Constants;
using IServiceProvider = System.IServiceProvider;

namespace VSXtra.Core
{

  // ================================================================================================
  /// <summary>
  /// 
  /// </summary>
  // ================================================================================================
  [CLSCompliant(false)]
  [ComVisible(true)]
  public class OleMenuCommandService : MenuCommandService, IOleCommandTarget
  {
    internal static TraceSwitch _MenuService = 
      new TraceSwitch("MENUSERVICE", "MenuCommandService: Track menu command routing");

    private static uint _QueryStatusCount;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new menu command service.
    /// </summary>
    /// <param name="serviceProvider">Service provider object</param>
    // --------------------------------------------------------------------------------------------
    public OleMenuCommandService(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
      Provider = serviceProvider;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new menu command service.
    /// </summary>
    /// <param name="serviceProvider">Service provider object</param>
    /// <param name="parentCommandTarget">Parent command target object</param>
    // --------------------------------------------------------------------------------------------
    public OleMenuCommandService(IServiceProvider serviceProvider, 
      IOleCommandTarget parentCommandTarget)
      : base(serviceProvider)
    {
      if (parentCommandTarget == null)
      {
        throw new ArgumentNullException("parentCommandTarget");
      }
      ParentTarget = parentCommandTarget;
      Provider = serviceProvider;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the parent command target.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IOleCommandTarget ParentTarget { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the ServiceProvider object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IServiceProvider Provider { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Finds the specified command.
    /// </summary>
    /// <param name="guid">Command GUID</param>
    /// <param name="id">Command ID</param>
    /// <param name="hrReturn">Status</param>
    /// <returns>
    /// The MenuCommand object representing the command.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    private MenuCommand FindCommand(Guid guid, int id, ref int hrReturn)
    {
      hrReturn = (int)Constants.OLECMDERR_E_UNKNOWNGROUP;
      MenuCommand result = null;
      // --- First query the IMenuCommandService and ask it to FindCommand
      var menuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
      if (menuCommandService != null)
      {
        result = menuCommandService.FindCommand(new CommandID(guid, id));
      }
      
      // --- If the IMenuCommandService cames back without a command, then ask ourselves
      if (result == null && this != menuCommandService)
      {
        result = FindCommand(guid, id);
      }

      // --- Check if the command has been found
      if (result == null)
      {
        var commands = GetCommandList(guid);
        if (commands != null)
        {
          // --- The default error now must be "Not Supported" because the command group is known
          hrReturn = (int)Constants.OLECMDERR_E_NOTSUPPORTED;
          Debug.WriteLineIf(_MenuService.TraceVerbose, "\t...VSMCS Found group");
          
          // --- Get the list of command inside this group
          foreach (var command in
            from MenuCommand command in commands
            let vsCommand = command as IOleMenuCommand
            where (null != vsCommand) && (vsCommand.DynamicItemMatch(id))
            select command)
          {
            Debug.WriteLineIf(_MenuService.TraceVerbose, "\t...VSMCS Found command2");
            hrReturn = NativeMethods.S_OK;
            result = command;
          }
        }
      }
      else
      {
        Debug.WriteLineIf(_MenuService.TraceVerbose, "\t... VSMCS Found command");
        hrReturn = NativeMethods.S_OK;
      }
      return result;
    }


    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Invokes a command on the local form or in the global environment.
    /// </summary>
    /// <remarks>
    /// The local form is first searched for the given command ID. If it is found, it is invoked.
    /// Otherwise the the command ID is passed to the global environment command handler, 
    /// if one is available.
    /// </remarks>
    /// <param name="commandID">Command ID</param>
    // --------------------------------------------------------------------------------------------
    public override bool GlobalInvoke(CommandID commandID)
    {
      // --- Is it local?
      if (base.GlobalInvoke(commandID)) return true;

      // pass it to the global handler
      var uiShellSvc = GetService(typeof(SVsUIShell)) as IVsUIShell;
      if (uiShellSvc == null) return false;
      Object dummy = null;
      var tmpGuid = commandID.Guid;
      return !NativeMethods.Failed(uiShellSvc.PostExecCommand(ref tmpGuid,
        (uint) commandID.ID, 0, ref dummy));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Invokes a command on the local form or in the global environment.
    /// </summary>
    /// <remarks>
    /// The local form is first searched for the given command ID. If it is found, it is invoked.
    /// Otherwise the the command ID is passed to the global environment command handler, 
    /// if one is available.
    /// </remarks>
    /// <param name="commandID">Command ID</param>
    /// <param name="arg">Command arguments</param>
    // --------------------------------------------------------------------------------------------
    public override bool GlobalInvoke(CommandID commandID, object arg)
    {
      // --- Is it local?
      if (base.GlobalInvoke(commandID, arg)) return true;

      // --- Pass it to the global handler
      var uiShellSvc = GetService(typeof(SVsUIShell)) as IVsUIShell;
      if (uiShellSvc == null) return false;
      var dummy = arg;
      var tmpGuid = commandID.Guid;
      return !NativeMethods.Failed(uiShellSvc.PostExecCommand(ref tmpGuid, 
        (uint)commandID.ID, 0, ref dummy));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This is called by a menu command when it's status has changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void OnCommandsChanged(MenuCommandsChangedEventArgs e)
    {
      base.OnCommandsChanged(e);
      if (_QueryStatusCount != 0) return;
      // --- UpdateCommandUI(0) can not be called inside QueryStatus because this will cause an 
      // --- infinite sequence of calls to QueryStatus during idle time.
      var uiShellSvc = GetService(typeof(SVsUIShell)) as IVsUIShell;
      if (uiShellSvc != null)
      {
        NativeMethods.ThrowOnFailure(uiShellSvc.UpdateCommandUI(0));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Shows the context menu with the given command ID at the given location.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void ShowContextMenu(CommandID menuID, int x, int y)
    {
      var cui = GetService(typeof(NativeMethods.OleComponentUIManager)) as IOleComponentUIManager;
      Debug.Assert(cui != null, "no component UI manager, so we can't display a context menu");
      if (cui == null) return;
      var pt = new[] { new POINTS() };
      pt[0].x = (short)x;
      pt[0].y = (short)y;

      var tmpGuid = menuID.Guid;
      NativeMethods.ThrowOnFailure(cui.ShowContextMenu(0, ref tmpGuid, menuID.ID, pt, this));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the HiWord value of a System.Uint32 value.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static uint HiWord(uint val)
    {
      return ((val >> 16) & 0xFFFF);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the LoWord value of a System.Uint32 value.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static uint LoWord(uint val)
    {
      return (val & 0xFFFF);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes a specified command or displays help for a command.
    /// </summary>
    /// <param name="guidGroup">
    /// Unique identifier of the command group; can be NULL to specify the standard group.
    /// </param>
    /// <param name="nCmdId">
    /// The command to be executed. This command must belong to the group specified with pguidCmdGroup.
    /// </param>
    /// <param name="nCmdExcept">
    /// Values taken from the OLECMDEXECOPT enumeration, which describe how the object should 
    /// execute the command.
    /// </param>
    /// <param name="pIn">
    /// Pointer to a VARIANTARG structure containing input arguments. Can be NULL.
    /// </param>
    /// <param name="vOut">
    /// Pointer to a VARIANTARG structure to receive command output. Can be NULL.
    /// </param>
    /// <returns>
    /// This method supports the standard return values E_FAIL and E_UNEXPECTED, 
    /// as well as the following: 
    /// <para>S_OK: The command was executed successfully.</para>
    /// <para>
    /// OLECMDERR_E_UNKNOWNGROUP: The pguidCmdGroup parameter is not NULL but does not specify 
    /// a recognized command group.
    /// </para>
    /// <para>
    /// OLECMDERR_E_NOTSUPPORTED: The nCmdID parameter is not a valid command in the group 
    /// identified by pguidCmdGroup.
    /// </para>
    /// <para>
    /// OLECMDERR_E_DISABLED: The command identified by nCmdID is currently disabled and 
    /// cannot be executed.
    /// </para>
    /// <para>
    /// OLECMDERR_E_NOHELP: The caller has asked for help on the command identified by 
    /// nCmdID, but no help is available.
    /// </para>
    /// <para>OLECMDERR_E_CANCELED: The user canceled the execution of the command.</para>
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.Exec(ref Guid guidGroup, uint nCmdId, uint nCmdExcept, IntPtr pIn, 
      IntPtr vOut)
    {
      const uint vsCmdOptQueryParameterList = 1;

      // --- According with MSDN the Guid for the command group can be null and in this case the default
      // --- command group should be used. Given the interop definition of IOleCommandTarget, the only way
      // --- to detect a null guid is to try to access it and catch the NullReferenceException.
      Guid commandGroup;
      try
      {
        commandGroup = guidGroup;
      }
      catch (NullReferenceException)
      {
        // --- Here we assume that the only reason for the exception is a null guidGroup.
        // --- We do not handle the default command group as definied in the spec for IOleCommandTarget,
        // --- so we have to return OLECMDERR_E_NOTSUPPORTED.
        return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
      }

      var hr = NativeMethods.S_OK;
      var cmd = FindCommand(commandGroup, (int)nCmdId, ref hr);
      // --- If the command is not supported check if it can be handled by the parent command service
      if ((cmd == null || !cmd.Supported) && ParentTarget != null)
      {
        return ParentTarget.Exec(ref commandGroup, nCmdId, nCmdExcept, pIn, vOut);
      }

      if (cmd != null)
      {
        // --- Try to see if the command is a IOleMenuCommand.
        var vsCmd = cmd as IOleMenuCommand;
        // --- Check the execution flags;
        var loWord = LoWord(nCmdExcept);
        // --- If the command is not an OleMenuCommand, it can not handle the show help option.
        if (((uint)OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP == loWord) && (null == vsCmd))
        {
          return NativeMethods.S_OK;
        }
        object o = null;
        if (pIn != IntPtr.Zero)
        {
          o = Marshal.GetObjectForNativeVariant(pIn);
        }
        if (null == vsCmd)
        {
          cmd.Invoke(o);
        }
        else
        {
          switch (loWord)
          {
            // --- Default execution of the command: call the Invoke method
            case (uint)OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER:
            case (uint)OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER:
            case (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT:
              {
                var invokeEx = vsCmd as IMenuCommandInvokeEx;
                if (null != invokeEx)
                {
                  invokeEx.Invoke(o, vOut, (OLECMDEXECOPT)loWord);
                }
                else
                {
                  vsCmd.Invoke(o, vOut);
                }
              }
              break;

            case (uint)OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP:
              // --- Check the hi word of the flags to see what kind of help is needed. 
              // --- We handle only the request for the parameters list.
              if (vsCmdOptQueryParameterList == HiWord(nCmdExcept) && IntPtr.Zero != vOut)
              {
                // --- In this case vOut is a pointer to a VARIANT that will receive
                // --- the parameters description.
                if (!string.IsNullOrEmpty(vsCmd.ParametersDescription))
                {
                  Marshal.GetNativeVariantForObject(vsCmd.ParametersDescription, vOut);
                }
              }
              break;

            default:
              break;
          }
        }
      }
      return hr;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the object for the status of one or more commands generated by user interface events.
    /// </summary>
    /// <param name="guidGroup">
    /// Unique identifier of the command group; can be NULL to specify the standard group. All the 
    /// commands that are passed in the prgCmds array must belong to the group specified by 
    /// pguidCmdGroup.
    /// </param>
    /// <param name="nCmdId">The number of commands in the prgCmds array.</param>
    /// <param name="oleCmd">
    /// A caller-allocated array of OLECMD structures that indicate the commands for which the 
    /// caller needs status information. This method fills the cmdf member of each structure with 
    /// values taken from the OLECMDF enumeration.
    /// </param>
    /// <param name="oleText">
    /// Pointer to an OLECMDTEXT structure in which to return name and/or status information of a 
    /// single command. Can be NULL to indicate that the caller does not need this information.
    /// </param>
    /// <returns>
    /// This method supports the standard return values E_FAIL and E_UNEXPECTED, 
    /// as well as the following: 
    /// <para>S_OK: The command was executed successfully.</para>
    /// <para>E_POINTER: The prgCmds argument is NULL.</para>
    /// <para>
    /// OLECMDERR_E_UNKNOWNGROUP: The pguidCmdGroup parameter is not NULL but does not specify 
    /// a recognized command group.
    /// </para>
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.QueryStatus(ref Guid guidGroup, uint nCmdId, OLECMD[] oleCmd, IntPtr oleText)
    {
      // --- According with MSDN the Guid for the command group can be null and in this case the default
      // --- command group should be used. Given the interop definition of IOleCommandTarget, the only way
      // --- to detect a null guid is to try to access it and catch the NullReferenceException.
      Guid commandGroup;
      try
      {
        commandGroup = guidGroup;
      }
      catch (NullReferenceException)
      {
        // --- Here we assume that the only reason for the exception is a null guidGroup.
        // --- We do not handle the default command group as definied in the spec for IOleCommandTarget,
        // --- so we have to return OLECMDERR_E_NOTSUPPORTED.
        return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
      }

      _QueryStatusCount++;
      var hr = NativeMethods.S_OK;
      try
      {
        for (uint i = 0; i < oleCmd.Length && NativeMethods.Succeeded(hr); i++)
        {
          var cmd = FindCommand(commandGroup, (int)oleCmd[i].cmdID, ref hr);
          oleCmd[i].cmdf = 0;
          if ((cmd != null) && NativeMethods.Succeeded(hr))
          {
            oleCmd[i].cmdf = (uint)cmd.OleStatus;
          }

          if ((oleCmd[i].cmdf & (int)NativeMethods.tagOLECMDF.OLECMDF_SUPPORTED) != 0)
          {
            // --- Find if the caller needs the text of the command
            if ((IntPtr.Zero != oleText) && 
              (NativeMethods.OLECMDTEXT.GetFlags(oleText) == NativeMethods.OLECMDTEXT.OLECMDTEXTF.OLECMDTEXTF_NAME))
            {
              string textToSet = null;
              if (cmd is DesignerVerb)
              {
                textToSet = ((DesignerVerb)cmd).Text;
              }
              else if (cmd is IOleMenuCommand)
              {
                textToSet = ((IOleMenuCommand)cmd).Text;
              }
              if (textToSet != null)
              {
                NativeMethods.OLECMDTEXT.SetText(oleText, textToSet);
              }
            }
          }
          else if (ParentTarget != null)
          {
            // --- If the command is not supported and this command service has a parent,
            // --- ask the parent about the command.
            OLECMD[] newOleArray = { oleCmd[i] };
            hr = ParentTarget.QueryStatus(ref commandGroup, 1, newOleArray, oleText);
            oleCmd[i] = newOleArray[0];
          }
          // --- If the flags are zero, the shell prefers that we return not supported, or else 
          // --- no one else will get asked
          //
          if (oleCmd[i].cmdf == 0)
          {
            hr = NativeMethods.OLECMDERR_E_NOTSUPPORTED;
          }
        }
      }
      finally
      {
        if (0 < _QueryStatusCount)
          _QueryStatusCount--;
      }
      return hr;
    }
  }
}


