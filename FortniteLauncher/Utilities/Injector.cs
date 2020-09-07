using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public static class Injector
{
    public static void InjectDll(int processId, string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("DLL not found");
            return;
        }
        var handle = Win32.OpenProcess(2 | 0x0400 | 8 | 0x0020 | 0x0010, false, processId);

        var loadLibrary = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

        var size = (uint)((path.Length + 1) * Marshal.SizeOf(typeof(char)));
        var address = Win32.VirtualAllocEx(handle, IntPtr.Zero, size, 0x1000 | 0x2000, 4);

        Win32.WriteProcessMemory(handle, address, Encoding.Default.GetBytes(path), size, out UIntPtr bytesWritten);

        Win32.CreateRemoteThread(handle, IntPtr.Zero, 0, loadLibrary, address, 0, IntPtr.Zero);
    }
}
