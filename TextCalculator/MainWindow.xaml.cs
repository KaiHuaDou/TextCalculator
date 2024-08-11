using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TextCalculator.Properties;

namespace TextCalculator;

public partial class MainWindow : Window
{
    public MainWindow( )
    {
        InitializeComponent( );
    }

    private void WindowLoaded(object o, RoutedEventArgs e)
        => Dispatcher.InvokeAsync(InitGUI);

    private void InitGUI( )
    {
        FontFamilyBox.ItemsSource = Fonts.SystemFontFamilies.Select(o => o.Source).OrderBy(o => o);
        try
        {
            TopmostBox.IsChecked = Settings.Default.Topmost;
            WindowTopmost(null, null);
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(Settings.Default.FontFamily);
            FontFamilySelectionChanged(null, null);
            FontSizeBox.Text = Settings.Default.FontSize.ToString( );
            FontSizeTextChanged(null, null);
            EyeProtectBox.IsChecked = Settings.Default.EyeProtect;
            EyeProtectChecked(null, null);

            mainBox.FontWeight = (FontWeight) new FontWeightConverter( ).ConvertBack(Settings.Default.Bold, null, null, null);
            AutoCopyResult.IsChecked = Settings.Default.AutoCopy;
            DuplicateResult.IsChecked = Settings.Default.Duplicate;
            RoundLengthBox.Text = Settings.Default.RoundLength.ToString( );
        }
        catch
        {
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(mainBox.FontFamily.Source);
            FontSizeBox.Text = mainBox.FontSize.ToString( );
            Settings.Default.RoundLength = 3;
            RoundLengthBox.Text = "3";
        }
    }

    public void SetInputNow(char c)
    {
        int selectIndex = mainBox.SelectionStart;
        char[] textArray = mainBox.Text.ToCharArray( );
        textArray[mainBox.SelectionStart - 1] = c;
        mainBox.Text = new string(textArray);
        mainBox.SelectionStart = selectIndex;
    }

    private (int, int, string) GetCharIndex(string text)
    {
        string[] lines = text.Split("\r\n");
        int charIndex = 0, lineIndex = 0;
        foreach (string line in lines)
        {
            charIndex += line.Length + 2;
            if (charIndex > mainBox.SelectionStart)
                break;
            lineIndex++;
        }
        return (lineIndex, charIndex, lines[lineIndex]);
    }

    private void WindowTopmost(object o, RoutedEventArgs e)
        => Topmost = (bool) TopmostBox.IsChecked;

    private void FontFamilySelectionChanged(object o, SelectionChangedEventArgs e)
        => mainBox.FontFamily = new FontFamily(FontFamilyBox.SelectedValue.ToString( ));

    private void FontSizeTextChanged(object o, TextChangedEventArgs e)
        => mainBox.FontSize = double.TryParse(FontSizeBox.Text, out double result) ? result : 22;

    private void RoundLengthChanged(object o, TextChangedEventArgs e)
        => Settings.Default.RoundLength = int.TryParse(RoundLengthBox.Text, out int result) ? result : 3;

    private void EyeProtectChecked(object o, RoutedEventArgs e)
    {
        SolidColorBrush green = new(Color.FromArgb(0xFF, 0xCF, 0xE8, 0xCC));
        mainBox.Background = EyeProtectBox.IsChecked == true ? green : Brushes.White;
    }

    private void WindowClosing(object o, CancelEventArgs e)
    {
        Settings.Default.Topmost = Topmost;
        Settings.Default.AutoCopy = (bool) AutoCopyResult.IsChecked;
        Settings.Default.Duplicate = (bool) DuplicateResult.IsChecked;
        Settings.Default.FontFamily = mainBox.FontFamily.Source;
        Settings.Default.FontSize = mainBox.FontSize;
        Settings.Default.Bold = (bool) BoldBox.IsChecked;
        Settings.Default.EyeProtect = (bool) EyeProtectBox.IsChecked;
        Settings.Default.Save( );
    }
}
