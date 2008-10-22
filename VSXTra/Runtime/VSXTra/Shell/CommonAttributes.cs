// ================================================================================================
// CommonAttributes.cs
//
// Created: 2008.07.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  #region PromoteAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute marks a command method or a service class to be promoted to the parent level.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public sealed class PromoteAttribute : Attribute
  {
  }

  #endregion

  #region ManualBindAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute signs that a menu command handler or service registration is to be bound 
  /// manually by the VSPackage developer.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ManualBindAttribute : Attribute
  {
  }

  #endregion
}