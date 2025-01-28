using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Tessera.Services;

public interface IFilesService
{
    public Task<IReadOnlyList<IStorageFile?>> OpenFilesAsync();
    public Task<IStorageFile?> SaveFileAsync();

    public void OpenFolderAndSelectFile(string path);

    public void OpenFileWithDialog(string path);
}