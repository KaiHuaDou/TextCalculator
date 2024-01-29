using System.Windows;

namespace TextCalculator;

public partial class MainWindow : Window
{
    public MainWindow( )
    {
        InitializeComponent( );
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

    private void WindowTopmost(object sender, RoutedEventArgs e)
        => Topmost = !Topmost;
}
