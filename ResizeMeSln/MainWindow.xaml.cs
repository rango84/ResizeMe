using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using forms=System.Windows.Forms;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using ResizeMe.Core;
using ResizeMe.ViewModel;

namespace ResizeMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //TODO- Work in progress
    public partial class MainWindow : Window
    {
        private ProcessorViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new ProcessorViewModel();
            _viewModel.BrowseFolder = OpenFolder;
            _viewModel.NotifyMessage = NotifyMessage;
            DataContext = _viewModel;   
        }

        private void NotifyMessage(string message, string caption) 
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenFolder(Action<string> onBrowse) 
        {
            forms.FolderBrowserDialog Open = new forms.FolderBrowserDialog();
            if (Open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                onBrowse(Open.SelectedPath);
            }
        }

        private void _Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //TODO: revisit this
        private void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutResizeMe().ShowDialog();
        }
    }
}
