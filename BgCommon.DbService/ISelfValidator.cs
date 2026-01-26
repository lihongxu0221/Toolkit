namespace BgCommon.DbService;

/// <summary>
/// 定义一个可验证对象的契约.
/// </summary>
public interface ISelfValidator
{
    /// <summary>
    /// Gets a value indicating whether 获取一个值，该值指示对象当前是否存在验证错误.
    /// </summary>
    bool HasErrors { get; }

    /// <summary>
    /// 执行对象的验证逻辑.
    /// </summary>
    void Validate();
}