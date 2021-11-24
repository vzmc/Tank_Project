using System;
using UnityEngine;

namespace Data
{
    public class SyncValue<T>
    {
        private T value;
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged?.Invoke(value);
            }
        }
        public SyncValue()
        {
            Value = default;
        }
        public SyncValue(T initValue)
        {
            Value = initValue;
        }
        public event Action<T> OnValueChanged;
    }
    
    public enum ShellMotionType
    {
        Line,
        Parabola
    }

    public enum VirtualCameraType
    {
        FollowPlayer,
        TopDown
    }
    
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        public SyncValue<ShellMotionType> LoadedShellType { get; } = new(ShellMotionType.Line);
        public SyncValue<bool> IsUseLowParabola { get; } = new(true);
        public SyncValue<VirtualCameraType> CurrentCameraType { get; } = new(VirtualCameraType.FollowPlayer);
        public SyncValue<bool> IsMenuOpen { get; } = new(false);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        public void SwitchLoadedShellType()
        {
            LoadedShellType.Value = LoadedShellType.Value switch
            {
                ShellMotionType.Line => ShellMotionType.Parabola,
                ShellMotionType.Parabola => ShellMotionType.Line,
                _ => ShellMotionType.Line
            };
        }
        
        public void SwitchCameraType()
        {
            CurrentCameraType.Value = CurrentCameraType.Value switch
            {
                VirtualCameraType.FollowPlayer => VirtualCameraType.TopDown,
                VirtualCameraType.TopDown => VirtualCameraType.FollowPlayer,
                _ => VirtualCameraType.FollowPlayer
            };
        }
    }
}
