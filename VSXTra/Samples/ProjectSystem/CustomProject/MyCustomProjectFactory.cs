/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra.ProjectSystem.Samples.CustomProject
{
	/// <summary>
	/// Represent the methods for creating projects within the solution.
	/// </summary>
	[Guid("7C65038C-1B2F-41E1-A629-BED71D161F6F")]
  public class MyCustomProjectFactory : ProjectFactoryBase<CustomProjectPackage>
	{
		#region Constructors
		/// <summary>
		/// Explicit default constructor.
		/// </summary>
		public MyCustomProjectFactory()
		{
		}
		#endregion

		#region Overriden implementation
		/// <summary>
		/// Creates a new project by cloning an existing template project.
		/// </summary>
		/// <returns></returns>
		protected override ProjectNode CreateProject()
		{
			MyCustomProjectNode project = new MyCustomProjectNode(Package);
			project.SetSite((IOleServiceProvider)((IServiceProvider)Package).GetService(typeof(IOleServiceProvider)));
			return project;
		}
		#endregion
	}
}