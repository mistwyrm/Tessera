using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tessera.Models
{
    public partial class EntityObject : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _path;

        [ObservableProperty]
        private ObservableCollection<string>? _tags;

        [ObservableProperty]
        private bool _editing = false;
    }
}
