using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DeepSearcher.ViewModels;
using Ookii.Dialogs.Wpf;

namespace DeepSearcher.Commands
{
    public class ChooseFolderCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var vm = parameter as SelectFolderPathViewModel;
            var dialog = new VistaFolderBrowserDialog();
            if (Directory.Exists(vm.FolderPath))
                dialog.SelectedPath = vm.FolderPath;
            bool? result = dialog.ShowDialog();
            if (result != null && (bool) result)
            {
                vm.FolderPath = dialog.SelectedPath;
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
    }
}
