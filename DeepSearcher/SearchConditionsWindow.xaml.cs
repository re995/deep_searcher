using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DeepSearcher.Models;
using DeepSearcher.Models.TypeConverters;
using DeepSearcher.ViewModels;

namespace DeepSearcher
{
    /// <summary>
    /// Interaction logic for SearchConditionsWindow.xaml
    /// </summary>
    public partial class SearchConditionsWindow : Window
    {
        public SearchConditionsWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
            DataGrid.ItemsSource = mainViewModel.SearchConditions;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = new List<SearchConditionViewModel>();
            foreach (SearchConditionViewModel item in DataGrid.ItemsSource)
            {
                if (item.Selected)
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (var item in itemsToRemove)
            {
                (DataContext as MainViewModel).SearchConditions.Remove(item);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
