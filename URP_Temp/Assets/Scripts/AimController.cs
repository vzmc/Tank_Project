using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private Renderer aimPointPrefab;
    [SerializeField] private FortController fortController;
    
    [SerializeField] private LayerMask checkLayers;
    [SerializeField] private float maxDistance;
    
    private Transform cameraTransform;
    private Renderer aimPointRenderer;

    private Vector3 aimPoint;

    private void Awake()
    {
        aimPointRenderer = Instantiate(aimPointPrefab);
        aimPointRenderer.enabled = false;

        cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward), out var hitInfo, maxDistance, checkLayers))
        {
            aimPoint = hitInfo.point;
            aimPointRenderer.enabled = true;
            aimPointRenderer.transform.position = aimPoint;

            Debug.DrawLine(cameraTransform.position, aimPoint, Color.green);
        }
        else
        {
            aimPoint = cameraTransform.position + cameraTransform.forward * maxDistance;
            aimPointRenderer.enabled = false;
        }
        
        fortController.SetAimPoint(aimPoint);
    }
}
