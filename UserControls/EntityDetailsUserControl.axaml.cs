using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Tessera.UserControlViewModels;

namespace Tessera.UserControls
{
    public partial class EntityDetailsUserControl : UserControl
    {
        public EntityDetailsUserControl()
        {
            InitializeComponent();
        }

        private void AddTagButton_Click(object? sender, RoutedEventArgs e)
        {
            ((EntityDetailsUserControlViewModel)DataContext).AddTagButtonClicked();
        }

        private void NewTagTextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            if (((EntityDetailsUserControlViewModel)DataContext).AddingTag)
            {
                ((EntityDetailsUserControlViewModel)DataContext).AddTag(NewTagTextBox.Text);
                NewTagTextBox.Text = null;
            }
        }

        private void NewTagTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ((EntityDetailsUserControlViewModel)DataContext).AddingTag)
            {
                ((EntityDetailsUserControlViewModel)DataContext).AddTag(NewTagTextBox.Text);
                NewTagTextBox.Text = null;
            }

        }

        private void NewTagTextBox_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox &&
                e.Property == TextBox.IsVisibleProperty &&
                !(bool)e.OldValue
                && (bool)e.NewValue)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }
    }
}