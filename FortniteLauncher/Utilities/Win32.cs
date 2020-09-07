using System;
using System.Runtime.InteropServices;

public static class Win32
{
    /*
    TERMINATE = 0x0001
    SUSPEND_RESUME = 0x0002
    GET_CONTEXT = 0x0008
    SET_CONTEXT = 0x0010
    SET_INFORMATION = 0x0020
    QUERY_INFORMATION = 0x0040
    SET_THREAD_TOKEN = 0x0080
    IMPERSONATE = 0x0100
    DIRECT_IMPERSONATION = 0x0200
    */

    [DllImport("kernel32.dll")]
    public static extern Int32 SuspendThread(IntPtr hThread);
    [DllImport("kernel32.dll")]
    public static extern int ResumeThread(IntPtr hThread);
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hHandle);
}
