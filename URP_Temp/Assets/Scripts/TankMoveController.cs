using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankMoveController : MonoBehaviour
{
    [SerializeField] private Transform leftWheelsRoot;
    [SerializeField] private Transform rightWheelsRoot;

    [SerializeField] private float motorTorue;
    [SerializeField] private float steerTorue;
    
    private WheelCollider[] leftWheels;
    private WheelCollider[] rightWheels;
    
    private Vector2 currentMoveInputValue;

    private void Awake()
    {
        leftWheels = leftWheelsRoot.GetComponentsInChildren<WheelCollider>();
        rightWheels = rightWheelsRoot.GetComponentsInChildren<WheelCollider>();
    }

    private void FixedUpdate()
    {
        var leftMotorTorue = motorTorue * currentMoveInputValue.y;
        var rightMotorTorue = motorTorue * currentMoveInputValue.y;
        
        leftMotorTorue += steerTorue * currentMoveInputValue.x;
        rightMotorTorue -= steerTorue * currentMoveInputValue.x;

        leftMotorTorue = Mathf.Clamp(leftMotorTorue, -motorTorue, motorTorue);
        rightMotorTorue = Mathf.Clamp(rightMotorTorue, -motorTorue, motorTorue);

        foreach (var wheel in leftWheels)
        {
            wheel.motorTorque = leftMotorTorue;
        }
        
        foreach (var wheel in rightWheels)
        {
            wheel.motorTorque = rightMotorTorue;
        }
    }

    private void OnMove(InputValue inputValue)
    {
        currentMoveInputValue = inputValue.Get<Vector2>();
    }
}
