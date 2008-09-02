/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project.Samples.CustomProject.UnitTests
{
	public static class MockIVsRegisterProjectTypes
	{
		private static GenericMockFactory factory;

		static MockIVsRegisterProjectTypes()
		{
			factory = new GenericMockFactory("MockIVsRegisterProjectTypes", new Type[] { typeof(IVsRegisterProjectTypes) });
		}

		public static BaseMock GetInstance()
		{
			return factory.GetInstance();
		}
	}
}