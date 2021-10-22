using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChinemachineTargetController : MonoBehaviour
{
    [SerializeField] private Transform cinemachineTarget;
    [SerializeField] private float minPitch = -45;
    [SerializeField] private float maxPitch = 75;

    private Vector2 currentLookInput;

    private bool appFocused;
    private bool isLookControlled;
    
    // cinemachine
    private float targetPitch;
    private float targetYaw;

    private void Start()
    {
        var eulerAngles = cinemachineTarget.rotation.eulerAngles;
        targetPitch = eulerAngles.x;
        targetYaw = eulerAngles.y;

        ChangeLookControlState(true);
    }

    private void Update()
    {
        CameraRotation();
    }
    
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (currentLookInput.sqrMagnitude >= float.Epsilon)
        {
            targetPitch += currentLookInput.y * Time.deltaTime;
            targetYaw += currentLookInput.x * Time.deltaTime;
            // clamp our rotations so our values are limited 360 degrees
            targetPitch = ClampPitch(ClampAngle(targetPitch));
            targetYaw = ClampAngle(targetYaw);
        }
        // Cinemachine will follow this target
        cinemachineTarget.rotation = Quaternion.Euler(targetPitch, targetYaw, 0);
    }
    
    // 角度を-360度~360度に収める
    private float ClampAngle(float angle)
    {
        angle %= 360;
        return angle;
    }

    private float ClampPitch(float pitch)
    {
        var result = Mathf.Clamp(pitch, minPitch, maxPitch);
        return result;
    }
    
    private void ChangeLookControlState(bool isControlled)
    {
        Cursor.lockState = isControlled ? CursorLockMode.Locked : CursorLockMode.None;
        isLookControlled = isControlled;
        
        Debug.Log($"LookControlled = {isControlled}");
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        appFocused = hasFocus;
    }

    private void OnLook(InputValue inputValue)
    {
        currentLookInput = appFocused && isLookControlled ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    private void OnChangeLookControl(InputValue inputValue)
    {
        //Debug.Log(inputValue.isPressed ? "Button is pressed" : "Button is released");
        ChangeLookControlState(!inputValue.isPressed);
    }
}
