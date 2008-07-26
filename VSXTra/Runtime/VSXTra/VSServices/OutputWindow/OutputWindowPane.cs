// ================================================================================================
// OutputWindowPane.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is a wrapper class around an IVsOutputWindowPane instance to manage output handling 
  /// for the window pane.
  /// </summary>
  // ================================================================================================
  public sealed class OutputWindowPane : TextWriter 
  {
    #region Private fields

    private readonly OutputPaneDefinition _PaneDefinition;
    private readonly IVsOutputWindowPane _Pane;
    private string _Name;
    private bool _HasOutput;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an output pane instance using the specified output pane definition and 
    /// IVsOutputWindowPane instance.
    /// </summary>
    /// <param name="paneDef">Pane definition instance</param>
    /// <param name="pane">Physical output window pane</param>
    /// <remarks>
    /// This constructor is to be used only by the OutputWindow class.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    internal OutputWindowPane(OutputPaneDefinition paneDef, IVsOutputWindowPane pane)
    {
      _PaneDefinition = paneDef;
      if (paneDef != null) _Name = paneDef.Name;
      _Pane = pane;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the name of the output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string Name
    {
      get
      {
        if (IsVirtual) return _Name;
        string name = string.Empty;
        _Pane.GetName(ref name);
        return name;
      }
      set
      {
        if (IsVirtual)
          _Name = value;
        else
        _Pane.SetName(value);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks if this output pane is virtual or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsVirtual
    {
      get { return _PaneDefinition != null && _PaneDefinition.IsSilent || _Pane == null; }
    }

    #endregion 

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Activates this output window pane, shows the pane in the output window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Activate()
    {
      if (IsVirtual) return;
      _Pane.Activate();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Hides this output window pane, undisplays it in the output window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Hide()
    {
      if (IsVirtual) return;
      _Pane.Hide();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Clears the content of the output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Clear()
    {
      if (IsVirtual) return;
      _Pane.Clear();
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Outputs the specified string to this window pane.
    /// </summary>
    /// <param name="output">String to send to the output.</param>
    // --------------------------------------------------------------------------------------------
    private void OutputString(string output)
    {
      if (IsVirtual) return;
      _Pane.OutputStringThreadSafe(output);
      if (_PaneDefinition.AutoActivate && !_HasOutput)
      {
        Activate();
      }
      _HasOutput = true;
    }

    #endregion

    #region TextWriter overrides

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the encoding of the output window pane. 
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override Encoding Encoding
    {
      get { return Encoding.UTF8; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Writes a character to the output pane.
    /// </summary>
    /// <param name="value">The character to write to the output pane.</param>
    // --------------------------------------------------------------------------------------------
    public override void Write(char value)
    {
      Write(value.ToString());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Writes a subarray of characters to the output pane.
    /// </summary>
    /// <param name="buffer">The character array to write data from.</param>
    /// <param name="index">Starting index in the buffer.</param>
    /// <param name="count">The number of characters to write.</param>
    // --------------------------------------------------------------------------------------------
    public override void Write(char[] buffer, int index, int count)
    {
      var sb = new StringBuilder(count + 2);
      for (int i = 0; i < count; i++)
      {
        sb.Append(buffer[index + i]);
      }
      Write(sb.ToString());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Writes the specified string to the output of this window pane.
    /// </summary>
    /// <param name="output">String to send to the output.</param>
    /// <remarks>
    /// This operation is thread safe or not according to the ThreadSafe property. If
    /// the output window pane is silent, output is not send to any physical pane.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public override void Write(string output)
    {
      OutputString(output);
    }

    #endregion
  }
}
