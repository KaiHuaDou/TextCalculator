using System.Collections.Generic;
using NCalc;
using static System.Math;

namespace TextCalculator;
public static class Calculator
{
    private static readonly Dictionary<string, string> filters = new( )
    {
        // 中文标点
        {"“", ""}, {"”", ""}, {"‘", ""}, {"’", ""},
        {"（", "("}, {"）", ")"}, {"、", "/"}, {"，", ","},
        {"……", "^"}, {"——", "-"}, {"《", "<"}, {"》", ">"},
        // 汉字运算
        {"一", "1"}, {"二", "2"}, {"三", "3"}, {"四", "4"},
        {"五", "5"}, {"六", "6"}, {"七", "7"}, {"八", "8"},
        {"九", "9"}, {"十", "10"}, {"百", "00"},
        {"千", "000"}, {"万", "0000"}, {"亿", "00000000"},
        {"加","+"}, {"减","-"}, {"乘","*"}, {"除","/"}, {"百分之", "0.01*"},
        // 特殊符号
        {"^", "**"}, {"×", "*"}, {"÷", "/"}, {"'", ""},
        // 多级括号
        {"{", "("}, {"}", ")"}, {"[", "("}, {"]", ")"},
        // 数学运算
        {"%", "*0.01"}
    };

    public static string Calculate(string expr)
    {
        if (double.TryParse(expr, out _))
            return "";
        Expression e = new(expr)
        {
            Options = ExpressionOptions.IgnoreCaseAtBuiltInFunctions
        };
        e.EvaluateFunction += (name, args) =>
        {
            try
            {
                List<double> parameters = [];
                foreach (Expression parameter in args.Parameters)
                    parameters.Add(double.Parse(parameter.Evaluate( ).ToString( )));
                double result = ParseFunction(name, parameters);
                if (!double.IsNaN(result))
                    args.Result = result.ToString( );
            }
            catch { }
        };
        e.EvaluateParameter += (name, args) =>
        {
            double result = ParseParameter(name);
            if (!double.IsNaN(result))
                args.Result = result;
        };
        try
        {
            return e.Evaluate( ).ToString( );
        }
        catch
        {
            return "";
        }
    }

    public static (string, string) Filter(string expr)
    {
        expr = expr.Trim( );
        foreach (KeyValuePair<string, string> filter in filters)
            expr = expr.Replace(filter.Key, filter.Value);
        string equalMark = "=";
        while (expr.EndsWith('='))
        {
            expr = expr[..^1];
            equalMark = "";
        }
        return (expr, equalMark);
    }

    private static double CubicEquation(double a, double b, double c, double d)
    {
        double u = (9 * a * b * c - 27 * a * a * d - 2 * b * b * b) / (54 * a * a * a);
        double v = Sqrt(3 * (4 * a * c * c * c - b * b * c * c - 18 * a * b * c * d + 27 * a * a * d * d + 4 * b * b * b * d)) / (18 * a * a);
        double m = 0, n = 0;
        if (Abs(u + v) >= Abs(u - v))
            m = Pow(u + v, 1.0 / 3);
        else if (Abs(u + v) < Abs(u - v))
            m = Pow(u - v, 1.0 / 3);
        if (m != 0)
            n = (b * b - 3 * a * c) / (9 * a * a * m);
        else if (m == 0)
            n = 0;
        return m + n - b / (3 * a);
    }

    private static double Deg(double rad) => rad * 180 / PI;

    private static double FourEquation(double a, double b, double c, double d, double e)
    {
        double A = 8 * a * c - 3 * b * b;
        double C = b * b * b - 4 * a * b * c + 8 * a * a * d;
        double B = (b * b - 4 * a * c) * (b * b - 4 * a * c) + 2 * b * C - 64 * a * a * a * e;
        double q = (2 * A * A * A - 9 * A * B + 27 * C * C) / 2;
        double p = 3 * B - A * A;
        double delta = q * q + p * p * p;
        double s1 = Pow(-q + Sqrt(delta), 1 / 3);
        double s2 = Pow(-q - Sqrt(delta), 1 / 3);
        return s1 + s2;
    }

    private static double Heron(double a, double b, double c)
    {
        double p = 0.5 * (a + b + c);
        return Sqrt(p * (p - a) * (p - b) * (p - c));
    }

    private static bool IsTriangle(double a, double b, double c)
        => (a + b > c) && (a + c > b) && (b + c > a);

    private static double LinearEquation(double a, double b) => -b / a;

    private static double ParseFunction(string name, List<double> args)
    {
        return name.ToUpperInvariant( ) switch
        {
            // 单参数
            "ACOSH" or "ARCCOSH" => Acosh(args[0]),
            "ASINH" or "ARCSINH" => Asinh(args[0]),
            "CBRT" => Cbrt(args[0]),
            "COT" => 1.0 / Tan(args[0]),
            "CSC" => 1.0 / Cos(args[0]),
            "DEG" or "DEGREE" => Deg(args[0]),
            "ILOGB" => ILogB(args[0]),
            "LOGE" or "LN" => Log(args[0]),
            "LOG2" or "L2" => Log2(args[0]),
            "RAD" or "RADIAN" => Rad(args[0]),
            "SEC" => 1.0 / Sin(args[0]),
            "SGN" => Sign(args[0]),
            "SIN" => Sin(args[0]),
            "TRUNC" => Truncate(args[0]),

            // 双参数
            "ATAN2" => Atan2(args[0], args[1]),
            "LINEAR" or "LINEAREQUATION" => LinearEquation(args[0], args[1]),
            "SCALEB" => ScaleB(args[0], (int) args[1]),

            // 多参数
            "FUSEMULADD" or "FUSEDMULTIPLYADD" => FusedMultiplyAdd(args[0], args[1], args[2]),
            "TRI" or "ISTRIANGLE" => IsTriangle(args[0], args[1], args[2]) ? 1 : 0,
            "HERON" => Heron(args[0], args[1], args[2]),
            "QUADRATIC" => QuadraticEquation(true, args[0], args[1], args[2]),
            "QUADRADELTA" => QuadraticEquation(false, args[0], args[1], args[2]),
            "CUBIC" => CubicEquation(args[0], args[1], args[2], args[3]),
            "FOUR" => FourEquation(args[0], args[1], args[2], args[3], args[4]),

            _ => double.NaN,
        };
    }

    private static double ParseParameter(string name)
    {
        return name switch
        {
            "E" or "EE" => E,
            "PI" or "π" => PI,
            "TAU" => Tau,
            "INF" => double.PositiveInfinity,
            _ => double.NaN
        };
    }

    private static double QuadraticEquation(bool isSolve, double a, double b, double c)
    {
        double delta = b * b - 4 * a * c;
        return isSolve
            ? delta switch
            {
                > 0 => (-b + Sqrt(delta)) / (2 * a),
                0 => -b / (2 * a),
                < 0 => double.NaN,
                _ => double.NaN,
            }
            : delta;
    }

    private static double Rad(double deg) => deg * PI / 180;
}
