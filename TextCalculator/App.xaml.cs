using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace TextCalculator;
public partial class App : Application
{
    public static Settings Settings { get; set; }

    public static bool SettingsLoaded { get; set; }

    public static string SettingsFile => Path.Join(
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

    private void AppStartup(object o, StartupEventArgs e)
    {
        try
        {
            var json = File.ReadAllText(App.SettingsFile);
            Settings = JsonSerializer.Deserialize(json, SettingsSerializeContext.Default.Settings) ?? new Settings( );
        }
        catch
        {
            Settings = new Settings( );
        }

        SettingsLoaded = true;
    }

    private void AppExit(object o, ExitEventArgs e)
    {
        var json = JsonSerializer.Serialize(Settings, SettingsSerializeContext.Default.Settings);
        File.WriteAllText(SettingsFile, json);
    }
}
