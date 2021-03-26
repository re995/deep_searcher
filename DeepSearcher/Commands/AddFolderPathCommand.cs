using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DeepSearcher.ViewModels;

namespace DeepSearcher.Commands
{
    public class AddFolderPathCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var vm = parameter as MainViewModel;
            var window = new SelectFolderPathWindow(vm.SearchPaths.Select(p => p.Path).ToList());
            window.ShowDialog();
            if (window.FolderPathViewModel.FolderPath == null)
                return;
            vm.SearchPaths.Add(new PathItem(window.FolderPathViewModel.FolderPath, true));
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
