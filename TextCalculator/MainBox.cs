using System;
using System.Windows;
using System.Windows.Input;
using TextCalculator.Properties;

namespace TextCalculator;
public partial class MainWindow
{
    private void MainBoxKeyDown(object o, KeyEventArgs e)
    {
        if (e.Key is Key.ImeProcessed or not Key.Return)
            return;
        e.Handled = true;

        (int lineIndex, int charIndex, string raw) = GetCharIndex(mainBox.Text);
        string parsedRaw = Calculator.ExprFilter(raw);
        if (string.IsNullOrWhiteSpace(parsedRaw))
        {
            mainBox.Text = mainBox.Text.Insert(charIndex - 2, "\r\n");
            mainBox.SelectionStart = charIndex - 1;
            return;
        }
        switch (parsedRaw)
        {
            case "cls" or "clear": mainBox.Text = ""; return;
            case "exit": Application.Current.Shutdown( ); return;
        }

        double answer = Calculator.Calculate(parsedRaw);
        answer = Math.Round(answer, Settings.Default.RoundLength, MidpointRounding.AwayFromZero);
        if (AutoCopyResult.IsChecked == true)
            Clipboard.SetText(answer.ToString( ));
        string result = !double.IsNaN(answer)
            ? $"{(raw[^1] == '=' ? "" : "=")}{answer}\r\n"
            : "\r\n";
        // 此处为 Windows 系统使用 CRLF 换行符，导致在换行处理时有 2 字符的偏差。
        bool flag = lineIndex + 1 >= mainBox.Text.Split("\r\n").Length;
        mainBox.Text = mainBox.Text.Insert(charIndex - 2, result);
        mainBox.SelectionStart = charIndex + (flag ? result.Length : result.Length - 2);
    }

    private void ClearBox(object o, RoutedEventArgs e)
        => mainBox.Clear( );

    private void CopyLine(object o, RoutedEventArgs e)
        => Clipboard.SetText(GetCharIndex(mainBox.Text).Item3);

    private void CopyAction(object o, RoutedEventArgs e)
    {
        string raw = GetCharIndex(mainBox.Text).Item3;
        int equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[..equalIndex]);
    }

    private void CopyResult(object o, RoutedEventArgs e)
    {
        string raw = GetCharIndex(mainBox.Text).Item3;
        int equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[(equalIndex + 1)..]);
    }
}
