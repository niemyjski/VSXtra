/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Reflection;
using Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;

namespace Microsoft.VisualStudio.Project.Samples.CustomProject.UnitTests
{
	public abstract class BaseTest
	{
		protected Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider serviceProvider;
		protected TestContext testContextInstance;
		protected static string projectFile = "MyCustomProject.myproj";

		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[TestInitialize()]
		public virtual void Initialize()
		{
			serviceProvider = Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider.CreateOleServiceProviderWithBasicServices();
			// Solution Support
			serviceProvider.AddService(typeof(SVsSolution), MockIVsSolution.GetInstance(), false);
			// Project Types Support
			serviceProvider.AddService(typeof(SVsRegisterProjectTypes), MockIVsRegisterProjectTypes.GetInstance(), false);
			// UIShell Support
			BaseMock uiShell = MockIVsUIShell.GetInstance();
			serviceProvider.AddService(typeof(SVsUIShell), (IVsUIShell)uiShell, false);
			serviceProvider.AddService(typeof(SVsUIShellOpenDocument), (IVsUIShellOpenDocument)uiShell, false);
			// Shell Support
			serviceProvider.AddService(typeof(SVsShell), MockIVsShell.GetInstance(), false);
			// Build Manager support
			serviceProvider.AddService(typeof(SVsSolutionBuildManager), MockIVsSolutionBuildManager.GetInstance(), false);
			// ILocalRegistry support
			serviceProvider.AddService(typeof(SLocalRegistry), (ILocalRegistry)MockILocalRegistry.GetInstance(), false);
			// IVsTaskList support
			serviceProvider.AddService(typeof(SVsTaskList), MockIVsTaskList.GetInstance(), false);
		}

		protected virtual void SetMsbuildEngine(ProjectFactory factory)
		{
			FieldInfo buildEngine = typeof(ProjectFactory).GetField("buildEngine", BindingFlags.Instance | BindingFlags.NonPublic);
			buildEngine.SetValue(factory, Engine.GlobalEngine);
			Microsoft.Build.BuildEngine.Project msbuildproject = Engine.GlobalEngine.CreateNewProject();
			FieldInfo buildProject = typeof(ProjectFactory).GetField("buildProject", BindingFlags.Instance | BindingFlags.NonPublic);
			buildProject.SetValue(factory, msbuildproject);
		}
	}
}
