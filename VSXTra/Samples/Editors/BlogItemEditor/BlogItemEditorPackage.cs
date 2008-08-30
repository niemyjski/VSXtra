// ================================================================================================
// BlogItemEditorPackage.cs
//
// Created: 2008.08.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra;

namespace DeepDiver.BlogItemEditor
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "BlogItemEditor", "DeepDiver", 1)]
  [ProvideEditorFactory(typeof(BlogItemEditorFactory), 200, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
  [ProvideEditorExtension(typeof(BlogItemEditorFactory),
    BlogItemEditorPackage.BlogFileExtension,
    32,
    ProjectGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
    TemplateDir = @"..\..\Templates",
    NameResourceID = 200)]
  [ProvideEditorLogicalView(typeof(BlogItemEditorFactory), "{7651a703-06e5-11d1-8ebd-00a0c90f26ea}")]
  [Guid(GuidList.guidBlogItemEditorPkgString)]
  public sealed class BlogItemEditorPackage : PackageBase
  {
    #region Constant values

    public const string BlogFileExtension = ".blit";

    #endregion
  }
}