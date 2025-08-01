using System.Runtime.Serialization;

namespace BgCommon.Authority.Models;

/// <summary>
/// 用户信息.
/// </summary>
[Serializable]
public class UserInfo : ObservableObject
{
    private string userId = string.Empty;
    private string userName = string.Empty;
    private string password = string.Empty;
    private UserAuthority authority = UserAuthority.Operator;
    private string lastLoginTime = string.Empty;
    private string lastLoginIp = string.Empty;
    private bool isOnline = false;
    private bool enable = true;

    public string UserId
    {
        get => userId;
        set => SetProperty(ref userId, value);
    }

    public string UserName
    {
        get => userName;
        set => SetProperty(ref userName, value);
    }

    public string DisplayName => Ioc.Instance?.GetString(UserName) ?? UserName;

    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    public UserAuthority Authority
    {
        get => authority;
        set => SetProperty(ref authority, value);
    }

    public string LastLoginTime
    {
        get => lastLoginTime;
        set => SetProperty(ref lastLoginTime, value);
    }

    public string LastLoginIp
    {
        get => lastLoginIp;
        set => SetProperty(ref lastLoginIp, value);
    }

    public bool IsOnline
    {
        get => isOnline;
        set => SetProperty(ref isOnline, value);
    }

    public bool Enable
    {
        get => enable;
        set => SetProperty(ref enable, value);
    }

    public UserInfo()
    {
    }

    public UserInfo(string userName, string password, UserAuthority authority)
        : this(Guid.NewGuid().ToString().Replace("-", string.Empty), userName, password, authority)
    {

    }

    public UserInfo(string userId, string userName, string password, UserAuthority authority)
    {
        UserId = userId;
        UserName = userName;
        Password = password;
        Authority = authority;
        UserId = userId;
        UserName = userName;
        Password = password;
        Authority = authority;
    }

    /// <summary>
    /// 在序列化期间，设置字段值
    /// </summary>
    /// <param name="context">上下文</param>
    /// <remarks><see cref="OnSerializingAttribute"/></remarks>
    [OnSerializing]
    protected virtual void OnSerializing(StreamingContext context)
    {
        // 举例：在序列化期间，设置字段值
    }

    /// <summary>
    /// 序列化完成后，设置字段值
    /// </summary>
    /// <param name="context">上下文</param>
    /// <remarks><see cref="OnSerializedAttribute"/> </remarks>
    [OnSerialized]
    protected virtual void OnSerialized(StreamingContext context)
    {
        // 举例：在序列化完成后，设置字段值
    }

    /// <summary>
    /// 在反序列化期间，为字段设置默认值 
    /// </summary>
    /// <param name="context">上下文</param>
    /// <remarks><see cref="OnDeserializedAttribute"/> 标记该方法在反序列化期间被调用</remarks>
    [OnDeserializing]
    protected virtual void OnDeserializing(StreamingContext context)
    {
        // 举例：在反序列化期间，为字段设置默认值
    }

    /// <summary>
    /// 序列化完成后，设置字段值
    /// </summary>
    /// <param name="context">上下文</param>
    /// <remarks><see cref="OnDeserializedAttribute"/> 标记该方法在反序列化之后被调用</remarks>
    [OnDeserialized]
    protected virtual void OnDeserialized(StreamingContext context)
    {
        // 举例：序列化完成后，设置字段值

    }
}