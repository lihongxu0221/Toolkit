namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 程序集文件项.
/// </summary>
/// <param name="Name">程序集名称.</param>
/// <param name="FilePath">程序集路径.</param>
/// <param name="Extend">扩展名.</param>
public record AssemblyItem(string Name, string FilePath,string Extend);