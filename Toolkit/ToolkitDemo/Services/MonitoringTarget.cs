using BgCommon.Localization.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToolKitDemo.Services;

/// <summary>
/// 定义了后台服务可以监控的目标领域.
/// 每个成员都是一个独立的标志，可以组合使用.
/// </summary>
[Flags]
[TypeConverter(typeof(EnumLocalizationConverter))]
public enum MonitoringTarget
{
    /// <summary>
    /// 无监控对象.
    /// 表示监控线程应处于休眠状态，不执行任何检查.
    /// </summary>
    [Display(Name = "无监控对象")]
    None = 0,

    /// <summary>
    /// 应用程序自身的性能指标.
    /// 包括内存占用、CPU使用率和总体运行时间等.
    /// </summary>
    [Display(Name = "进程状态")]
    Application = 1 << 0,

    /// <summary>
    /// 所有外部设备的聚合状态.
    /// 主要指检查所有设备的连接状态和工作情况.
    /// </summary>
    [Display(Name = "设备状态")]
    Devices = 1 << 1,

    /// <summary>
    /// 系统的核心运行状态.
    /// 指的是业务逻辑是否正在积极运行、暂停或已停止的状态，通常用于控制界面上的三色灯等.
    /// </summary>
    [Display(Name = "运行状态")]
    OperationalState = 1 << 2,

    /// <summary>
    /// 一个便利成员，包含了所有可监控的目标。
    /// 使用它可以一次性启动或停止所有监控任务。
    /// </summary>
    [Display(Name = "监控所有")]
    ALL = Application | Devices | OperationalState,
}