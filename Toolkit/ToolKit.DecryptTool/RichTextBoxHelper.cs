namespace ToolKit.DecryptTool;

/// <summary>
/// 提供一组静态方法，以帮助在 Windows Forms 应用程序中记录日志并将文本追加到 RichTextBox 控件.
/// 该类旨在简化向 RichTextBox 控件添加日志消息（包括错误消息）的过程.
/// 它确保所有操作都在 UI 线程上执行，从而使其可以从任何线程安全地调用.
/// </summary>
public static class RichTextBoxHelper
{
    /// <summary>
    /// 将异常消息及其堆栈跟踪追加到指定的 RichTextBox 中，并将文本颜色设置为红色.
    /// 该方法确保操作在 UI 线程上执行，从而使其可以从任何线程安全地调用.
    /// </summary>
    /// <param name="txtLog">要追加日志消息的 RichTextBox.</param>
    /// <param name="ex">其消息和堆栈跟踪将被追加到 RichTextBox 的异常.</param>
    public static void AppendLog(this RichTextBox txtLog, Exception ex)
    {
        var message = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";
        AppendLog(txtLog, message, Color.Red);
    }

    /// <summary>
    /// 将一条消息追加到指定的 RichTextBox 中，并将文本颜色设置为白色.
    /// 该方法确保操作在 UI 线程上执行，从而使其可以从任何线程安全地调用.
    /// </summary>
    /// <param name="txtLog">要追加日志消息的 RichTextBox.</param>
    /// <param name="msg">要追加到 RichTextBox 的消息.</param>
    public static void AppendLog(this RichTextBox txtLog, string msg)
    {
        AppendLog(txtLog, msg, Color.White);
    }

    /// <summary>
    /// 将格式化的日志消息追加到指定的 RichTextBox 中，并将文本颜色设置为提供的颜色.
    /// 该方法确保操作在 UI 线程上执行，从而使其可以从任何线程安全地调用.
    /// </summary>
    /// <param name="txtLog">要追加日志消息的 RichTextBox.</param>
    /// <param name="msg">要追加到 RichTextBox 的消息.</param>
    /// <param name="color">要追加的文本的颜色.</param>
    public static void AppendLog(this RichTextBox txtLog, string msg, Color color)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.BeginInvoke(new Action<RichTextBox, string, Color>(AppendLog), txtLog, msg, color);
        }
        else
        {
            var formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {msg}{Environment.NewLine}";
            txtLog.AppendText(formattedMessage, color);
        }
    }

    /// <summary>
    /// 将指定颜色的文本追加到 RichTextBox 控件中，并滚动到最新添加的文本位置.
    /// 该方法确保操作在 UI 线程上执行，从而使其可以从任何线程安全地调用.
    /// </summary>
    /// <param name="txtLog">要追加文本的 RichTextBox.</param>
    /// <param name="text">要追加到 RichTextBox 的文本.</param>
    /// <param name="color">要追加的文本的颜色.</param>
    private static void AppendText(this RichTextBox txtLog, string text, Color color)
    {
        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.SelectionLength = 0;
        txtLog.SelectionColor = color;
        txtLog.AppendText(text);
        txtLog.ScrollToCaret();
    }
}