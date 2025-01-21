using CommunityToolkit.Mvvm.ComponentModel;
using Tessera.UserControlViewModels;

namespace Tessera.ViewModels
{
    public partial class FileEntitySortingViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _tab3Content;
        [ObservableProperty]
        private ViewModelBase _tab2Content;
        [ObservableProperty]
        private ViewModelBase _tab1Content;

        public FileEntitySortingViewModel()
        {
            Tab1Content = new FileListUserControlViewModel();
            ((FileListUserControlViewModel)Tab1Content).FileClicked += FileSelected;

            DisplayEntityList(this, null);
        }



        public void DisplayEntityDetails(object? sender, string? e)
        {
            Tab2Content = new FileListUserControlViewModel(((EntityListUserControlViewModel)Tab2Content).SelectedEntity.Id);
            ((FileListUserControlViewModel)Tab2Content).DisplayBackButton = true;
            ((FileListUserControlViewModel)Tab2Content).BackButtonClicked += DisplayEntityList;
            ((FileListUserControlViewModel)Tab2Content).RefreshDataTriggered += RefreshData;
            ((FileListUserControlViewModel)Tab2Content).FileClicked += FileSelected;
        }

        public void DisplayEntityList(object? sender, string? e)
        {
            Tab2Content = new EntityListUserControlViewModel();
            ((EntityListUserControlViewModel)Tab2Content).EntityDoubleClicked += DisplayEntityDetails;
            ((EntityListUserControlViewModel)Tab2Content).EntityClicked += EntitySelected;
        }

        public override void RefreshData()
        {
            RefreshData(this, null);
        }

        public void RefreshData(object? sender, string? e)
        {
            Tab1Content.RefreshData();
            Tab2Content.RefreshData();
        }

        public void EntitySelected(object? sender, string? e)
        {
            Tab3Content = new EntityDetailsUserControlViewModel(((EntityListUserControlViewModel)Tab2Content).SelectedEntity);
        }

        public void FileSelected(object? sender, string? e)
        {
            Tab3Content = new FileDetailsUserControlViewModel(((FileListUserControlViewModel)sender).SelectedFile);
        }
    }

}
