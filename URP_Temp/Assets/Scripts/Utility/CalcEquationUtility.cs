using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    /// <summary>
    /// 方程式の解を求める
    /// </summary>
    public static class CalcEquationUtility
    {
        /// <summary>
        /// 一次方程式
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a0"></param>
        /// <returns></returns>
        public static List<double> CalcLinearEquation(double a1, double a0)
        {
            if (a1 == 0)
            {
                return new List<double>();
            }
            
            var x = -a0 / a1;
            return new List<double> { x };
        }
    
        /// <summary>
        /// 二次方程式
        /// </summary>
        /// <param name="a2"></param>
        /// <param name="a1"></param>
        /// <param name="a0"></param>
        /// <returns></returns>
        public static List<double> CalcQuadraticEquation(double a2, double a1, double a0)
        {
            if (a2 == 0)
            {
                return CalcLinearEquation(a1, a0);
            }

            var delta = a1 * a1 - 4 * a2 * a0;
            if (delta < 0)
            {
                return new List<double>();
            }

            var rootDelta = Math.Sqrt(delta);
            var x1 = (-a1 - rootDelta) / (2 * a2);
            var x2 = (-a1 + rootDelta) / (2 * a2);

            return new List<double> { x1, x2 };
        }

        /// <summary>
        /// 三次方程式
        /// </summary>
        /// <param name="a3"></param>
        /// <param name="a2"></param>
        /// <param name="a1"></param>
        /// <param name="a0"></param>
        /// <returns></returns>
        public static List<double> CalcCubicEquation(double a3, double a2, double a1, double a0)
        {
            if (a3 == 0)
            {
                return CalcQuadraticEquation(a2, a1, a0);
            }
            
            var A = a2 * a2 - 3 * a3 * a1;
            var B = a2 * a1 - 9 * a3 * a0;
            var C = a1 * a1 - 3 * a2 * a0;
            var delta = B * B - 4 * A * C;

            if (A == 0 && B == 0)
            {
                var x = -a1 / a2;
                return new List<double> {x, x, x};
            }

            if (delta > 0)
            {
                var rootDelta = Math.Sqrt(delta);
                var y1 = A * a2 + 3 * a3 * ((-B + rootDelta) / 2);
                var y2 = A * a2 + 3 * a3 * ((-B - rootDelta) / 2);
                
                const double oneThird = 1.0 / 3.0;
                var x1 = (-a2 - (Math.Pow(y1, oneThird) + Math.Pow(y2, oneThird))) / (3 * a3);
                if (!(Math.Abs(y1 - y2) < float.Epsilon))
                {
                    return new List<double> { x1 };
                }
                
                var x2 = (-2 * a2 + (Math.Pow(y1, oneThird) + Math.Pow(y2, oneThird))) / (6 * a3);
                var x3 = x2;
                return new List<double> { x1, x2, x3 };
            }

            if (delta == 0 && A != 0)
            {
                var k = B / A;
                var x1 = -a2 / a3 + k;
                var x2 = -k / 2;
                var x3 = x2;

                return new List<double> { x1, x2, x3 };
            }

            if (delta < 0 && A > 0)
            {
                var rootA = Math.Sqrt(A);
                var t = (2 * A * a2 - 3 * a3 * B) / (2 * A * rootA);
                var theta = Math.Acos(t);
                var thetaD3 = theta / 3;
                var cosThetaD3 = Math.Cos(thetaD3);
                var sinThetaD3 = Math.Sin(thetaD3);
                var root3 = Math.Sqrt(3);

                var x1 = (-a2 - 2 * rootA * cosThetaD3) / (3 * a3);
                var x2 = (-a2 + rootA * (cosThetaD3 + root3 * sinThetaD3)) / (3 * a3);
                var x3 = (-a2 + rootA * (cosThetaD3 - root3 * sinThetaD3)) / (3 * a3);

                return new List<double> { x1, x2, x3 };
            }

            return new List<double>();
        }

        /// <summary>
        /// 四次方程式
        /// </summary>
        /// <param name="a4"></param>
        /// <param name="a3"></param>
        /// <param name="a2"></param>
        /// <param name="a1"></param>
        /// <param name="a0"></param>
        /// <returns></returns>
        public static List<double> CalcQuarticEquation(double a4, double a3, double a2, double a1, double a0)
        {
            if (a4 == 0)
            {
                return CalcCubicEquation(a3, a2, a1, a0);
            }
            
            var A4 = 1.0;
            var A3 = a3 / a4;
            var A2 = a2 / a4;
            var A1 = a1 / a4;
            var A0 = a0 / a4;

            var B3 = A3 / 4;
            
            var na4 = 1.0;
            var na2 = A2 - 6 * B3 * B3;
            var na1 = A1 - 2 * A2 * B3 + 8 * B3 * B3 * B3;
            var na0 = A0 - A1 * B3 + A2 * B3 * B3 - 3 * B3 * B3 * B3 * B3;

            var resultList = CalcQuarticEquationWithoutA3(na4, na2, na1, na0).Select(y => y - B3).ToList();
            return resultList;
        }
        
        /// <summary>
        /// 四次方程式(三次の項目がない時)
        /// </summary>
        /// <param name="a4"></param>
        /// <param name="a2"></param>
        /// <param name="a1"></param>
        /// <param name="a0"></param>
        /// <returns></returns>
        public static List<double> CalcQuarticEquationWithoutA3(double a4, double a2, double a1, double a0)
        {
            if (a4 == 0)
            {
                return CalcQuadraticEquation(a2, a1, a0);
            }
            
            var p = a2 / a4;
            var q = a1 / a4;
            var r = a0 / a4;
        
            var a = 1.0;
            var b = 2 * p;
            var c = p * p - 4 * r;
            var d = -q * q;

            var uList = CalcCubicEquation(a, b, c, d);
            if (!uList.Any(x => x >= 0))
            {
                return new List<double>();
            }

            var u = uList.First(x => x >= 0);
            var rootU = Math.Sqrt(u);

            var a_1 = 1.0;
            var b_1 = rootU;
            var c_1 = (p + u) / 2 - (rootU * q) / (2 * u);

            var x12 = CalcQuadraticEquation(a_1, b_1, c_1);
        
            var a_2 = 1.0;
            var b_2 = -rootU;
            var c_2 = (p + u) / 2 + (rootU * q) / (2 * u);
        
            var x34 = CalcQuadraticEquation(a_2, b_2, c_2);

            var resultList = x12.Concat(x34).ToList();
            return resultList;
        }
    }
}
