using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    [SerializeField] private Renderer aimPointPrefab;
    [SerializeField] private FortController fortController;
    
    [SerializeField] private LayerMask checkLayers;
    [SerializeField] private float maxDistance;

    private Camera mainCamera;
    private Transform cameraTransform;
    private Renderer aimPointRenderer;

    private Vector3 aimPoint;
    private Vector2 currentPointPosition;
    
    private bool LostControl => Cursor.lockState == CursorLockMode.None || !Application.isFocused;

    private void Awake()
    {
        aimPointRenderer = Instantiate(aimPointPrefab);
        aimPointRenderer.enabled = false;

        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
    }

    private void FixedUpdate()
    {
        if (LostControl)
        {
            return;
        }
        
        if (Physics.Raycast(mainCamera.ScreenPointToRay(currentPointPosition), out var hitInfo, maxDistance, checkLayers))
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

    private void OnPoint(InputValue inputValue)
    {
        if (LostControl)
        {
            return;
        }
        
        currentPointPosition = inputValue.Get<Vector2>();
        Debug.Log($"CurrentPointPosition = {currentPointPosition}");
    }
}
