using System;
using Unity.Mathematics;
using UnityEngine;

public static class EquationUtility
{
    public static (double?, double?) CalcQuadraticEquation(double a, double b, double c)
    {
        if (a == 0)
        {
            return (null, null);
        }

        var delta = b * b - 4 * a * c;
        if (delta < 0)
        {
            return (null, null);
        }
        
        var x1 = (-b - Math.Sqrt(delta)) / (2 * a);
        var x2 = (-b + Math.Sqrt(delta)) / (2 * a);
        
        return (x1, x2);
    }

    public static (double?, double?, double?) CalcCubicEquation(double a, double b, double c, double d)
    {
        var A = b * b - 3 * a * c;
        var B = b * c - 9 * a * d;
        var C = c * c - 3 * b * d;
        var delta = B * B - 4 * A * C;

        if (A == 0 && B == 0)
        {
            var x = -c / b;
            return (x, x, x);
        }

        if (delta > 0)
        {
            var y1 = A * b + 3 * a * ((-B + Math.Sqrt(delta)) / 2);
            var y2 = A * b + 3 * a * ((-B - Math.Sqrt(delta)) / 2);

            var oneThird = 1.0 / 3;
            var x1 = (-b - (Math.Pow(y1, oneThird) + Math.Pow(y2, oneThird))) / (3 * a);
            var x2 = (double?)null;
            var x3 = (double?)null;

            if (Math.Abs(y1 - y2) < float.Epsilon)
            {
                x2 = (-2 * b + (Math.Pow(y1, oneThird) + Math.Pow(y2, oneThird))) / (6 * a);
                x3 = x2;
            }

            return (x1, x2, x3);
        }

        if (delta == 0 && A != 0)
        {
            var k = B / A;
            var x1 = -b / a + k;
            var x2 = -k / 2;
            var x3 = x2;

            return (x1, x2, x3);
        }

        if (delta < 0 && A > 0)
        {
            var t = (2 * A * b - 3 * a * B) / (2 * A * Math.Sqrt(A));
            var theta = Math.Acos(t);
            var thetaD3 = theta / 3;
            var root3 = Math.Sqrt(3);

            var x1 = (-b - 2 * Math.Sqrt(A) * Math.Cos(thetaD3)) / (3 * a);
            var x2 = (-b + Math.Sqrt(A) * (Math.Cos(thetaD3) + root3 * Math.Sin(thetaD3))) / (3 * a);
            var x3 = (-b + Math.Sqrt(A) * (Math.Cos(thetaD3) - root3 * Math.Sin(thetaD3))) / (3 * a);
            
            return (x1, x2, x3);
        }
        
        return (null, null, null);
    }

    public static (double?, double?, double?, double?) CalcQuarticEquation_SP(double a4, double a2, double a1, double a0)
    {
        var p = a2 / a4;
        var q = a1 / a4;
        var r = a0 / a4;
        
        var a = 1f;
        var b = 2 * p;
        var c = p * p - 4 * r;
        var d = -q * q;

        var us = CalcCubicEquation(a, b, c, d);
        // Debug.Log($"u1 = {us.Item1}, u2 = {us.Item2}, u3 = {us.Item3}");

        var u = 0.0;
        if (us.Item1.HasValue && us.Item1.Value > 0)
        {
            u = us.Item1.Value;
        }
        else if (us.Item2.HasValue && us.Item2.Value > 0)
        {
            u = us.Item2.Value;
        }
        else if (us.Item3.HasValue && us.Item3.Value > 0)
        {
            u = us.Item3.Value;
        }
        else
        {
            return (null, null, null, null);
        }

        var a_1 = 1;
        var b_1 = Math.Sqrt(u);
        var c_1 = (p + u) / 2 - (Math.Sqrt(u) * q) / (2 * u);

        var x12 = CalcQuadraticEquation(a_1, b_1, c_1);
        
        var a_2 = 1;
        var b_2 = -Math.Sqrt(u);
        var c_2 = (p + u) / 2 + (Math.Sqrt(u) * q) / (2 * u);
        
        var x34 = CalcQuadraticEquation(a_2, b_2, c_2);
        
        return (x12.Item1, x12.Item2, x34.Item1, x34.Item2);
    }
}
