// ================================================================================================
// Guids.cs
//
// Created: 2008.08.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace DeepDiver.ActivityLogPackage
{
  internal static class GuidList
  {
    public const string guidActivityLogCmdSetString = "a65dbb90-8170-41e9-9d61-f83473318792";
    public const string guidActivityLogPkgString = "2143af3f-3a53-4ac2-b9e2-467a39ff1482";

    public static readonly Guid guidActivityLogCmdSet = new Guid(guidActivityLogCmdSetString);
  }
}