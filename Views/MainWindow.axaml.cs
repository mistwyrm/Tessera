using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;

namespace Tessera.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.PointerPressed += Window_PointerPressed;
        }

        private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            this.Focus();
        }
    }
}