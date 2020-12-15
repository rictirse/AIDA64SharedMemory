using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace AIDA64
{
    class SharedMemSaver : IDisposable
    {
        #region Win32 API stuff
        public const int FILE_MAP_READ = 0x0004;

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr OpenFileMapping(int dwDesiredAccess,
            bool bInheritHandle, StringBuilder lpName);

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr MapViewOfFile(IntPtr hFileMapping,
            int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow,
            int dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll")]
        internal static extern bool UnmapViewOfFile(IntPtr map);

        [DllImport("kernel32.dll")]
        internal static extern bool CloseHandle(IntPtr hObject);
        #endregion

        bool fileOpen = false;
        bool disposed = false;
        IntPtr map;
        IntPtr handle;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                CloseView();
            }

            disposed = true;
        }

        ~SharedMemSaver()
        {
            Dispose(false);
        }

        public bool OpenView()
        {
            if (!fileOpen)
            {
                StringBuilder sharedMemFile = new StringBuilder("AIDA64_SensorValues");
                handle = OpenFileMapping(FILE_MAP_READ, false, sharedMemFile);
                if (handle == IntPtr.Zero)
                {
                    throw new Exception("Unable to open file mapping.");
                }
                map = MapViewOfFile(handle, FILE_MAP_READ, 0, 0, 0);
                if (map == IntPtr.Zero)
                {
                    throw new Exception("Unable to read shared memory.");
                }
                fileOpen = true;
            }
            return fileOpen;
        }

        public void CloseView()
        {
            if (fileOpen)
            {
                UnmapViewOfFile(map);
                CloseHandle(handle);
            }
        }

        public string GetData()
        {
            if (fileOpen)
            {
                string data = (string)Marshal.PtrToStringAnsi(map);
                return data;
            }
            else
            {
                return null;
            }
        }
    }
}
