using System.Linq;
using Cinemachine;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class FireController : MonoBehaviour
{
    [SerializeField] private PredictionLineController predictionLineController;
    [SerializeField] private ShellController lineShellPrefab;
    [SerializeField] private ShellController parabolaShellPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shellSpeed;
    [SerializeField] private float shellLifeSpan;

    [Header("着弾予測地点用")]
    [SerializeField] private LandingPointController landingPointPrefab;
    
    public float ShellSpeed => shellSpeed;
    public ShellController LoadedShellPrefab { get; private set; }
    
    private void Awake()
    {
        ShareDataManager.Instance.CurrentShellType.SubscribeValueChangeEvent(ChangeShellType);
    }

    private void ChangeShellType(TrajectoryType type)
    {
        LoadedShellPrefab = type switch
        {
            TrajectoryType.Line => lineShellPrefab,
            TrajectoryType.Parabola => parabolaShellPrefab,
            _ => null
        };
    }
    
    private void Fire()
    {
        var shell = Instantiate(LoadedShellPrefab, firePoint.position, firePoint.rotation);
        shell.Shot(firePoint.forward * shellSpeed, shellLifeSpan, true);
        
        if (predictionLineController.HitPointList.Count > 0)
        {
            var landingPoint = predictionLineController.HitPointList.First();
            var landingPointController = Instantiate(landingPointPrefab, landingPoint, Quaternion.identity);
            landingPointController.SetOwner(shell.gameObject);
        }
    }

    private void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Fire();
        }
    }
}
