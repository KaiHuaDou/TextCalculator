using System.Collections.Generic;
using System.Diagnostics;
using NCalc;
using static System.Math;

namespace TextCalculator;
public static class Calculator
{
    public static double Deg(double rad) => rad * 180 / PI;
    public static double Rad(double deg) => deg * PI / 180;

    public static double Heron(double a, double b, double c)
    {
        double p = 0.5 * (a + b + c);
        return Sqrt(p * (p - a) * (p - b) * (p - c));
    }

    public static double LinearEquation(double a, double b) => -b / a;

    public static double QuadraticEquation(bool isSolve, double a, double b, double c)
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

    public static double CubicEquation(double a, double b, double c, double d)
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

    public static bool IsTriangle(double a, double b, double c)
        => (a + b > c) && (a + c > b) && (b + c > a);

    public static double Calculate(string expr)
    {
        Expression e = new(expr.ToUpperInvariant( ));
        e.EvaluateFunction += (string name, FunctionArgs args) =>
        {
            List<double> parameters = [];
            foreach (Expression parameter in args.Parameters)
            {
                try
                {
                    parameters.Add(double.Parse(parameter.Evaluate( ).ToString( )));
                }
                catch
                {
                    args.Result = double.NaN;
                    return;
                }
            }
            args.Result = ParseFunction(name, parameters);
        };
        e.EvaluateParameter += (string name, ParameterArgs args) => args.Result = ParseParameter(name);
        string evaluated = "NaN";
        try { evaluated = e.Evaluate( ).ToString( ); } catch { }
        return double.TryParse(evaluated, out double result) ? result : double.NaN;
    }

    public static string ParseFunction(string name, IList<double> args)
    {
        return (name switch
        {
            // 单参数
            "ABS" => Abs(args[0]),
            "ACOS" or "ARCCOS" => Acos(args[0]),
            "ACOSH" or "ARCCOSH" => Acosh(args[0]),
            "ASIN" or "ARCSIN" => Asin(args[0]),
            "ASINH" or "ARCSINH" => Asinh(args[0]),
            "ATAN" or "ARCTAN" => Atan(args[0]),
            "CBRT" => Cbrt(args[0]),
            "CEIL" or "CEILING" => Ceiling(args[0]),
            "COS" => Cos(args[0]),
            "COT" => 1.0 / Tan(args[0]),
            "CSC" => 1.0 / Cos(args[0]),
            "DEG" or "DEGREE" => Deg(args[0]),
            "EXP" => Exp(args[0]),
            "FLOOR" => Floor(args[0]),
            "ILOGB" => ILogB(args[0]),
            "LOGE" or "LN" => Log(args[0]),
            "LOG10" or "LG" => Log10(args[0]),
            "LOG2" or "L2" => Log2(args[0]),
            "RAD" or "RADIAN" => Rad(args[0]),
            "RND" or "ROUND" => Round(args[0]),
            "SEC" => 1.0 / Sin(args[0]),
            "SGN" or "SIGN" => Sign(args[0]),
            "SIN" => Sin(args[0]),
            "SQRT" => Sqrt(args[0]),
            "TAN" => Tan(args[0]),
            "TRUNC" or "TRUNCATE" => Truncate(args[0]),

            // 双参数
            "ATAN2" => Atan2(args[0], args[1]),
            "LOG" => Log(args[0], args[1]),
            "LINEAR" or "LINEAREQUATION" => LinearEquation(args[0], args[1]),
            "POW" => Pow(args[0], args[1]),
            "SCALEB" => ScaleB(args[0], (int) args[1]),

            // 多参数
            "FUSEMULADD" or "FUSEDMULTIPLYADD" => FusedMultiplyAdd(args[0], args[1], args[2]),
            "TRI" or "ISTRIANGLE" => IsTriangle(args[0], args[1], args[2]) ? 1 : 0,
            "HERON" => Heron(args[0], args[1], args[2]),
            "QUADRATIC" or "QUADRATICEQUATION" => QuadraticEquation(true, args[0], args[1], args[2]),
            "QUADRADELTA" or "QUADRATICDELTA" => QuadraticEquation(false, args[0], args[1], args[2]),
            "CUBIC" or "CUBICEQUATION" => CubicEquation(args[0], args[1], args[2], args[3]),

            _ => double.NaN,
        }).ToString( );
    }

    public static string ParseParameter(string name)
    {
        return (name switch
        {
            "E" => E,
            "PI" or "π" => PI,
            "TAU" => Tau,
            _ => double.NaN
        }).ToString( );
    }

    public static string ExprFilter(string expr)
    {
        expr = expr.Trim( );
        (string, string)[] filters =
        [
            ("“","\""), ("”","\""), ("（","("), ("）",")"), ("、", "/"),
            ("×", "*"), ("÷", "/"), ("=", "")
        ];
        foreach ((string, string) filter in filters)
            expr = expr.Replace(filter.Item1, filter.Item2);
        return expr;
    }
}
