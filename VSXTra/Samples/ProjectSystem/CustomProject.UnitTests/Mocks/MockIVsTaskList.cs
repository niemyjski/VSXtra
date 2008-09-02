/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project.Samples.CustomProject.UnitTests
{
	public static class MockIVsTaskList
	{
		private static GenericMockFactory factory;

		static MockIVsTaskList()
		{
			factory = new GenericMockFactory("MockIVsTaskList", new Type[] { typeof(IVsTaskList) });
		}

		public static BaseMock GetInstance()
		{
			return factory.GetInstance();
		}
	}
}