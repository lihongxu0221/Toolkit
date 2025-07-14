namespace BgCommon.Wpf.Prism.Authority.Models;

/// <summary>
/// 登录结果
/// </summary>
public class LoginResult
{
    /// <summary>
    /// Gets or sets 错误码.<br/>
    /// 0 : 取消登录<br/>
    /// 1 : 登录成功<br/>
    /// -1 ：登录失败<br/>
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Gets a value indicating whether 是否登录成功
    /// </summary>
    public bool Success => Code == 1;

    /// <summary>
    /// Gets or sets 提示信息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}