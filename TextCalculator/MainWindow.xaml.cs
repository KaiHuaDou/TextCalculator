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
        Height = App.Settings.Height;
        Width = App.Settings.Width;
    }

    private void WindowLoaded(object o, RoutedEventArgs e)
    {
        InitGUI( );

        // Preheating NCalc
        Task.Run(( ) => new NCalc.Expression("1+1").Evaluate( ));
    }

    private void InitGUI( )
    {
        TopmostBox.IsChecked = App.Settings.Topmost;
        WindowTopmost(null, null);
        AutoCopyResult.IsChecked = App.Settings.AutoCopy;
        DuplicateResult.IsChecked = App.Settings.Duplicate;
        RoundLengthBox.Text = App.Settings.RoundLength.ToString( );
        try
        {
            MainBox.FontFamily = new FontFamily(App.Settings.FontFamily);
            FontSizeBox.Text = App.Settings.FontSize.ToString( );
            FontSizeTextChanged(null, null);
            EyeProtectBox.IsChecked = App.Settings.EyeProtect;
            EyeProtectChecked(null, null);

            MainBox.FontWeight = (FontWeight) new FontWeightConverter( ).ConvertBack(App.Settings.Bold, null, null, null);
        }
        catch
        {
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(MainBox.FontFamily.Source);
            FontSizeBox.Text = MainBox.FontSize.ToString( );
        }
    }

    private void WindowTopmost(object o, RoutedEventArgs e)
    {
        Topmost = (bool) TopmostBox.IsChecked;
    }

    private void FontFamilySelectionChanged(object o, SelectionChangedEventArgs e)
    {
        MainBox.FontFamily = new FontFamily(FontFamilyBox.SelectedValue.ToString( ));
    }

    private void FontSizeTextChanged(object o, TextChangedEventArgs e)
    {
        MainBox.FontSize = double.TryParse(FontSizeBox.Text, out var result) ? result : 22;
    }

    private void RoundLengthChanged(object o, TextChangedEventArgs e)
    {
        App.Settings.RoundLength = int.TryParse(RoundLengthBox.Text, out var result) ? result : 3;
    }

    private void EyeProtectChecked(object o, RoutedEventArgs e)
    {
        SolidColorBrush green = new(Color.FromArgb(0xFF, 0xCF, 0xE8, 0xCC));
        MainBox.Background = EyeProtectBox.IsChecked == true ? green : Brushes.White;
    }

    private void WindowClosing(object o, CancelEventArgs e)
    {
        App.Settings.Topmost = Topmost;
        App.Settings.AutoCopy = (bool) AutoCopyResult.IsChecked;
        App.Settings.Duplicate = (bool) DuplicateResult.IsChecked;
        App.Settings.FontFamily = MainBox.FontFamily.Source;
        App.Settings.FontSize = MainBox.FontSize;
        App.Settings.Bold = (bool) BoldBox.IsChecked;
        App.Settings.EyeProtect = (bool) EyeProtectBox.IsChecked;
        App.Settings.Height = ActualHeight;
        App.Settings.Width = ActualWidth;
    }

    private void ExpanderExpanded(object o, RoutedEventArgs e)
    {
        if (FontFamilyBox.ItemsSource != null)
        {
            return;
        }

        try
        {
            FontFamilyBox.ItemsSource = Fonts.SystemFontFamilies.Select(o => o.Source).Order( );
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(App.Settings.FontFamily);
        }
        catch
        {
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(MainBox.FontFamily.Source);
        }
    }
}
