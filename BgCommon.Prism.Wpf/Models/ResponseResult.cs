namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 泛型请求返回结果类（继承自基础返回结果类）.
/// 用于返回带指定数据类型结果集的接口响应，统一接口返回格式，包含状态码、操作结果、提示信息及泛型数据.
/// </summary>
/// <typeparam name="T">结果集数据类型（如 List&lt;UserInfo&gt;、Role、string 等）.</typeparam>
public class ResponseResult<T> : ResponseResult
{
    /// <summary>
    /// Gets or sets 返回的泛型结果集.
    /// 存储接口业务逻辑处理后的核心数据，类型与泛型参数 T 一致，无数据时返回默认值（引用类型为 null）.
    /// </summary>
    public new T? Result { get; set; }

    /// <summary>
    /// 构建操作成功的泛型返回结果.
    /// </summary>
    /// <param name="code">成功状态码，默认值为 1（表示操作成功），可根据业务场景自定义.</param>
    /// <param name="message">成功提示信息，默认值为“操作成功”，可根据业务场景自定义.</param>
    /// <param name="data">泛型结果集数据，默认值为 T 的默认值（无数据时可留空）.</param>
    /// <returns>包含成功状态、提示信息及泛型数据的 ResponseResult&lt;T&gt; 实例.</returns>
    public static ResponseResult<T> ToSuccess(int code = 1, string message = "操作成功", T? data = default)
        => new ResponseResult<T>() { Code = code, Success = true, Message = message, Result = data };

    /// <summary>
    /// 构建操作失败的泛型返回结果.
    /// </summary>
    /// <param name="message">失败提示信息（必填），需明确说明失败原因（如“参数校验失败”“数据不存在”）.</param>
    /// <param name="code">失败状态码，默认值为 -1（表示操作失败），可根据业务场景自定义错误码.</param>
    /// <param name="result">失败时返回的附加数据（可选），可用于返回错误详情或部分处理结果.</param>
    /// <returns>包含失败状态、错误信息及附加数据的 ResponseResult&lt;T&gt; 实例.</returns>
    public static ResponseResult<T> ToFail(string message, int code = -1, T? result = default)
        => new ResponseResult<T>() { Code = code, Success = false, Message = message, Result = result };
}

/// <summary>
/// 基础请求返回结果类.
/// 用于返回无指定数据类型（或任意类型）结果集的接口响应，统一接口返回格式，包含状态码、操作结果、提示信息及基础数据对象.
/// </summary>
public class ResponseResult
{
    /// <summary>
    /// Gets or sets 响应状态码.
    /// 用于标识接口请求的处理状态，核心默认值说明：
    /// 0 ：取消登录（登录流程中断）；
    /// 1 ：操作成功（通用成功状态）；
    /// -1 ：操作失败（通用失败状态）；
    /// 可根据业务需求扩展自定义状态码（如 200 表示查询成功、400 表示参数错误等）.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 接口请求是否处理成功.
    /// true 表示操作成功，false 表示操作失败，与 Code 字段协同标识状态（优先以 Code 为准，Success 用于快速判断）.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets 返回的基础结果集.
    /// 存储接口业务逻辑处理后的核心数据，类型为 object（支持任意数据类型），无数据时返回 null.
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// Gets or sets 接口响应的提示信息.
    /// 用于向调用方返回直观的操作结果说明（如成功提示、失败原因），默认值为空字符串.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 构建操作成功的基础返回结果.
    /// </summary>
    /// <param name="code">成功状态码，默认值为 1（表示操作成功），可根据业务场景自定义.</param>
    /// <param name="message">成功提示信息，默认值为“操作成功”，可根据业务场景自定义.</param>
    /// <param name="data">基础结果集数据（可选），无数据时可留空，支持任意数据类型.</param>
    /// <returns>包含成功状态、提示信息及基础数据的 ResponseResult 实例.</returns>
    public static ResponseResult ToSuccess(int code = 1, string message = "操作成功", object? data = null)
        => new ResponseResult() { Code = code, Success = true, Message = message, Result = data };

    /// <summary>
    /// 构建操作失败的基础返回结果.
    /// </summary>
    /// <param name="message">失败提示信息（必填），需明确说明失败原因（如“权限不足”“服务器内部错误”）.</param>
    /// <param name="code">失败状态码，默认值为 -1（表示操作失败），可根据业务场景自定义错误码.</param>
    /// <param name="result">失败时返回的附加数据（可选），可用于返回错误详情或部分处理结果.</param>
    /// <returns>包含失败状态、错误信息及附加数据的 ResponseResult 实例.</returns>
    public static ResponseResult ToFail(string message, int code = -1, object? result = default)
        => new ResponseResult() { Code = code, Success = false, Message = message, Result = result };
}