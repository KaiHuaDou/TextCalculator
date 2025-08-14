using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextCalculator;

public partial class MainWindow : Window
{
    public MainWindow( )
    {
        InitializeComponent( );
    }

    private void WindowLoaded(object o, RoutedEventArgs e)
    {
        while (!App.SettingsLoaded) ;

        Height = App.Settings.Height;
        Width = App.Settings.Width;

        InitGUI( );

        Task.Run(( ) => new NCalc.Expression("1+1").Evaluate( ));
    }

    private void InitGUI( )
    {
        try
        {
            mainBox.FontFamily = new FontFamily(App.Settings.FontFamily);
            TopmostBox.IsChecked = App.Settings.Topmost;
            WindowTopmost(null, null);
            FontSizeBox.Text = App.Settings.FontSize.ToString( );
            FontSizeTextChanged(null, null);
            EyeProtectBox.IsChecked = App.Settings.EyeProtect;
            EyeProtectChecked(null, null);

            mainBox.FontWeight = (FontWeight) new FontWeightConverter( ).ConvertBack(App.Settings.Bold, null, null, null);
            AutoCopyResult.IsChecked = App.Settings.AutoCopy;
            DuplicateResult.IsChecked = App.Settings.Duplicate;
            RoundLengthBox.Text = App.Settings.RoundLength.ToString( );
        }
        catch
        {
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(mainBox.FontFamily.Source);
            FontSizeBox.Text = mainBox.FontSize.ToString( );
            App.Settings.RoundLength = 3;
            RoundLengthBox.Text = "3";
        }
    }

    private void WindowTopmost(object o, RoutedEventArgs e)
        => Topmost = (bool) TopmostBox.IsChecked;

    private void FontFamilySelectionChanged(object o, SelectionChangedEventArgs e)
        => mainBox.FontFamily = new FontFamily(FontFamilyBox.SelectedValue.ToString( ));

    private void FontSizeTextChanged(object o, TextChangedEventArgs e)
        => mainBox.FontSize = double.TryParse(FontSizeBox.Text, out double result) ? result : 22;

    private void RoundLengthChanged(object o, TextChangedEventArgs e)
        => App.Settings.RoundLength = int.TryParse(RoundLengthBox.Text, out int result) ? result : 3;

    private void EyeProtectChecked(object o, RoutedEventArgs e)
    {
        SolidColorBrush green = new(Color.FromArgb(0xFF, 0xCF, 0xE8, 0xCC));
        mainBox.Background = EyeProtectBox.IsChecked == true ? green : Brushes.White;
    }

    private void WindowClosing(object o, CancelEventArgs e)
    {
        App.Settings.Topmost = Topmost;
        App.Settings.AutoCopy = (bool) AutoCopyResult.IsChecked;
        App.Settings.Duplicate = (bool) DuplicateResult.IsChecked;
        App.Settings.FontFamily = mainBox.FontFamily.Source;
        App.Settings.FontSize = mainBox.FontSize;
        App.Settings.Bold = (bool) BoldBox.IsChecked;
        App.Settings.EyeProtect = (bool) EyeProtectBox.IsChecked;
        App.Settings.Height = ActualHeight;
        App.Settings.Width = ActualWidth;
    }

    private void ExpanderExpanded(object o, RoutedEventArgs e)
    {
        if (FontFamilyBox.ItemsSource != null)
            return;
        try
        {
            FontFamilyBox.ItemsSource = Fonts.SystemFontFamilies.Select(o => o.Source).Order( );
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(App.Settings.FontFamily);
        }
        catch
        {
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(mainBox.FontFamily.Source);
        }
    }
}
