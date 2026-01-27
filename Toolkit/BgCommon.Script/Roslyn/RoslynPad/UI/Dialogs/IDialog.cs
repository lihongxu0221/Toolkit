namespace RoslynPad.UI;

/// <summary>
/// 定义对话框的基本接口.
/// </summary>
public interface IDialog
{
    /// <summary>
    /// 异步显示对话框.
    /// </summary>
    /// <returns>表示异步操作的任务.</returns>
    Task ShowAsync();

    /// <summary>
    /// 关闭对话框.
    /// </summary>
    void Close();
}