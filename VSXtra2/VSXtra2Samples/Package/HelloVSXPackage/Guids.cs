// Guids.cs
// MUST match guids.h
using System;

namespace DeepDiver.HelloVSXPackage
{
    static class GuidList
    {
        public const string guidHelloVSXPackagePkgString = "6a2d5951-3bc4-4587-8bf8-b3c0fe15cb1e";
        public const string guidHelloVSXPackageCmdSetString = "3880df00-66db-4c49-8eeb-f3d6eb7c2129";

        public static readonly Guid guidHelloVSXPackageCmdSet = new Guid(guidHelloVSXPackageCmdSetString);
    };
}