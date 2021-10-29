using UnityEngine;
using UnityEngine.InputSystem;

public class TankMoveController : MonoBehaviour
{
    [SerializeField] private WheelCollider[] frontWheels;
    [SerializeField] private WheelCollider[] backWheels;
    
    [SerializeField] private float maxMotorTorque;
    [SerializeField] private float maxSteerAngel;
    [SerializeField] private float maxBreakTorque;
    
    private float currentAccelerateInputValue;
    private float currentTurnInputValue;

    private void FixedUpdate()
    {
        var steerAngel = maxSteerAngel * currentTurnInputValue;
        foreach (var wheel in frontWheels)
        {
            wheel.steerAngle = steerAngel;
            UpdateWheel(wheel);
        }
        
        foreach (var wheel in backWheels)
        {
            UpdateWheel(wheel);
        }
    }

    private void UpdateWheel(WheelCollider wheel)
    {
        var motorTorque = currentAccelerateInputValue * wheel.rpm >= 0 ? maxMotorTorque * currentAccelerateInputValue : 0;
        wheel.motorTorque = motorTorque;
        
        var breakTorque = currentAccelerateInputValue * wheel.rpm < -float.Epsilon ? maxBreakTorque * Mathf.Abs(currentAccelerateInputValue) : 0;
        wheel.brakeTorque = breakTorque;
    }

    private void OnAccelerate(InputValue inputValue)
    {
        currentAccelerateInputValue = inputValue.Get<float>();
    }

    private void OnTurn(InputValue inputValue)
    {
        currentTurnInputValue = inputValue.Get<float>();
    }
}
