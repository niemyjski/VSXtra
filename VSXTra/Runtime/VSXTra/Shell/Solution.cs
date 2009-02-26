// ================================================================================================
// Solution.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Hierarchy;
using VSXtra.Package;

namespace VSXtra.Shell
{
  // ================================================================================================
  /// <summary>
  /// This class provides operations on the Solution structure.
  /// </summary>
  // ================================================================================================
  public static class Solution
  {
    #region Private fields

    private static readonly IVsSolution _Solution;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the solution service object
    /// </summary>
    // --------------------------------------------------------------------------------------------
    static Solution()
    {
      _Solution = SiteManager.GetGlobalService<SVsSolution, IVsSolution>();
      if (_Solution == null)
      {
        throw new NotSupportedException(typeof(SVsSolution).FullName);
      }
    }

    #endregion

    #region Traversal methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Traverses through all the solution items in DFS order strating from the solution node.
    /// </summary>
    /// <returns>
    /// Enumeration of solution items.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<HierarchyTraversalInfo> TraverseItems()
    {
      if (_Solution == null) yield break;
      var solutionHierarchy = _Solution as IVsHierarchy;
      if (null != solutionHierarchy)
      {
        foreach (var item in TraverseHierarchyItem(solutionHierarchy, 
                                                   VSConstants.VSITEMID_ROOT, 0, true))
        {
          yield return item;
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enumerates over the hierarchy items for the given hierarchy traversing into nested
    /// hierarchies.
    /// </summary>
    /// <param name="hierarchy">Hierarchy to enumerate over.</param>
    /// <param name="itemid">Item id of the hierarchy item to start traversal from.</param>
    /// <param name="recursionLevel">Depth of recursion. e.g. if recursion started with the Solution
    /// node, then : Level 0 -- Solution node, Level 1 -- children of Solution, etc.</param>
    /// <param name="hierIsSolution">true if hierarchy is Solution Node. This is needed to special
    /// case the children of the solution to work around a bug with VSHPROPID_FirstChild and
    /// VSHPROPID_NextSibling implementation of the Solution.</param>
    /// <returns></returns>
    // --------------------------------------------------------------------------------------------
    private static IEnumerable<HierarchyTraversalInfo> TraverseHierarchyItem(
      IVsHierarchy hierarchy, 
      uint itemid, 
      int recursionLevel, 
      bool hierIsSolution)
    {
      IntPtr nestedHierarchyObj;
      uint nestedItemId;
      var hierGuid = typeof(IVsHierarchy).GUID;

      // Check first if this node has a nested hierarchy. If so, then there really are two 
      // identities for this node: 1. hierarchy/itemid 2. nestedHierarchy/nestedItemId.
      // We will recurse and call EnumHierarchyItems which will display this node using
      // the inner nestedHierarchy/nestedItemId identity.
      var hr = hierarchy.GetNestedHierarchy(itemid, ref hierGuid, out nestedHierarchyObj, out nestedItemId);
      if (VSConstants.S_OK == hr && IntPtr.Zero != nestedHierarchyObj)
      {
        var nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHierarchyObj) as IVsHierarchy;
        Marshal.Release(nestedHierarchyObj); // we are responsible to release the refcount on the out IntPtr parameter
        if (nestedHierarchy != null)
        {
          foreach (var item in TraverseHierarchyItem(nestedHierarchy, nestedItemId, recursionLevel, false))
          {
            yield return item;
          }
        }
      }
      else
      {
        // --- Return the current hierarchy item
        yield return new HierarchyTraversalInfo(hierarchy, itemid, recursionLevel);
        recursionLevel++;

        // --- Get the first child node of the current hierarchy being walked.
        // --- NOTE: to work around a bug with the Solution implementation of VSHPROPID_FirstChild,
        // --- we keep track of the recursion level. If we are asking for the first child under
        // --- the Solution, we use VSHPROPID_FirstVisibleChild instead of _FirstChild. 
        // --- In VS 2005 and earlier, the Solution improperly enumerates all nested projects
        // --- in the Solution (at any depth) as if they are immediate children of the Solution.
        // --- Its implementation _FirstVisibleChild is correct however, and given that there is
        // --- not a feature to hide a SolutionFolder or a Project, thus _FirstVisibleChild is 
        // --- expected to return the identical results as _FirstChild.
        object pVar;
        hr = hierarchy.GetProperty(itemid, 
                                   (hierIsSolution && recursionLevel == 1)
                                     ? (int) __VSHPROPID.VSHPROPID_FirstVisibleChild
                                     : (int) __VSHPROPID.VSHPROPID_FirstChild, 
                                   out pVar);
        ErrorHandler.ThrowOnFailure(hr);
        if (VSConstants.S_OK == hr)
        {
          // --- We are using DFS so at each level we recurse to check if the node has any 
          // --- children and then look for siblings.
          var childId = GetItemId(pVar);
          while (childId != VSConstants.VSITEMID_NIL)
          {
            foreach (var item in TraverseHierarchyItem(hierarchy, childId, recursionLevel, false))
            {
              yield return item;
            }
            // --- NOTE: to work around a bug with the Solution implementation of VSHPROPID_NextSibling,
            // --- we keep track of the recursion level. If we are asking for the next sibling under
            // --- the Solution, we use VSHPROPID_NextVisibleSibling instead of _NextSibling. 
            // --- In VS 2005 and earlier, the Solution improperly enumerates all nested projects
            // --- in the Solution (at any depth) as if they are immediate children of the Solution.
            // --- Its implementation   _NextVisibleSibling is correct however, and given that there is
            // --- not a feature to hide a SolutionFolder or a Project, thus _NextVisibleSibling is 
            // --- expected to return the identical results as _NextSibling.
            hr = hierarchy.GetProperty(childId,
                                       (hierIsSolution && recursionLevel == 1)
                                         ? (int)__VSHPROPID.VSHPROPID_NextVisibleSibling
                                         : (int)__VSHPROPID.VSHPROPID_NextSibling,
                                       out pVar);
            if (VSConstants.S_OK == hr)
            {
              childId = GetItemId(pVar);
            }
            else
            {
              ErrorHandler.ThrowOnFailure(hr);
              break;
            }
          }
        }
      }
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the item id.
    /// </summary>
    /// <param name="pvar">VARIANT holding an itemid.</param>
    /// <returns>Item Id of the concerned node</returns>
    // --------------------------------------------------------------------------------------------
    private static uint GetItemId(object pvar)
    {
      if (pvar == null) return VSConstants.VSITEMID_NIL;
      if (pvar is int) return (uint)(int)pvar;
      if (pvar is uint) return (uint)pvar;
      if (pvar is short) return (uint)(short)pvar;
      if (pvar is ushort) return (ushort)pvar;
      if (pvar is long) return (uint)(long)pvar;
      return VSConstants.VSITEMID_NIL;
    }

    #endregion
  }

  /// <summary>
  /// This class represents the information when traversing the solution hierarchy
  /// </summary>
  // ================================================================================================
  public class HierarchyTraversalInfo
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Hierarchy node information
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public HierarchyItem HierarchyNode { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Depth of the node within the hierarchy
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int Depth { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyTraversalInfo"/> class.
    /// </summary>
    /// <param name="hierarchyNode">The hierarchy node.</param>
    /// <param name="depth">The depth of the node.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyTraversalInfo(HierarchyItem hierarchyNode, int depth)
    {
      HierarchyNode = hierarchyNode;
      Depth = depth;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyTraversalInfo"/> class.
    /// </summary>
    /// <param name="hierarchyNode">The hierarchy node.</param>
    /// <param name="itemId">The item id.</param>
    /// <param name="depth">The depth of the node.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyTraversalInfo(IVsHierarchy hierarchyNode, uint itemId, int depth)
    {
      HierarchyNode = new HierarchyItem(hierarchyNode, itemId);
      Depth = depth;
    }
  }
}