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
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
namespace ResizeMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public delegate void UpdateProgress(int progress, string file);
    public partial class MainWindow : Window
    {

        public delegate void WorkerDelegate(double scale);
        
        string m_SourceFolder;
        string m_DestFolder;
        List<string> FilesToProcess;
        private WorkerDelegate asynchDelegate;
        private int m_MaxFiles;
        public MainWindow()
        {
            InitializeComponent();
            FilesToProcess = new List<string>();   
        }

        private void OpenFolder(FolderType FolderType)
        {
            try
            {
                FolderBrowserDialog Open = new FolderBrowserDialog();
                if (Open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    switch (FolderType)
                    {
                        case MainWindow.FolderType.Destination:
                            m_DestFolder = Open.SelectedPath;
                            textBoxDest.Text = m_DestFolder;
                            break;
                        case MainWindow.FolderType.Source:
                            m_SourceFolder = Open.SelectedPath;
                            textBoxSource.Text = m_SourceFolder;
                            break;
                        default:
                            throw new Exception("Unknown Folder Type");
                    }
                }
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source == DestBtnBrowse)
                OpenFolder(FolderType.Destination);
            else if (e.Source == SrcBtnBrowse)
                OpenFolder(FolderType.Source);
            else
                System.Windows.MessageBox.Show("Invalid Operation");
        }

        public enum FolderType 
        {
            Destination,
            Source
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(m_DestFolder) || String.IsNullOrEmpty(m_SourceFolder))
            {
                System.Windows.MessageBox.Show("You need to select destination and source folders");
                return;
            }
            try
            {
                double scale = ParseScale();
                if (scale == 0)
                    return;
                if ((m_MaxFiles= Helper.GetValidFilesInDirectory(m_SourceFolder)) == 0) 
                {
                    System.Windows.MessageBox.Show("No Jpeg files found");
                    return;
                }
                fileProgressBar.Maximum = m_MaxFiles;
                asynchDelegate = new WorkerDelegate(Resize);
                IAsyncResult res = asynchDelegate.BeginInvoke(scale,callback,null);
                lblStatus.Content = "Resizing....";
                btnGo.IsEnabled = false;
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void callback(IAsyncResult res)
        {
            System.Windows.MessageBox.Show("Resizing is completed");
            Dispatcher.Invoke((Action)delegate() 
                { 
                    lblStatus.Content = "Finished";
                    btnGo.IsEnabled = true;
                    fileProgressBar.Value = 0;
                    lblCurrentFile.Content = String.Empty;
                }, null);
        }
        public void Resize(double scale) 
        {
               
            ImageEngine imgEng = new ImageEngine(m_SourceFolder, m_DestFolder,scale);
            imgEng.progressDelegate += new UpdateProgress(UpdateUI);
            imgEng.GetFilesInDirectory();
            imgEng.Process();
        }

        public void UpdateUI(int progress, string file) 
        {
            Dispatcher.Invoke((Action)delegate() 
            {
                UpdateProgressBar(progress);
                UpdateFileLabel(file);
            }, null);   
        }

        public void UpdateProgressBar(int val) 
        {
            fileProgressBar.Value = val;
        }
        public void UpdateFileLabel(string File) 
        {
            lblCurrentFile.Content = File;
        }

        private double ParseScale() 
        {
            double val = 0;
            val = Convert.ToDouble(textBoxScale.Text);
            return val;
        }

        private int GetNumberOfFiles()
        {
            return Directory.GetFiles(m_SourceFolder).Length;
        }

        private void _Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutResizeMe().ShowDialog();

        }
    }
}
