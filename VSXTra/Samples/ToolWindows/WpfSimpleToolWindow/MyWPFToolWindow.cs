// ================================================================================================
// MyToolWindow.cs
//
// Created: 2008.08.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Windows;

namespace DeepDiver.WPFSimpleToolWindow
{
    [Guid( "e6be3132-5e44-47cf-8ab8-7ff70e3e5d5d" )]
    [InitialCaption("$ToolWindowTitle")]
    [BitmapResourceId(301)]
    public class MyWPFToolWindow : WpfToolWindowPane<WPFSimpleToolWindowPackage, MyControl>
    {
    }
}