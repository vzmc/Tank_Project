using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

        var A = accelerationVector.sqrMagnitude / 4;
        var B = -(Vector3.Dot(distanceVector, accelerationVector) + v * v);
        var C = distanceVector.sqrMagnitude;

        var delta = B * B - 4 * A * C;
        Debug.Log($"Delta = {delta}");

        if (delta < 0)
        {
            Debug.LogWarning("Delta < 0! No result!");
            return Vector3.zero;
        }

        var tt_1 = (-B + math.sqrt(delta)) / (2 * A);
        var tt_2 = (-B - math.sqrt(delta)) / (2 * A);
        Debug.Log($"tt_1 = {tt_1}, tt_2 = {tt_2}");

        var t_1 = tt_1 > 0 ? math.sqrt(tt_1) : 0;
        var t_2 = tt_2 > 0 ? math.sqrt(tt_2) : 0;
        Debug.Log($"t_1 = {t_1}, t_2 = {t_2}");

        if (t_1 <= 0 && t_2 <= 0)
        {
            Debug.LogWarning("Both t is illegal! No result!");
            return Vector3.zero;
        }
        
        var t = useMinTime ? math.min(t_1, t_2) : math.max(t_1, t_2);
        if (useMinTime && t <= 0)
        {
            t = math.max(t_1, t_2);
        }
        Debug.Log($"Result t = {t}");

        var vx = x / t - ax * t / 2;
        var vy = y / t - ay * t / 2;
        var vz = z / t - az * t / 2;
        
        var fireVector = new Vector3((float)vx, (float)vy, (float)vz);
        Debug.Log($"FireVector = {fireVector}");
        
        return fireVector;
    }
    
    public static Vector3 CalcFireVector(Vector3 distanceVector, Vector3 accelerationVector, float fortLength, float fireSpeed, bool useMinTime = true)
    {
        var v = fireSpeed;
        var l = fortLength;

        var x = distanceVector.x;
        var y = distanceVector.y;
        var z = distanceVector.z;
        
        var ax = accelerationVector.x;
        var ay = accelerationVector.y;
        var az = accelerationVector.z;
        
        var A = accelerationVector.sqrMagnitude / 4;
        var B = 0;
        var C = -(Vector3.Dot(distanceVector, accelerationVector) + v * v);
        var D = -2 * l / v;
        var E = distanceVector.sqrMagnitude - (l * l) / (v * v);


        // var delta = B * B - 4 * A * C;
        // Debug.Log($"Delta = {delta}");
        //
        // if (delta < 0)
        // {
        //     Debug.LogWarning("Delta < 0! No result!");
        //     return Vector3.zero;
        // }
        //
        // var tt_1 = (-B + math.sqrt(delta)) / (2 * A);
        // var tt_2 = (-B - math.sqrt(delta)) / (2 * A);
        // Debug.Log($"tt_1 = {tt_1}, tt_2 = {tt_2}");
        //
        // var t_1 = tt_1 > 0 ? math.sqrt(tt_1) : 0;
        // var t_2 = tt_2 > 0 ? math.sqrt(tt_2) : 0;
        // Debug.Log($"t_1 = {t_1}, t_2 = {t_2}");
        //
        // var rt_1 = t_1 > 0 ? t_1 - l / v : 0;
        // var rt_2 = t_2 > 0 ? t_2 - l / v : 0;
        // Debug.Log($"rt_1 = {rt_1}, rt_1 = {rt_1}");
        //
        // if (rt_1 <= 0 && rt_2 <= 0)
        // {
        //     Debug.LogWarning("Both t is illegal! No result!");
        //     return Vector3.zero;
        // }
        //
        // var t = useMinTime ? math.min(rt_1, rt_2) : math.max(rt_1, rt_2);
        // if (useMinTime && t <= 0)
        // {
        //     t = math.max(rt_1, rt_2);
        // }
        // Debug.Log($"Result t = {t}");
        //
        // var vx = (x - 0.5 * ax * t * t) / (t + l / v);
        // var vy = (y - 0.5 * ay * t * t) / (t + l / v);
        // var vz = (z - 0.5 * az * t * t) / (t + l / v);
        //
        // // var vx = x / t - ax * t / 2;
        // // var vy = y / t - ay * t / 2;
        // // var vz = z / t - az * t / 2;
        //
        // var fireVector = new Vector3((float)vx, (float)vy, (float)vz);
        // Debug.Log($"FireVector = {fireVector}");
        //
        return Vector3.zero;
    }
}
