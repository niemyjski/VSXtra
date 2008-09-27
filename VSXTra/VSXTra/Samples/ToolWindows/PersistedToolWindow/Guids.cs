// ================================================================================================
// Guids.cs
//
// Created: 2008.07.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace DeepDiver.PersistedToolWindow
{
  static class GuidList
  {
    public const string guidPersistedToolWindowPkgString = "859dbfb9-90fd-44ed-b018-8e7cc598eaec";
    public const string guidPersistedToolWindowCmdSetString = "D394C2A8-C496-404c-9542-2EC31B7F864D";
    public static readonly Guid guidPersistedToolWindowCmdSet = new Guid(guidPersistedToolWindowCmdSetString);
  };
}