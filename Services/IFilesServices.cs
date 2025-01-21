using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Tessera.Services;

public interface IFilesService
{
    public Task<IReadOnlyList<IStorageFile?>> OpenFilesAsync();
    public Task<IStorageFile?> SaveFileAsync();
}