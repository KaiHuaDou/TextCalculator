using System.ComponentModel;
using System.Linq;
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
        => Dispatcher.InvokeAsync(InitGUI);

    private void InitGUI( )
    {
        FontFamilyBox.ItemsSource = Fonts.SystemFontFamilies.Select(o => o.Source).Order( );
        try
        {
            TopmostBox.IsChecked = App.Settings.Topmost;
            WindowTopmost(null, null);
            FontFamilyBox.SelectedIndex = FontFamilyBox.Items.IndexOf(App.Settings.FontFamily);
            FontFamilySelectionChanged(null, null);
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
    }
}
