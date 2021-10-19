using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FortController : MonoBehaviour
{
    [SerializeField] private Transform fort;
    [SerializeField] private Transform barrel;
    [SerializeField] private float angleSpeed = 2;
    
    private Transform cameraTransform;
    private Vector3 targetAngles;
    
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        fort.localRotation = Quaternion.RotateTowards(fort.localRotation, Quaternion.Euler(0, targetAngles.y, 0), angleSpeed * Time.deltaTime);
        barrel.localRotation = Quaternion.RotateTowards(barrel.localRotation, Quaternion.Euler(targetAngles.x, 0, 0), angleSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        var targetVector = Vector3.forward;
        if (Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward), out var hitInfo))
        {
            targetVector = transform.InverseTransformDirection((hitInfo.point - barrel.position).normalized);
            Debug.DrawLine(barrel.position, hitInfo.point, Color.red);
        }
        
        targetAngles = Quaternion.FromToRotation(Vector3.forward, targetVector).eulerAngles;
    }
}
