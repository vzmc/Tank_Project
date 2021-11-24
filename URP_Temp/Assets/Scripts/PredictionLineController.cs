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
    private ShellMotionType shellMotionType;

    private Vector3 StartPoint => startTransform.position;
    private Vector3 StartDirection => startTransform.forward;
    private float StartSpeed => fireController.ShellSpeed;
    private Vector3 Acceleration => Physics.gravity;

    private void Awake()
    {
        shellMotionType = DataManager.Instance.LoadedShellType.Value;
        DataManager.Instance.LoadedShellType.OnValueChanged += type => shellMotionType = type;
        forecastLine = Instantiate(forecastLinePrefab);
    }

    private void OnEnable()
    {
        if (forecastLine == null)
            return;
        forecastLine.enabled = true;
    }

    private void OnDisable()
    {
        if (forecastLine == null)
            return;
        forecastLine.enabled = false;
    }

    private void Update()
    {
        switch (shellMotionType)
        {
            case ShellMotionType.Line:
            {
                DrawLine(StartPoint, StartDirection, maxDistance, checkLayers);
                break;
            }
            case ShellMotionType.Parabola:
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
        }
        else
        {
            endPoint = startPoint + fireDirection * maxDistance;
        }
        
        DrawForecastLine(new[] { startPoint, endPoint });
    }

    private void DrawParabola(Vector3 startPoint, Vector3 startVelocity, Vector3 acceleration, float timeStep, int maxStep, float checkRadius, LayerMask checkLayers)
    {
        var pointList = new List<Vector3>();
        for (int i = 0; i < maxStep; i++)
        {
            var point = CalcParabolaUtility.CalcParabolaPoint(startPoint, startVelocity, acceleration, timeStep * i);
            if (Physics.CheckSphere(point, checkRadius, checkLayers))
            {
                pointList.Add(point);
                break;
            }
            
            if (lineDownSample > 1)
            {
                if (i % lineDownSample == 0)
                {
                    pointList.Add(point);
                }
            }
            else
            {
                pointList.Add(point);
            }
        }

        DrawForecastLine(pointList);
    }

    private void DrawForecastLine(IReadOnlyCollection<Vector3> points)
    {
        forecastLine.positionCount = points.Count;
        forecastLine.SetPositions(points.ToArray());
    }
}
