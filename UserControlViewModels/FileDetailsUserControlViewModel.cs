using CommunityToolkit.Mvvm.ComponentModel;
using Tessera.Models;
using Tessera.ViewModels;

namespace Tessera.UserControlViewModels
{
    public partial class FileDetailsUserControlViewModel : ViewModelBase
    {
        [ObservableProperty]
        private FileObject _file;

        public FileDetailsUserControlViewModel(FileObject file)
        {
            File = file;
        }
    }

}
