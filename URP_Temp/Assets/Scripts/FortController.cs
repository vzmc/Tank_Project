using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FortController : MonoBehaviour
{
    enum AimMode
    {
        Line,
        Parabola,
    }
    
    [SerializeField] private MeshRenderer landingPointPrefab;
    [SerializeField] private LineRenderer fireLinePrefab;

    [SerializeField] private FireController fireController;

    [SerializeField] private Transform fort;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform barrelTip;
    
    [SerializeField] private float fortRotateSpeed = 25;
    [SerializeField] private float barrelRotateSpeed = 30;
    [SerializeField] private float maxBarrelPitch = 15;
    [SerializeField] private float minBarrelPitch = -45;

    [SerializeField] private float rayCastDistance = 500;
    [SerializeField] private LayerMask rayCastTarget;
    [SerializeField] private AimMode aimMode;
    
    private MeshRenderer landingPointRenderer;
    private LineRenderer fireLineRenderer;

    private Vector3 worldAimPoint;
    private Vector3 localAimVector;
    private Vector3 rotateTargetAngles;

    public void SetAimPoint(Vector3 worldPoint)
    {
        worldAimPoint = worldPoint;
        
        switch (aimMode)
        {
            case AimMode.Line:
                var worldVector = worldPoint - barrel.position;
                localAimVector = transform.InverseTransformDirection(worldVector.normalized);
                break;
            case AimMode.Parabola:
                // todo
                localAimVector = Vector3.forward;
                break;
        }
        
        rotateTargetAngles = Quaternion.FromToRotation(Vector3.forward, localAimVector).eulerAngles;
    }

    private void Awake()
    {
        landingPointRenderer = Instantiate(landingPointPrefab);
        landingPointRenderer.enabled = false;
        fireLineRenderer = Instantiate(fireLinePrefab);
    }

    private void Update()
    {
        fort.localRotation = Quaternion.RotateTowards(fort.localRotation, Quaternion.Euler(0, rotateTargetAngles.y, 0), fortRotateSpeed * Time.deltaTime);
        barrel.localRotation = Quaternion.RotateTowards(barrel.localRotation, Quaternion.Euler(ClampBarrelPitch(rotateTargetAngles.x), 0, 0), barrelRotateSpeed * Time.deltaTime);
        
        Debug.DrawLine(barrel.position, worldAimPoint, Color.green);
        
        // 射線描く
        Vector3 landingPoint;
        if (Physics.Raycast(new Ray(barrelTip.position, barrelTip.forward), out var hitInfo, rayCastDistance, rayCastTarget))
        {
            landingPoint = hitInfo.point;
            landingPointRenderer.enabled = true;
            landingPointRenderer.transform.position = landingPoint;
        }
        else
        {
            landingPoint = barrelTip.position + barrelTip.forward * rayCastDistance;
            landingPointRenderer.enabled = false;
        }
        
        DrawFireLine(barrelTip.position, landingPoint);

        if (Vector3.Distance(worldAimPoint, landingPoint) < 0.2f)
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

    private void FixedUpdate()
    {
    }

    private void DrawFireLine(params Vector3[] points)
    {
        fireLineRenderer.positionCount = points.Length;
        fireLineRenderer.SetPositions(points);
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

    private Vector3 CalcFireVector(Vector3 startPoint, Vector3 targetPoint, float fireSpeed)
    {
        Vector3 fireVector = Vector3.forward;

        Vector3 aimVector = targetPoint - startPoint;

        float y = aimVector.y;
        float x = Mathf.Sqrt(aimVector.x * aimVector.x + aimVector.z * aimVector.z);
        
        
        return fireVector;
    }
}
