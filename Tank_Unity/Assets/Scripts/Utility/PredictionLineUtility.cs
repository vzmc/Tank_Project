using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// 予測線計算用ユーティリティ
    /// </summary>
    public static class PredictionLineUtility
    {
        public static IReadOnlyCollection<Vector3> GetParabolaPredictionPoints(Vector3 startPoint, Vector3 startVelocity, Vector3 acceleration, float timeStep, int maxStep,
            out Vector3? outHitPoint, int downSample = 1, float hitCheckRadius = 0, int hitCheckLayerMask = Physics.DefaultRaycastLayers)
        {
            outHitPoint = null;
            var pointList = new List<Vector3> { startPoint };
            
            var currentPoint = startPoint;
            for (var step = 1; step <= maxStep; step++)
            {
                var previousPoint = currentPoint;
                currentPoint = CalcParabolaPoint(startPoint, startVelocity, acceleration, timeStep * step);
                
                var checkRay = new Ray(currentPoint, currentPoint - previousPoint);
                var checkDistance = (currentPoint - previousPoint).magnitude;
                if (PhysicsCast(checkRay, hitCheckRadius, out var hitInfo, checkDistance, hitCheckLayerMask))
                {
                    pointList.Add(hitInfo.point);
                    outHitPoint = hitInfo.point;
                    break;
                }

                if (downSample > 1)
                {
                    if (step % downSample == 0)
                    {
                        pointList.Add(currentPoint);
                    }
                }
                else
                {
                    pointList.Add(currentPoint);
                }
            }
            
            return pointList;
        }

        /// <summary>
        /// 放物線上の座標を求める
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="startVelocity"></param>
        /// <param name="accelerationVector"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static Vector3 CalcParabolaPoint(Vector3 startPosition, Vector3 startVelocity, Vector3 accelerationVector, float time)
        {
            return startPosition + time * startVelocity + 0.5f * time * time * accelerationVector;
        }
        
        private static bool PhysicsCast(Ray ray, float radius, out RaycastHit info, float maxDistance, int layerMask)
        {
            return radius > 0 ? Physics.SphereCast(ray, radius, out info, maxDistance, layerMask) : Physics.Raycast(ray, out info, maxDistance, layerMask);
        }
    }
}
