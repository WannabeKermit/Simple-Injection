using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Simple_Injection.Etc
{
    public static class Native
    {
        #region pinvoke
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryAllocation flAllocationType, MemoryProtection flProtect);
        
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, uint lpNumberOfBytesWritten);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, int dwThreadId);
        
        [DllImport("kernel32.dll")]
        public static extern void SuspendThread(IntPtr hThread);
        
        [DllImport("kernel32.dll")]
        public static extern bool GetThreadContext(IntPtr hThread, ref Context lpContext);
        
        // x64 Overload for GetThreadContext
        
        [DllImport("kernel32.dll")] 
        public static extern bool GetThreadContext(IntPtr hThread, ref Context64 lpContext);
        
        [DllImport("kernel32.dll")]
        public static extern bool SetThreadContext(IntPtr hThread, ref Context lpContext);
        
        // x64 Overload for SetThreadContext
        
        [DllImport("kernel32.dll")]
        public static extern bool SetThreadContext(IntPtr hThread, ref Context64 lpContext);
        
        [DllImport("kernel32.dll")]
        public static extern void ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern bool QueueUserAPC(IntPtr pfnAPC, IntPtr hThread, IntPtr dwData);
        
        [DllImport("ntdll.dll")]
        public static extern void RtlCreateUserThread(IntPtr hProcess, IntPtr lpThreadSecurity, bool bCreateSuspended, uint dwStackZeroBits, IntPtr pStackReserved, IntPtr pStackCommit, IntPtr pStartAddress, IntPtr pStartParameter, out IntPtr hThread, IntPtr pClientId);
        
        [DllImport("kernel32.dll")]
        public static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryInformation lpBuffer, int dwLength);
        
        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);
        
        [DllImport("kernel32.dll")]
        public static extern void WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        
        [DllImport("kernel32.dll")]
        public static extern void CloseHandle(IntPtr hHandle);
        
        [DllImport("kernel32.dll")]
        public static extern void VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryAllocation dwFreeType);
        
        [DllImport("user32.dll")]
        public static extern void PostMessage(IntPtr hWnd, WindowsMessage dwMsg, IntPtr wParam, IntPtr lParam);

        #endregion
        
        #region Permissions
        
        public enum MemoryAllocation
        {
            AllAccess = 0x3000,
            Release = 0x8000
        }

        public enum MemoryProtection
        {
            PageExecuteReadWrite = 0x40
        }

        public enum ThreadAccess
        {
            AllAccess = 0x1A
        }

        public enum Flags
        {
            ContextControl = 0x10001
        }

        public enum WindowsMessage
        {
            WmKeydown = 0x100
        }
        
        #endregion
        
        #region Structures
        
        [StructLayout(LayoutKind.Sequential)]
        private struct FloatingSaveArea
        {
            private readonly uint ControlWord; 
            private readonly uint StatusWord; 
            private readonly uint TagWord; 
            
            private readonly uint ErrorOffset; 
            private readonly uint ErrorSelector; 
            
            private readonly uint DataOffset;
            private readonly uint DataSelector; 
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)] 
            private readonly byte[] RegisterArea; 
            
            private readonly uint Cr0NpxState; 
            
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Context
        {
            public uint ContextFlags;
            
            private readonly uint Dr0;
            private readonly uint Dr1;
            private readonly uint Dr2;
            private readonly uint Dr3;
            private readonly uint Dr6;
            private readonly uint Dr7;
            
            private readonly FloatingSaveArea FloatingSave;
            
            private readonly uint SegGs;
            private readonly uint SegFs;
            private readonly uint SegEs;
            private readonly uint SegDs;
            
            private readonly uint Edi;
            private readonly uint Esi;
            private readonly uint Ebx;
            private readonly uint Edx;
            private readonly uint Ecx;
            private readonly uint Eax;
            
            private readonly uint Ebp;
            public uint Eip;
            private readonly uint SegCs;
            private readonly uint EFlags;
            private readonly uint Esp;
            private readonly uint SegSs;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            private readonly byte[] ExtendedRegisters;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct M128A
        {
            private readonly ulong High;
            private readonly long Low;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 16)]
        private struct SaveFormat
        {
            private readonly ushort ControlWord;
            private readonly ushort StatusWord;
            private readonly byte TagWord;
            
            private readonly byte Reserved1;
            
            private readonly ushort ErrorOpcode;
            private readonly uint ErrorOffset;
            private readonly ushort ErrorSelector;
            
            private readonly ushort Reserved2;
            
            private readonly uint DataOffset;
            private readonly ushort DataSelector;
            
            private readonly ushort Reserved3;
            
            private readonly uint MxCsr;
            private readonly uint MxCsr_Mask;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private readonly M128A[] FloatRegisters;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            private readonly M128A[] XmmRegisters;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
            private readonly byte[] Reserved4;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 16)]
        public struct Context64
        {
            private readonly ulong P1Home;
            private readonly ulong P2Home;
            private readonly ulong P3Home;
            private readonly ulong P4Home;
            private readonly ulong P5Home;
            private readonly ulong P6Home;

            public Flags ContextFlags;
            private readonly uint MxCsr;

            private readonly ushort SegCs;
            private readonly ushort SegDs;
            private readonly ushort SegEs;
            private readonly ushort SegFs;
            private readonly ushort SegGs;
            private readonly ushort SegSs;
            private readonly uint EFlags;

            private readonly ulong Dr0;
            private readonly ulong Dr1;
            private readonly ulong Dr2;
            private readonly ulong Dr3;
            private readonly ulong Dr6;
            private readonly ulong Dr7;

            private readonly ulong Rax;
            private readonly ulong Rcx;
            private readonly ulong Rdx;
            private readonly ulong Rbx;
            private readonly ulong Rsp;
            private readonly ulong Rbp;
            private readonly ulong Rsi;
            private readonly ulong Rdi;
            private readonly ulong R8;
            private readonly ulong R9;
            private readonly ulong R10;
            private readonly ulong R11;
            private readonly ulong R12;
            private readonly ulong R13;
            private readonly ulong R14;
            private readonly ulong R15;
            public ulong Rip;

            private readonly SaveFormat DummyUnionName;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            private readonly M128A[] VectorRegister;
            private readonly ulong VectorControl;

            private readonly ulong DebugControl;
            private readonly ulong LastBranchToRip;
            private readonly ulong LastBranchFromRip;
            private readonly ulong LastExceptionToRip;
            private readonly ulong LastExceptionFromRip;
        }
        
        [StructLayout(LayoutKind.Sequential)] 
        public struct MemoryInformation 
        {
            private readonly IntPtr BaseAddress;
            
            private readonly IntPtr AllocationBase;
            private readonly uint AllocationProtect; 
            
            public readonly uint RegionSize;
            
            private readonly uint State;
            private readonly uint Protect;
            private readonly uint Type; 
        }
        
        #endregion
    }
}