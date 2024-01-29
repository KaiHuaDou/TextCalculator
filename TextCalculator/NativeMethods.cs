using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TextCalculator;
internal static class NativeMethods
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    public static IntPtr GetWindowHwndSource(DependencyObject window)
        => (PresentationSource.FromDependencyObject(window) as HwndSource).Handle;
}
