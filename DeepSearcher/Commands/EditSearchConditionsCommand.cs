using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DeepSearcher.Models;
using DeepSearcher.ViewModels;

namespace DeepSearcher.Commands
{
    public class EditSearchConditionsCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var vm = parameter as MainViewModel;
            return vm != null && vm.ProgressBarVisibility == Visibility.Hidden;
        }

        public void Execute(object parameter)
        {
            var vm = parameter as MainViewModel;
            var window = new SearchConditionsWindow(vm);
            window.ShowDialog();
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
