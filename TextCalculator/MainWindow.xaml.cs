using System.ComponentModel;
using System.Drawing.Text;
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
        FontFamilyBox.ItemsSource =
            new InstalledFontCollection( ).Families.Select(o => o.Name).OrderBy(o => o);
        try
        {
            TopmostBox.IsChecked = Settings.Default.Topmost;
            AutoCopyResult.IsChecked = Settings.Default.AutoCopy;
            DuplicateResult.IsChecked = Settings.Default.Duplicate;
            RoundLengthBox.Text = Settings.Default.RoundLength.ToString( );
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(Settings.Default.FontFamily);
            FontSizeBox.Text = Settings.Default.FontSize.ToString( );
            //BoldBox.IsChecked = Settings.Default.Bold;
            mainBox.FontWeight = (FontWeight) new FontWeightConverter( ).ConvertBack(Settings.Default.Bold, null, null, null);
            EyeProtectBox.IsChecked = Settings.Default.EyeProtect;

            WindowTopmost(null, null);
            FontFamilySelectionChanged(null, null);
            FontSizeTextChanged(null, null);
            EyeProtectChecked(null, null);
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
        SolidColorBrush white = new(Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6));
        SolidColorBrush green = new(Color.FromArgb(0xFF, 0xCF, 0xE8, 0xCC));
        mainBox.Background = EyeProtectBox.IsChecked == true ? green : white;
        //mainBox.Foreground = EyeProtectBox.IsChecked == true ? white : black;
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
