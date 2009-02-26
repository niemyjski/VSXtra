using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DeepDiver.WPFSimpleToolWindow
{
  /// <summary>
  /// Interaction logic for MyControl.xaml
  /// </summary>
  public partial class MyControl : UserControl
  {
    public MyControl()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", ToString()), "VSXtra WPF Simple Tool Window");
    }
  }
}
