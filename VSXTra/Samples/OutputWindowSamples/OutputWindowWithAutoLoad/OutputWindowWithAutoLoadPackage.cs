// ================================================================================================
// OutputWindowWithAutoLoadPackage.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.OutputWindowWithAutoLoad
{
  // ================================================================================================
  /// <summary>
  /// This sample demonstrates how to use VSXtra to define a package with automatic load and using 
  /// the OutputWindow class.
  /// </summary>
  /// <remarks>
  /// 	<para>Points of interest:</para>
  /// 	<list type="bullet">
  /// 		<item></item>
  /// 		<item>
  ///             The <see cref="OutputWindowWithAutoLoadPackage"/> class derives from
  ///             <see cref="PackageBase"/> and not from <see cref="Package"/>.
  ///         </item>
  /// 		<item>
  /// 			<see cref="XtraProvideAutoLoadAttribute"/> attribute is used where UI contexts
  ///             are represented by a type and not a Guid.
  ///         </item>
  /// 		<item>
  ///             The <see cref="Console"/> class is used to provide output.
  ///         </item>
  /// 	</list>
  /// </remarks>
  // ================================================================================================
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "OutputWithAutoLoad", "DeepDiver", 1)]
  [Guid(GuidList.guidOutputWindowWithAutoLoadPkgString)]
  [XtraProvideAutoLoad(typeof(UIContext.NoSolution))]
  public sealed class OutputWindowWithAutoLoadPackage : PackageBase
  {
    protected override void Initialize()
    {
      Console.WriteLine();
      Console.WriteLine("This package demonstrates how to use the Output window.");
      
      Console.Write("*** Boolean values: ");
      Console.Write(true);
      Console.Write("|");
      Console.WriteLine(false);
      
      Console.Write("*** Character values: ");
      Console.Write('H');
      Console.Write("|");
      Console.Write('e');
      Console.Write("|");
      Console.Write('l');
      Console.Write("|");
      Console.Write('l');
      Console.Write("|");
      Console.WriteLine('o');
      
      Console.Write("*** Integral values: ");
      Console.Write((byte)0xab);
      Console.Write("|");
      Console.Write((short)12345);
      Console.Write("|");
      Console.Write(123456789);
      Console.Write("|");
      Console.WriteLine(1234567890123456L);
      
      Console.Write("*** String values: ");
      Console.Write("Hello");
      Console.Write("|");
      Console.WriteLine("World");

      Console.Write("*** Floating point values: ");
      Console.Write((Single)Math.PI);
      Console.Write("|");
      Console.WriteLine(Math.E);

      Console.Write("*** Char arrays: ");
      var chars = new [] { 'H', 'e', 'l', 'l', 'o' };
      Console.Write(chars);
      Console.Write("|");
      Console.WriteLine(chars);
      Console.WriteLine("End of demonstration.");
      Console.WriteLine();
    }
  }
}