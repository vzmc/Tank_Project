using Cinemachine;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera followPlayerCamera;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;
    [SerializeField] private Image aimMark;

    private void Awake()
    {
        DataManager.Instance.CurrentCameraType.OnValueChanged += ChangeCameraType;
    }

    private void ChangeCameraType(VirtualCameraType type)
    {
        switch (type)
        {
            case VirtualCameraType.FollowPlayer:
            {
                followPlayerCamera.Priority = 1;
                topDownCamera.Priority = 0;
                Cursor.lockState = CursorLockMode.Locked;
                aimMark.enabled = true;
                break;
            }
            case VirtualCameraType.TopDown:
            {
                followPlayerCamera.Priority = 0;
                topDownCamera.Priority = 1;
                Cursor.lockState = CursorLockMode.Confined;
                aimMark.enabled = false;
                break;
            }
            default:
            {
                break;
            }
        }
    }

    private void OnChangeCamera(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            DataManager.Instance.SwitchCameraType();
        }
    }
}
