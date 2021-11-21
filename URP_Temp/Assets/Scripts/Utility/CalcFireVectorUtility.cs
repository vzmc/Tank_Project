using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// 放物線砲弾発射方向計算ユーティリティ
    /// </summary>
    public static class CalcFireVectorUtility
    {
        /// <summary>
        /// 砲身長さ考慮しない関数
        /// </summary>
        /// <param name="fireVector"></param>
        /// <param name="distanceVector"></param>
        /// <param name="accelerationVector"></param>
        /// <param name="fireSpeed"></param>
        /// <param name="useMinTime"></param>
        /// <returns></returns>
        public static bool CalcFireVector(ref Vector3 fireVector, Vector3 distanceVector, Vector3 accelerationVector, float fireSpeed, bool useMinTime = true)
        {
            var v = fireSpeed;

            var x = distanceVector.x;
            var y = distanceVector.y;
            var z = distanceVector.z;
        
            var ax = accelerationVector.x;
            var ay = accelerationVector.y;
            var az = accelerationVector.z;

            var a = accelerationVector.sqrMagnitude / 4;
            var b = -(Vector3.Dot(distanceVector, accelerationVector) + Mathf.Pow(v, 2));
            var c = distanceVector.sqrMagnitude;

            var resultList = CalcEquationUtility.CalcQuadraticEquation(a, b, c);
            if (!resultList.Any(tt => tt > 0))
            {
                Debug.LogWarning("Out of range!");
                return false;
            }

            var tArray = resultList.Where(tt => tt > 0).Select(Math.Sqrt).ToArray();
            var t = useMinTime ? tArray.Min() : tArray.Max();
            Debug.Log($"Result t = {t}");

            var vx = x / t - 0.5 * ax * t;
            var vy = y / t - 0.5 * ay * t;
            var vz = z / t - 0.5 * az * t;
        
            fireVector.Set((float)vx, (float)vy, (float)vz);
            Debug.Log($"FireVector = {fireVector}");

            return true;
        }

        /// <summary>
        /// 砲身長さ考慮する関数
        /// </summary>
        /// <param name="fireVector"></param>
        /// <param name="distanceVector"></param>
        /// <param name="accelerationVector"></param>
        /// <param name="fortLength"></param>
        /// <param name="shellSpeed"></param>
        /// <param name="useMinTime"></param>
        /// <returns></returns>
        public static bool CalcFireVector(ref Vector3 fireVector, Vector3 distanceVector, Vector3 accelerationVector, float fortLength, float shellSpeed, bool useMinTime = true)
        {
            if (Mathf.Abs(fortLength) <= float.Epsilon)
            {
                return CalcFireVector(ref fireVector, distanceVector, accelerationVector, shellSpeed, useMinTime);
            }
            
            var v = shellSpeed;
            var l = fortLength;

            var x = distanceVector.x;
            var y = distanceVector.y;
            var z = distanceVector.z;
        
            var ax = accelerationVector.x;
            var ay = accelerationVector.y;
            var az = accelerationVector.z;
        
            var a4 = accelerationVector.sqrMagnitude / 4;
            var a2 = -(Vector3.Dot(distanceVector, accelerationVector) + Mathf.Pow(v, 2));
            var a1 = -2 * l * v;
            var a0 = distanceVector.sqrMagnitude - Mathf.Pow(l, 2);
            Debug.Log($"a4 = {a4}, a2 = {a2}, a1 = {a1}, a0 = {a0}");

            var resultList = CalcEquationUtility.CalcQuarticEquation(a4, 0, a2, a1, a0);
            var str = new StringBuilder();
            resultList.ForEach(t => str.Append($"{t}, "));
            Debug.Log($"resultList = {str}");
            if (!resultList.Any(t => t > 0))
            {
                Debug.LogWarning("Out of range!");
                return false;
            }

            var tArray = resultList.Where(t => t > 0).ToArray();
            var t = useMinTime ? tArray.Min() : tArray.Max();
            Debug.Log($"t = {t}");

            var vx = (x - 0.5 * ax * t * t) / (t + l / v);
            var vy = (y - 0.5 * ay * t * t) / (t + l / v);
            var vz = (z - 0.5 * az * t * t) / (t + l / v);

            fireVector.Set((float)vx, (float)vy, (float)vz);
            Debug.Log($"FireVector = {fireVector}");

            return true;
        }
    }
}
