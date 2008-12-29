// ================================================================================================
// FileHierarchyManager.cs
//
// Created: 2008.11.27, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using VSXtra.Hierarchy;

namespace DeepDiver.HierarchySort
{
  // ================================================================================================
  /// <summary>
  /// This class is repsonsible the File Hierarchy
  /// </summary>
  // ================================================================================================
  public sealed class NumberHierarchyManager : HierarchyManager<HierarchySortPackage>
  {
    /// <summary>Random generator used to create node numbers</summary>
    private static readonly Random _Random = new Random();

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the random generator used by the nodes
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static int RandomNumber
    {
      get { return _Random.Next(1, 1000); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an instance of <see cref="TopLevelNode"/> as the root instance of the hierarchy.
    /// </summary>
    /// <returns>
    /// Newly created root node.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected override HierarchyNode CreateHierarchyRoot()
    {
      return new TopLevelNode(this);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Compares the sort order of the specified nodes.
    /// </summary>
    /// <param name="node1">First node</param>
    /// <param name="node2">Second node</param>
    // --------------------------------------------------------------------------------------------
    public override int CompareNodes(HierarchyNode node1, HierarchyNode node2)
    {
      var numNode1 = node1 as INumberedNode;
      var numNode2 = node2 as INumberedNode;
      return numNode1 == null || numNode2 == null 
        ? base.CompareNodes(node1, node2) 
        : numNode2.Number - numNode1.Number;
    }
  }
}