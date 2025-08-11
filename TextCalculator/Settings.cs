namespace TextCalculator;

public class Settings( )
{
    public string FontFamily { get; set; } = "Consolas";
    public double FontSize { get; set; } = 24;
    public bool Bold { get; set; } = true;
    public bool Topmost { get; set; }
    public bool AutoCopy { get; set; }
    public int RoundLength { get; set; } = 9;
    public bool Duplicate { get; set; }
    public bool EyeProtect { get; set; } = true;
    public double Height { get; set; } = 600;
    public double Width { get; set; } = 800;
}
