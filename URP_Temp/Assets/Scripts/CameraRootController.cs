using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRootController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    // Update is called once per frame
    void Update()
    {
        transform.position = followTarget.position;
    }
}
