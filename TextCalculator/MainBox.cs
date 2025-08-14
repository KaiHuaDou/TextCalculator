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

        int lineIndex = mainBox.GetLineIndexFromCharacterIndex(mainBox.SelectionStart);
        string line = mainBox.GetLineText(lineIndex);

        if (string.IsNullOrWhiteSpace(line))
            return;

        int lineStartIndex = mainBox.GetCharacterIndexFromLineIndex(lineIndex);
        int lineLength = mainBox.GetLineLength(lineIndex);
        int lineEndIndex = lineStartIndex + lineLength;

        (string expr, string equalMark) = Calculator.Filter(line);
        string answer = Calculator.Calculate(expr);
        string result = FormatResult(answer, equalMark);

        int insertIndex = mainBox.Text[lineEndIndex - 1] switch
        {
            '\r' => lineEndIndex - 1,
            '\n' => lineEndIndex - 2,
            _ => lineEndIndex
        };
        mainBox.Text = mainBox.Text.Insert(insertIndex, result);
        mainBox.SelectionStart = insertIndex + result.Length;

        e.Handled = true;
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
        => Clipboard.SetText(mainBox.GetLineText(mainBox.GetLineIndexFromCharacterIndex(mainBox.CaretIndex)));

    private void CopyAction(object o, RoutedEventArgs e)
    {
        string raw = mainBox.GetLineText(mainBox.GetLineIndexFromCharacterIndex(mainBox.CaretIndex));
        int equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[..equalIndex]);
    }

    private void CopyResult(object o, RoutedEventArgs e)
    {
        string raw = mainBox.GetLineText(mainBox.GetLineIndexFromCharacterIndex(mainBox.CaretIndex));
        int equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[(equalIndex + 1)..]);
    }
}
