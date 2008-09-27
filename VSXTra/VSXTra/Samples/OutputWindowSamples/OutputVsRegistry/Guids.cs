// Guids.cs
// MUST match guids.h
using System;

namespace DeepDiver.OutputVsRegistry
{
    static class GuidList
    {
        public const string guidOutputVsRegistryPkgString = "9a43d7ee-e04f-4222-a9ed-a41b7adc52e7";
        public const string guidOutputVsRegistryCmdSetString = "b91d4400-91ed-4e6f-939b-8e44cc5d5a68";

        public static readonly Guid guidOutputVsRegistryCmdSet = new Guid(guidOutputVsRegistryCmdSetString);
    };
}