﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Controls = MahApps.Metro.Controls;

namespace SpeechCommander.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ToggleFileMenuFlyout(object sender, RoutedEventArgs e)
        {
            FileFlyout.IsOpen = !FileFlyout.IsOpen;
        }
    }
}
