using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LZTools
{
    public static class LZInject
    {
        // Windows API函数
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        // 常量
        private const int PROCESS_CREATE_THREAD = 0x0002;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_VM_READ = 0x0010;
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint PAGE_READWRITE = 0x04;

        /// <summary>
        /// 向指定进程注入DLL
        /// </summary>
        /// <param name="processId">目标进程ID</param>
        /// <param name="dllPath">DLL路径</param>
        /// <returns>是否注入成功</returns>
        public static bool inDll(int processId, string dllPath)
        {
            // 打开目标进程
            IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, processId);
            if (hProcess == IntPtr.Zero)
            {
                Console.WriteLine("无法打开目标进程。错误代码: " + Marshal.GetLastWin32Error());
                return false;
            }

            // 在目标进程中分配内存
            IntPtr pAllocatedMemory = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (pAllocatedMemory == IntPtr.Zero)
            {
                Console.WriteLine("无法在目标进程中分配内存。错误代码: " + Marshal.GetLastWin32Error());
                CloseHandle(hProcess);
                return false;
            }

            // 将DLL路径写入目标进程的内存
            byte[] dllPathBytes = Encoding.Unicode.GetBytes(dllPath);
            if (!WriteProcessMemory(hProcess, pAllocatedMemory, dllPathBytes, (uint)dllPathBytes.Length, out int bytesWritten))
            {
                Console.WriteLine("无法写入目标进程内存。错误代码: " + Marshal.GetLastWin32Error());
                CloseHandle(hProcess);
                return false;
            }

            // 获取LoadLibraryW函数的地址
            IntPtr pLoadLibrary = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW");
            if (pLoadLibrary == IntPtr.Zero)
            {
                Console.WriteLine("无法获取LoadLibraryW函数的地址。错误代码: " + Marshal.GetLastWin32Error());
                CloseHandle(hProcess);
                return false;
            }
            
            // 创建远程线程来加载DLL
            IntPtr hRemoteThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, pLoadLibrary, pAllocatedMemory, 0, out uint threadId);
            if (hRemoteThread == IntPtr.Zero)
            {
                Console.WriteLine("无法创建远程线程。错误代码: " + Marshal.GetLastWin32Error());
                CloseHandle(hProcess);
                return false;
            }

            Console.WriteLine("DLL注入成功！线程ID: " + threadId);

            // 关闭句柄
            CloseHandle(hRemoteThread);
            CloseHandle(hProcess);

            return true;
        }
    }
}
