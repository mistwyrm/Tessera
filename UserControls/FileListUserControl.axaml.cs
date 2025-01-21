using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Tessera.Models;
using Tessera.UserControlViewModels;
using Tessera.Utilities;
using Tessera.ViewModels;

namespace Tessera.UserControls
{
    public partial class FileListUserControl : UserControl
    {
        public FileListUserControl()
        {
            DataContext = new FileListUserControlViewModel();
            InitializeComponent();
            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);
            KeyDown += View_KeyDown;
        }

        async void Drop(object? sender, DragEventArgs e)
        {
            if (e.Source is Control c
                && c.FindAncestorOfType<CustomListBox>() != null)
            {
                // Drag from another CustomListBox
                if (e.Data.Contains(Constants.ListBoxItemFormat)
                    && c.FindAncestorOfType<CustomListBox>() != e.Data.Get(Constants.DragSourceFormat))
                {
                    if (e.Data.Get(Constants.ListBoxItemFormat) is IList ilist)
                    {
                        ((FileListUserControlViewModel)DataContext).AddFilesToEntity(((FileListUserControlViewModel)DataContext).EntityId, ilist.OfType<FileObject>());
                    }
                }
                // Drag from file explorer
                else if (e.Data.Contains(DataFormats.Files))
                {
                   ((FileListUserControlViewModel)DataContext).AddFilesOnDrag((IEnumerable<IStorageItem>)e.Data.Get(DataFormats.Files));
                }
            }
        }

        void DragOver(object? sender, DragEventArgs e)
        {
            if ((e.Data.Contains(Constants.ListBoxItemFormat) || e.Data.Contains(DataFormats.Files))
                && e.Source is Control c
                && c.FindAncestorOfType<CustomListBox>() != null
                && c.FindAncestorOfType<CustomListBox>() != e.Data.Get(Constants.DragSourceFormat))
            {
                e.DragEffects = DragDropEffects.Move;
            }
            else
            {
                e.DragEffects = e.DragEffects & (DragDropEffects.None);
            }
        }

        public void SearchClick(object sender, RoutedEventArgs args)
        {
            SearchQuery.Focus();
        }

        public async void RemoveFilesFromEntity(object sender, RoutedEventArgs args)
        {
            if (FileListBox.SelectedItems.Count != 0)
            {
                await ((FileListUserControlViewModel)DataContext).RemoveFilesFromEntity(FileListBox.SelectedItems);
            }

            if (this.DataContext is ViewModelBase ViewModel)
            {
                ViewModel.RefreshData();
            }
        }

        public async void RenameFile(object sender, RoutedEventArgs args)
        {
            ((FileListUserControlViewModel)DataContext).ToggleFileEditing();
        }

        private void FileNameTextBox_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox &&
                textBox.Text is not null &&
                textBox.FindAncestorOfType<CustomListBox>() != null && 
                e.Property == TextBox.IsVisibleProperty && 
                !(bool)e.OldValue
                && (bool)e.NewValue)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        public void DeleteFiles(object sender, RoutedEventArgs args)
        {
            
        }

        private void FileOnClick(object? sender, TappedEventArgs e)
        {
            ((FileListUserControlViewModel)DataContext).ItemClicked();
        }

        private void FileNameTextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            if (((FileListUserControlViewModel)DataContext).SelectedFile.Editing)
            {
                ((FileListUserControlViewModel)DataContext).UpdateFile();
            }
        }

        private void FileNameTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ((FileListUserControlViewModel)DataContext).SelectedFile.Editing)
            {
                ((FileListUserControlViewModel)DataContext).UpdateFile();
            }

            // Disable standard keyboard shortcuts for list box while renaming
            else if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Tab && ((FileListUserControlViewModel)DataContext).SelectedFile.Editing)
            {
                e.Handled = true;
            }
        }

        private void View_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 && !((FileListUserControlViewModel)DataContext).SelectedFile.Editing)
            {
                ((FileListUserControlViewModel)DataContext).ToggleFileEditing();
            }
        }
    }
}