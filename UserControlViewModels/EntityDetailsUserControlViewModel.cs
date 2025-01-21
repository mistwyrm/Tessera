using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Tessera.Models;
using Tessera.Services;
using Tessera.ViewModels;

namespace Tessera.UserControlViewModels
{
    public partial class EntityDetailsUserControlViewModel : ViewModelBase
    {
        IFileEntityRelationsService? FileEntityRelationsService = App.Current.Services.GetService<IFileEntityRelationsService>();

        [ObservableProperty]
        private bool _addingTag = false;

        [ObservableProperty]
        private EntityObject _entity;

        public EntityDetailsUserControlViewModel(EntityObject entity)
        {
            Entity = entity;
        }

        public void AddTagButtonClicked()
        {
            AddingTag = true;
        }

        public void AddTag(string NewTag)
        {
            if (!Entity.Tags.Contains(NewTag) && NewTag is not null)
            {
                FileEntityRelationsService.AddTagToEntity(Entity.Id, NewTag);
                Entity.Tags.Add(NewTag);
            }
            AddingTag = false;
        }
    }

}
