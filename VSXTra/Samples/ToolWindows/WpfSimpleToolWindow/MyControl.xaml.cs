﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void button1_Click( object sender, RoutedEventArgs e )
        {
            MessageBox.Show( string.Format( CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", ToString() ), "VSXtra WPF Simple Tool Window" );
        }
    }
}
