using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

namespace TextCalculator;
public partial class MainWindow
{
    private void MainBoxKeyDown(object o, KeyEventArgs e)
    {
        if (e.Key is Key.ImeProcessed or not Key.Return)
        {
            return;
        }

        (_, var lineEnd, var line) = GetLine( );

        if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || line.StartsWith('；'))
        {
            return;
        }

        (var expr, var equalMark) = Calculator.Filter(line);
        var answer = Calculator.Calculate(expr);
        var result = FormatResult(answer, equalMark);

        var insertIndex = MainBox.Text[lineEnd - 1] switch
        {
            '\r' or '\n' => lineEnd - 1,
            _ => lineEnd
        };
        MainBox.Text = MainBox.Text.Insert(insertIndex, result);
        MainBox.SelectionStart = insertIndex + result.Length;

        e.Handled = true;
    }

    private (int, int, string) GetLine( )
    {
        int i, j;
        i = j = MainBox.CaretIndex - 1;
        while (MainBox.Text[i] != '\n' && i > 0)
        {
            i--;
        }

        while (MainBox.Text[j] != '\r' && MainBox.Text[j] != '\n' && j < MainBox.Text.Length - 1)
        {
            j++;
        }

        j++;
        return (i, j, MainBox.Text[i..j].Trim( ));
    }

    private string FormatResult(string answer, string equalMark)
    {
        if (double.TryParse(answer, out var value))
        {
            answer = Math.Round(value, App.Settings.RoundLength, MidpointRounding.AwayFromZero).ToString( );
        }

        if (AutoCopyResult.IsChecked == true)
        {
            Clipboard.SetText(answer);
        }

        var duplicate = DuplicateResult.IsChecked == true ? answer : "";
        return string.IsNullOrWhiteSpace(answer)
            ? "\r\n"
            : $"{equalMark}{answer}\r\n{duplicate}";
    }

    private void ClearBox(object o, RoutedEventArgs e)
    {
        MainBox.Clear( );
    }

    private void CopyLine(object o, RoutedEventArgs e)
    {
        Clipboard.SetText(GetLine( ).Item3);
    }

    private void CopyAction(object o, RoutedEventArgs e)
    {
        var raw = GetLine( ).Item3;
        var equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[..equalIndex]);
    }

    private void CopyResult(object o, RoutedEventArgs e)
    {
        var raw = GetLine( ).Item3;
        var equalIndex = raw.LastIndexOf('=');
        Clipboard.SetText(equalIndex == -1 ? raw : raw[(equalIndex + 1)..]);
    }

    private void SaveContent(object o, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(MainBox.Text))
        {
            return;
        }

        SaveFileDialog dialog = new( )
        {
            Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
            DefaultExt = ".txt"
        };
        if (dialog.ShowDialog( ) != true)
        {
            return;
        }

        try
        {
            File.WriteAllText(dialog.FileName, MainBox.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法写入文件\n{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
