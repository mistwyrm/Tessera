using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Tessera.Models;
using Tessera.Services;
using Tessera.ViewModels;

namespace Tessera.UserControlViewModels
{
    public partial class EntityListUserControlViewModel : ViewModelBase
    {
        [ObservableProperty]
        public ObservableCollection<EntityObject>? _entities;

        [ObservableProperty]
        public EntityObject _selectedEntity;

        [ObservableProperty]
        public bool _displayTags = false;

        IFileEntityRelationsService? FileEntityRelationsService = App.Current.Services.GetService<IFileEntityRelationsService>();

        public EntityListUserControlViewModel()
        {
            UpdateEntityList();
        }

        public event EventHandler<string> EntityDoubleClicked;

        public event EventHandler<string> EntityClicked;


        public void ItemDoubleClicked()
        {
            EntityDoubleClicked?.Invoke(this, String.Empty);
        }

        public void ItemClicked()
        {
            EntityClicked?.Invoke(this, String.Empty);
        }

        public void UpdateEntityList()
        {
            Entities = FileEntityRelationsService.GetEntities();
        }

        internal void AddFilesToEntity(int entityId, IEnumerable<FileObject> files)
        {
            FileEntityRelationsService.AddFilesToEntity(entityId, files);
            UpdateEntityList();
        }

        internal async Task<bool> ToggleEntityEditing()
        {
            SelectedEntity.Editing = true;
            return true;
        }

        internal async Task<bool> UpdateEntity()
        {
            SelectedEntity.Editing = false;
            FileEntityRelationsService.UpdateEntity(SelectedEntity);
            return true;
        }
    }

}
