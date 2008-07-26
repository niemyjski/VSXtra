using System;

namespace VSXtra
{
  public static class VsCommands
  {
    private static string CreateCommandName(string prefix, string command)
    {
      return String.Format("{0}.{1}", prefix, command);
    }

    public static class Window
    {
      private static string Prefix { get { return "Window"; }}

      public static void CloseAllDocuments()
      {
        VsIde.ExecuteMethod(CreateCommandName(Prefix, "CloseAllDocuments"), String.Empty);
      }
    }
  }
}