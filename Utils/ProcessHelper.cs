using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Runtime.InteropServices;

namespace NG.Automation.Core.Utils
{
    public class ProcessHelper
    {

        //[DllImport("kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId", SetLastError = true)]
        //public static extern uint WTSGetActiveConsoleSessionId();

        //[DllImport("Wtsapi32.dll", EntryPoint = "WTSQueryUserToken", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool WTSQueryUserToken(uint SessionId, ref IntPtr phToken);

        //[DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool CloseHandle([InAttribute()] IntPtr hObject);

        //[DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUserW", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool CreateProcessAsUser(
        //    [InAttribute()] 
        //    IntPtr hToken, 
        //    [InAttribute(), MarshalAs(UnmanagedType.LPWStr)]
        //    string lpApplicationName, 
        //    System.IntPtr lpCommandLine, 
        //    [InAttribute()]
        //    IntPtr lpProcessAttributes, 
        //    [InAttribute()]
        //    IntPtr lpThreadAttributes, 
        //    [MarshalAs(UnmanagedType.Bool)]
        //    bool bInheritHandles, 
        //    uint dwCreationFlags, 
        //    [InAttribute()]
        //    IntPtr lpEnvironment, 
        //    [InAttribute(), MarshalAsAttribute(UnmanagedType.LPWStr)]
        //    string lpCurrentDirectory, [InAttribute()]
        //    ref STARTUPINFOW lpStartupInfo,
        //    out PROCESS_INFORMATION lpProcessInformation);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct SECURITY_ATTRIBUTES
        //{
        //    public uint nLength;
        //    public IntPtr lpSecurityDescriptor;
        //    [MarshalAs(UnmanagedType.Bool)]
        //    public bool bInheritHandle;
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //public struct STARTUPINFOW
        //{
        //    public uint cb;
        //    [MarshalAs(UnmanagedType.LPWStr)]
        //    public string lpReserved;
        //    [MarshalAs(UnmanagedType.LPWStr)]
        //    public string lpDesktop;
        //    [MarshalAs(UnmanagedType.LPWStr)]
        //    public string lpTitle;
        //    public uint dwX;
        //    public uint dwY;
        //    public uint dwXSize;
        //    public uint dwYSize;
        //    public uint dwXCountChars;
        //    public uint dwYCountChars;
        //    public uint dwFillAttribute;
        //    public uint dwFlags;
        //    public ushort wShowWindow;
        //    public ushort cbReserved2;
        //    public IntPtr lpReserved2;
        //    public IntPtr hStdInput;
        //    public IntPtr hStdOutput;
        //    public IntPtr hStdError;
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //public struct PROCESS_INFORMATION
        //{
        //    public IntPtr hProcess;
        //    public IntPtr hThread;
        //    public uint dwProcessId;
        //    public uint dwThreadId;
        //}

        #region P/Invoke WTS APIs
        /// <summary> 
        /// Struct, Enum and P/Invoke Declarations of WTS APIs. 
        /// </summary> 
        ///  

        private const int WTS_CURRENT_SERVER_HANDLE = 0;
        private enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WTS_SESSION_INFO
        {
            public UInt32 SessionID;
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] UInt32 Reserved,
            [MarshalAs(UnmanagedType.U4)] UInt32 Version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref UInt32 pSessionInfoCount
            );

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);
        #endregion

        #region P/Invoke CreateProcessAsUser
        /// <summary> 
        /// Struct, Enum and P/Invoke Declarations for CreateProcessAsUser. 
        /// </summary> 
        ///  

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("ADVAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            string lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation
            );

        [DllImport("KERNEL32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CloseHandle(IntPtr hHandle);
        #endregion


        public static void CloseProcess(string name)
        {
            Run("taskkill", "/IM " + name + " /f");
        }

        public static bool ProcessExists(string name)
        {
            List<Process> lst = Process.GetProcessesByName(name).ToList();

            return lst.Count > 0;
        }

        public static IEnumerable<Process> GetProcesses(string name)
        {
            return Process.GetProcessesByName(name).ToList();
        }

        public static void RunNative(string file, string arguemnts)
        {
            // http://code.msdn.microsoft.com/windowsdesktop/CSCreateProcessAsUserFromSe-b682134e/sourcecode?fileId=50832&pathId=163624599
            PROCESS_INFORMATION tProcessInfo;
            STARTUPINFO tStartUpInfo = new STARTUPINFO();
            tStartUpInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

            bool ChildProcStarted = CreateProcessAsUser(
                IntPtr.Zero,             // Token of the logged-on user. hToken
                file + " " + arguemnts,      // Name of the process to be started. 
                null,               // Any command line arguments to be passed. 
                IntPtr.Zero,        // Default Process' attributes. 
                IntPtr.Zero,        // Default Thread's attributes. 
                false,              // Does NOT inherit parent's handles. 
                0,                  // No any specific creation flag. 
                null,               // Default environment path. 
                Path.GetDirectoryName(file),               // Default current directory. 
                ref tStartUpInfo,   // Process Startup Info.  
                out tProcessInfo    // Process information to be returned. 
                );
        }

        public static void Run(string file, string arguemnts, bool wait4exit = true)
        {
            Process p = new Process();
            p.StartInfo.Arguments = arguemnts;
            p.StartInfo.FileName = file;
            if (file.Contains("\\"))
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(file);
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.UseShellExecute = true;
            p.Start();
            if (wait4exit)
                p.WaitForExit();
        }

        public static IDictionary<int, string> ProcessCommandLine(string processName)
        {
            // http://stackoverflow.com/questions/504208/how-to-read-command-line-arguments-of-another-process-in-c/504378%23504378

            IDictionary<int, string> retDictionary = new Dictionary<int, string>();

            string wmiQuery = string.Format("select ProcessID,CommandLine from Win32_Process where Name='{0}'", processName);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
            ManagementObjectCollection retObjectCollection = searcher.Get();
            foreach (ManagementObject retObject in retObjectCollection)
                retDictionary[int.Parse(retObject["ProcessID"].ToString())] = retObject["CommandLine"].ToString();
            return retDictionary;
        }
    }
}
