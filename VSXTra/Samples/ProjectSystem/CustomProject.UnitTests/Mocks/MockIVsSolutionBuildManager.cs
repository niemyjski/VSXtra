/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project.Samples.CustomProject.UnitTests
{
	public static class MockIVsSolutionBuildManager
	{
		private static GenericMockFactory factory;

		static MockIVsSolutionBuildManager()
		{
			factory = new GenericMockFactory("MockIVsSolutionBuildManager", new Type[] { typeof(IVsSolutionBuildManager2), typeof(IVsSolutionBuildManager3) });
		}

		public static BaseMock GetInstance()
		{
			return factory.GetInstance();
		}
	}
}