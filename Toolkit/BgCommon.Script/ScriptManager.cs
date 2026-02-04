using System.Collections.Specialized;

namespace BgCommon.Script;

/// <summary>
/// 脚本管理器类，负责管理脚本上下文集合、处理生命周期事件及提供编辑执行接口.
/// </summary>
public sealed class ScriptManager : IDisposable
{
    /// <summary>
    /// 脚本配置信息.
    /// </summary>
    private readonly ScriptConfig config;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptManager"/> class.
    /// </summary>
    /// <param name="config">脚本配置信息对象.</param>
    public ScriptManager(ScriptConfig config)
    {
        // 验证配置参数是否为空.
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        this.config = config;

        // 初始化脚本上下文集合.
        // 订阅集合变更事件以管理上下文事件.
        this.Contexts = new ObservableCollection<ScripteContext>();
        this.Contexts.CollectionChanged += this.Contexts_CollectionChanged;
    }

    /// <summary>
    /// Gets 脚本配置信息.
    /// </summary>
    public ScriptConfig Config => this.config;

    /// <summary>
    /// Gets 脚本上下文集合.
    /// </summary>
    public ObservableCollection<ScripteContext> Contexts { get; private set; }

    /// <summary>
    /// 显示脚本编辑器.
    /// </summary>
    public void ShowEditor()
    {
        // 方法体逻辑保持原样.
    }

    /// <summary>
    /// 显示指定脚本的编辑器.
    /// </summary>
    /// <param name="scriptName">脚本名称.</param>
    public void ShowEditor(string scriptName)
    {
        // 验证脚本名称.
        ArgumentNullException.ThrowIfNull(scriptName, nameof(scriptName));
    }

    /// <summary>
    /// 执行指定的脚本.
    /// </summary>
    /// <param name="scriptName">脚本名称.</param>
    /// <returns>执行结果对象.</returns>
    public ScriptResult Execute(string scriptName)
    {
        // 验证脚本名称.
        ArgumentNullException.ThrowIfNull(scriptName, nameof(scriptName));
        return new ScriptResult(true, "执行完成.");
    }

    /// <summary>
    /// 释放管理器占用的资源，并注销所有上下文事件.
    /// </summary>
    public void Dispose()
    {
        if (this.Contexts != null)
        {
            // 循环注销所有上下文的事件处理程序.
            for (int i = 0; i < this.Contexts.Count; i++)
            {
                this.HanlderEvent(this.Contexts[i], false);
            }

            // 注销集合变更事件.
            this.Contexts.CollectionChanged -= this.Contexts_CollectionChanged;
        }

        // 通知 GC 不需要调用终结器.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 响应上下文集合变更事件.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">变更事件参数.</param>
    private void Contexts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        // 处理新增项的事件注册.
        if (eventArgs.Action == NotifyCollectionChangedAction.Add && eventArgs.NewItems != null)
        {
            foreach (var newItem in eventArgs.NewItems)
            {
                if (newItem is ScripteContext addedContext)
                {
                    this.HanlderEvent(addedContext, true);
                }
            }

            return;
        }

        // 处理删除项的事件注销.
        if (eventArgs.Action == NotifyCollectionChangedAction.Remove && eventArgs.OldItems != null)
        {
            foreach (var oldItem in eventArgs.OldItems)
            {
                if (oldItem is ScripteContext removedContext)
                {
                    this.HanlderEvent(removedContext, false);
                }
            }
        }
    }

    /// <summary>
    /// 批量注册或注销脚本上下文的事件处理程序.
    /// </summary>
    /// <param name="context">脚本上下文实例.</param>
    /// <param name="isSubscribing">指示是注册 (true) 还是注销 (false) 事件.</param>
    private void HanlderEvent(ScripteContext context, bool isSubscribing)
    {
        // 验证上下文对象.
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (isSubscribing)
        {
            // 注册所有相关生命周期事件.
            context.OnDispose += this.Context_OnDispose;
            context.OnInitialized += this.Context_OnInitialized;
            context.OnError += this.Context_OnError;
            context.OnCompiling += this.Context_OnCompiling;
            context.OnCompiled += this.Context_OnCompiled;
            context.OnRunning += this.Context_OnRunning;
            context.OnRuned += this.Context_OnRuned;
            context.OnSaved += this.Context_OnSaved;
        }
        else
        {
            // 注销所有相关生命周期事件.
            context.OnDispose -= this.Context_OnDispose;
            context.OnInitialized -= this.Context_OnInitialized;
            context.OnError -= this.Context_OnError;
            context.OnCompiling -= this.Context_OnCompiling;
            context.OnCompiled -= this.Context_OnCompiled;
            context.OnRunning -= this.Context_OnRunning;
            context.OnRuned -= this.Context_OnRuned;
            context.OnSaved -= this.Context_OnSaved;
        }
    }

    /// <summary>
    /// 上下文初始化完成时的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnInitialized(object? sender, EventArgs eventArgs)
    {
    }

    /// <summary>
    /// 上下文资源释放时的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnDispose(object? sender, EventArgs eventArgs)
    {
        if (sender is ScripteContext disposedContext)
        {
            // 当上下文释放时，注销其在该管理器中的事件订阅.
            this.HanlderEvent(disposedContext, false);
        }
    }

    /// <summary>
    /// 脚本保存后的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnSaved(object? sender, EventArgs eventArgs)
    {
    }

    /// <summary>
    /// 脚本发生错误时的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnError(object? sender, EventArgs eventArgs)
    {
    }

    /// <summary>
    /// 脚本开始编译时的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnCompiling(object? sender, EventArgs eventArgs)
    {
    }

    /// <summary>
    /// 脚本编译完成时的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnCompiled(object? sender, EventArgs eventArgs)
    {
    }

    /// <summary>
    /// 脚本开始运行时的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnRunning(object? sender, EventArgs eventArgs)
    {
    }

    /// <summary>
    /// 脚本运行结束后的回调.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="eventArgs">事件参数.</param>
    private void Context_OnRuned(object? sender, EventArgs eventArgs)
    {
    }
}