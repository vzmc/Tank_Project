using System.Collections;
using System.Collections.Generic;
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

        var tt_1 = (-B + Mathf.Sqrt(delta)) / (2 * A);
        var tt_2 = (-B - Mathf.Sqrt(delta)) / (2 * A);
        Debug.Log($"tt_1 = {tt_1}, tt_2 = {tt_2}");

        var t_1 = tt_1 > 0 ? Mathf.Sqrt(tt_1) : float.PositiveInfinity;
        var t_2 = tt_2 > 0 ? Mathf.Sqrt(tt_2) : float.PositiveInfinity;
        Debug.Log($"t_1 = {t_1}, t_2 = {t_2}");

        if (float.IsPositiveInfinity(t_1) && float.IsPositiveInfinity(t_2))
        {
            Debug.LogWarning("Both t is illegal! No result!");
            return Vector3.zero;
        }
        
        var t = useMinTime ? Mathf.Min(t_1, t_2) : Mathf.Max(t_1, t_2);
        Debug.Log($"Result t = {t}");
        
        var vx = x / t - ax * t / 2;
        var vy = y / t - ay * t / 2;
        var vz = z / t - az * t / 2;
        
        var fireVector = new Vector3(vx, vy, vz);
        Debug.Log($"FireVector = {fireVector}");
        
        return fireVector;
    }
}
