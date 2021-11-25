using Data;
using UnityEngine;
using Utility;

public class FortController : MonoBehaviour
{
    [SerializeField] private FireController fireController;

    [SerializeField] private Transform fort;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform barrelTip;
    
    [SerializeField] private float fortRotateSpeed = 25;
    [SerializeField] private float barrelRotateSpeed = 30;
    [SerializeField] private float maxBarrelPitch = 15;
    [SerializeField] private float minBarrelPitch = -45;

    private float fortLength;
    private TrajectoryType aimType;
    private bool usingLowParabola;

    private Vector3 worldFireVector;
    private Vector3 localAimDirection;
    private Vector3 rotateTargetAngles;
    
    private void Awake()
    {
        fortLength = (barrelTip.position - barrel.position).magnitude;
        Debug.Log($"fortLength = {fortLength}");
        
        ShareDataManager.Instance.CurrentAimType.SubscribeValueChangeEvent(type => aimType = type);
        ShareDataManager.Instance.UsingLowParabola.SubscribeValueChangeEvent(usingLow => usingLowParabola = usingLow);
    }

    private void Update()
    {
        fort.localRotation = Quaternion.RotateTowards(fort.localRotation, Quaternion.Euler(0, rotateTargetAngles.y, 0), fortRotateSpeed * Time.deltaTime);
        barrel.localRotation = Quaternion.RotateTowards(barrel.localRotation, Quaternion.Euler(ClampBarrelPitch(rotateTargetAngles.x), 0, 0), barrelRotateSpeed * Time.deltaTime);
    }
    
    public void SetAimPoint(Vector3 worldAimPoint)
    {
        switch (aimType)
        {
            case TrajectoryType.Line:
            {
                worldFireVector = worldAimPoint - barrel.position;
                localAimDirection = transform.InverseTransformDirection(worldFireVector.normalized);
                break;
            }
            case TrajectoryType.Parabola:
            {
                var distanceVector = worldAimPoint - barrel.position;
                var resultVector= CalcFireVectorUtility.CalcFireVector(distanceVector, Physics.gravity, fortLength, fireController.ShellSpeed, usingLowParabola);
                if (resultVector.HasValue)
                {
                    worldFireVector = resultVector.Value;
                }
                else
                {
                    var vector = distanceVector;
                    vector.y = new Vector2(vector.x, vector.z).magnitude;
                    worldFireVector = vector;
                }
                
                localAimDirection = transform.InverseTransformDirection(worldFireVector.normalized);
                break;
            }
        }
        
        rotateTargetAngles = Quaternion.FromToRotation(Vector3.forward, localAimDirection).eulerAngles;
    }
    
    private float ClampBarrelPitch(float angle)
    {
        // (180, 360)範囲内の角度を(-180, 0)に変換する
        if (angle > 180f)
        {
            angle -= 360f;
        }
        angle = Mathf.Clamp(angle, minBarrelPitch, maxBarrelPitch);
        return angle;
    }
}
