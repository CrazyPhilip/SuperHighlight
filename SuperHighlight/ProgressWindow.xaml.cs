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
using System.Windows.Shapes;
using SuperHighlight.Models;
using SuperHighlight.ViewModels;

namespace SuperHighlight
{
    /// <summary>
    /// ProgressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressViewModel progressViewModel;

        public ProgressWindow(string SelectedLanguage, string SelectedFont, string SelectedFontSize, string SelectedTheme, string OutputFolderPath, List<FileInformation> FileList)
        {
            InitializeComponent();

            progressViewModel = new ProgressViewModel(SelectedLanguage, SelectedFont, SelectedFontSize.ToString(), SelectedTheme, OutputFolderPath, FileList);

            DataContext = progressViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
