﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;

namespace AvalonDock
{
    static class WindowHelper
    {
        public static void SetParentToMainWindowOf(this Window window, Visual element)
        {
            var wndParent = Window.GetWindow(element);
            if (wndParent != null)
                window.Owner = wndParent;
            else
            {
                IntPtr parentHwnd;
                if (GetParentWindowHandle(element, out parentHwnd))
                    Win32Helper.SetOwner(new WindowInteropHelper(window).Handle, parentHwnd);
            }
        }

        public static IntPtr GetParentWindowHandle(this Window window)
        {
            if (window.Owner != null)
                return new WindowInteropHelper(window.Owner).Handle;
            else
                return Win32Helper.GetOwner(new WindowInteropHelper(window).Handle);
        }


        public static bool GetParentWindowHandle(this Visual element, out IntPtr hwnd)
        {
            hwnd = IntPtr.Zero;
            HwndSource wpfHandle = PresentationSource.FromVisual(element) as HwndSource;
            
            if (wpfHandle == null)
                return false;

            hwnd = Win32Helper.GetParent(wpfHandle.Handle);
            if (hwnd == IntPtr.Zero)
                hwnd = wpfHandle.Handle;
            return true;
        }

        public static void SetParentWindowToNull(this Window window)
        {
            if (window.Owner != null)
                window.Owner = null;
            else
            {
                Win32Helper.SetOwner(new WindowInteropHelper(window).Handle, IntPtr.Zero);
            }
        }
    }
}
