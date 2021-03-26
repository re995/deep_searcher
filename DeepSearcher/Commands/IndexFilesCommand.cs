using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DeepSearcher.Indexing;
using DeepSearcher.Models;
using DeepSearcher.ViewModels;
using Ookii.Dialogs.Wpf;
using ProgressBarStyle = Ookii.Dialogs.Wpf.ProgressBarStyle;

namespace DeepSearcher.Commands
{
    public class IndexFilesCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var vm = parameter as MainViewModel;
            var dialog = new ProgressDialog();
            dialog.ProgressBarStyle = ProgressBarStyle.MarqueeProgressBar;
            dialog.WindowTitle = "Indexing...";
            dialog.Text = "Files indexing in progress";
            dialog.DoWork += (sender, args) =>
            {
                foreach (var path in vm.SearchPaths)
                {
                    dialog.ReportProgress(0, null, null, path.Path);
                    Index(path.Path, dialog);
                }

            };
            dialog.ProgressChanged += (sender, args) =>
            {
                dialog.Description = args.UserState.ToString();
            };

            dialog.CancellationText = "Aborting...";

            dialog.ShowDialog();
        }

        private void Index(string path, ProgressDialog dialog)
        {
            IList<IndexedItem> items = GetFilesToIndex(new DirectoryInfo(path),dialog);
            if (dialog.CancellationPending)
            {
                File.Delete(IndexPaths.GetIdxFilePath(path));
                return;
            }
            using (var writer = new IndexFileWriter(IndexPaths.GetIdxFilePath(path)))
            {
                writer.Write(items);
            }
        }

        public IList<IndexedItem> GetFilesToIndex(DirectoryInfo dir, ProgressDialog dialog)
        {
            var result = new List<IndexedItem>();
            FileAttributes attributes = File.GetAttributes(dir.FullName);
            if ((attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
            {
                try
                {
                    if (dialog.CancellationPending)
                        return result;
                    FileInfo[] fileInfos = dir.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        result.Add(new IndexedItem(fileInfo));
                    }

                    DirectoryInfo[] dirInfos = dir.GetDirectories();
                    foreach (DirectoryInfo dirInfo in dirInfos)
                    {
                        result.AddRange(GetFilesToIndex(dirInfo, dialog));
                    }
                }
                catch
                {
                    // log this?
                }
            }
            return result;
        }







        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
