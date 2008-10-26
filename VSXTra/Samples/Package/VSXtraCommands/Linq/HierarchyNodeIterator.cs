// ================================================================================================
// HierarchyNodeIterator.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace DeepDiver.VSXtraCommands
{
  /// <summary>
  /// Iterator for IVSHierarchy hierarchy
  /// </summary>
  public sealed class HierarchyNodeIterator : IEnumerable<HierarchyNode>
  {
    #region Fields

    private readonly IVsSolution _Solution;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNodeIterator"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    public HierarchyNodeIterator(IVsSolution solution)
    {
      if (solution == null)
      {
        throw new ArgumentNullException("solution");
      }
      _Solution = solution;
    }

    #endregion

    #region Public Implementation

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<HierarchyNode> GetEnumerator()
    {
      IEnumHierarchies penum;
      Guid nullGuid = Guid.Empty;
      int hr = _Solution.GetProjectEnum((uint) __VSENUMPROJFLAGS.EPF_ALLINSOLUTION, ref nullGuid, out penum);
      ErrorHandler.ThrowOnFailure(hr);
      if ((VSConstants.S_OK == hr) && (penum != null))
      {
        uint fetched;
        var rgelt = new IVsHierarchy[1];
        while (penum.Next(1, rgelt, out fetched) == 0 && fetched == 1)
        {
          foreach (HierarchyNode hier in Enumerate(rgelt[0], VSConstants.VSITEMID_ROOT, 0))
          {
            yield return hier;
          }
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

    #region Private Implementation

    private static IEnumerable<HierarchyNode> Enumerate(IVsHierarchy hierarchy, uint itemid, int recursionLevel)
    {
      yield return new HierarchyNode(hierarchy, itemid);

      object pVar;
      recursionLevel++;

      int hr = hierarchy.GetProperty(itemid, (int) __VSHPROPID.VSHPROPID_FirstChild, out pVar);
      ErrorHandler.ThrowOnFailure(hr);

      if (VSConstants.S_OK == hr)
      {
        uint childId = GetItemId(pVar);
        while (childId != VSConstants.VSITEMID_NIL)
        {
          foreach (HierarchyNode nestedNode in Enumerate(hierarchy, childId, recursionLevel))
          {
            yield return nestedNode;
          }

          hr = hierarchy.GetProperty(childId, (int) __VSHPROPID.VSHPROPID_NextSibling, out pVar);

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

    private static uint GetItemId(object pvar)
    {
      if (pvar == null) return VSConstants.VSITEMID_NIL;
      if (pvar is int) return (uint) (int) pvar;
      if (pvar is uint) return (uint) pvar;
      if (pvar is short) return (uint) (short) pvar;
      if (pvar is ushort) return (ushort) pvar;
      if (pvar is long) return (uint) (long) pvar;
      return VSConstants.VSITEMID_NIL;
    }

    #endregion
  }
}