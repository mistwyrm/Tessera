using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Tessera.Models;

namespace Tessera.Services
{
    internal interface IFileEntityRelationsService
    {
        public Task<bool> ImportFiles(IReadOnlyList<IStorageFile> files, int? entityId);

        public Task<bool> ImportFiles(IEnumerable<IStorageItem> files, int? entityId);

        public void UpdateFile(FileObject file);

        public bool AddFilesToEntity(int? entityId, IEnumerable<FileObject> files);

        public void CreateEntity(string name);

        public ObservableCollection<FileObject> GetFiles(int? entityId);

        public ObservableCollection<EntityObject> GetEntities();

        public EntityObject GetEntityFromId(int entityId);
        public void AddTagToEntity(int id, string newTag);
        void UpdateEntity(EntityObject selectedEntity);
    }
}
