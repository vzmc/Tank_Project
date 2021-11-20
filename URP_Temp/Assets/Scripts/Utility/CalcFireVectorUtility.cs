using System;
using System.Linq;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// 放物線砲弾発射方向ベクトルを求める
    /// </summary>
    public static class CalcFireVectorUtility
    {
        public static Vector3 CalcFireVector(Vector3 distanceVector, Vector3 accelerationVector, float fireSpeed, bool useMinTime = true)
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
                return Vector3.zero;
            }

            var tArray = resultList.Where(tt => tt > 0).Select(Math.Sqrt).ToArray();
            var t = useMinTime ? tArray.Min() : tArray.Max();
            Debug.Log($"Result t = {t}");

            var vx = x / t - ax * t / 2;
            var vy = y / t - ay * t / 2;
            var vz = z / t - az * t / 2;
        
            var fireVector = new Vector3((float)vx, (float)vy, (float)vz);
            Debug.Log($"FireVector = {fireVector}");

            return fireVector;
        }
    
        public static Vector3 CalcFireVector(Vector3 distanceVector, Vector3 accelerationVector, float fortLength, float shellSpeed, bool useMinTime = true)
        {
            if (fortLength == 0)
            {
                return CalcFireVector(distanceVector, accelerationVector, shellSpeed, useMinTime);
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
            if (!resultList.Any(t => t > 0))
            {
                Debug.LogWarning("Out of range!");
                return Vector3.zero;
            }

            var tArray = resultList.Where(t => t > 0).ToArray();
            var t = useMinTime ? tArray.Min() : tArray.Max();
            Debug.Log($"t = {t}");

            var vx = (x - ax * t * t / 2) / (t + l / v);
            var vy = (y - ay * t * t / 2) / (t + l / v);
            var vz = (z - az * t * t / 2) / (t + l / v);

            var fireVector = new Vector3((float)vx, (float)vy, (float)vz);
            Debug.Log($"FireVector = {fireVector}");

            return fireVector;
        }
    }
}
