using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeepSearcher.Helpers;
using DeepSearcher.Indexing;
using DeepSearcher.Interfaces;
using DeepSearcher.Models;
using DeepSearcher.ViewModels;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace DeepSearcher.Commands
{
    public class SearchCommand : ICommand
    {
        private const int MaxFileReadingSize = 100;

        private BackgroundWorker _worker;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private CancellationToken _cancellationToken;

        public SearchCommand()
        {
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public bool CanExecute(object parameter)
        {
            var vm = parameter as MainViewModel;
            if (!string.IsNullOrEmpty(vm.SearchSequence) && vm.SearchPaths.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            var vm = parameter as MainViewModel;

            // In this case the button is Cancel, so we want to cancel all work
            if (vm.SearchInProgress)
            {
                CancelAllWork(vm);
            }
            else
            {
                Application.Current.Dispatch(() => _worker = new BackgroundWorker());
                _worker.WorkerSupportsCancellation = true;
                _worker.DoWork += Search;
                _worker.RunWorkerCompleted += (sender, args) =>
                {
                    vm.ProgressBarVisibility = Visibility.Hidden;
                    vm.SearchInProgress = false;
                    _cancellationTokenSource.Cancel();
                    if (vm.SearchResultsWPF.Count != vm.SearchResults.Count)
                    {
                        Application.Current.Dispatch(() => vm.SearchResultsWPF.Clear());
                        vm.SearchResultsWPF = new ObservableCollection<ISearchItem>(vm.SearchResults);
                    }

                    vm.SearchResults.Clear();
                };

                vm.ResetCounters();
                UpdateSearchResults(vm);
                vm.SearchInProgress = true;
            }
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

        #region Methods

        private void UpdateSearchResults(MainViewModel vm)
        {
            if (_worker.IsBusy)
            {
                _worker.CancelAsync();
            }
            vm.SearchResultsWPF.Clear();
            Task.Factory.StartNew(() =>
            {
                while (_worker.CancellationPending)
                {

                }
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _worker.RunWorkerAsync(vm);
                vm.ProgressBarVisibility = Visibility.Visible;
            });
        }

        private void CancelAllWork(MainViewModel vm)
        {
            if (_worker != null && _worker.IsBusy)
            {
                _worker.CancelAsync();
                _cancellationTokenSource.Cancel();
                vm.CurrentProgressBarValue = 0;
                vm.ProgressBarMaxValue = 0;
            }
        }

        private void Search(object sender, DoWorkEventArgs e)
        {
            var watch = new Stopwatch();
            watch.Start();
            var vm = e.Argument as MainViewModel;
            List<SearchCondition> searchConditions = vm.SearchConditions.Select(p => p.Condition).ToList();
            var search = Task.Factory.StartNew(() =>
            {
                foreach (PathItem directory in vm.SearchPaths)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    if (directory.Checked)
                    {
                        if (!vm.DeepSearch && vm.SearchIndexed && IndexPaths.IdxExists(directory.Path))
                            LookForSequenceInIndexedDirectory(vm, directory.Path, vm.SearchSequence, searchConditions);
                        else
                            LookForSequenceInDirectory(vm, new DirectoryInfo(directory.Path), vm.SearchSequence, true, searchConditions);
                    }
                }
            }, _cancellationToken);
            var getSize = Task.Factory.StartNew(() =>
            {
                long result = 0;
                foreach (PathItem directory in vm.SearchPaths)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    if (directory.Checked)
                    {
                        if (!vm.DeepSearch && vm.SearchIndexed && IndexPaths.IdxExists(directory.Path))
                            result += GetIndexedDirectoryFilesCount(directory.Path);
                        else
                            result += GetFilesCount(new DirectoryInfo(directory.Path), vm, searchConditions);
                    }
                }
                return result;
            }, _cancellationToken).ContinueWith(task => vm.ProgressBarMaxValue = task.Result, _cancellationToken);
            Task.WaitAll(search);
            // GC forgets to collect the last byte[] read, so we want to collect them
            _cancellationTokenSource.Cancel();
            GC.Collect();
            watch.Stop();
            vm.CurrentProgressBarValue = 0;
            vm.ProgressBarMaxValue = 0;
            var builder = new StringBuilder();
            if (Debugger.IsAttached)
            {
                builder.AppendFormat("Everything took {0} \n", watch.Elapsed);
                builder.AppendFormat("Scanned {0} files \n", vm.TotalFilesCount);
                builder.AppendFormat("Average time for one file is {0}", TimeSpan.FromMilliseconds(((double)watch.ElapsedMilliseconds / vm.TotalFilesCount)));
            }
            else
            {
                builder.Append("Finished!");
            }

            MessageBox.Show(builder.ToString(), "Finished", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LookForSequenceInIndexedDirectory(MainViewModel vm, string path, string sequence, ICollection<SearchCondition> conditions = null)
        {
            using (var reader = new IndexFileReader(IndexPaths.GetIdxFilePath(path)))
            {
                IEnumerable<IndexedItem> items = reader.ReadAll();
                //Parallel.ForEach(items, (item) =>
                //{
                foreach (var item in items)
                {
                    var size = (double)item.Length / 1024 / 1024;
                    if (CheckSearchConditions(item, conditions) && CheckDateRange(item, vm.SearchDatesRange) && CheckSizeRange(item, vm.SizeRange))
                    {
                        if (sequence.Equals("*") || item.Name.ToLower().Contains(sequence.ToLower()))
                        {
                            vm.SearchResults.Add(item);
                            vm.TotalFilesMatch++;
                            _cancellationToken.ThrowIfCancellationRequested();
                        }

                        vm.TotalFilesSize += size;
                        vm.TotalFilesCount++;
                        vm.CurrentProgressBarValue++;
                    }
                }
                //});
            }
        }

        private long GetFilesCount(DirectoryInfo dir, MainViewModel vm, IList<SearchCondition> conditions = null, int currentDeepness = 0)
        {
            conditions = conditions ?? new List<SearchCondition>();
            _cancellationToken.ThrowIfCancellationRequested();
            long total = 0;
            FileAttributes attributes = File.GetAttributes(dir.FullName);
            if ((attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
            {
                try
                {
                    IEnumerable<ISearchItem> fileInfos = dir.GetFiles().Wrap();
                    foreach (ISearchItem fileInfo in fileInfos)
                    {
                        _cancellationToken.ThrowIfCancellationRequested();
                        if (CheckSearchConditions(fileInfo, conditions) && CheckDateRange(fileInfo, vm.SearchDatesRange) && CheckSizeRange(fileInfo, vm.SizeRange))
                        {
                            if (vm.DeepSearch)
                            {
                                if (((double)fileInfo.Length / 1024 / 1024) < MaxFileReadingSize)
                                    total++;
                            }
                            else
                            {
                                if (vm.SearchSequence.Equals("*") || fileInfo.Name.ToLower().Contains(vm.SearchSequence.ToLower()))
                                    total++;
                            }
                        }
                    }

                    DirectoryInfo[] dirInfos = dir.GetDirectories();
                    foreach (DirectoryInfo dirInfo in dirInfos)
                    {
                        if (vm.Deepness > currentDeepness || vm.Deepness == 0)
                            total += GetFilesCount(dirInfo, vm, conditions, currentDeepness + 1);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // log this?
                }
            }
            return total;
        }

        private long GetIndexedDirectoryFilesCount(string path)
        {
            using (var reader = new IndexFileReader(IndexPaths.GetIdxFilePath(path)))
            {
                return reader.ReadAll().Count();
            }
        }

        private static bool CheckSizeRange(ISearchItem file, SizeRange range)
        {
            if (range == null)
                return true;
            return range.IsInRange(file.Length);
        }

        private static bool CheckDateRange(ISearchItem file, CalendarDateRange range = null)
        {
            if (range == null)
                return true;
            return file.CreationTime >= range.Start && file.CreationTime <= range.End;
        }

        private static bool CheckSearchConditions(ISearchItem file, ICollection<SearchCondition> conditions = null)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return true;
            }

            foreach (var searchCondition in conditions)
            {
                if (searchCondition.IsIncluded(file))
                {
                    return true;
                }
            }

            return false;
        }

        private void LookForSequenceInDirectory(MainViewModel vm, DirectoryInfo directory, string sequence, bool isRecursive = true, IList<SearchCondition> conditions = null, int currentDeepness = 0)
        {
            conditions = conditions ?? new List<SearchCondition>();
            try
            {
                foreach (DirectoryInfo d in directory.GetDirectories())
                {
                    FileAttributes attributes = File.GetAttributes(d.FullName);
                    if ((attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    {
                        _cancellationToken.ThrowIfCancellationRequested();
                        if (isRecursive)
                        {
                            if (vm.Deepness > currentDeepness || vm.Deepness == 0)
                                LookForSequenceInDirectory(vm, d, sequence, isRecursive, conditions, currentDeepness + 1);
                        }
                    }
                }

                var parallelOptions = new ParallelOptions();
                parallelOptions.CancellationToken = _cancellationToken;
                byte[] bytes = Encoding.Default.GetBytes(sequence);
                //Parallel.ForEach(directory.GetFilesToIndex(), parallelOptions, (file) =>
                foreach (var file in directory.GetFiles().Wrap())
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    var size = (double)file.Length / 1024 / 1024;
                    if (CheckSearchConditions(file, conditions) && CheckDateRange(file, vm.SearchDatesRange) && CheckSizeRange(file, vm.SizeRange))
                    {
                        if (!vm.DeepSearch)
                        {
                            if (sequence.Equals("*") || file.Name.ToLower().Contains(sequence.ToLower()))
                            {
                                vm.SearchResults.Add(file);
                                vm.TotalFilesMatch++;
                                _cancellationToken.ThrowIfCancellationRequested();
                                //Application.Current.Dispatcher.InvokeAsync(() =>
                                //{
                                //    vm.SearchResultsWPF.Add(file);
                                //    _cancellationToken.ThrowIfCancellationRequested();
                                //},
                                //    DispatcherPriority.ContextIdle,
                                //    _cancellationToken);

                            }

                            vm.TotalFilesSize += size;
                            vm.TotalFilesCount++;
                            vm.CurrentProgressBarValue++;
                        }
                        else
                        {
                            if (size < MaxFileReadingSize)
                            {
                                var fileBytes = File.ReadAllBytes(file.FullName);
                                if (fileBytes.Contains(bytes))
                                {
                                    Application.Current.Dispatch(() => vm.SearchResultsWPF.Add(file));
                                    vm.SearchResults.Add(file);
                                    vm.TotalFilesMatch++;
                                }
                                fileBytes = null;
                                vm.TotalFilesSize += size;
                                vm.TotalFilesCount++;
                                vm.CurrentProgressBarValue++;
                            }
                        }
                    }
                    //});
                }
            }

            catch (ArgumentException)
            {
                MessageBox.Show("The path provided is not valid\nPlease try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (UnauthorizedAccessException)
            {
            }
            catch (PathTooLongException)
            {

            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("The path provided does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (IOException e)
            {
                if (e.Message.ToLower().Contains("another process"))
                    return;
                MessageBox.Show(string.Format("An error has occurred\n\n\nDetails:\n{0}", e), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (OperationCanceledException)
            {

            }

            catch (Exception e)
            {

            }

        }

        #endregion
    }
}
