using Cinemachine;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class FireController : MonoBehaviour
{
    [SerializeField] private PredictionLineController predictionLineController;
    [SerializeField] private Rigidbody lineShellPrefab;
    [SerializeField] private Rigidbody parabolaShellPrefab;
    [SerializeField] private float shellSpeed;
    [SerializeField] private float shellLifeTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float impulseStrength;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    
    [Header("着弾予測地点用")]
    [SerializeField] private LandingPointController landingPointPrefab;

    public float ShellSpeed => shellSpeed;
    private Rigidbody loadedShellPrefab;

    private void Awake()
    {
        ShareDataManager.Instance.CurrentShellType.SubscribeValueChangeEvent(ChangeShellType);
    }

    private void ChangeShellType(TrajectoryType type)
    {
        loadedShellPrefab = type switch
        {
            TrajectoryType.Line => lineShellPrefab,
            TrajectoryType.Parabola => parabolaShellPrefab,
            _ => null
        };
    }
    
    private void Fire()
    {
        var shellRigidbody = Instantiate(loadedShellPrefab, firePoint.position, firePoint.rotation);
        shellRigidbody.AddForce(firePoint.forward * shellSpeed, ForceMode.VelocityChange);
        Destroy(shellRigidbody.gameObject, shellLifeTime);
        
        impulseSource.GenerateImpulse(firePoint.forward * impulseStrength);

        var landingPoint = predictionLineController.HitPoint;
        if (landingPoint.HasValue)
        {
            var landingPointController = Instantiate(landingPointPrefab, landingPoint.Value, Quaternion.identity);
            landingPointController.SetOwner(shellRigidbody.gameObject);
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
