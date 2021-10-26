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
        var motorTorque = maxMotorTorque * currentAccelerateInputValue;
        var steerAngel = maxSteerAngel * currentTurnInputValue;
        var breakTorque = Mathf.Abs(currentAccelerateInputValue) <= float.Epsilon ? maxBreakTorque : 0;

        foreach (var wheel in frontWheels)
        {
            wheel.steerAngle = steerAngel;
            wheel.motorTorque = motorTorque;
            wheel.brakeTorque = breakTorque;
        }
        
        foreach (var wheel in backWheels)
        {
            wheel.motorTorque = motorTorque;
            wheel.brakeTorque = breakTorque;
        }
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
