using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPointController : MonoBehaviour
{
    [SerializeField] private Color beforeHit;
    [SerializeField] private Color afterHit;

    private Material material;
    private GameObject ownerObj;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        material.color = beforeHit;
    }

    public void SetOwner(GameObject owner)
    {
        ownerObj = owner;
    }

    private void Update()
    {
        if (ownerObj == null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ownerObj)
        {
            material.color = afterHit;
            Destroy(gameObject, 3f);
        }
    }
}
