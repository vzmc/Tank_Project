using System;
using System.Linq;
using UnityEngine;

public static class FireUtility
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

        var results = EquationUtility.CalcQuadraticEquation(a, b, c);
        var tList = results.Where(tt => tt > 0).Select(tt => Math.Sqrt(tt.Value)).ToArray();
        if (!tList.Any())
        {
            Debug.LogWarning("Out of range!");
            return Vector3.zero;
        }

        var t = useMinTime ? tList.Min() : tList.Max();
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

        var results = EquationUtility.CalcQuarticEquation_SP(a4, a2, a1, a0);
        Debug.Log($"t1 = {results[0]}, t2 = {results[1]}, t3 = {results[2]}, t4 = {results[3]}");

        var tList = results.Where(t => t > 0).Select(t => t.Value).ToArray();
        if (!tList.Any())
        {
            Debug.LogWarning("Out of range!");
            return Vector3.zero;
        }

        var t = useMinTime ? tList.Min() : tList.Max();
        Debug.Log($"t = {t}");

        var vx = (x - ax * t * t / 2) / (t + l / v);
        var vy = (y - ay * t * t / 2) / (t + l / v);
        var vz = (z - az * t * t / 2) / (t + l / v);

        var fireVector = new Vector3((float)vx, (float)vy, (float)vz);
        Debug.Log($"FireVector = {fireVector}");

        return fireVector;
    }

    // 問題あり
    // public static Vector3 CalcFireVector_Error(Vector3 distanceVector, Vector3 accelerationVector, double fortLength, double fireSpeed, bool useMinTime = true)
    // {
    //     var v = fireSpeed;
    //     var l = fortLength;
    //
    //     var x = distanceVector.x;
    //     var y = distanceVector.y;
    //     var z = distanceVector.z;
    //     
    //     var ax = accelerationVector.x;
    //     var ay = accelerationVector.y;
    //     var az = accelerationVector.z;
    //     
    //     var a4 = accelerationVector.sqrMagnitude / 4.0;
    //     var a2 = -(Vector3.Dot(distanceVector, accelerationVector) + Math.Pow(v, 2));
    //     var a1 = -2 * l * v;
    //     var a0 = distanceVector.sqrMagnitude - Math.Pow(l, 2);
    //     
    //     a2 /= a4;
    //     a1 /= a4;
    //     a0 /= a4;
    //     a4 = 1;
    //     
    //     Debug.Log($"a2 = {a2}, a1 = {a1}, a0 = {a0}");
    //
    //     var i5 = Math.Pow(a2, 2) + 12 * a0;
    //     var i6 = 2 * Math.Pow(a2, 3) + 27 * Math.Pow(a1, 2) - 72 * a0 * a2;
    //     
    //     var i7 = 4 * Math.Pow(i5, 3) + Math.Pow(i6, 2);
    //     if (i7 < 0)
    //     {
    //         Debug.LogWarning($"i7 = {i7} < 0. No result!");
    //         return Vector3.zero;
    //     }
    //     
    //     var i8 = i6 + Math.Sqrt(i7);
    //     if (i8 == 0)
    //     {
    //         Debug.LogWarning("i8 = 0. No result!");
    //         return Vector3.zero;
    //     }
    //     
    //     const double oneThird = 1.0 / 3;
    //     var i4 = (Math.Pow(2, oneThird) * i5) / (3 * Math.Pow(i8, oneThird)) + Math.Pow(i8, oneThird) / (3 * Math.Pow(2, oneThird)) - 2 * a2 / 3;
    //     if (i4 < 0)
    //     {
    //         Debug.LogWarning($"i4 = {i4} < 0. No result!");
    //     }
    //     
    //     var i3 = -8 * a1;
    //     
    //     var xList = new List<double>();
    //     var i2 = -i3 / (4 * Math.Sqrt(i4)) - (Math.Pow(2, oneThird) * i5) / (3 * Math.Pow(i8, oneThird)) - Math.Pow(i8, oneThird) / (3 * Math.Pow(2, oneThird)) - 4 * a2 / 3;
    //     if (i2 >= 0)
    //     {
    //         xList.Add(-Math.Sqrt(i2) / 2 - Math.Sqrt(i4) / 2);
    //         xList.Add(Math.Sqrt(i2) / 2 - Math.Sqrt(i4) / 2);
    //     }
    //     
    //     var i1 = i3 / (4 * Math.Sqrt(i4)) - (Math.Pow(2, oneThird) * i5) / (3 * Math.Pow(i8, oneThird)) - Math.Pow(i8, oneThird) / (3 * Math.Pow(2, oneThird)) - 4 * a2 / 3;
    //     if (i1 >= 0)
    //     {
    //         xList.Add(-Math.Sqrt(i1) / 2 + Math.Sqrt(i4) / 2);
    //         xList.Add(Math.Sqrt(i1) / 2 + Math.Sqrt(i4) / 2);
    //     }
    //
    //     StringBuilder sb = new StringBuilder();
    //     xList.ForEach(a => sb.Append(a + " | "));
    //     Debug.Log($"xList = {sb}");
    //
    //     xList = xList.Where(a => a > 0).ToList();
    //     if (xList.Count == 0)
    //     {
    //         Debug.Log("All result <= 0. No result!");
    //         return Vector3.zero;
    //     }
    //     
    //     var t = xList.Min();
    //     Debug.Log($"t = {t}");
    //
    //     var vx = (x - ax * t * t / 2) / (t + l / v);
    //     var vy = (y - ay * t * t / 2) / (t + l / v);
    //     var vz = (z - az * t * t / 2) / (t + l / v);
    //
    //     var fireVector = new Vector3((float)vx, (float)vy, (float)vz);
    //     Debug.Log($"FireVector = {fireVector}");
    //     
    //     Debug.Log($"FireSpeed = {fireVector.magnitude}");
    //     
    //     return fireVector;
    // }
}
