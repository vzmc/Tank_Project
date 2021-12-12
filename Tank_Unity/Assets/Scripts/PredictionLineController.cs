using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Utility;

/// <summary>
/// 予測線
/// </summary>
public class PredictionLineController : MonoBehaviour
{
    [SerializeField] private LineRenderer forecastLinePrefab;
    [SerializeField] private GameObject forecastLandingPointPrefab;
    [SerializeField] private Transform startTransform;
    [SerializeField] private FireController fireController;

    [Header("Line")]
    [SerializeField] private float maxDistance;
    
    [Header("Parabola")]
    [SerializeField] private int maxSteps;
    [SerializeField] private float checkTimeStep;
    [SerializeField] private float checkRadius;
    [SerializeField, Range(1, 20)] private int lineDownSample;
    
    [Header("CheckLayers")]
    [SerializeField] private LayerMask checkLayers;
    
    public Vector3? HitPoint { get; private set; }
    
    private LineRenderer forecastLine;
    private GameObject forecastLandingPoint;
    private TrajectoryType aimType;

    private Vector3 StartPoint => startTransform.position;
    private Vector3 StartDirection => startTransform.forward;
    private float StartSpeed => fireController.ShellSpeed;
    private Vector3 Acceleration => Physics.gravity;

    private void Awake()
    {
        ShareDataManager.Instance.CurrentAimType.SubscribeValueChangeEvent(type => aimType = type);
        forecastLine = Instantiate(forecastLinePrefab);
        forecastLandingPoint = Instantiate(forecastLandingPointPrefab);
        
        ShareDataManager.Instance.ForeCastOnOff.SubscribeValueChangeEvent(isOn =>
        {
            if (forecastLine != null)
            {
                forecastLine.enabled = isOn;
            }
            if (forecastLandingPoint != null)
            {
                forecastLandingPoint.SetActive(isOn);
            }
        });
    }

    private void Update()
    {
        switch (aimType)
        {
            case TrajectoryType.Line:
            {
                DrawLine(StartPoint, checkRadius, StartDirection, maxDistance, checkLayers);
                break;
            }
            case TrajectoryType.Parabola:
            {
                DrawParabola(StartPoint, StartDirection * StartSpeed, Acceleration, checkTimeStep, maxSteps,
                    out var hitPoint, lineDownSample, checkRadius, checkLayers);
                HitPoint = hitPoint;
                break;
            }
        }
    }
    
    private void DrawLine(Vector3 startPoint, float radius, Vector3 fireDirection, float distance, LayerMask hitLayers)
    {
        HitPoint = null;
        Vector3 endPoint;
        if (Physics.SphereCast(startPoint, radius, fireDirection, out var hitInfo, distance, hitLayers))
        {
            endPoint = hitInfo.point;
            HitPoint = endPoint;
        }
        else
        {
            endPoint = startPoint + distance * fireDirection;
        }
        
        DrawPredictionLine(new[] { startPoint, endPoint });
        DrawPredictionLandingPoint(HitPoint);
    }

    private void DrawParabola(Vector3 startPoint, Vector3 startVelocity, Vector3 acceleration, float timeStep, int maxStep,
        out Vector3? hitPoint, int downSample = 1, float radius = 0, int hitCheckLayerMask = Physics.DefaultRaycastLayers)
    {
        var points
            = PredictionLineUtility.GetParabolaPredictionPoints(startPoint, startVelocity, acceleration, timeStep, maxStep,
                out hitPoint, downSample, radius, hitCheckLayerMask);
        DrawPredictionLine(points);
        DrawPredictionLandingPoint(hitPoint);
    }

    private void DrawPredictionLine(IReadOnlyCollection<Vector3> points)
    {
        forecastLine.positionCount = points.Count;
        forecastLine.SetPositions(points.ToArray());
    }

    private void DrawPredictionLandingPoint(Vector3? position)
    {
        if (position.HasValue)
        {
            forecastLandingPoint.SetActive(true);
            forecastLandingPoint.transform.position = position.Value;
        }
        else
        {
            forecastLandingPoint.SetActive(false);
        }
    }
}
