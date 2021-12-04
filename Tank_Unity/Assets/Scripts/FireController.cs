using Cinemachine;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class FireController : MonoBehaviour
{
    [SerializeField] private Rigidbody lineShellPrefab;
    [SerializeField] private Rigidbody parabolaShellPrefab;
    [SerializeField] private float shellSpeed;
    [SerializeField] private float shellLifeTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float impulseStrength;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    
    [Header("着弾予測地点用")]
    [SerializeField] private LandingPointController landingPointPrefab;
    [SerializeField] private float checkTimeStep;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask checkLayers;

    public float ShellSpeed => shellSpeed;

    private Rigidbody loadedShellPrefab;
    private TrajectoryType loadedShellType;

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

        loadedShellType = type;
    }
    
    private void Fire()
    {
        var shellRigidbody = Instantiate(loadedShellPrefab, firePoint.position, firePoint.rotation);
        shellRigidbody.AddForce(firePoint.forward * shellSpeed, ForceMode.VelocityChange);
        Destroy(shellRigidbody.gameObject, shellLifeTime);
        
        impulseSource.GenerateImpulse(firePoint.forward * impulseStrength);

        var landingPoint = CalcLandingPoint(firePoint.forward);
        if (landingPoint.HasValue)
        {
            var landingPointController = Instantiate(landingPointPrefab, landingPoint.Value, Quaternion.identity);
            landingPointController.SetOwner(shellRigidbody.gameObject);
        }
    }

    private Vector3? CalcLandingPoint(Vector3 fireDirection)
    {
        return loadedShellType switch
        {
            TrajectoryType.Line => CalcLineLandingPoint(fireDirection),
            TrajectoryType.Parabola => CalcParabolaLandingPoint(fireDirection),
            _ => null
        };
    }
    
    private Vector3? CalcLineLandingPoint(Vector3 fireDirection)
    {
        if (Physics.Raycast(firePoint.position, fireDirection, out var hitInfo, shellSpeed * shellLifeTime, checkLayers))
        {
            return hitInfo.point;
        }

        return null;
    }
    
    private Vector3? CalcParabolaLandingPoint(Vector3 fireDirection)
    {
        var startPosition = firePoint.position;
        var startVelocity = shellSpeed * fireDirection;
        var acceleration = Physics.gravity;

        Vector3 point = Vector3.zero;
        var currentTime = 0f;
        while (currentTime <= shellLifeTime)
        {
            var previousPoint = point;
            point = CalcParabolaUtility.CalcParabolaPoint(startPosition, startVelocity, acceleration, currentTime);
            if (currentTime > 0 && Physics.Linecast(previousPoint, point, out var hitInfo, checkLayers))
            {
                return hitInfo.point;
            }
            currentTime += checkTimeStep;
        }

        return null;
    }

    private void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Fire();
        }
    }
}
