using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TextCalculator;
internal static partial class NativeMethods
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SwitchToThisWindow(
        IntPtr hWnd,
        [MarshalAs(UnmanagedType.Bool)] bool fAltTab);

    public static IntPtr GetWindowHwndSource(DependencyObject window)
        => (PresentationSource.FromDependencyObject(window) as HwndSource).Handle;
}
