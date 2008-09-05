/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace VSXtra.ProjectSystem.Samples.CustomProject.UnitTests
{
	public static class MockILocalRegistry
	{
		private static GenericMockFactory factory;

		static MockILocalRegistry()
		{
      factory = new GenericMockFactory("MockILocalRegistry", new Type[] { typeof(ILocalRegistry), typeof(ILocalRegistry3), typeof(ILocalRegistry4) });
		}

		public static BaseMock GetInstance()
		{
			BaseMock mock = factory.GetInstance();

			string name = string.Format("{0}.{1}", typeof(IVsWindowFrame).FullName, "SetProperty");
			mock.AddMethodReturnValues(name, new object[] { VSConstants.S_OK });

			name = string.Format("{0}.{1}", typeof(ILocalRegistry3).FullName, "GetLocalRegistryRoot");
			mock.AddMethodCallback(name, GetLocalRegistryRoot);
      name = string.Format("{0}.{1}", typeof(ILocalRegistry4).FullName, "GetLocalRegistryRootEx");
      mock.AddMethodCallback(name, GetLocalRegistryRootEx);

			return mock;
		}

		private static void GetLocalRegistryRoot(object caller, CallbackArgs arguments)
		{
			arguments.SetParameter(0, @"SOFTWARE\Microsoft\VisualStudio\9.0");
			arguments.ReturnValue = VSConstants.S_OK;
		}
  
    private static void GetLocalRegistryRootEx(object caller, CallbackArgs arguments)
    {
      arguments.SetParameter(1, 0U);
      arguments.SetParameter(2, @"SOFTWARE\Microsoft\VisualStudio\9.0");
      arguments.ReturnValue = VSConstants.S_OK;
    }
  }
}