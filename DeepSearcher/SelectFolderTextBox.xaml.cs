﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DeepSearcher
{
    /// <summary>
    /// Interaction logic for SelectFolderTextBox.xaml
    /// </summary>
    public partial class SelectFolderTextBox : TextBox
    {
        private Popup Popup
        {
            get
            {
                return this.Template.FindName("PART_Popup", this) as Popup;
            }
        }

        private ListBox ItemList
        {
            get
            {
                return this.Template.FindName("PART_ItemList", this) as ListBox;
            }
        }

        private Grid Root
        {
            get
            {
                return this.Template.FindName("root", this) as Grid;
            }
        }

        //12-25-08 : Add Ghost image when picking from ItemList
        //TextBlock TempVisual { get { return this.Template.FindName("PART_TempVisual", this) as TextBlock; } }
        private ScrollViewer Host
        {
            get
            {
                return this.Template.FindName("PART_ContentHost", this) as ScrollViewer;
            }
        }

        private UIElement TextBoxView
        {
            get
            {
                foreach (object o in LogicalTreeHelper.GetChildren(Host))
                    return o as UIElement;
                return null;
            }
        }

        private bool _loaded = false;
        private string lastPath;

        public SelectFolderTextBox()
        {
            InitializeComponent();
        }


        private bool prevState = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _loaded = true;
            this.KeyDown += new KeyEventHandler(AutoCompleteTextBox_KeyDown);
            this.PreviewKeyDown += new KeyEventHandler(AutoCompleteTextBox_PreviewKeyDown);
            ItemList.PreviewMouseDown += new MouseButtonEventHandler(ItemList_PreviewMouseDown);
            ItemList.KeyDown += new KeyEventHandler(ItemList_KeyDown);
            //TempVisual.MouseDown += new MouseButtonEventHandler(TempVisual_MouseDown);
            //09-04-09 Based on SilverLaw's approach 
            Popup.CustomPopupPlacementCallback += new CustomPopupPlacementCallback(Repositioning);


            Window parentWindow = getParentWindow();
            if (parentWindow != null)
            {
                parentWindow.Deactivated += delegate
                {
                    prevState = Popup.IsOpen;
                    Popup.IsOpen = false;
                };
                parentWindow.Activated += delegate { Popup.IsOpen = prevState; };
            }


        }

        private Window getParentWindow()
        {
            DependencyObject d = this;
            while (d != null && !(d is Window))
                d = LogicalTreeHelper.GetParent(d);
            return d as Window;
        }

        //09-04-09 Based on SilverLaw's approach 
        private CustomPopupPlacement[] Repositioning(Size popupSize, Size targetSize, Point offset)
        {
            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(new Point((0.01 - offset.X), (Root.ActualHeight - offset.Y)), PopupPrimaryAxis.None)
            };
        }

        private void TempVisual_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string text = Text;
            ItemList.SelectedIndex = -1;
            Text = text;
            Popup.IsOpen = false;
        }

        private void AutoCompleteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //12-25-08 - added PageDown Support
            if (ItemList.Items.Count > 0 && !(e.OriginalSource is ListBoxItem))
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.Down:
                    case Key.Prior:
                    case Key.Next:
                        ItemList.Focus();
                        ItemList.SelectedIndex = 0;
                        ListBoxItem lbi = ItemList.ItemContainerGenerator.ContainerFromIndex(ItemList.SelectedIndex) as ListBoxItem;
                        lbi.Focus();
                        e.Handled = true;
                        break;
                    case Key.Escape:
                        Popup.IsOpen = false;
                        e.Handled = true;
                        break;

                }
        }


        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is ListBoxItem)
            {

                ListBoxItem tb = e.OriginalSource as ListBoxItem;

                e.Handled = true;
                switch (e.Key)
                {
                    case Key.Enter:
                        Text = (tb.Content as string);
                        updateSource();
                        break;
                        //12-25-08 - added "\" support when picking in list view
                    case Key.Oem5:
                        Text = (tb.Content as string) + "\\";
                        e.Handled = false;
                        break;
                        //12-25-08 - roll back if escape is pressed
                    case Key.Escape:
                        Text = lastPath.TrimEnd('\\') + "\\";
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
                //12-25-08 - Force focus back the control after selected.
                if (e.Handled)
                {
                    Keyboard.Focus(this);
                    Popup.IsOpen = false;
                    this.Select(Text.Length, 0); //Select last char
                }
            }
        }


        private void AutoCompleteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Popup.IsOpen = false;
                updateSource();
            }
        }

        private void updateSource()
        {
            if (this.GetBindingExpression(TextBox.TextProperty) != null)
                this.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void ItemList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TextBlock tb = e.OriginalSource as TextBlock;
                if (tb != null)
                {
                    Text = tb.Text;
                    updateSource();
                    Popup.IsOpen = false;
                    e.Handled = true;
                }
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (e.Changes.ToList()[0].AddedLength == Text.Length)
            {
                return;
            }

            if (_loaded)
            {
                try
                {
                    //if (lastPath != FullName.GetDirectoryName(this.Text))
                    //if (textBox.Text.EndsWith("\\"))                        
                    {
                        lastPath = Text;
                        string[] paths = Lookup(this.Text);

                        ItemList.Items.Clear();
                        foreach (string path in paths)
                            if (!(String.Equals(path, this.Text, StringComparison.CurrentCultureIgnoreCase)))
                                ItemList.Items.Add(path);
                    }


                    Popup.IsOpen = ItemList.Items.Count > 0;

                    //ItemList.Items.Filter = p =>
                    //{
                    //    string path = p as string;
                    //    return path.StartsWith(this.Text, StringComparison.CurrentCultureIgnoreCase) &&
                    //        !(String.Equals(path, this.Text, StringComparison.CurrentCultureIgnoreCase));
                    //};
                }
                catch
                {

                }
            }
        }


        private string[] Lookup(string path)
        {
            try
            {
                bool fullDirectoryExists = Directory.Exists(path);
                if (fullDirectoryExists || Directory.Exists(Path.GetDirectoryName(path)))
                {
                    DirectoryInfo lookupFolder;
                    lookupFolder = fullDirectoryExists ? new DirectoryInfo(path) : new DirectoryInfo(Path.GetDirectoryName(path));

                    if (lookupFolder != null)
                    {
                        DirectoryInfo[] AllItems = lookupFolder.GetDirectories();
                        List<string> list = new List<string>();
                        foreach (DirectoryInfo di in AllItems)
                        {
                            FileAttributes attributes = File.GetAttributes(di.FullName);
                            if ((attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                            {
                                if (di.FullName.StartsWith(path, StringComparison.CurrentCultureIgnoreCase))
                                    list.Add(di.FullName);
                            }
                        }
                        return list.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new string[0];
        }
    }
}
