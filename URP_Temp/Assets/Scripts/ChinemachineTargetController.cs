using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChinemachineTargetController : MonoBehaviour
{
    [SerializeField] private Transform cinemachineTarget;

    private Vector2 currentLookInput;

    private bool applicationFocused;
    private bool isLookControlled = true;
    
    // cinemachine
    private float targetPitch;
    private float targetYaw;

    private void Start()
    {
        var eulerAngles = cinemachineTarget.rotation.eulerAngles;
        targetPitch = eulerAngles.x;
        targetYaw = eulerAngles.y;
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
            targetPitch = ClampAngle(targetPitch);
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
    
    private void ChangeCurserLockState(bool islock)
    {
        Cursor.lockState = islock ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        applicationFocused = hasFocus;
    }

    private void OnLook(InputValue inputValue)
    {
        currentLookInput = applicationFocused && isLookControlled ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    private void OnChangeLookControl(InputValue inputValue)
    {
        Debug.Log(inputValue.isPressed ? "Button is pressed" : "Button is released");
        isLookControlled = !inputValue.isPressed;
        ChangeCurserLockState(isLookControlled);
    }
}
