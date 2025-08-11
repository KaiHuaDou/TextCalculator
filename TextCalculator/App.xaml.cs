using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using SingleInstanceCore;

namespace TextCalculator;
public partial class App : Application, ISingleInstance
{
    public static Settings Settings { get; set; }

    public static string SettingsFile = Path.Join(
        Path.GetDirectoryName(Environment.ProcessPath),
        "settings.json"
    );

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
        => MainWindow.Activate( );

    private void AppStartup(object sender, StartupEventArgs e)
    {
        if (!this.InitializeAsFirstInstance("TextCalculator_1_0_1"))
            Current.Shutdown( );
        try
        {
            string json = File.ReadAllText(SettingsFile);
            Settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings( );
        }
        catch
        {
            Settings = new Settings( );
        }
    }

    private void AppExit(object sender, ExitEventArgs e)
    {
        string json = JsonSerializer.Serialize(Settings);
        File.WriteAllText(SettingsFile, json);
        SingleInstance.Cleanup( );
    }
}
