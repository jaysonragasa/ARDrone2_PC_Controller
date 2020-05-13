using System;
using System.Windows;

namespace WpfApplication1.Commons
{
    public class Utility
    {
        public static IntPtr GetWindowHandle(Window window)
        {
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
            return helper.Handle;
        }
    }
}
