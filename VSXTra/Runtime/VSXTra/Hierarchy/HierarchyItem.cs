// ================================================================================================
// HierarchyItem.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Selection;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to wrap properties of an IVsHierarchy into a class in order to display
  /// in the Properties window
  /// </summary>
  // ================================================================================================
  public class HierarchyItem: CustomTypeDescriptorBase
  {
    #region Private fields

    private readonly IVsHierarchy _Hierarchy;
    private readonly uint _ItemId;
    private readonly HandleProperty _IconHandle;
    private readonly GuidProperty _TypeGuid;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this class wrapping around the specified hierarchy item.
    /// </summary>
    /// <param name="hierarchy">The hierarchy to wrap.</param>
    /// <param name="itemId">The item id in the hierarchy.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyItem(IVsHierarchy hierarchy, uint itemId)
    {
      _Hierarchy = hierarchy;
      _ItemId = itemId;
      _IconHandle = new HandleProperty(this, __VSHPROPID.VSHPROPID_IconHandle);
      _TypeGuid = new GuidProperty(this, __VSHPROPID.VSHPROPID_TypeGuid);
    }

    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the hierarchy object.
    /// </summary>
    /// <value>The hierarchy object.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public IVsHierarchy Hierarchy
    {
      get { return _Hierarchy; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the item id of this node.
    /// </summary>
    /// <value>The item id of this node.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("ID")]
    [Category("Identification")]
    [Description("Item ID within the hierarchy.")]
    public HierarchyId Id
    {
      get { return _ItemId; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent id of this item within the hierarchy.
    /// </summary>
    /// <value>The parent id within the hierarchy.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Parent Node ID")]
    [Category("Hierarchy info")]
    [Description("Item ID of the parent node; VSITEMID_NIL, if no parent.")]
    public HierarchyId ParentId
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_Parent); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the name of this item.
    /// </summary>
    /// <value>The name of this item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Name")]
    [Category("Names and Paths")]
    [Description("Name of the item.")]
    public string Name
    {
      get { return GetProperty<string>(__VSHPROPID.VSHPROPID_Name); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the name of this item.
    /// </summary>
    /// <value>The name of this item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Save Name")]
    [Category("Names and Paths")]
    [Description("File name specified on the File Save menu.")]
    public string SaveName
    {
      get { return GetProperty<string>(__VSHPROPID.VSHPROPID_SaveName); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the name of this item.
    /// </summary>
    /// <value>The name of this item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Type Name")]
    [Category("Names and Paths")]
    [Description("Displays name to identify type of node.")]
    public string TypeName
    {
      get { return GetProperty<string>(__VSHPROPID.VSHPROPID_TypeName); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this item.
    /// </summary>
    /// <value>The parent hierarchy.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyItem ParentHierarchy
    {
      get
      {
        var parentHierarchy = GetProperty(__VSHPROPID.VSHPROPID_ParentHierarchy) as IVsHierarchy;
        return parentHierarchy == null 
          ? null
          : new HierarchyItem(parentHierarchy, VSConstants.VSITEMID_ROOT);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the 32-bit signed handle for the Image list holding icons for the hierarchy items.
    /// </summary>
    /// <value>Image list handle value.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("ImageList Handle")]
    [Category("Appereance")]
    [Description("Handle of the Image list holding icons for the item.")]
    public int ImageListHandle
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_IconImgList); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the 32-bit signed handle for the Image list holding icons for the hierarchy items.
    /// </summary>
    /// <value>Image list handle value.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Icon Index")]
    [Category("Appereance")]
    [Description("Index of the icon within the image list.")]
    public int IconIndex
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_IconIndex); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the 32-bit signed handle for the Image list holding icons for the hierarchy items.
    /// </summary>
    /// <value>Image list handle value.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Icon Handle")]
    [Category("Appereance")]
    [Description("Handle of the icon for the item.")]
    public HandleProperty IconHandle
    {
      get { return _IconHandle; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the 32-bit signed handle for the Image list holding icons for the hierarchy items.
    /// </summary>
    /// <value>Image list handle value.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Type Guid")]
    [Category("Type Info")]
    [Description("GUID identifying the type of the hierarchy item.")]
    public GuidProperty TypeGuid
    {
      get { return _TypeGuid; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the node is expandable or not.
    /// </summary>
    /// <value>flag indicating if the node is expandable or not..</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Expandable")]
    [Category("Appereance")]
    [Description("Indicates if the node is expandable or not.")]
    public bool Expandable
    {
      get { return GetProperty<bool>(__VSHPROPID.VSHPROPID_Expandable); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the node is expandable or not.
    /// </summary>
    /// <value>flag indicating if the node is expandable or not..</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Expand by default")]
    [Category("Appereance")]
    [Description("Indicates if the node is expanded by default or not.")]
    public bool ExpandByDefault
    {
      get { return GetProperty<bool>(__VSHPROPID.VSHPROPID_ExpandByDefault); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy item id.
    /// </summary>
    /// <value>The parent hierarchy item id.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Item ID in Parent Hierarchy")]
    [Category("Identification")]
    [Description("If the current hierachy is nested into a parent hierarchy, this value tells the ID used in the parent hierarchy.")]
    public HierarchyId ParentHierarchyItemId
    {
      get
      {
        var id = GetProperty(__VSHPROPID.VSHPROPID_ParentHierarchyItemid);
        if (id is int) return (uint) (int) id;
        if (id is uint) return (uint) id;
        return HierarchyId.Nil;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a value indicating whether this instance is nested hierachy.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is nested hierachy; otherwise, <c>false</c>.
    /// </value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Is Nested Hierarchy?")]
    [Category("Hierarchy info")]
    [Description("This flag tells if this hierarchy item is nested into an owner hierarchy or not.")]
    public bool IsNestedHierachy
    {
      get { return Id.IsRoot && !ParentHierarchyItemId.IsNil; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the first child of this hierarchy item.
    /// </summary>
    /// <value>The first child of this hierarchy item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("First Child ID")]
    [Category("Hierarchy info")]
    [Description("Item ID of the first child node; VSITEMID_NIL, if this item has no children.")]
    public HierarchyId FirstChild
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_FirstChild); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the first child of this hierarchy item.
    /// </summary>
    /// <value>The first child of this hierarchy item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("First Visible Child ID")]
    [Category("Hierarchy info")]
    [Description("Item ID of the first visible child node; VSITEMID_NIL, if this item has no visible children.")]
    public HierarchyId FirstVisibleChild
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_FirstVisibleChild); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the first child of this hierarchy item.
    /// </summary>
    /// <value>The first child of this hierarchy item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Next Sibling ID")]
    [Category("Hierarchy info")]
    [Description("Item ID of the subsquent sibling of this item; VSITEMID_NIL, if this item has no more siblings.")]
    public HierarchyId NextSibling
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_NextSibling); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the first child of this hierarchy item.
    /// </summary>
    /// <value>The first child of this hierarchy item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Next Visible Sibling ID")]
    [Category("Hierarchy info")]
    [Description("Item ID of the subsquent visible sibling of this item; VSITEMID_NIL, if this item has no more siblings.")]
    public HierarchyId NextVisibleSibling
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_NextVisibleSibling); }
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected T GetProperty<T>(__VSHPROPID propId)
    {
      return (T)GetProperty(propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected T GetProperty<T>(int propId)
    {
      return (T)GetProperty(propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected object GetProperty(__VSHPROPID propId)
    {
      return GetProperty((int) propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected object GetProperty(int propId)
    {
      if (propId == (int)__VSHPROPID.VSHPROPID_NIL) return null;
      object propValue;
      _Hierarchy.GetProperty(_ItemId, propId, out propValue);
      return propValue;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified GUID hierarchy property.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected object GetGuidProperty(int propId)
    {
      if (propId == (int)__VSHPROPID.VSHPROPID_NIL) return null;
      Guid propValue;
      _Hierarchy.GetGuidProperty(_ItemId, propId, out propValue);
      return propValue;
    }

    #endregion

    #region Overrides of CustomTypeDescriptorBase

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Name of the component.
    /// </summary>
    /// <remarks>
    /// When this class is used to expose property in the Properties window, this should be the 
    /// name associated with this instance.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override string ComponentName
    {
      get { return string.IsNullOrEmpty(Name) ? "<none>" : Name; }
    }

    #endregion
  }
}