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
        int lineIndex = mainBox.GetLineIndexFromCharacterIndex(mainBox.CaretIndex);
        string raw = mainBox.GetLineText(lineIndex);
        if (raw is "cls" or "clear")
        {
            mainBox.Text = "";
            return;
        }
        string result = $"={Calculator.Calculate(Calculator.ExprFilter(raw))}";
        while (mainBox.CaretIndex < mainBox.Text.Length && mainBox.Text[mainBox.CaretIndex] != '\n')
            mainBox.CaretIndex++;
        mainBox.Text = mainBox.Text.Insert(mainBox.CaretIndex, result);
        while (mainBox.CaretIndex < mainBox.Text.Length && mainBox.Text[mainBox.CaretIndex] != '\n')
            mainBox.CaretIndex++;
    }
}
