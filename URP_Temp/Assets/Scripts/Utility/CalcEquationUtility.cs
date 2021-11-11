using System;
using System.Linq;

namespace Utility
{
    public static class CalcEquationUtility
    {
        public static double CalcLinearEquation(double a, double b)
        {
            var x = -b / a;
            return x;
        }
    
        public static double?[] CalcQuadraticEquation(double a, double b, double c)
        {
            if (a == 0)
            {
                return new double?[] {null, null};
            }

            var delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                return new double?[] {null, null};;
            }

            var rootDelta = Math.Sqrt(delta);
            var x1 = (-b - rootDelta) / (2 * a);
            var x2 = (-b + rootDelta) / (2 * a);
        
            return new double?[] {x1, x2};
        }

        public static double?[] CalcCubicEquation(double a, double b, double c, double d)
        {
            var A = b * b - 3 * a * c;
            var B = b * c - 9 * a * d;
            var C = c * c - 3 * b * d;
            var delta = B * B - 4 * A * C;

            if (A == 0 && B == 0)
            {
                var x = -c / b;
                return new double?[] {x, x, x};
            }

            if (delta > 0)
            {
                var rootDelta = Math.Sqrt(delta);
                var y1 = A * b + 3 * a * ((-B + rootDelta) / 2);
                var y2 = A * b + 3 * a * ((-B - rootDelta) / 2);

                const double oneThird = 1.0 / 3.0;
                var x1 = (-b - (Math.Pow(y1, oneThird) + Math.Pow(y2, oneThird))) / (3 * a);
                var x2 = (double?)null;
                var x3 = (double?)null;

                if (Math.Abs(y1 - y2) < float.Epsilon)
                {
                    x2 = (-2 * b + (Math.Pow(y1, oneThird) + Math.Pow(y2, oneThird))) / (6 * a);
                    x3 = x2;
                }

                return new double?[] {x1, x2, x3};
            }

            if (delta == 0 && A != 0)
            {
                var k = B / A;
                var x1 = -b / a + k;
                var x2 = -k / 2;
                var x3 = x2;

                return new double?[] {x1, x2, x3};
            }

            if (delta < 0 && A > 0)
            {
                var rootA = Math.Sqrt(A);
                var t = (2 * A * b - 3 * a * B) / (2 * A * rootA);
                var theta = Math.Acos(t);
                var thetaD3 = theta / 3;
                var cosThetaD3 = Math.Cos(thetaD3);
                var sinThetaD3 = Math.Sin(thetaD3);
                var root3 = Math.Sqrt(3);

                var x1 = (-b - 2 * rootA * cosThetaD3) / (3 * a);
                var x2 = (-b + rootA * (cosThetaD3 + root3 * sinThetaD3)) / (3 * a);
                var x3 = (-b + rootA * (cosThetaD3 - root3 * sinThetaD3)) / (3 * a);
            
                return new double?[] {x1, x2, x3};
            }
        
            return new double?[] {null, null, null};
        }

        public static double?[] CalcQuarticEquation_SP(double a4, double a2, double a1, double a0)
        {
            var p = a2 / a4;
            var q = a1 / a4;
            var r = a0 / a4;
        
            var a = 1f;
            var b = 2 * p;
            var c = p * p - 4 * r;
            var d = -q * q;

            var uArray = CalcCubicEquation(a, b, c, d);
            var ut = uArray.FirstOrDefault(x => x > 0);
            if (!ut.HasValue)
            {
                return new double?[] {null, null, null, null};
            }

            var u = ut.Value;
            var rootU = Math.Sqrt(u);

            var a_1 = 1;
            var b_1 = rootU;
            var c_1 = (p + u) / 2 - (rootU * q) / (2 * u);

            var x12 = CalcQuadraticEquation(a_1, b_1, c_1);
        
            var a_2 = 1;
            var b_2 = -rootU;
            var c_2 = (p + u) / 2 + (rootU * q) / (2 * u);
        
            var x34 = CalcQuadraticEquation(a_2, b_2, c_2);
        
            return new double?[] {x12[0], x12[1], x34[0], x34[1]};
        }
    }
}
