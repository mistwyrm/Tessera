using System.Collections;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Tessera.Models;
using Tessera.UserControlViewModels;
using Tessera.Utilities;

namespace Tessera.UserControls
{
    public partial class EntityListUserControl : UserControl
    {

        public EntityListUserControl()
        {
            DataContext = new EntityListUserControlViewModel();
            InitializeComponent();

            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);
            KeyDown += View_KeyDown;
        }

        private void EntityOnDoubleClick(object? sender, TappedEventArgs e)
        {
            ((EntityListUserControlViewModel)DataContext).ItemDoubleClicked();
        }

        private void EntityOnClick(object? sender, TappedEventArgs e)
        {
            ((EntityListUserControlViewModel)DataContext).ItemClicked();
        }

        async void Drop(object? sender, DragEventArgs e)
        {
            if (e.Source is Control c && c.DataContext is EntityObject entity)
            {
                if (e.Data.Contains(Constants.ListBoxItemFormat))
                {
                    if (e.Data.Get(Constants.ListBoxItemFormat) is IList ilist)
                    {
                        ((EntityListUserControlViewModel)DataContext).AddFilesToEntity(entity.Id, ilist.OfType<FileObject>());
                    }
                }
                else if (e.Data.Contains(DataFormats.Files))
                {
                    Trace.WriteLine(e.Data.Get(DataFormats.Files));
                }
            }
        }

        void DragOver(object? sender, DragEventArgs e)
        {

            if ((e.Data.Contains(Constants.ListBoxItemFormat) || e.Data.Contains(DataFormats.Files)) && e.Source is Control c && c.DataContext is EntityObject entity)
            {
                e.DragEffects = e.DragEffects & (DragDropEffects.Move);
            }
            else
            {
                e.DragEffects = e.DragEffects & (DragDropEffects.None);
            }
        }

        public async void RenameFile(object sender, RoutedEventArgs args)
        {
            ((EntityListUserControlViewModel)DataContext).ToggleEntityEditing();
        }

        private void EntityNameTextBox_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
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

        private void EntityNameTextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            if (((EntityListUserControlViewModel)DataContext).SelectedEntity.Editing)
            {
                ((EntityListUserControlViewModel)DataContext).UpdateEntity();
            }
        }

        private void EntityNameTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ((EntityListUserControlViewModel)DataContext).SelectedEntity.Editing)
            {
                ((EntityListUserControlViewModel)DataContext).UpdateEntity();
            }

            // Disable standard keyboard shortcuts for list box while renaming
            else if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Tab && ((FileListUserControlViewModel)DataContext).SelectedFile.Editing)
            {
                e.Handled = true;
            }
        }

        private void View_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 && !((EntityListUserControlViewModel)DataContext).SelectedEntity.Editing)
            {
                ((EntityListUserControlViewModel)DataContext).ToggleEntityEditing();
            }
        }

        public void DeleteEntities(object sender, RoutedEventArgs args)
        {

        }
    }
}