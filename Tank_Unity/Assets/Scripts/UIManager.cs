using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text aimType;
    [SerializeField] private Text shellType;
    [SerializeField] private Text parabolaType;
    [SerializeField] private Text forecastOnOff;

    private readonly Dictionary<TrajectoryType, string> trajectoryTypeString = new()
    {
        { TrajectoryType.Line, "直線" },
        { TrajectoryType.Parabola, "放物線" }
    };

    private void Awake()
    {
        ShareDataManager.Instance.CurrentAimType.SubscribeValueChangeEvent(type => aimType.text = trajectoryTypeString[type]);
        ShareDataManager.Instance.CurrentShellType.SubscribeValueChangeEvent(type => shellType.text = trajectoryTypeString[type]);
        ShareDataManager.Instance.UsingLowParabola.SubscribeValueChangeEvent(isLow => parabolaType.text = isLow ? "低" : "高");
        ShareDataManager.Instance.ForeCastOnOff.SubscribeValueChangeEvent(isOn => forecastOnOff.text = isOn ? "ON" : "OFF");
    }
}
