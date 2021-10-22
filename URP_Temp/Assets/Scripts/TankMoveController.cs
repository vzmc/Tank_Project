using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankMoveController : MonoBehaviour
{
    [SerializeField] private WheelCollider[] leftWheels;
    [SerializeField] private WheelCollider[] rightWheels;
    
    [SerializeField] private float motorTorue;
    [SerializeField] private float steerTorue;
    
    private new Rigidbody rigidbody;
    private Vector2 currentMoveInputValue;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
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

        // rigidbody.AddForce(transform.forward * currentMoveInputValue.y * motorTorue, ForceMode.Acceleration);
        // rigidbody.AddTorque(transform.up * currentMoveInputValue.x * steerTorue, ForceMode.Acceleration);
    }

    private void OnMove(InputValue inputValue)
    {
        currentMoveInputValue = inputValue.Get<Vector2>();
    }
}
