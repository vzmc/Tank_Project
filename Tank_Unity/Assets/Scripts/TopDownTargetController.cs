using Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownTargetController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float screenMoveSpeed;
    
    private Vector2 moveDirection = Vector2.zero;
    private Camera mainCamera;
    
    private VirtualCameraType currentCameraType;
    
    private bool LostControl => Cursor.lockState == CursorLockMode.None || !Application.isFocused;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        ShareDataManager.Instance.CurrentCameraType.SubscribeValueChangeEvent(type => currentCameraType = type);
    }

    private void Update()
    {
        if (currentCameraType != VirtualCameraType.TopDown || LostControl)
        {
            return;
        }

        var moveVector = screenMoveSpeed * Time.deltaTime * moveDirection;
        target.Translate(new Vector3(moveVector.x, 0f, moveVector.y));
    }
    
    private void ChangeLookControlState(bool isControlled)
    {
        Cursor.lockState = isControlled ? CursorLockMode.Confined : CursorLockMode.None;
        Debug.Log($"LookControlled = {isControlled}");
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (currentCameraType != VirtualCameraType.TopDown)
        {
            return;
        }
        ChangeLookControlState(hasFocus);
    }

    private void OnPoint(InputValue inputValue)
    {
        if (currentCameraType != VirtualCameraType.TopDown || LostControl)
        {
            moveDirection = Vector2.zero;
            return;
        }
        
        var pointPos = inputValue.Get<Vector2>();
        var viewPoint = mainCamera.ScreenToViewportPoint(pointPos);
        Debug.Log($"Pointer view point = {viewPoint}");

        moveDirection.x = viewPoint.x switch
        {
            < 0.05f => -1f,
            > 0.95f => 1f,
            _ => 0f
        };

        moveDirection.y = viewPoint.y switch
        {
            < 0.05f => -1f,
            > 0.95f => 1f,
            _ => 0f
        };
    }
    
    private void OnChangeLookControl(InputValue inputValue)
    {
        if (currentCameraType != VirtualCameraType.TopDown)
        {
            return;
        }
        ChangeLookControlState(!inputValue.isPressed);
    }
}
