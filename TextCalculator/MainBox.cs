using System;
using System.Windows;
using System.Windows.Input;

namespace TextCalculator;
public partial class MainWindow
{
    private void MainBoxKeyDown(object o, KeyEventArgs e)
    {
        if (e.Key is Key.ImeProcessed or not Key.Return)
            return;

        (_, int lineEnd, string line) = GetLine( );

        if (string.IsNullOrWhiteSpace(line))
            return;

        (string expr, string equalMark) = Calculator.Filter(line);
        string answer = Calculator.Calculate(expr);
        string result = FormatResult(answer, equalMark);

        int insertIndex = mainBox.Text[lineEnd - 1] switch
        {
            '\r' or '\n' => lineEnd - 1,
            _ => lineEnd
        };
        mainBox.Text = mainBox.Text.Insert(insertIndex, result);
        mainBox.SelectionStart = insertIndex + result.Length;

        e.Handled = true;
    }

    private (int, int, string) GetLine( )
    {
        int i, j;
        i = j = mainBox.CaretIndex - 1;
        while (mainBox.Text[i] != '\n' && i > 0)
            i--;
        while (mainBox.Text[j] != '\r' && mainBox.Text[j] != '\n' && j < mainBox.Text.Length - 1)
            j++;
        j++;
        return (i, j, mainBox.Text[i..j].Trim( ));
    }

    private string FormatResult(string answer, string equalMark)
    {
        if (double.TryParse(answer, out double value))
        {
            answer = Math.Round(value, App.Settings.RoundLength, MidpointRounding.AwayFromZero).ToString( );
        }
        if (AutoCopyResult.IsChecked == true)
        {
            Clipboard.SetText(answer);
        }
        string duplicate = DuplicateResult.IsChecked == true ? answer : "";
        return string.IsNullOrWhiteSpace(answer)
            ? "\r\n"
            : $"{equalMark}{answer}\r\n{duplicate}";
    }

    private void ClearBox(object o, RoutedEventArgs e)
        => mainBox.Clear( );

    private void CopyLine(object o, RoutedEventArgs e)
        => Clipboard.SetText(GetLine( ).Item3);

    private void CopyAction(object o, RoutedEventArgs e)
    {
        string raw = GetLine( ).Item3;
        int equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[..equalIndex]);
    }

    private void CopyResult(object o, RoutedEventArgs e)
    {
        string raw = GetLine( ).Item3;
        int equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[(equalIndex + 1)..]);
    }
}
