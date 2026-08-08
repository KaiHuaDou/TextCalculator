using System;
using System.Collections.Generic;

using NCalc;
using NCalc.Handlers;

using static System.Math;

namespace TextCalculator;
public static class Calculator
{
    private static readonly Dictionary<string, string> filters = new( )
    {
        // 中文标点
        {"“", ""}, {"”", ""}, {"‘", ""}, {"’", ""},
        {"（", "("}, {"）", ")"}, {"【", "("}, {"】", ")"},{"「", "("}, {"」", ")"},
        {"、", "/"}, {"，", ""},
        {"……", "^"}, {"——", "-"}, {"《", "<"}, {"》", ">"},
        // 汉字运算
        {"一", "1"}, {"二", "2"}, {"三", "3"}, {"四", "4"},
        {"五", "5"}, {"六", "6"}, {"七", "7"}, {"八", "8"},
        {"九", "9"}, {"十", "10"}, {"零", "0"},
        {"百", "00"}, {"千", "000"}, {"万", "0000"}, {"亿", "00000000"},
        {"加","+"}, {"减","-"}, {"乘","*"}, {"除","/"}, {"百分之", "0.01*"},
        // 特殊符号
        {"^", "**"}, {"×", "*"}, {"÷", "/"}, {"'", ""}, {",", ""},
        // 多级括号
        {"{", "("}, {"}", ")"}, {"[", "("}, {"]", ")"},
        // 数学运算
        {"%", "*0.01"}
    };

    public static string Calculate(string expr)
    {
        if (double.TryParse(expr, out _))
        {
            return "";
        }

        Expression e = new(expr)
        {
            Options = ExpressionOptions.IgnoreCaseAtBuiltInFunctions | ExpressionOptions.AllowBooleanCalculation
        };
        ParseFunction(e.Functions);
        e.EvaluateParameter += (name, args) =>
        {
            var result = ParseParameter(name);
            if (!double.IsNaN(result))
            {
                args.Result = result;
            }
        };
        try
        {
            return e.Evaluate<double>( ).ToString( );
        }
        catch
        {
            return "";
        }
    }

    public static (string, string) Filter(string expr)
    {
        foreach (var filter in filters)
        {
            expr = expr.Replace(filter.Key, filter.Value);
        }

        var equalMark = "=";
        while (expr.EndsWith('='))
        {
            expr = expr[..^1];
            equalMark = "";
        }

        return (expr, equalMark);
    }

    private static double CubicEquation(double a, double b, double c, double d)
    {
        var u = (9 * a * b * c - 27 * a * a * d - 2 * b * b * b) / (54 * a * a * a);
        var v = Sqrt(3 * (4 * a * c * c * c - b * b * c * c - 18 * a * b * c * d + 27 * a * a * d * d + 4 * b * b * b * d)) / (18 * a * a);
        double m = 0, n = 0;
        if (Abs(u + v) >= Abs(u - v))
        {
            m = Pow(u + v, 1.0 / 3);
        }
        else if (Abs(u + v) < Abs(u - v))
        {
            m = Pow(u - v, 1.0 / 3);
        }

        if (m != 0)
        {
            n = (b * b - 3 * a * c) / (9 * a * a * m);
        }
        else if (m == 0)
        {
            n = 0;
        }

        return m + n - b / (3 * a);
    }

    private static double Deg(double rad)
    {
        return rad * 180 / PI;
    }

    private static double FourEquation(double a, double b, double c, double d, double e)
    {
        var A = 8 * a * c - 3 * b * b;
        var C = b * b * b - 4 * a * b * c + 8 * a * a * d;
        var B = (b * b - 4 * a * c) * (b * b - 4 * a * c) + 2 * b * C - 64 * a * a * a * e;
        var q = (2 * A * A * A - 9 * A * B + 27 * C * C) / 2;
        var p = 3 * B - A * A;
        var delta = q * q + p * p * p;
        var s1 = Pow(-q + Sqrt(delta), 1 / 3);
        var s2 = Pow(-q - Sqrt(delta), 1 / 3);
        return s1 + s2;
    }

