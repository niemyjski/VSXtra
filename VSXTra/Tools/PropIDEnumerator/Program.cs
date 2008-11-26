using System;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;

namespace PropIDEnumerator
{
  class Program
  {
    static void Main(string[] args)
    {
      var enumQuery =
        from enumType in Enum.GetNames(typeof (__VSHPROPID))
        select new {Name = enumType, Value = (int) Enum.Parse(typeof (__VSHPROPID), enumType)};
      foreach (var e in enumQuery.OrderBy(en => en.Value))
      {
        Console.WriteLine("{0} = {1}", e.Value, e.Name);
      }
    }
  }
}
