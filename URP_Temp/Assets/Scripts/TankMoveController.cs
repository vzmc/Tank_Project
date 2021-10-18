using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankMoveController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    
    private new Rigidbody rigidbody;
    private Vector2 currentMoveInputValue;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigidbody.AddForce(transform.forward * currentMoveInputValue.y * moveSpeed, ForceMode.Acceleration);
        rigidbody.AddTorque(transform.up * currentMoveInputValue.x * rotateSpeed, ForceMode.Acceleration);
    }

    private void OnMove(InputValue inputValue)
    {
        currentMoveInputValue = inputValue.Get<Vector2>();
    }
}
