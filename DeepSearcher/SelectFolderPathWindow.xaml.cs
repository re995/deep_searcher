using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using DeepSearcher.ViewModels;
using Ookii.Dialogs.Wpf;

namespace DeepSearcher
{
    /// <summary>
    /// Interaction logic for SelectFolderPathWindow.xaml
    /// </summary>
    public partial class SelectFolderPathWindow : Window
    {
        public SelectFolderPathViewModel FolderPathViewModel { get; private set; }

        public IList<string> CurrentPaths { get; set; }

        public SelectFolderPathWindow(IList<string> currentPaths)
        {
            InitializeComponent();
            FolderPathViewModel = Resources["FolderPathViewModel"] as SelectFolderPathViewModel;
            CurrentPaths = currentPaths;
        }

        private void FolderPathDialog_OnClosing(object sender, CancelEventArgs e)
        {
        }

        private bool HandlePathAlreadyExists()
        {
            if (CurrentPaths.Contains(FolderPathViewModel.FolderPath))
            {
                return TaskDialog.OSSupportsTaskDialogs ? ShowVistaDialog("The selected path already configured",
                    "The selected path already configured", "Do you want to discard it?", "This will discard the path you chose",
                    "You will be returned to the dialog.\nThis time choose a path that isn't already configured") :
                    ShowRegularDialog("The selected path already configured.\nDo you want to discard it?\nChoose \"Yes\" to discard it\nChoose \"No\" to be returned to the dialog. This time choose a path that isn't already configured", "The selected path already configured");
            }
            return false;
        }

        private bool HandlePathInvalid()
        {
            if (!Directory.Exists(FolderPathViewModel.FolderPath))
            {
                return TaskDialog.OSSupportsTaskDialogs ? ShowVistaDialog("The path entered is invalid", "The path entered is invalid",
                    "Do you want to discard it?", "This will discard the path you chose",
                    "You will be returned to the dialog.\nThis time choose a valid path")
                    : ShowRegularDialog("The path entered is invalid.\nDo you want to discard it?\nChoose \"Yes\" to discard it\nChoose \"No\" to be returned to the dialog. This time choose a valid path", "The path entered is invalid");
            }

            return false;

        }

        private bool ShowVistaDialog(string title,string mainInstruction,string content,string yesNote,string noNote)
        {
            using (var dialog = new TaskDialog())
            {
                dialog.WindowTitle = title;
                dialog.MainInstruction = mainInstruction;
                dialog.Content = content;
                dialog.ButtonStyle = TaskDialogButtonStyle.CommandLinks;
                var yesButton = new TaskDialogButton("Yes");
                yesButton.CommandLinkNote = yesNote;
                var noButton = new TaskDialogButton("No");
                noButton.CommandLinkNote = noNote;
                dialog.Buttons.Add(yesButton);
                dialog.Buttons.Add(noButton);
                TaskDialogButton result = dialog.ShowDialog(this);
                if (result == yesButton)
                {
                    FolderPathViewModel.FolderPath = null;
                    return false;
                }
                if (result == noButton)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ShowRegularDialog(string content,string title)
        {
            MessageBoxResult result = MessageBox.Show(content, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                FolderPathViewModel.FolderPath = null;
                return false;
            }
            if (result == MessageBoxResult.No)
            {
                return true;
            }

            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool cancel = HandlePathAlreadyExists() || HandlePathInvalid();
            if (!cancel)
                Close();
        }
    }
}
