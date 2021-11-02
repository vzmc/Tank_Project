using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private Renderer aimPointPrefab;
    [SerializeField] private FortController fortController;
    [SerializeField] private LayerMask rayCastLayerMask;
    [SerializeField] private float rayCastDistance = 500;
    
    private Transform cameraTransform;
    private Renderer aimPointRenderer;

    private Vector3 aimPoint;

    private void Awake()
    {
        aimPointRenderer = Instantiate(aimPointPrefab);
        aimPointRenderer.enabled = false;
        cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward), out var hitInfo, rayCastDistance, rayCastLayerMask))
        {
            aimPoint = hitInfo.point;
            aimPointRenderer.enabled = true;
            aimPointRenderer.transform.position = aimPoint;

            Debug.DrawLine(cameraTransform.position, aimPoint, Color.green);
        }
        else
        {
            aimPoint = cameraTransform.position + cameraTransform.forward * rayCastDistance;
            aimPointRenderer.enabled = false;
        }
        
        fortController.SetAimPoint(aimPoint);
    }
}
