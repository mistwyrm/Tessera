using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tessera.UserControlViewModels;

namespace Tessera.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            CurrentPage = new FileEntitySortingViewModel();
        }

        [ObservableProperty]
        private ViewModelBase _currentPage;
    }
}

