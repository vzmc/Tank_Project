using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using UnityEngine;
using UnityEngine.Profiling;
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

    //public Vector3? HitPoint { get; private set; } = null;
    public List<Vector3> HitPointList { get; } = new List<Vector3>();
    
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

        CreateSimulationScene();
    }

    private void Update()
    {
        HitPointList.Clear();
        
        switch (aimType)
        {
            case TrajectoryType.Line:
            {
                DrawLine(StartPoint, checkRadius, StartDirection, maxDistance, checkLayers);
                break;
            }
            case TrajectoryType.Parabola:
            {
                //DrawParabola(StartPoint, StartDirection * StartSpeed, Acceleration, Time.fixedDeltaTime, maxSteps, lineDownSample, checkRadius, checkLayers);

                if (isDrawLine)
                {
                    Profiler.BeginSample("CalcPointsPhysic");
                    var points = GetSimulatePoints(maxSteps, lineDownSample);
                    Profiler.EndSample();
                    DrawPredictionLine(points);
                    DrawPredictionLandingPoint(HitPointList);
                }
                break;
            }
        }
    }
    
    private void DrawLine(Vector3 startPoint, float radius, Vector3 fireDirection, float distance, LayerMask hitLayers)
    {
        if (!isDrawLine) return;
        
        Vector3 endPoint;
        if (Physics.SphereCast(startPoint, radius, fireDirection, out var hitInfo, distance, hitLayers))
        {
            endPoint = hitInfo.point;
            HitPointList.Add(hitInfo.point);
        }
        else
        {
            endPoint = startPoint + distance * fireDirection;
        }
        
        DrawPredictionLine(new[] { startPoint, endPoint });
        DrawPredictionLandingPoint(HitPointList);
    }

    private void DrawParabola(Vector3 startPoint, Vector3 startVelocity, Vector3 acceleration, float timeStep, int maxStep, 
        int downSample = 1, float radius = 0, int hitCheckLayerMask = Physics.DefaultRaycastLayers)
    {
        if (!isDrawLine) return;
        
        Profiler.BeginSample("CalcPointsManual");
        var points
            = PredictionLineUtility.GetParabolaPredictionPoints(startPoint, startVelocity, acceleration, timeStep, maxStep, 
                out var hitPoint, downSample, radius, hitCheckLayerMask);
        Profiler.EndSample();

        if (hitPoint.HasValue)
        {
            HitPointList.Add(hitPoint.Value);
        }
        
        DrawPredictionLine(points);
        DrawPredictionLandingPoint(HitPointList);
    }

    private void DrawPredictionLine(IReadOnlyCollection<Vector3> points)
    {
        forecastLine.positionCount = points.Count;
        forecastLine.SetPositions(points.ToArray());
    }

    private void DrawPredictionLandingPoint(IReadOnlyCollection<Vector3> points)
    {
        if (points.Count > 0)
        {
            forecastLandingPoint.SetActive(true);
            forecastLandingPoint.transform.position = points.First();
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
        
        shell.Shot(StartSpeed * StartDirection, -1f, false);
        shell.onHitEvent += hitPoint =>
        {
            HitPointList.Add(hitPoint);
        };

        var physicsScene = simulationScene.GetPhysicsScene();
        for (var step = 1; step <= maxStep; step++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);

            if (HitPointList.Count > 0)
            {
                pointList.Add(HitPointList.First());
                Destroy(shell.gameObject);
                return pointList;
            }
            
            if (step % downSample == 0)
            {
                pointList.Add(shell.transform.position);
            }
        }
        
        Destroy(shell.gameObject);
        return pointList;
    }
}
