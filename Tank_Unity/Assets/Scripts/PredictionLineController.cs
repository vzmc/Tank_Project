using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private float checkRadius;
    [SerializeField, Range(1, 20)] private int lineDownSample;
    
    [Header("CheckLayers")]
    [SerializeField] private LayerMask checkLayers;

    [Header("Stage Object")] 
    [SerializeField] private Transform stage;
    
    public Vector3? HitPoint { get; private set; }
    
    private LineRenderer forecastLine;
    private GameObject forecastLandingPoint;
    private TrajectoryType aimType;
    private bool isDrawLine;

    private Scene simulationScene;

    private Vector3 StartPoint => startTransform.position;
    private Vector3 StartDirection => startTransform.forward;
    private float StartSpeed => fireController.ShellSpeed;
    private Vector3 Acceleration => Physics.gravity;

    private void Start()
    {
        forecastLine = Instantiate(forecastLinePrefab);
        forecastLandingPoint = Instantiate(forecastLandingPointPrefab);
        
        ShareDataManager.Instance.CurrentAimType.SubscribeValueChangeEvent(type => aimType = type);
        ShareDataManager.Instance.ForeCastOnOff.SubscribeValueChangeEvent(isOn =>
        {
            isDrawLine = isOn;
            if (forecastLine != null)
            {
                forecastLine.enabled = isOn;
            }
            if (forecastLandingPoint != null)
            {
                forecastLandingPoint.SetActive(isOn);
            }
        });

        //CreateSimulationScene();
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
                DrawParabola(StartPoint, StartDirection * StartSpeed, Acceleration, Time.fixedDeltaTime, maxSteps,
                    out var hitPoint, lineDownSample, checkRadius, checkLayers);
                HitPoint = hitPoint;
                
                // if (isDrawLine)
                // {
                //     var points = GetSimulatePoints(maxSteps, lineDownSample);
                //     DrawPredictionLine(points);
                //     HitPoint = points.Last();
                //     DrawPredictionLandingPoint(HitPoint);
                // }
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

        if (isDrawLine)
        {
            DrawPredictionLine(new[] { startPoint, endPoint });
            DrawPredictionLandingPoint(HitPoint);
        }
    }

    private void DrawParabola(Vector3 startPoint, Vector3 startVelocity, Vector3 acceleration, float timeStep, int maxStep,
        out Vector3? hitPoint, int downSample = 1, float radius = 0, int hitCheckLayerMask = Physics.DefaultRaycastLayers)
    {
        var points
            = PredictionLineUtility.GetParabolaPredictionPoints(startPoint, startVelocity, acceleration, timeStep, maxStep,
                out hitPoint, downSample, radius, hitCheckLayerMask);

        if (isDrawLine)
        {
            DrawPredictionLine(points);
            DrawPredictionLandingPoint(hitPoint);
        }
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

    private void CreateSimulationScene()
    {
        simulationScene = SceneManager.CreateScene("Simulation",  new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        var obj = Instantiate(stage, stage.position, stage.rotation);
        SceneManager.MoveGameObjectToScene(obj.gameObject, simulationScene);
        foreach (var render in obj.GetComponentsInChildren<Renderer>())
        {
            render.enabled = false;
        }
    }

    private IReadOnlyCollection<Vector3> GetSimulatePoints(int maxStep, int downSample = 1)
    {
        var pointList = new List<Vector3> { StartPoint };
        var shell = Instantiate(fireController.LoadedShellPrefab, StartPoint, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(shell.gameObject, simulationScene);
        foreach (var render in shell.GetComponentsInChildren<Renderer>())
        {
            render.enabled = false;
        }
        shell.AddForce(StartSpeed * StartDirection, ForceMode.VelocityChange);

        var physicsScene = simulationScene.GetPhysicsScene();
        for (var step = 1; step <= maxStep; step++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            if (step % downSample == 0)
            {
                pointList.Add(shell.position);
            }
        }
        
        Destroy(shell.gameObject);
        return pointList;
    }
}
