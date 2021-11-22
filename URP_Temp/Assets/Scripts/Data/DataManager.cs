
using System;
using System.Diagnostics;
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
        public SyncValue(T init)
        {
            value = init;
        }
        public event Action<T> OnValueChanged;
    }
    
    public enum ShellMotionType
    {
        Line,
        Parabola
    }
    
    public class DataManager : MonoBehaviour
    {
        private static DataManager instance;
        public static DataManager Instance
        {
            get => instance;
            private set => instance = value;
        }
        
        public SyncValue<ShellMotionType> LoadedShellType { get; } = new SyncValue<ShellMotionType>(ShellMotionType.Line);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}
