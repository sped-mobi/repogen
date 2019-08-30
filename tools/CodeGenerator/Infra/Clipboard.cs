// -----------------------------------------------------------------------
// <copyright file="Clipboard.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Windows
{
#if true


    internal static class Clipboard
    {
        /// <summary>
        /// Defines the cfUnicodeText
        /// </summary>
        internal const uint cfUnicodeText = 13;

        /// <summary>
        /// The OpenClipboard
        /// </summary>
        public static void OpenClipboard()
        {
            var num = 10;
            while (true)
            {
                if (OpenClipboard(default(IntPtr)))
                {
                    break;
                }

                if (--num == 0)
                {
                    ThrowWin32();
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// The SetText
        /// </summary>
        /// <param name="text" >The text<see cref="string" /></param>
        public static void SetText(string text)
        {
            OpenClipboard();
            EmptyClipboard();
            var hGlobal = default(IntPtr);
            try
            {
                var bytes = (text.Length + 1) * 2;
                hGlobal = Marshal.AllocHGlobal(bytes);
                if (hGlobal == default(IntPtr))
                {
                    ThrowWin32();
                }

                var target = GlobalLock(hGlobal);
                if (target == default(IntPtr))
                {
                    ThrowWin32();
                }

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                }
                finally
                {
                    GlobalUnlock(target);
                }

                if (SetClipboardData(cfUnicodeText, hGlobal) == default(IntPtr))
                {
                    ThrowWin32();
                }

                hGlobal = default(IntPtr);
            }
            finally
            {
                if (hGlobal != default(IntPtr))
                {
                    Marshal.FreeHGlobal(hGlobal);
                }

                CloseClipboard();
            }
        }

        /// <summary>
        /// The CloseClipboard
        /// </summary>
        /// <returns>The <see cref="bool" /></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseClipboard();

        /// <summary>
        /// The EmptyClipboard
        /// </summary>
        /// <returns>The <see cref="bool" /></returns>
        [DllImport("user32.dll")]
        internal static extern bool EmptyClipboard();

        /// <summary>
        /// The GlobalLock
        /// </summary>
        /// <param name="hMem" >The hMem<see cref="IntPtr" /></param>
        /// <returns>The <see cref="IntPtr" /></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GlobalLock(IntPtr hMem);

        /// <summary>
        /// The GlobalUnlock
        /// </summary>
        /// <param name="hMem" >The hMem<see cref="IntPtr" /></param>
        /// <returns>The <see cref="bool" /></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GlobalUnlock(IntPtr hMem);

        /// <summary>
        /// The OpenClipboard
        /// </summary>
        /// <param name="hWndNewOwner" >The hWndNewOwner<see cref="IntPtr" /></param>
        /// <returns>The <see cref="bool" /></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        /// <summary>
        /// The SetClipboardData
        /// </summary>
        /// <param name="uFormat" >The uFormat<see cref="uint" /></param>
        /// <param name="data" >The data<see cref="IntPtr" /></param>
        /// <returns>The <see cref="IntPtr" /></returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        /// <summary>
        /// The ThrowWin32
        /// </summary>
        internal static void ThrowWin32()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

#endif
}
