/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Microsoft.VisualStudio.Project.Samples.CustomProject.UnitTests
{
	public static class MockIVsUIShell
	{
		private static GenericMockFactory factory;

		static MockIVsUIShell()
		{
			factory = new GenericMockFactory("MockIVsUIShell", new Type[] { typeof(IVsUIShell), typeof(IVsUIShellOpenDocument) });
		}

		public static BaseMock GetInstance()
		{
			return factory.GetInstance();
		}
	}
}