namespace BgCommon.Prism.Wpf.Modules.Logging.Models;

/// <summary>
/// 显示日志级别的类.
/// </summary>
public class DisplayLevel
{
    public NLog.LogLevel? Level { get; set; }

    public string Name { get; set; }

    public DisplayLevel(string name)
    {
        Name = name;
        try
        {
            Level = NLog.LogLevel.FromString(name);
        }
        catch
        {
        }
    }

    public DisplayLevel(NLog.LogLevel level)
    {
        Level = level;
        Name = level.Name;
    }

    /// <summary>
    /// 获取全部的日志级别.
    /// </summary>
    public static DisplayLevel[] GetAllLevels(bool isShowAll = true)
    {
        IList<DisplayLevel> allLevels = new List<DisplayLevel>();
        if (isShowAll)
        {
            allLevels.Add(new DisplayLevel("ALL"));
        }

        foreach (NLog.LogLevel level in NLog.LogLevel.AllLevels)
        {
            allLevels.Add(new DisplayLevel(level));
        }

        return allLevels.ToArray();
    }
}