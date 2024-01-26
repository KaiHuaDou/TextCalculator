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

        int lineIndex = mainBox.GetLineIndexFromCharacterIndex(mainBox.SelectionStart);
        string raw = mainBox.GetLineText(lineIndex);
        raw = Calculator.ExprFilter(raw);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }
        else if (raw is "cls" or "clear")
        {
            mainBox.Text = "";
            e.Handled = true;
            return;
        }
        else if (raw == "exit")
        {
            Application.Current.Shutdown( );
        }

        string result = $"={Calculator.Calculate(raw)}";
        do { mainBox.SelectionStart++; }
        while (mainBox.SelectionStart < mainBox.Text.Length && mainBox.Text[mainBox.SelectionStart] is not '\n' or '\r');
        int index = mainBox.SelectionStart;
        mainBox.Text = mainBox.Text.Insert(mainBox.SelectionStart, result);
        mainBox.SelectionStart = index + result.Length;
    }
}
