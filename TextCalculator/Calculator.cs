using System.Collections.Generic;
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

    public static double Calculate(string expr)
    {
        if (string.IsNullOrWhiteSpace(expr))
            return double.NaN;
        Expression e = new(expr.ToUpperInvariant( ));
        e.EvaluateFunction += (string name, FunctionArgs args) =>
        {
            if (double.TryParse(args.Parameters[0].Evaluate( ).ToString( ), out double result) != true)
            {
                args.Result = double.NaN;
                return;
            }
            List<double> parameters = [];
            foreach (Expression parameter in args.Parameters)
            {
                parameters.Add(double.Parse(parameter.Evaluate( ).ToString( )));
            }
            args.Result = ParseFunction(name, parameters);
        };
        e.EvaluateParameter += (string name, ParameterArgs args) => args.Result = ParseParameter(name);
        try
        {
            string evaluated = e.Evaluate( ).ToString( );
            return double.TryParse(evaluated, out double result) ? result : double.NaN;
        }
        catch
        {
            return double.NaN;
        }
    }

    public static double ParseFunction(string name, List<double> args)
    {
        return name switch
        {
            // 单参数
            "ABS" => Abs(args[0]),
            "ACOS" => Acos(args[0]),
            "ACOSH" => Acosh(args[0]),
            "ASIN" => Asin(args[0]),
            "ASINH" => Asinh(args[0]),
            "ATAN" => Atan(args[0]),
            "CBRT" => Cbrt(args[0]),
            "CEIL" or "CEILING" => Ceiling(args[0]),
            "COS" => Cos(args[0]),
            "COT" => 1.0 / Tan(args[0]),
            "CSC" => 1.0 / Cos(args[0]),
            "DEG" => Deg(args[0]),
            "EXP" => Exp(args[0]),
            "FLOOR" => Floor(args[0]),
            "ILOGB" => ILogB(args[0]),
            "LOG" => Log(args[0]),
            "LOG10" => Log10(args[0]),
            "LOG2" => Log2(args[0]),
            "RAD" => Rad(args[0]),
            "RND" or "ROUND" => Round(args[0]),
            "SEC" => 1.0 / Sin(args[0]),
            "SGN" or "SIGN" => Sign(args[0]),
            "SIN" => Sin(args[0]),
            "SQRT" => Sqrt(args[0]),
            "TAN" => Tan(args[0]),
            "TRUNC" or "TRUNCATE" => Truncate(args[0]),

            // 双参数
            "ATAN2" => Atan2(args[0], args[1]),
            "POW" => Pow(args[0], args[1]),
            "SCALEB" => ScaleB(args[0], (int) args[1]),

            // 多参数
            "FUSEMULADD" or "FUSEDMULTIPLYADD" => FusedMultiplyAdd(args[0], args[1], args[2]),
            "HERON" => Heron(args[0], args[1], args[2]),

            _ => double.NaN,
        };
    }

    public static double ParseParameter(string name)
    {
        return name switch
        {
            "E" => E,
            "PI" => PI,
            "TAU" => Tau,
            _ => double.NaN
        };
    }

    public static string ExprFilter(string expr)
    {
        expr = expr.Trim( );
        (string, string)[] filters =
        [
            ("“","\""), ("”","\""), ("（","("), ("）",")"), ("、", "/")
        ];
        foreach ((string, string) filter in filters)
        {
            expr = expr.Replace(filter.Item1, filter.Item2);
        }
        return expr;
    }
}
