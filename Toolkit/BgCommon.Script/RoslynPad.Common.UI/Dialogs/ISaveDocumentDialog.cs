namespace RoslynPad.UI;

/// <summary>
/// 保存文档对话框接口.
/// </summary>
public interface ISaveDocumentDialog : IDialog
{
    /// <summary>
    /// Gets or sets 文档名称.
    /// </summary>
    string DocumentName { get; set; }

    /// <summary>
    /// Gets 保存结果.
    /// </summary>
    SaveResult Result { get; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否允许编辑名称.
    /// </summary>
    bool AllowNameEdit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否显示“不保存”选项.
    /// </summary>
    bool ShowDoNotSave { get; set; }

    /// <summary>
    /// Gets 文件路径.
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// Gets or sets 文件路径工厂函数.
    /// </summary>
    Func<string, string>? FilePathFactory { get; set; }
}

/// <summary>
/// 保存结果枚举.
/// </summary>
public enum SaveResult
{
    /// <summary>
    /// 取消操作.
    /// </summary>
    Cancel,

    /// <summary>
    /// 确认保存.
    /// </summary>
    Save,

    /// <summary>
    /// 不保存.
    /// </summary>
    DoNotSave
}