using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRootController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    
    void Update()
    {
        transform.position = followTarget.position;
    }
}
