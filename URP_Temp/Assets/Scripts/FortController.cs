using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FortController : MonoBehaviour
{
    [SerializeField] private MeshRenderer landingPointPrefab;
    [SerializeField] private LineRenderer fireLinePrefab;

    [SerializeField] private Transform fort;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform barrelTip;

    [SerializeField] private LayerMask rayCastTarget;
    
    [SerializeField] private float fortRotateSpeed = 25;
    [SerializeField] private float barrelRotateSpeed = 30;
    [SerializeField] private float maxBarrelPitch = 15;
    [SerializeField] private float minBarrelPitch = -45;

    [SerializeField] private float rayCastDistance = 500;
    
    private Transform cameraTransform;
    private MeshRenderer landingPointRenderer;
    private LineRenderer fireLineRenderer;
    
    private Vector3 rotateTargetAngles;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;

        landingPointRenderer = Instantiate(landingPointPrefab);
        fireLineRenderer = Instantiate(fireLinePrefab);
    }

    private void Update()
    {
        fort.localRotation = Quaternion.RotateTowards(fort.localRotation, Quaternion.Euler(0, rotateTargetAngles.y, 0), fortRotateSpeed * Time.deltaTime);
        barrel.localRotation = Quaternion.RotateTowards(barrel.localRotation, Quaternion.Euler(ClampBarrelPitch(rotateTargetAngles.x), 0, 0), barrelRotateSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // 砲台と砲管の回転角度計算
        Vector3 aimPoint;
        if (Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward), out var hitInfo, rayCastDistance, rayCastTarget))
        {
            aimPoint = hitInfo.point;
        }
        else
        {
            aimPoint = cameraTransform.position + cameraTransform.forward * rayCastDistance;
        }
        var worldAimDirection = (aimPoint - barrel.position).normalized;
        var localAimDirection = transform.InverseTransformDirection(worldAimDirection);
        rotateTargetAngles = Quaternion.FromToRotation(Vector3.forward, localAimDirection).eulerAngles;
        Debug.DrawLine(barrel.position, aimPoint, Color.green);
        
        // 射線描く
        Vector3 landingPoint;
        if (Physics.Raycast(new Ray(barrelTip.position, barrelTip.forward), out var hit, rayCastDistance, rayCastTarget))
        {
            landingPoint = hit.point;
        }
        else
        {
            landingPoint = barrelTip.position + barrelTip.forward * rayCastDistance;
        }
        landingPointRenderer.transform.position = landingPoint;
        fireLineRenderer.SetPosition(0, barrelTip.position);
        fireLineRenderer.SetPosition(1, landingPoint);

        if (Vector3.Distance(aimPoint, landingPoint) < 0.1)
        {
            var green = Color.green;
            green.a = 0.5f;
            landingPointRenderer.material.color = green;
            fireLineRenderer.startColor = green;
            fireLineRenderer.endColor = green;
        }
        else
        {
            var red = Color.red;
            red.a = 0.5f;
            landingPointRenderer.material.color = red;
            fireLineRenderer.startColor = red;
            fireLineRenderer.endColor = red;
        }
    }

    private float ClampBarrelPitch(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        angle = Mathf.Clamp(angle, minBarrelPitch, maxBarrelPitch);

        return angle;
    }
}
