// ================================================================================================
// MyControl.cs
//
// Created: 2008.08.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Security.Permissions;
using System.Windows.Forms;

namespace DeepDiver.SimpleToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class implements the user interface of the simple tool window
  /// </summary>
  // ================================================================================================
  public partial class MyControl : UserControl
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the control
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public MyControl()
    {
      InitializeComponent();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary> 
    /// Let this control process the mnemonics.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
    protected override bool ProcessDialogChar(char charCode)
    {
      if (charCode != ' ' && ProcessMnemonic(charCode))
      {
        return true;
      }
      return base.ProcessDialogChar(charCode);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enable the IME status handling for this control.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override bool CanEnableIme
    {
      get { return true; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Command execution method
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
    private void button1_Click(object sender, System.EventArgs e)
    {
      MessageBox.Show(this,
                      string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                      "VSXtra Simple Tool Window");
    }
  }
}
