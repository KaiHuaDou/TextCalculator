using System;
using System.Windows;
using SingleInstanceCore;
using static TextCalculator.NativeMethods;

namespace TextCalculator;
public partial class App : Application, ISingleInstance
{
    public static class Program
    {
        [STAThread]
        public static void Main( )
        {
            App app = new( );
            app.InitializeComponent( );
            app.Run( );
        }
    }

    public void OnInstanceInvoked(string[] args)
        => SwitchToThisWindow(GetWindowHwndSource(MainWindow), true);

    private void AppStartup(object sender, StartupEventArgs e)
    {
        if (!this.InitializeAsFirstInstance("TextCalculator_1_0_0"))
            Current.Shutdown( );
    }

    private void AppExit(object sender, ExitEventArgs e)
        => SingleInstance.Cleanup( );
}
