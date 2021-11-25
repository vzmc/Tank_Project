using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        public event Action<T> OnValueChanged;
        
        public SyncValue()
        {
            Value = default;
        }
        public SyncValue(T initValue)
        {
            Value = initValue;
        }

        // コールバック登録し、一回実行させる
        public void SubscribeValueChangeEvent(Action<T> callback)
        {
            callback.Invoke(Value);
            OnValueChanged += callback;
        }
    }
    
    public enum TrajectoryType
    {
        Line,
        Parabola
    }

    public enum VirtualCameraType
    {
        FollowPlayer,
        TopDown
    }
    
    public class ShareDataManager : MonoBehaviour
    {
        public static ShareDataManager Instance { get; private set; }

        public SyncValue<TrajectoryType> CurrentAimType { get; } = new(TrajectoryType.Line);
        public SyncValue<TrajectoryType> CurrentShellType { get; } = new(TrajectoryType.Line);
        public SyncValue<bool> UsingLowParabola { get; } = new(true);
        public SyncValue<bool> ForeCastOnOff { get; } = new(false);
        public SyncValue<VirtualCameraType> CurrentCameraType { get; } = new(VirtualCameraType.FollowPlayer);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        public void SwitchAimType()
        {
            CurrentAimType.Value = CurrentAimType.Value switch
            {
                TrajectoryType.Line => TrajectoryType.Parabola,
                TrajectoryType.Parabola => TrajectoryType.Line,
                _ => TrajectoryType.Line
            };
        }
        
        public void SwitchShellType()
        {
            CurrentShellType.Value = CurrentShellType.Value switch
            {
                TrajectoryType.Line => TrajectoryType.Parabola,
                TrajectoryType.Parabola => TrajectoryType.Line,
                _ => TrajectoryType.Line
            };
        }

        public void SwitchParabolaType()
        {
            UsingLowParabola.Value = !UsingLowParabola.Value;
        }

        public void SwitchForeCastOnOff()
        {
            ForeCastOnOff.Value = !ForeCastOnOff.Value;
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

        private void OnChangeAimType(InputValue inputValue)
        {
            if (inputValue.isPressed)
            {
                SwitchAimType();
            }
        }
        
        private void OnChangeShellType(InputValue inputValue)
        {
            if (inputValue.isPressed)
            {
                SwitchShellType();
            }
        }

        private void OnChangeParabolaType(InputValue inputValue)
        {
            if (inputValue.isPressed)
            {
                SwitchParabolaType();
            }
        }
        
        private void OnChangeForecastOnOff(InputValue inputValue)
        {
            if (inputValue.isPressed)
            {
                SwitchForeCastOnOff();
            }
        }
        
        private void OnChangeCameraType(InputValue inputValue)
        {
            if (inputValue.isPressed)
            {
                SwitchCameraType();
            }
        }
    }
}
