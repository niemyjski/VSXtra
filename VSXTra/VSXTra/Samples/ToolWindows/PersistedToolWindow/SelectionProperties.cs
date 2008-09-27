// ================================================================================================
// SelectionProperties.cs
//
// Created: 2008.07.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using VSXtra;

namespace DeepDiver.PersistedToolWindow
{
  // ================================================================================================
  /// <summary>
	/// This class holds the properties that will be displayed in the Properties window for the 
	/// selected object.
	/// </summary>
	/// <remarks>
	/// While all these properties are read-only, defining the set method would make them writable.
	/// We derive from CustomTypeDescriptor, which is an ICustomTypeDescriptor, and the only part that 
	/// we overload is the ComponentName.
  /// </remarks>
  // ================================================================================================
  public class SelectionProperties : CustomTypeDescriptorBase
	{
    private Guid _PersistanceGuid = Guid.Empty;
		private int _Index = -1;

    // --------------------------------------------------------------------------------------------
    /// <summary>
		/// This class holds the properties for one item. The list of properties could be modified to 
		/// display a different set of properties.
		/// </summary>
		/// <param name="caption">Window Title</param>
		/// <param name="persistence">Persistence Guid</param>
    // --------------------------------------------------------------------------------------------
    public SelectionProperties(string caption, Guid persistence)
		{
			Caption = caption;
			_PersistanceGuid = persistence;
		}

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Caption property that will be exposed in the Properties window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string Caption { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
		/// Guid corresponding to the tool window.
		/// </summary>
    // --------------------------------------------------------------------------------------------
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DisplayName("Persistence GUID")]
		[Description("Guids used to uniquely identify the window type.")]
		[Category("Advanced")]
		public String PersistenceGuid
		{
			get { return _PersistanceGuid.ToString("B"); }
		}

    // --------------------------------------------------------------------------------------------
    /// <summary>
		/// Index of the window in our list. We use this internally to avoid having to search the list 
		/// of windows when the selection is changed from the Property window.
		/// This property will not be visible because we are using the Browsable(false) attribute.
		/// </summary>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
		public int Index
		{
			get { return _Index; }
			set { _Index = value; }
		}

    // --------------------------------------------------------------------------------------------
    /// <summary>
		/// String that will be displayed in the Properties window combo box.
		/// </summary>
    // --------------------------------------------------------------------------------------------
    protected override string ComponentName
		{
			get { return Caption; }
		}
	}
}
