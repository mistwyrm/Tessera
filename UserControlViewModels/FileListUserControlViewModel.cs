using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Tessera.Models;
using Tessera.Services;
using Tessera.ViewModels;

namespace Tessera.UserControlViewModels
{
    public partial class FileListUserControlViewModel : ViewModelBase
    {
        /// <summary>
        /// Files displayed in UserControl
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FileObject>? _files;

        /// <summary>
        /// Files retrieved from the database
        /// </summary>
        private ObservableCollection<FileObject>? DatabaseFiles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty]
        private string? _searchQuery;

        /// <summary>
        /// EntityId to get file list for. If null returns all files with no entity.
        /// </summary>
        public int? EntityId { get; set; }

        /// <summary>
        /// If an entity is given to filter files, the entity's name
        /// </summary>
        [ObservableProperty]
        public string? _entityName;

        /// <summary>
        /// The currently selected file
        /// </summary>
        [ObservableProperty]
        public FileObject _selectedFile;

        /// <summary>
        /// Display a button to navigate back to the previous page
        /// </summary>
        [ObservableProperty]
        public bool _displayBackButton = false;

        IFileEntityRelationsService FileEntityRelationsService = App.Current?.Services?.GetService<IFileEntityRelationsService>();

        public event EventHandler<string>? BackButtonClicked;
        public event EventHandler<string>? RefreshDataTriggered;
        public event EventHandler<string>? FileClicked;

        [RelayCommand]
        private void BackButtonOnClick()
        {
            BackButtonClicked?.Invoke(this, String.Empty);
        }

        public void ItemClicked()
        {
            FileClicked?.Invoke(this, String.Empty);
        }



        public void OpenFileLocation()
        {
            IFilesService FilesService = App.Current?.Services?.GetService<IFilesService>();
            if (File.Exists(SelectedFile.Path))
            {
                if (File.Exists(SelectedFile.Path))
                {
                    FilesService.OpenFolderAndSelectFile(SelectedFile.Path);
                }
            }
            else
            {
                /// Add error message if file does not exist
            }
        }

        public void OpenFileWith()
        {
            IFilesService FilesService = App.Current?.Services?.GetService<IFilesService>();
            if (File.Exists(SelectedFile.Path))
            {
                FilesService.OpenFileWithDialog(SelectedFile.Path);
            }
            else
            {
                /// Add error message if file does not exist
            }
        }

        public void OpenFile()
        {
            IFilesService FilesService = App.Current?.Services?.GetService<IFilesService>();
            if (File.Exists(SelectedFile.Path)) {
                Process.Start(new ProcessStartInfo(){
                    FileName = SelectedFile.Path,
                    UseShellExecute = true
                });
            }
            else
            {
                /// Add error message if file does not exist
            }
        }



        public FileListUserControlViewModel()
        {
            if (FileEntityRelationsService is null)
                throw new NullReferenceException("Missing FileEntityRelationsService instance.");
            DatabaseFiles = FileEntityRelationsService.GetFiles(EntityId);
            Files = DatabaseFiles;
        }

        public FileListUserControlViewModel(int entityId)
        {
            this.EntityId = entityId;
            if (FileEntityRelationsService is null)
                throw new NullReferenceException("Missing FileEntityRelationsService instance.");
            EntityName = FileEntityRelationsService.GetEntityFromId(entityId).Name;
            RefreshData();
        }

        public override void RefreshData()
        {
            StackTrace stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                MethodBase method = stackTrace.GetFrame(i).GetMethod();
                string className = method.DeclaringType != null ? method.DeclaringType.Name : "Unknown Class";
                string methodName = method.Name;
            }

            if (FileEntityRelationsService != null)
            {
                DatabaseFiles = FileEntityRelationsService.GetFiles(EntityId);

                // Filter files based on search query
                if (SearchQuery != null)
                {
                    Files = new ObservableCollection<FileObject>(DatabaseFiles.Where(i => i.Name.ToLower().Contains(SearchQuery.ToLower())));
                }
                else
                {
                    Files = DatabaseFiles;
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(SearchQuery) && DatabaseFiles != null)
            {
                if (SearchQuery != null)
                {
                    Files = new ObservableCollection<FileObject>(DatabaseFiles.Where(i => i.Name.Contains(SearchQuery)));
                }
            }
        }

        internal void AddFilesToEntity(int? entityId, IEnumerable<FileObject> files)
        {
            if (FileEntityRelationsService != null)
            {
                FileEntityRelationsService.AddFilesToEntity(entityId, files);
            }
            
            RefreshData();
        }

        internal async Task<bool> RemoveFilesFromEntity(IList files)
        {
            if (FileEntityRelationsService != null)
            {
                FileEntityRelationsService.AddFilesToEntity(null, files.OfType<FileObject>());
            }
            RefreshDataTriggered?.Invoke(this, String.Empty);
            return true;
        }

        internal async Task<bool> ToggleFileEditing()
        {
            SelectedFile.Editing = true;
            return true;
        }

        internal async Task<bool> UpdateFile()
        {
            SelectedFile.Editing = false;
            if (FileEntityRelationsService is null)
                throw new NullReferenceException("Missing FileEntityRelationsService instance.");
            FileEntityRelationsService.UpdateFile(SelectedFile);
            return true;
        }


        [RelayCommand]
        public async Task AddFilesOnClick()
        {
            IFilesService FilesService = App.Current?.Services?.GetService<IFilesService>();
            if (FilesService is null) 
                throw new NullReferenceException("Missing FilesService instance.");
            if (FileEntityRelationsService is null) 
                throw new NullReferenceException("Missing FileEntityRelationsService instance.");

            IReadOnlyList<IStorageFile> files = await FilesService.OpenFilesAsync();
            await FileEntityRelationsService.ImportFiles(files, EntityId);
            RefreshData();
        }

        public async void AddFilesOnDrag(IEnumerable<IStorageItem> files)
        {
            if (FileEntityRelationsService is null)
                throw new NullReferenceException("Missing FileEntityRelationsService instance.");
            await FileEntityRelationsService.ImportFiles(files, EntityId);
        }

        [RelayCommand]
        public async Task DeleteFiles()
        {

        }
    }

}
