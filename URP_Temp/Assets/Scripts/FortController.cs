using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FortController : MonoBehaviour
{
    [SerializeField] private MeshRenderer signalPrefab;
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private Rigidbody cannonBallPrefab;
    [SerializeField] private Transform cannonTip;
    [SerializeField] private Transform fort;
    [SerializeField] private Transform barrel;
    [SerializeField] private float angleSpeed;
    [SerializeField] private float maxBarrelPitch;
    [SerializeField] private float minBarrelPitch;
    [SerializeField] private float cannonForece;
    
    private Transform cameraTransform;
    private Vector3 targetAngles;

    private bool hasTarget;

    private MeshRenderer signal;
    private LineRenderer line;

    private Vector3 lineEnd;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;

        signal = Instantiate(signalPrefab);
        line = Instantiate(linePrefab);
    }

    private void Update()
    {
        fort.localRotation = Quaternion.RotateTowards(fort.localRotation, Quaternion.Euler(0, targetAngles.y, 0), angleSpeed * Time.deltaTime);

        var xAngle = targetAngles.x;
        if (xAngle > 180)
        {
            xAngle -= 360;
        }
        xAngle = Mathf.Clamp(xAngle, minBarrelPitch, maxBarrelPitch);
        barrel.localRotation = Quaternion.RotateTowards(barrel.localRotation, Quaternion.Euler(xAngle, 0, 0), angleSpeed * Time.deltaTime);
        
        signal.transform.position = lineEnd;
        line.SetPosition(0, barrel.position);
        line.SetPosition(1, lineEnd);
        
        Debug.Log($"xAngle = {xAngle}");
        Debug.Log($"targetAngles = {targetAngles}");
    }

    private void FixedUpdate()
    {
        var targetPoint = Vector3.zero;
        var startPoint = barrel.position;
        
        if (Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward), out var hitInfo))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            targetPoint = cameraTransform.position + cameraTransform.forward * 1000;
        }

        if (Physics.Raycast(new Ray(startPoint, barrel.forward), out var hit))
        {
            lineEnd = hit.point;
        }
        else
        {
            lineEnd = startPoint + barrel.forward * 1000;
        }

        var worldVector = (targetPoint - startPoint).normalized;
        var localVector = transform.InverseTransformDirection(worldVector);
        targetAngles = Quaternion.FromToRotation(Vector3.forward, localVector).eulerAngles;
        
        Debug.DrawLine(startPoint, hitInfo.point, Color.green);
    }

    private void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            var ball = Instantiate(cannonBallPrefab, cannonTip.position, Quaternion.identity);
            ball.AddForce(barrel.forward * cannonForece, ForceMode.VelocityChange);
            Destroy(ball.gameObject, 5);
        }
    }
}
