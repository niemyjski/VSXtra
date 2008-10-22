// ================================================================================================
// ServiceAttributes.cs
//
// Created: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This attributes tells if a service class should be automatically created at registration time.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class AutoCreateServiceAttribute : Attribute
  {
  }
}