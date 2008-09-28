// ================================================================================================
// HierarchyAttributes.cs
//
// Created: 2008.09.27, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class defines the base class of all hierarchy attributes.
  /// </summary>
  // ================================================================================================
  public abstract class HierarchyAttribute : Attribute 
  {
  }

  // ================================================================================================
  /// <summary>
  /// This attribute signs that a node do not have to trigger events coming from its children.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class DoNotTriggerHierarchyEventsAttribute : HierarchyAttribute
  {
  }

  // ================================================================================================
  /// <summary>
  /// This attribute signs that a node do not have to trigger events coming from its parent.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class DoNotTriggerTrackerEventsAttribute : HierarchyAttribute
  {
  }
}