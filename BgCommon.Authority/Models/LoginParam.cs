using System.Text.Json.Serialization;

namespace BgCommon.Authority.Models;

public class LoginParam
{
    public LoginParam()
    {
        this.UserName = string.Empty;
        this.Password = string.Empty;
        this.IsAutoLogin = false;
        this.IsRemember = false;
        this.IsAllowAutoLogin = true;
        this.IsAllowRemember = true;
    }

    public LoginParam(string username, string password, bool isAutoLogin, bool isRemember)
    {
        this.UserName = username;
        this.Password = password;
        this.IsAutoLogin = isAutoLogin;
        this.IsRemember = isRemember;
        this.IsAllowAutoLogin = true;
        this.IsAllowRemember = true;
    }

    /// <summary>
    /// Gets or sets 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否自动登陆
    /// </summary>
    public bool IsAutoLogin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否记住密码
    /// </summary>
    public bool IsRemember { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否允许自动登录
    /// </summary>
    [JsonIgnore]
    public bool IsAllowAutoLogin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否允许记住密码
    /// </summary>
    [JsonIgnore]
    public bool IsAllowRemember { get; set; }
}