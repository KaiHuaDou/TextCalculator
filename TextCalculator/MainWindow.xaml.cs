using System.Windows;
using System.Windows.Input;

namespace TextCalculator;

public partial class MainWindow : Window
{
    public MainWindow( )
    {
        InitializeComponent( );
    }

    private void MainBoxKeyDown(object o, KeyEventArgs e)
    {
        if (e.Key is Key.ImeProcessed or not Key.Return)
            return;
        e.Handled = true;

        string[] lines = mainBox.Text.Split("\r\n");
        int lineIndex = 0, charIndex = 0;
        foreach (string line in lines)
        {
            charIndex += line.Length + 2;
            if (charIndex > mainBox.SelectionStart)
                break;
            lineIndex++;
        }
        string raw = lines[lineIndex];
        raw = Calculator.ExprFilter(raw);
        if (string.IsNullOrWhiteSpace(raw))
        {
            mainBox.Text = mainBox.Text.Insert(charIndex - 2, "\r\n");
            mainBox.SelectionStart = charIndex - 1;
            return;
        }
        else if (raw is "cls" or "clear")
        {
            mainBox.Text = "";
            return;
        }
        else if (raw == "exit")
        {
            Application.Current.Shutdown( );
        }
        string result = $"={Calculator.Calculate(raw)}\r\n";
        // 此处为 Windows 系统使用 CRLF 换行符，导致在换行处理时有 2 字符的偏差。
        bool flag = lineIndex + 1 >= lines.Length;
        mainBox.Text = mainBox.Text.Insert(charIndex - 2, result);
        mainBox.SelectionStart = charIndex + (flag ? result.Length : result.Length - 2);
    }
}
