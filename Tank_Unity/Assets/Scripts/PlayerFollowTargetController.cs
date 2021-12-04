using Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFollowTargetController : MonoBehaviour
{
    [SerializeField] private Transform cinemachineTarget;
    [SerializeField] private float minPitch = -45;
    [SerializeField] private float maxPitch = 75;
    
    private VirtualCameraType currentCameraType;
    private Vector2 currentLookInput;

    private bool appHasFocus;
    private bool LostControl => Cursor.lockState == CursorLockMode.None || !Application.isFocused;
    
    // cinemachine
    private float targetPitch;
    private float targetYaw;

    private void Awake()
    {
        currentCameraType = ShareDataManager.Instance.CurrentCameraType.Value;
        ShareDataManager.Instance.CurrentCameraType.OnValueChanged += type => currentCameraType = type;
    }

    private void Start()
    {
        var eulerAngles = cinemachineTarget.rotation.eulerAngles;
        targetPitch = eulerAngles.x;
        targetYaw = eulerAngles.y;

        ChangeLookControlState(true);
    }

    private void Update()
    {
        if (currentCameraType != VirtualCameraType.FollowPlayer || LostControl)
        {
            return;
        }
        CameraRotation();
    }
    
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (currentLookInput.sqrMagnitude > float.Epsilon)
        {
            targetPitch += currentLookInput.y * Time.deltaTime;
            targetYaw += currentLookInput.x * Time.deltaTime;
            // clamp our rotations so our values are limited 360 degrees
            targetPitch = ClampPitch(ClampAngle(targetPitch));
            targetYaw = ClampAngle(targetYaw);
        }
        // Cinemachine will follow this target
        cinemachineTarget.rotation = Quaternion.Euler(targetPitch, targetYaw, 0);
    }
    
    // 角度を-360度~360度に収める
    private float ClampAngle(float angle)
    {
        angle %= 360;
        return angle;
    }

    private float ClampPitch(float pitch)
    {
        var result = Mathf.Clamp(pitch, minPitch, maxPitch);
        return result;
    }
    
    private void ChangeLookControlState(bool isControlled)
    {
        Cursor.lockState = isControlled ? CursorLockMode.Locked : CursorLockMode.None;
        Debug.Log($"LookControlled = {isControlled}");
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (currentCameraType != VirtualCameraType.FollowPlayer)
        {
            return;
        }
        ChangeLookControlState(hasFocus);
    }

    private void OnLook(InputValue inputValue)
    {
        if (currentCameraType != VirtualCameraType.FollowPlayer)
        {
            return;
        }
        currentLookInput = LostControl ? Vector2.zero : inputValue.Get<Vector2>();
    }

    private void OnChangeLookControl(InputValue inputValue)
    {
        if (currentCameraType != VirtualCameraType.FollowPlayer)
        {
            return;
        }
        ChangeLookControlState(!inputValue.isPressed);
    }
}
