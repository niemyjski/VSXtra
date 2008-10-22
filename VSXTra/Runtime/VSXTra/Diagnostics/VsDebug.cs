using System;
using System.Diagnostics;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is a lightweight version of the Debug class, it sends its messages to the Visual 
  /// Studio output window.
  /// </summary>
  /// <remarks>
  /// By default output is sent to the Debug pane but you can change it through the 
  /// <see cref="OutputPane"/> property.
  /// </remarks>
  // ================================================================================================
  public static class VsDebug
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the default output to the Debug pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    static VsDebug()
    {
      try
      {
        OutputPane = OutputWindow.Debug;
      }
      catch (SystemException)
      {
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the output window pane used to send messages to.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane OutputPane { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks for a condition and outputs the call stack if the condition is false.
    /// </summary>
    /// <param name="condition">
    /// true to prevent a message being displayed; otherwise, false.
    /// </param>
    // --------------------------------------------------------------------------------------------
    [Conditional("DEBUG")]
    public static void Assert(bool condition)
    {
      try
      {
        if (!condition)
        {
          DisplayIndent();
          OutputPane.WriteLine("Assertion failed");
          DisplayIndent();
          OutputPane.WriteLine(Environment.StackTrace);
        }
      }
      catch (SystemException)
      {
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks for a condition and displays a message if the condition is false.
    /// </summary>
    /// <param name="condition">
    /// true to prevent a message being displayed; otherwise, false.
    /// </param>
    /// <param name="message">A message to display.</param>
    // --------------------------------------------------------------------------------------------
    [Conditional("DEBUG")]
    public static void Assert(bool condition, string message)
    {
      try
      {
        if (!condition)
        {
          DisplayIndent();
          OutputPane.Write("Assertion failed: ");
          OutputPane.WriteLine(message);
        }
      }
      catch (SystemException)
      {
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks for a condition and displays both specified messages if the condition is false.
    /// </summary>
    /// <param name="condition">
    /// true to prevent a message being displayed; otherwise, false.
    /// </param>
    /// <param name="message">A message to display.</param>
    /// <param name="details">A detailed message to display.</param>
    // --------------------------------------------------------------------------------------------
    [Conditional("DEBUG")]
    public static void Assert(bool condition, string message, string details)
    {
      try
      {
        if (!condition)
        {
          DisplayIndent();
          OutputPane.Write("Assertion failed: ");
          OutputPane.WriteLine(message);
          DisplayIndent();
          OutputPane.WriteLine(details);
        }
      }
      catch (SystemException)
      {
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Emits the specified error message.
    /// </summary>
    /// <param name="message">A message to emit.</param>
    // --------------------------------------------------------------------------------------------
    [Conditional("DEBUG")]
    public static void Fail(string message)
    {
      try
      {
        DisplayIndent();
        OutputPane.WriteLine(message);
      }
      catch (SystemException)
      {
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Emits an error message and a detailed error message.
    /// </summary>
    /// <param name="message">A message to emit.</param>
    /// <param name="details">A detailed message to emit.</param>
    // --------------------------------------------------------------------------------------------
    [Conditional("DEBUG")]
    public static void Fail(string message, string details)
    {
      try
      {
        DisplayIndent();
        OutputPane.WriteLine(message);
        DisplayIndent();
        OutputPane.WriteLine(details);
      }
      catch (SystemException)
      {
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Increases the current IndentLevel by one.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [Conditional("DEBUG")]
    public static void Indent()
    {
      IndentLevel++;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Decreases the current IndentLevel by one.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [Conditional("DEBUG")]
    public static void Unindent()
    {
      if (IndentLevel > 0) IndentLevel--;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the indent level.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static int IndentLevel { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the indent size
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static int IndentSize { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays spaces according to the current indentation level and size.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void DisplayIndent()
    {
      OutputPane.Write(string.Empty.PadLeft(IndentLevel*IndentSize, ' '));      
    }
  }
}