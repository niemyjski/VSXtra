﻿/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio;
using VSXtra.ProjectSystem;
using VSXtra.ProjectSystem.Samples.CustomProject;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VSXtra.ProjectSystem.Samples.CustomProject.UnitTests
{
	[TestClass]
	public class MyCustomFileNodeTest : BaseTest
	{
		private CustomProjectPackage customProjectPackage;
		private MyCustomProjectFactory customProjectFactory;
		private MyCustomProjectNode projectNode;

		[ClassInitialize]
		public static void TestClassInitialize(TestContext context)
		{
			projectFile = Path.Combine(context.TestDeploymentDir, projectFile);
		}

		[TestInitialize()]
		public override void Initialize()
		{
			base.Initialize();
			customProjectPackage = new CustomProjectPackage();
			((IVsPackage)customProjectPackage).SetSite(serviceProvider);

			customProjectFactory = new MyCustomProjectFactory();

			base.SetMsbuildEngine(customProjectFactory);

			int canCreate;

			if(VSConstants.S_OK == ((IVsProjectFactory)customProjectFactory).CanCreateProject(projectFile, 2, out canCreate))
			{
				PrivateType type = new PrivateType(typeof(MyCustomProjectFactory));
				PrivateObject obj = new PrivateObject(customProjectFactory, type);
				projectNode = (MyCustomProjectNode)obj.Invoke("PreCreateForOuter", new object[] { IntPtr.Zero });

				Guid iidProject = new Guid();
				int pfCanceled;
				projectNode.Load(projectFile, "", "", 2, ref iidProject, out pfCanceled);
			}
		}

		[TestMethod()]
		public void GetAutomationObjectTest()
		{
			ProjectElement element =
				projectNode.GetProjectElement(new BuildItem("AssemblyInfo", "Compile"));

			Assert.IsNotNull(projectNode.CreateFileNode(element).GetAutomationObject(), "AutomationObject is null");
		}
	}
}