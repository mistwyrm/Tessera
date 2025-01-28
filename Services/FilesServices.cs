using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Tessera.Services;

public class FilesService : IFilesService
{
    private readonly Window _target;

    public FilesService(Window target)
    {
        _target = target;
    }

    public async Task<IStorageFile?> OpenFileAsync()
    {
        var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        return files.Count >= 1 ? files[0] : null;
    }

    public async Task<IReadOnlyList<IStorageFile?>> OpenFilesAsync()
    {
        return await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Text File",
            AllowMultiple = true
        });
    }

    public async Task<IStorageFile?> SaveFileAsync()
    {
        return await _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Save Text File"
        });
    }

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc, [Out] out IntPtr ppidl, uint sfgaoIn, [Out] out uint psfgaoOut);

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern void SHOpenWithDialog(IntPtr hwnd, ref OPENASINFO openasinfo);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OPENASINFO
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pcszFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pcszClass;
        [MarshalAs(UnmanagedType.I4)]
        public OPEN_AS_INFO_FLAGS oaifInFlags;
    }

    [Flags]
    public enum OPEN_AS_INFO_FLAGS
    {
        OAIF_ALLOW_REGISTRATION = 0x00000001,   // Enable the "always use this program" checkbox. If not passed, it will be disabled.
        OAIF_REGISTER_EXT = 0x00000002,         // Do the registration after the user hits the OK button.
        OAIF_EXEC = 0x00000004,                 // Execute file after registering.
        OAIF_FORCE_REGISTRATION = 0x00000008,   // Force the Always use this program checkbox to be checked.
        OAIF_HIDE_REGISTRATION = 0x00000020,    // Hide the Always use this program checkbox.
        OAIF_URL_PROTOCOL = 0x00000040,         // The value for the extension that is passed is actually a protocol, so the Open With dialog box should show applications that are registered as capable of handling that protocol.
        OAIF_FILE_IS_URI = 0x00000080           // The location pointed to by the pcszFile parameter is given as a URI.
    }

    public void OpenFileWithDialog(string path)
    {
        IntPtr hwndParent = Process.GetCurrentProcess().MainWindowHandle;
        OPENASINFO openasinfo = new OPENASINFO();
        openasinfo.pcszFile = path;
        openasinfo.pcszClass = String.Empty;
        openasinfo.oaifInFlags = OPEN_AS_INFO_FLAGS.OAIF_EXEC;

        SHOpenWithDialog(IntPtr.Zero, ref openasinfo);
    }

    public void OpenFolderAndSelectFile(string path)
    {
        string folderPath = Path.GetDirectoryName(path);
        string file = Path.GetFileName(path);

        IntPtr ppidlFolder;
        uint psfgaoOut;
        SHParseDisplayName(folderPath, IntPtr.Zero, out ppidlFolder, 0, out psfgaoOut);
        // Check if the folder exists
        if (ppidlFolder == IntPtr.Zero)
        {
            return;
        }

        IntPtr ppidlFile;
        SHParseDisplayName(Path.Combine(folderPath, file), IntPtr.Zero, out ppidlFile, 0, out psfgaoOut);

        IntPtr[] ppidlFiles;
        // Check if the folder exists
        if (ppidlFile == IntPtr.Zero)
        {
            ppidlFiles = new IntPtr[0];
        }
        else
        {
            ppidlFiles = new IntPtr[] { ppidlFile };
        }

        SHOpenFolderAndSelectItems(ppidlFolder, (uint)ppidlFiles.Length, ppidlFiles, 0);

        Marshal.FreeCoTaskMem(ppidlFolder);
        if (ppidlFile != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(ppidlFile);
        }
    }
}