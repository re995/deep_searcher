using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DeepSearcher.Models;
using DeepSearcher.ViewModels;

namespace DeepSearcher.Commands
{
    public class AddEmptySearchConditionCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var vm = parameter as MainViewModel;
            vm.SearchConditions.Add(new SearchConditionViewModel(SearchCondition.Empty));
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
