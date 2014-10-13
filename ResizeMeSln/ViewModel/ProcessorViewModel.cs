using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ResizeMe.Command;
using System.ComponentModel;
using System.Threading;
using System.IO;
using ResizeMe.Core;

namespace ResizeMe.ViewModel
{
    public class ProcessorViewModel : INotifyPropertyChanged
    {
        public ProcessorViewModel()
        {
            BrowseInputFolderCommand = new ActionCommand(BrowseInputFolderExecuteCommand, () => true);
            BrowseOutputFolderCommand = new ActionCommand(BrowseOutputFolderExecuteCommand, () => true);
            ProcessCommand = new ActionCommand(ProcessImages, () =>
                FilesToProcess.Count > 0 && PercentOfOriginal > 0 && !String.IsNullOrEmpty(OutputFolder));
            FilesToProcess = new List<string>();
            SetDefaults();
        }

        public Action<Action<string>> BrowseFolder { get; set; }
        public Action<string,string> NotifyMessage { get; set; }

        public ICommand BrowseInputFolderCommand { get; private set; }
        public ICommand BrowseOutputFolderCommand { get; private set; }
        public ICommand ProcessCommand { get; private set; }

        public List<string> FilesToProcess { get; private set; }

        private string _inputFolder;
        public string InputFolder
        {
            get { return _inputFolder; }
            set
            {
                _inputFolder = value;
                OnPropertyChanged("InputFolder");
            }
        }

        private string _outputFolder;
        public string OutputFolder
        {
            get { return _outputFolder; }
            set
            {
                _outputFolder = value;
                OnPropertyChanged("OutputFolder");
            }
        }

        private double _percentOfOriginal;
        public double PercentOfOriginal
        {
            get { return _percentOfOriginal; }
            set
            {
                _percentOfOriginal = value;
                OnPropertyChanged("PercentOfOriginal");
            }
        }

        private int _fileCount;
        public int FileCount
        {
            get { return _fileCount; }
            private set
            {
                _fileCount = value;
                OnPropertyChanged("FileCount");
            }
        }

        private int _fileProcessed;
        public int FileProcessed
        {
            get { return _fileProcessed; }
            set
            {
                _fileProcessed = value;
                OnPropertyChanged("FileProcessed");
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private void BrowseInputFolderExecuteCommand()
        {
            BrowseFolder((folder) =>
            {
                InputFolder = folder;
                //change this 
                GetFilesInDirectory();
            });
        }

        private void BrowseOutputFolderExecuteCommand()
        {
            BrowseFolder((folder) => { OutputFolder = folder; });
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private void ProcessImages()
        {
            BackgroundWorker bgWorker = new BackgroundWorker();
            FileCount = FilesToProcess.Count();
            Action<object, DoWorkEventArgs> dowork = (obj, e) =>
            {
                ImageEngine engine = new ImageEngine(PercentOfOriginal);
                int count = 0;
                foreach (string file in FilesToProcess)
                {
                    string outputFile = OutputFolder + "\\" + Path.GetFileName(file);
                    engine.ProcessImage(file, outputFile);
                    count++;
                    bgWorker.ReportProgress(count);
                }
            };
            Action<object, ProgressChangedEventArgs> progressChanged = (obj, e) =>
            {
                FileProcessed = e.ProgressPercentage;
            };
            Action<object, RunWorkerCompletedEventArgs> completed = (obj, e) =>
            {
                Status = "Finished";
                if (e.Error != null) 
                {
                    NotifyMessage("Error has occurred while processing files", "Error");
                    SetDefaults();
                }
            };

            bgWorker.DoWork += new DoWorkEventHandler(dowork);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(progressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(completed);
            bgWorker.WorkerReportsProgress = true;
            Status = "Processing...";
            bgWorker.RunWorkerAsync();

        }

        public void GetFilesInDirectory()
        {
            string[] files = Directory.GetFiles(InputFolder);
            for (int i = 0; i != files.Length; ++i)
            {
                string ext = System.IO.Path.GetExtension(files[i]);
                if (ext.ToLower() == ".jpg")
                    FilesToProcess.Add(System.IO.Path.GetFullPath(files[i]));
            }
        }

        private void SetDefaults() 
        {
            FileCount = 1; //default so progress bar doesnt look like its filled by default 
            FileProcessed = 0;
        }
    }
}
