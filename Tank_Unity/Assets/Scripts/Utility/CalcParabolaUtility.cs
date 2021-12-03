using UnityEngine;

namespace Utility
{
    /// <summary>
    /// 放物線計算ユーティリティ
    /// </summary>
    public static class CalcParabolaUtility
    {
        /// <summary>
        /// 放物線上の座標を求める
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="startVelocity"></param>
        /// <param name="accelerationVector"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Vector3 CalcParabolaPoint(Vector3 startPosition, Vector3 startVelocity, Vector3 accelerationVector, float time)
        {
            var x = CalcParabolaFunc(startPosition.x, startVelocity.x, accelerationVector.x, time);
            var y = CalcParabolaFunc(startPosition.y, startVelocity.y, accelerationVector.y, time);
            var z = CalcParabolaFunc(startPosition.z, startVelocity.z, accelerationVector.z, time);
            
            return new Vector3(x, y, z);
        }
        
        private static float CalcParabolaFunc(float s0, float v0, float a, float t)
        {
            return s0 + v0 * t + 0.5f * a * t * t;
        }
    }
}
