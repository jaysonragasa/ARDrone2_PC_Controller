using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Commons
{
    public class KeyboardHook : IDisposable
    {
        public delegate IntPtr HookDelegate(
        Int32 Code, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hHook, Int32 nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr UnhookWindowsHookEx(IntPtr hHook);


        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowsHookEx(
            Int32 idHook, HookDelegate lpfn, IntPtr hmod,
            Int32 dwThreadId);

        public event EventHandler<EventArgs> KeyBoardKeyPressed;

        private HookDelegate keyBoardDelegate;
        private IntPtr keyBoardHandle;
        private const Int32 WH_KEYBOARD_LL = 13;
        private bool disposed = false;

        public KeyboardHook()
        {
            keyBoardDelegate = KeyboardHookDelegate;
            keyBoardHandle = SetWindowsHookEx(WH_KEYBOARD_LL, keyBoardDelegate, IntPtr.Zero, 0);
        }

        private IntPtr KeyboardHookDelegate(Int32 Code, IntPtr wParam, IntPtr lParam)
        {
            if (Code < 0)
            {
                return CallNextHookEx(
                    keyBoardHandle, Code, wParam, lParam);
            }

            if (KeyBoardKeyPressed != null)
                KeyBoardKeyPressed(this, new EventArgs());

            return CallNextHookEx(
                keyBoardHandle, Code, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (keyBoardHandle != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(keyBoardHandle);
                }

                disposed = true;
            }
        }

        ~KeyboardHook()
        {
            Dispose(false);
        }
    }
}