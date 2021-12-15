using System;
using Cinemachine;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float impulseStrength;
    
    public event Action<Vector3> onHitEvent;

    public void Shot(Vector3 velocity, float lifeSpan, bool isImpulse)
    {
        rigidbody.AddForce(velocity, ForceMode.VelocityChange);
        if (isImpulse)
            impulseSource.GenerateImpulse(velocity.normalized * impulseStrength);

        if (lifeSpan > float.Epsilon)
        {
            Destroy(gameObject, lifeSpan);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        onHitEvent?.Invoke(other.GetContact(0).point);
    }
}
