using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireController : MonoBehaviour
{
    [SerializeField] private Rigidbody shellPrefab;
    [SerializeField] private float shellSpeed;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireImpulse;
    [SerializeField] private float destroyTime;

    public float ShellSpeed => shellSpeed;

    private void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            var shellRigidbody = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
            shellRigidbody.GetComponent<CinemachineImpulseSource>()?.GenerateImpulse(firePoint.forward * fireImpulse);
            shellRigidbody.AddForce(firePoint.forward * shellSpeed, ForceMode.VelocityChange);
            shellRigidbody.gameObject.layer = gameObject.layer;
            Destroy(shellRigidbody.gameObject, destroyTime);
        }
    }
}
