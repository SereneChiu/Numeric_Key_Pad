﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NumericKeyPad;

namespace NumericKeyPadApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nindex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern UInt32 GetWindowLong(IntPtr hWnd, int index);
        public MainWindow()
        {
            InitializeComponent();
            UserControl1 pad = new UserControl1(null);
            this.Content = pad;
            this.Topmost = true;
        }
    }
}
