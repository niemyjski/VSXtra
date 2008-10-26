// Guids.cs
// MUST match guids.h
using System;

namespace DeepDiver.DynamicCommands
{
  internal static class GuidList
  {
    public const string guidDynamicCommandsCmdSetString = "167aa10e-db32-449c-ab8a-09239a0cb36a";
    public const string guidDynamicCommandsPkgString = "958692d7-8846-4754-b721-170860696341";

    public static readonly Guid guidDynamicCommandsCmdSet = new Guid(guidDynamicCommandsCmdSetString);
  } ;
}