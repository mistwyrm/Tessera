using CommunityToolkit.Mvvm.ComponentModel;

namespace Tessera.Models
{
    public partial class FileObject : ObservableObject
    {
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _path;

        [ObservableProperty]
        private bool _editing = false;
    }
}
