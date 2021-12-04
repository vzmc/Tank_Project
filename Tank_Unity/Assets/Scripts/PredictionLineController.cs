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
    [SerializeField] private int lineDownSample;
    
    [Header("CheckLayers")]
    [SerializeField] private LayerMask checkLayers;

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
            enabled = isOn;
        });
    }

    private void OnEnable()
    {
        if (forecastLine != null)
        {
            forecastLine.enabled = true;
        }
        if (forecastLandingPoint != null)
        {
            forecastLandingPoint.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (forecastLine != null)
        {
            forecastLine.enabled = false;
        }
        if (forecastLandingPoint != null)
        {
            forecastLandingPoint.SetActive(false);
        }
    }

    private void Update()
    {
        switch (aimType)
        {
            case TrajectoryType.Line:
            {
                DrawLine(StartPoint, StartDirection, maxDistance, checkLayers);
                break;
            }
            case TrajectoryType.Parabola:
            {
                DrawParabola(StartPoint, StartDirection * StartSpeed, Acceleration, checkTimeStep, maxSteps, checkRadius, checkLayers);
                break;
            }
        }
    }
    
    private void DrawLine(Vector3 startPoint, Vector3 fireDirection, float maxDistance, LayerMask checkLayers)
    {
        Vector3 endPoint;
        if (Physics.Raycast(startPoint, fireDirection, out var hitInfo, maxDistance, checkLayers))
        {
            endPoint = hitInfo.point;
            DrawForecastLandingPoint(endPoint);
        }
        else
        {
            endPoint = startPoint + fireDirection * maxDistance;
            DrawForecastLandingPoint(null);
        }
        
        DrawForecastLine(new[] { startPoint, endPoint });
    }

    private void DrawParabola(Vector3 startPoint, Vector3 startVelocity, Vector3 acceleration, float timeStep, int maxStep, float checkRadius, LayerMask checkLayers)
    {
        var pointList = new List<Vector3>();
        
        Vector3 point = Vector3.zero;
        bool hit = false;
        for (int step = 0; step < maxStep; step++)
        {
            Vector3 previousPoint = point;
            point = CalcParabolaUtility.CalcParabolaPoint(startPoint, startVelocity, acceleration, timeStep * step);
            if (step > 0 && Physics.Linecast(previousPoint, point, out var hitInfo, checkLayers))
            {
                pointList.Add(hitInfo.point);
                DrawForecastLandingPoint(hitInfo.point);
                hit = true;
                break;
            }
            
            if (lineDownSample > 1)
            {
                if (step % lineDownSample == 0)
                {
                    pointList.Add(point);
                }
            }
            else
            {
                pointList.Add(point);
            }
        }

        if (!hit)
        {
            DrawForecastLandingPoint(null);
        }
        
        DrawForecastLine(pointList);
    }

    private void DrawForecastLine(IReadOnlyCollection<Vector3> points)
    {
        forecastLine.positionCount = points.Count;
        forecastLine.SetPositions(points.ToArray());
    }

    private void DrawForecastLandingPoint(Vector3? position)
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
