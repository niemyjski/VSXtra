// ================================================================================================
// CustomProjectPackage.cs
//
// Created: 2008.09.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace VSXtra.ProjectSystem.Samples.CustomProject
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
	[ProvideObject(typeof(GeneralPropertyPage))]
	[ProvideProjectFactory(typeof(MyCustomProjectFactory), "My Custom Project", "My Custom Project Files (*.myproj);*.myproj", "myproj", "myproj", @"..\..\Templates\Projects\MyCustomProject", LanguageVsTemplate = "MyCustomProject", NewProjectRequireNewFolderVsTemplate = false)]
	[ProvideProjectItem(typeof(MyCustomProjectFactory), "My Items", @"..\..\Templates\ProjectItems\MyCustomProject", 500)]
	[Guid(GuidStrings.guidCustomProjectPkgString)]
	public sealed class CustomProjectPackage : ProjectPackageBase
	{
		#region Overriden Implentation
		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initilaization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			this.RegisterProjectFactory(new MyCustomProjectFactory());
		}
		#endregion
	}
}