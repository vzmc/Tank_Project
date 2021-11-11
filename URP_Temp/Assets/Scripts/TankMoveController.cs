using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class TankMoveController : MonoBehaviour
{
    [SerializeField] private WheelCollider[] frontWheels;
    [SerializeField] private WheelCollider[] backWheels;
    
    [SerializeField] private float maxMotorTorque;
    [SerializeField] private float maxBreakTorque;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float steerAngelChangeSpeed;
    
    private float currentAccelerateInputValue;
    private float currentBrakeInputValue;
    private float currentTurnInputValue;

    private void FixedUpdate()
    {
        foreach (var wheel in frontWheels)
        {
            UpdateWheelSteerAngle(wheel);
            UpdateWheelTorque(wheel);
        }
        
        foreach (var wheel in backWheels)
        {
            UpdateWheelTorque(wheel);
        }
    }

    private void UpdateWheelSteerAngle(WheelCollider wheel)
    {
        var targetSteerAngle = maxSteerAngle * currentTurnInputValue;
        var currentSteerAngle = wheel.steerAngle;
        wheel.steerAngle = Mathf.Clamp(targetSteerAngle, currentSteerAngle - steerAngelChangeSpeed * Time.deltaTime, currentSteerAngle + steerAngelChangeSpeed * Time.deltaTime);
    }
    
    private void UpdateWheelTorque(WheelCollider wheel)
    {
        var motorTorque = currentAccelerateInputValue * wheel.rpm >= 0 ? maxMotorTorque * currentAccelerateInputValue : 0;
        wheel.motorTorque = motorTorque;
        
        var brakeTorque1 = currentAccelerateInputValue * wheel.rpm < 0 ? maxBreakTorque * Mathf.Abs(currentAccelerateInputValue) : 0;
        var brakeTorque2 = currentBrakeInputValue * maxBreakTorque;
        var brakeTorque = Mathf.Max(brakeTorque1, brakeTorque2);
        wheel.brakeTorque = brakeTorque;
    }

    private void OnAccelerate(InputValue inputValue)
    {
        currentAccelerateInputValue = inputValue.Get<float>();
    }

    private void OnBrake(InputValue inputValue)
    {
        currentBrakeInputValue = inputValue.Get<float>();
    }

    private void OnTurn(InputValue inputValue)
    {
        currentTurnInputValue = inputValue.Get<float>();
    }
}