    private static double Heron(double a, double b, double c)
    {
        var p = 0.5 * (a + b + c);
        return Sqrt(p * (p - a) * (p - b) * (p - c));
    }

    private static bool IsTriangle(double a, double b, double c)
    {
        return (a + b > c) && (a + c > b) && (b + c > a);
    }

    private static double LinearEquation(double a, double b)
    {
        return -b / a;
    }

    private static void ParseFunction(IDictionary<string, ExpressionFunction> f)
    {
        f["ACOSH"] = args => Acosh((double) args.Evaluate(0));
        f["ARCCOSH"] = args => Acosh((double) args.Evaluate(0));
        f["ASINH"] = args => Asinh((double) args.Evaluate(0));
        f["ARCSINH"] = args => Asinh((double) args.Evaluate(0));
        f["CBRT"] = args => Cbrt((double) args.Evaluate(0));
        f["COT"] = args => 1.0 / Tan((double) args.Evaluate(0));
        f["CSC"] = args => 1.0 / Cos((double) args.Evaluate(0));
        f["DEG"] = args => Deg((double) args.Evaluate(0));
        f["DEGREE"] = args => Deg((double) args.Evaluate(0));
        f["ILOGB"] = args => ILogB((double) args.Evaluate(0));
        f["LOGE"] = args => Log((double) args.Evaluate(0));
        f["LN"] = args => Log((double) args.Evaluate(0));
        f["LOG2"] = args => Log2((double) args.Evaluate(0));
        f["L2"] = args => Log2((double) args.Evaluate(0));
        f["RAD"] = args => Rad((double) args.Evaluate(0));
        f["RADIAN"] = args => Rad((double) args.Evaluate(0));
        f["SEC"] = args => 1.0 / Sin((double) args.Evaluate(0));
        f["SGN"] = args => Sign((double) args.Evaluate(0));
        f["SIN"] = args => Sin((double) args.Evaluate(0));
        f["TRUNC"] = args => Truncate((double) args.Evaluate(0));

        f["ATAN2"] = args => Atan2((double) args.Evaluate(0), (double) args.Evaluate(1));
        f["LINEAR"] = args => LinearEquation((double) args.Evaluate(0), (double) args.Evaluate(1));

        f["TRI"] = args => IsTriangle((double) args.Evaluate(0), (double) args.Evaluate(1), (double) args.Evaluate(2)) ? 1.0 : 0.0;
        f["ISTRIANGLE"] = args => IsTriangle((double) args.Evaluate(0), (double) args.Evaluate(1), (double) args.Evaluate(2)) ? 1.0 : 0.0;
        f["HERON"] = args => Heron((double) args.Evaluate(0), (double) args.Evaluate(1), (double) args.Evaluate(2));
        f["QUAD"] = args => QuadraticEquation(true, (double) args.Evaluate(0), (double) args.Evaluate(1), (double) args.Evaluate(2));
        f["QUADDELTA"] = args => QuadraticEquation(false, (double) args.Evaluate(0), (double) args.Evaluate(1), (double) args.Evaluate(2));
        f["CUBIC"] = args => CubicEquation((double) args.Evaluate(0), (double) args.Evaluate(1), (double) args.Evaluate(2), (double) args.Evaluate(3));
        f["FOUR"] = args => FourEquation((double) args.Evaluate(0), (double) args.Evaluate(1), (double) args.Evaluate(2), (double) args.Evaluate(3), (double) args.Evaluate(4));
    }

    private static double ParseParameter(string name)
    {
        return name switch
        {
            "E" or "EE" => E,
            "PI" or "π" => PI,
            "TAU" or "τ" => Tau,
            "INF" => double.PositiveInfinity,
            _ => double.NaN
        };
    }

    private static double QuadraticEquation(bool isSolve, double a, double b, double c)
    {
        var delta = b * b - 4 * a * c;
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

    private static double Rad(double deg)
    {
        return deg * PI / 180;
    }
}
