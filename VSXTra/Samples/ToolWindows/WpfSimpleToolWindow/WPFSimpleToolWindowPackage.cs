// ================================================================================================
// SimpleToolWindowPackage.cs
//
// Created: 2008.08.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.WPFSimpleToolWindow
{
    [PackageRegistration( UseManagedResourcesOnly = true )]
    [DefaultRegistryRoot( "Software\\Microsoft\\VisualStudio\\9.0" )]
    [InstalledProductRegistration( false, "#110", "#112", "1.0", IconResourceID = 400 )]
    [ProvideLoadKey( "Standard", "1.0", "WPFSimpleToolWindow", "DeepDiver", 1 )]
    [ProvideMenuResource( 1000, 1 )]
    [XtraProvideToolWindow( typeof( MyWPFToolWindow ) )]
    [Guid( GuidList.guidWPFSimpleToolWindowPkgString )]
    public sealed class WPFSimpleToolWindowPackage : PackageBase
    {
        [CommandExecMethod]
        [CommandId( GuidList.guidWPFSimpleToolWindowCmdSetString, CmdIDs.cmdidShowWPFWindow )]
        [ShowToolWindowAction( typeof( MyWPFToolWindow ) )]
        private void ShowToolWindow()
        {
        }
    }
}