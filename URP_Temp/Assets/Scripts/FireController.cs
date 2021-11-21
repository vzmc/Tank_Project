using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class SyncValue<T>
{
    private T value;
    public T Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged?.Invoke(value);
        }
    }
    public SyncValue(T init)
    {
        value = init;
    }
    public event Action<T> OnValueChanged;
}

public enum ShellMotionType
{
    Line,
    Parabola
}

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

    public SyncValue<ShellMotionType> SyncShellType { get; } = new SyncValue<ShellMotionType>(ShellMotionType.Line);

    private void Start()
    {
        ChangeShellType(ShellMotionType.Line);
    }

    public void ChangeShellType(ShellMotionType type)
    {
        switch (type)
        {
            case ShellMotionType.Line:
            {
                loadedShellPrefab = lineShellPrefab;
                break;
            }
            case ShellMotionType.Parabola:
            {
                loadedShellPrefab = parabolaShellPrefab;
                break;
            }
        }
        
        SyncShellType.Value = type;
    }
    
    private void Fire()
    {
        var shellRigidbody = Instantiate(loadedShellPrefab, firePoint.position, firePoint.rotation);
        shellRigidbody.AddForce(firePoint.forward * shellSpeed, ForceMode.VelocityChange);
        Destroy(shellRigidbody.gameObject, shellLifeTime);
        
        impulseSource.GenerateImpulse(firePoint.forward * impulseStrength);

        var landingPoint = Vector3.zero;
        if (CalcLandingPoint(ref landingPoint))
        {
            var landingPointController = Instantiate(landingPointPrefab, landingPoint, Quaternion.identity);
            landingPointController.SetOwner(shellRigidbody.gameObject);
        }
    }

    private bool CalcLandingPoint(ref Vector3 landingPoint)
    {
        switch (SyncShellType.Value)
        {
            case ShellMotionType.Line:
            {
                return CalcLineLandingPoint(ref landingPoint);
            }
            case ShellMotionType.Parabola:
            {
                return CalcParabolaLandingPoint(ref landingPoint);
            }
        }
        return false;
    }

    private bool CalcLineLandingPoint(ref Vector3 landingPoint)
    {
        if (Physics.Raycast(firePoint.position, firePoint.forward, out var hitInfo, shellSpeed * shellLifeTime, checkLayers))
        {
            landingPoint = hitInfo.point;
            return true;
        }

        return false;
    }

    private bool CalcParabolaLandingPoint(ref Vector3 landingPoint)
    {
        var startPosition = firePoint.position;
        var startVelocity = shellSpeed * firePoint.forward;
        var acceleration = Physics.gravity;
        var currentTime = 0f;
        for (int i = 0; currentTime <= shellLifeTime; i++)
        {
            currentTime = i * checkTimeStep;
            var point = CalcParabolaUtility.CalcParabolaPoint(startPosition, startVelocity, acceleration, currentTime);
            if (Physics.CheckSphere(point, checkRadius, checkLayers))
            {
                landingPoint = point;
                return true;
            }
        }
        
        return false;
    }
    
    private void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Fire();
        }
    }

    private void OnChangeShellType(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            ChangeShellType(SyncShellType.Value == ShellMotionType.Line ? ShellMotionType.Parabola : ShellMotionType.Line);
        }
    }
}
