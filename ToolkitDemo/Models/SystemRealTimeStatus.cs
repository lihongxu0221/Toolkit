using BgCommon.Collections;

namespace ToolkitDemo.Models;

/// <summary>
/// 用于监控系统的实时状态.
/// </summary>
public partial class SystemRealTimeStatus : ObservableValidator
{
    /// <summary>
    /// 程序持续运行时间.
    /// </summary>
    [ObservableProperty]
    private double runningTime = 0;

    /// <summary>
    /// 系统时间.
    /// </summary>
    [ObservableProperty]
    private string operatingSystemTime = string.Empty;

    /// <summary>
    /// 程序占用内存大小.
    /// </summary>
    [ObservableProperty]
    private long memory = 0;

    /// <summary>
    /// 三色灯颜色.
    /// </summary>
    [ObservableProperty]
    private Color ledColor = Colors.Transparent;

    /// <summary>
    /// 系统状态.
    /// </summary>
    [ObservableProperty]
    private string systemStatus = string.Empty;

    /// <summary>
    /// 自动流程状态.
    /// </summary>
    [ObservableProperty]
    private string autoFlowStatus = string.Empty;

    /// <summary>
    /// 设备状态.
    /// </summary>
    [ObservableProperty]
    private string deviceStatus = string.Empty;

    /// <summary>
    /// 设备状态颜色.
    /// </summary>
    [ObservableProperty]
    private Color deviceStatusColor = Colors.Green;

    /// <summary>
    /// 运行中.
    /// </summary>
    [ObservableProperty]
    private bool isRunning = false;

    /// <summary>
    /// 暂停.
    /// </summary>
    [ObservableProperty]
    private bool isPause = false;

    // /// <summary>
    // /// 设备状态列表.
    // /// </summary>
    // [ObservableProperty]
    // private ObservableRangeCollection<DeviceDisplay> devices = new ObservableRangeCollection<DeviceDisplay>();

    /// <summary>
    /// 所有设备是否都连接上.
    /// </summary>
    [ObservableProperty]
    private bool isAllConnected = true;
}