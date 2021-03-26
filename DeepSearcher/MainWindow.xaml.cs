using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DeepSearcher.Interfaces;
using DeepSearcher.ViewModels;

namespace DeepSearcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = Resources["MainViewModel"] as MainViewModel;
            //NumericUpDown.LostFocus += (sender, args) => _vm.Deepness = NumericUpDown.Value;

            
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                string path = ((sender as ListViewItem).Content as ISearchItem).FullName;
                Process.Start("explorer.exe", "/select," + path);
            }
        }
        private void ListBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string path = ((sender as ListViewItem).Content as ISearchItem).FullName;
                Process.Start("explorer.exe", "/select," + path);
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SettingsLoader.Save((_vm).SearchPaths);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = new List<int>();

            var items = new List<ListViewItem>();
            for (int i = 0; i < PathsListView.Items.Count; i++)
            {
                items.Add(PathsListView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem);
            }
            for (int index = 0; index < items.Count; index++)
            {
                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(items[index]);
                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                var target = (CheckBox) myDataTemplate.FindName("chk", myContentPresenter);

                if ((bool) target.IsChecked)
                {
                    itemsToRemove.Add(index);
                }
            }

            for (int i = itemsToRemove.Count -1; i >= 0; i--)
            {
                var index = itemsToRemove[i];
                (_vm).SearchPaths.RemoveAt(index);
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem) child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender,
                                                RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked =
                  e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    if (string.IsNullOrEmpty(header))
                        return;
                    Sort(header, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header 
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }


                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            if (sortBy == "Size")
                sortBy = "Length";
            else if (sortBy == "Folder")
                sortBy = "DirectoryName";
            else if (sortBy == "Created")
                sortBy = "CreationTime";
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(SearchResults.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void DatePickerFrom_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePickerTo == null)
                return;
            DatePickerTo.BlackoutDates.Clear();
            var blackout = new CalendarDateRange(DateTime.MinValue, DatePickerFrom.SelectedDate.Value.AddDays(-1));
            if(DatePickerTo.SelectedDate < DatePickerFrom.SelectedDate)
                DatePickerTo.SelectedDate = DatePickerFrom.SelectedDate;
            DatePickerTo.BlackoutDates.Add(blackout);
        }
    }
}
