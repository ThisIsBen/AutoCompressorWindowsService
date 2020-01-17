using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompressorWindowsService
{
    static class CheckRemoteDriveFreeSpace
    {

    
        //Return the free space of a remote drive in byte
        public static long getRemoteDriveFreeSpace(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));

            if (!folderName.EndsWith("\\")) folderName += '\\';

            long free = 0, dummy1 = 0, dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, ref free, ref dummy1, ref dummy2))
                return free;

            return -1;
        }

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool GetDiskFreeSpaceEx
        (
            string lpszPath,                    // Must name a folder, must end with '\'.
            ref long lpFreeBytesAvailable,
            ref long lpTotalNumberOfBytes,
            ref long lpTotalNumberOfFreeBytes
        );
    
    }
}
