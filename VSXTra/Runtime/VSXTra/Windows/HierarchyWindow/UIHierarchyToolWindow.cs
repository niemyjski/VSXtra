// ================================================================================================
// UIHierarchyToolWindow.cs
//
// Created: 2008.09.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Hierarchy;
using VSXtra.Package;

namespace VSXtra.Windows
{
  // ================================================================================================
  /// <summary>
  /// This class represents a UI hierarchy tool window.
  /// </summary>
  /// <typeparam name="TPackage">Package owning the tool window.</typeparam>
  // ================================================================================================
  public abstract class UIHierarchyToolWindow<TPackage> :
    ClsIdToolWindowPane<TPackage>
    where TPackage: PackageBase
  {
    #region Private Fields

    private readonly List<HierarchyWindowAttribute> _Attributes;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new hierarchy tool window instance and sets up its style according to the 
    /// attributes defined.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected UIHierarchyToolWindow()
    {
      _Attributes = new List<HierarchyWindowAttribute>(GetType().AttributesOfType<HierarchyWindowAttribute>());
    }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the CLSID_VsUIHierarchyWindow class ID.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override Guid ToolWindowClassGuid 
    {
      get { return VSConstants.CLSID_VsUIHierarchyWindow; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the tool window with the specified style and initial hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      __UIHWINFLAGS flags = 0;
      // ReSharper disable AccessToModifiedClosure
      _Attributes.ForEach(attr => flags |= attr.StyleFlag);
      // ReSharper restore AccessToModifiedClosure
      SetUIWindowStyle(ref flags);
      object unkObj;
      var manager = HierarchyManager;
      HierarchyWindow.Init(manager, (uint)flags, out unkObj);
      Site(manager);
    }

    #endregion

    #region Virtual properties and methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the style of the hierarchy window.
    /// </summary>
    /// <param name="style">Style to set</param>
    /// <remarks>
    /// Override this method to set style flags manually.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void SetUIWindowStyle(ref __UIHWINFLAGS style)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to set up the initial hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected abstract HierarchyManager<TPackage> HierarchyManager { get; }

    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the IVsUIHierarchyWindow instance representing the native hierarchy window 
    /// behind this tool window instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsUIHierarchyWindow HierarchyWindow
    {
      get
      {
        var frame = Frame as IVsWindowFrame;
        if (frame != null)
        {
          Object docView;
          int hr = frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out docView);
          if (hr == VSConstants.S_OK)
            return (IVsUIHierarchyWindow)docView;
        }
        return null;
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a new hierachy to the hierarchy window
    /// </summary>
    /// <param name="hierarchy">Hierarchy to add to the window</param>
    /// <param name="dontSelectNew">
    /// Flag indicating if the the new hierarchy should not be selected.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public void AddUIHierarchy(HierarchyManager<TPackage> hierarchy, bool dontSelectNew)
    {
      if (HierarchyWindow != null)
      {
        ErrorHandler.ThrowOnFailure(
          HierarchyWindow.AddUIHierarchy(hierarchy,
                                         dontSelectNew
                                           ? (uint)__VSADDHIEROPTIONS.ADDHIEROPT_DontSelectNewHierarchy
                                           : 0));
        Site(hierarchy);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a new hierachy to the hierarchy window
    /// </summary>
    /// <param name="hierarchy">Hierarchy to add to the window</param>
    // --------------------------------------------------------------------------------------------
    public void AddUIHierarchy(HierarchyManager<TPackage> hierarchy)
    {
      AddUIHierarchy(hierarchy, true);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Expands the specified hierarchy node.
    /// </summary>
    /// <param name="node">Node to expand</param>
    // --------------------------------------------------------------------------------------------
    public void ExpandNode(HierarchyNode node)
    {
      ExpandNode(node, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Expands the specified hierarchy node.
    /// </summary>
    /// <param name="node">Node to expand</param>
    /// <param name="recursive">Set true to expand the node recursively</param>
    // --------------------------------------------------------------------------------------------
    public void ExpandNode(HierarchyNode node, bool recursive)
    {
      HierarchyWindow.ExpandNode(node.ManagerNode, (uint)node.HierarchyId, recursive);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Expands the specified hierarchy node.
    /// </summary>
    /// <param name="node">Node to expand</param>
    // --------------------------------------------------------------------------------------------
    public void CollapseNode(HierarchyNode node)
    {
      HierarchyWindow.CollapseNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Expands the specified hierarchy node.
    /// </summary>
    /// <param name="node">Node to expand</param>
    // --------------------------------------------------------------------------------------------
    public void ExpandParents(HierarchyNode node)
    {
      HierarchyWindow.ExpandParents(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Selects the specified node.
    /// </summary>
    /// <param name="node">Node to select</param>
    // --------------------------------------------------------------------------------------------
    public void SelectNode(HierarchyNode node)
    {
      HierarchyWindow.BoldNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a new node to the current selection.
    /// </summary>
    /// <param name="node">Node to select</param>
    // --------------------------------------------------------------------------------------------
    public void AddSelectNode(HierarchyNode node)
    {
      HierarchyWindow.AddSelectNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Extends the current selection into a range with the specified node.
    /// </summary>
    /// <param name="node">Node to select</param>
    // --------------------------------------------------------------------------------------------
    public void ExtendSelectNode(HierarchyNode node)
    {
      HierarchyWindow.ExtendSelectNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Higlights the specified node with bold font.
    /// </summary>
    /// <param name="node">Node to highlight with bold font</param>
    // --------------------------------------------------------------------------------------------
    public void BoldNode(HierarchyNode node)
    {
      HierarchyWindow.BoldNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the bold higlights from the specified node with.
    /// </summary>
    /// <param name="node">Node to remove the highlight from</param>
    // --------------------------------------------------------------------------------------------
    public void UnBoldNode(HierarchyNode node)
    {
      HierarchyWindow.UnBoldNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets Cut highlighting to the specified node.
    /// </summary>
    /// <param name="node">Node to remove the highlight from</param>
    // --------------------------------------------------------------------------------------------
    public void CutHighlightNode(HierarchyNode node)
    {
      HierarchyWindow.CutHighlightNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds Cut highlighting to the specified node.
    /// </summary>
    /// <param name="node">Node to remove the highlight from</param>
    // --------------------------------------------------------------------------------------------
    public void AddCutHighlightNode(HierarchyNode node)
    {
      HierarchyWindow.CutHighlightNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes Cut highlighting from the specified node.
    /// </summary>
    /// <param name="node">Node to remove the highlight from</param>
    // --------------------------------------------------------------------------------------------
    public void UnCutHighlightNode(HierarchyNode node)
    {
      HierarchyWindow.CutHighlightNode(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Starts editing of the specified node label.
    /// </summary>
    /// <param name="node">Node to remove the highlight from</param>
    // --------------------------------------------------------------------------------------------
    public void EditNodeLabel(HierarchyNode node)
    {
      HierarchyWindow.EditNodeLabel(node.ManagerNode, (uint)node.HierarchyId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the state of the item.
    /// </summary>
    /// <param name="manager">The manager.</param>
    /// <param name="id">The id.</param>
    /// <param name="stateMask">The state mask.</param>
    /// <returns></returns>
    // --------------------------------------------------------------------------------------------
    public __VSHIERARCHYITEMSTATE GetNodeState(IHierarchyManager manager, HierarchyId id, 
      __VSHIERARCHYITEMSTATE stateMask)
    {
      uint result;
      HierarchyWindow.GetNodeState(manager, (uint)id, (uint)stateMask, out result);
      return (__VSHIERARCHYITEMSTATE) (result);
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sites the specified hierarchy
    /// </summary>
    /// <param name="hierarchy">Hierarchy to site</param>
    // --------------------------------------------------------------------------------------------
    private void Site(HierarchyManager<TPackage> hierarchy)
    {
      // --- We do not site "null" hierarchies
      if (hierarchy == null) return;

      var oleSp = Frame.OleServiceProvider;
      if (oleSp != null)
      {
        hierarchy.SetSite(oleSp);
      }
      hierarchy.UIHierarchyWindow = HierarchyWindow;
      hierarchy.UIHierarchyToolWindow = this;
    }

    #endregion
  }
}