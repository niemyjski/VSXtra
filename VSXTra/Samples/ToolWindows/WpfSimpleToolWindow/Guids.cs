// ================================================================================================
// Guids.cs
//
// Created: 2009.02.06, by Shawn Hempel
// ================================================================================================
using System;

namespace DeepDiver.WPFSimpleToolWindow
{
  internal class GuidList
  {
    public const string guidWPFSimpleToolWindowCmdSetString = "438c39ee-bdd6-4cba-bf0d-f0f6181ae9ed";
    public readonly Guid guidWPFSimpleToolWindowCmdSet = new Guid(guidWPFSimpleToolWindowCmdSetString);

    public const string guidWPFSimpleToolWindowPkgString = "46d651b0-6efd-4527-8d8c-f9c3db42e93e";
    public readonly Guid guidWPFSimpleToolWindowPkg = new Guid(guidWPFSimpleToolWindowPkgString);
  }
}