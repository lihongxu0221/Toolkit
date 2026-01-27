using System.Runtime.CompilerServices;
using System.Xaml;

namespace BgCommon.Prism.Wpf.Localization.Markup;

/// <summary>
/// 提供绑定值的基类.
/// </summary>
public abstract class MarkupBindableBaseExtension :
    MarkupExtension,
    INotifyPropertyChanged,
    INotifyPropertyChanging
{
    private Assembly? assembly;
    private object? value;

    /// <summary>
    /// Gets 调用的程序集.
    /// </summary>
    public Assembly? ExecuteAssembly => this.assembly;

    /// <summary>
    /// Gets or sets 绑定的值.
    /// </summary>
    public object? Value
    {
        get => this.value;
        set => _ = this.SetProperty(ref this.value, value);
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 设置值.
    /// </summary>
    protected abstract void SetValue();

    /// <summary>
    /// 初始化设置其他参数.
    /// </summary>
    /// <param name="owner">载体.</param>
    protected virtual void Initial(DependencyObject owner)
    {
    }

    /// <summary>
    /// 设置容器.<br/>
    /// 获取使用到的XAML的程序集.<br/>
    /// </summary>
    /// <param name="serviceProvider">serviceProvider.</param>
    protected virtual void Initial(IServiceProvider serviceProvider)
    {
        // 反订阅多语言变更事件
        Ioc.Unsubscribe<ILocalizationProvider?>(p => this.SetValue());

        // 获取根对象（如 Window、UserControl）
        if (serviceProvider.GetService(typeof(IRootObjectProvider)) is IRootObjectProvider rootProvider)
        {
            if (rootProvider?.RootObject is FrameworkElement rootVisual)
            {
                IProvideValueTarget? target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
                if (target?.TargetObject is DependencyObject owner)
                {
                    this.Initial(owner);
                }

                // 获取绑定的XAML所在程序集
                Type type = rootVisual.GetType();
                this.assembly = type.Assembly;

                // 订阅多语言变更事件
                _ = Ioc.Subscribe<ILocalizationProvider?>(p => this.SetValue());
            }
        }
    }

    /// <inheritdoc/>
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        // 设置容器
        this.Initial(serviceProvider);

        // 设置初始值
        this.SetValue();
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget target ||
            target?.TargetObject is not Setter)
        {
            Binding binding = new (nameof(this.Value))
            {
                Source = this,
                Mode = BindingMode.OneWay,
            };

            return binding.ProvideValue(serviceProvider);
        }
        else
        {
            return new Binding(nameof(this.Value))
            {
                Source = this,
                Mode = BindingMode.OneWay,
            };
        }
    }

    /// <summary>
    /// 触发属性变更事件.
    /// </summary>
    /// <param name="e">属性变更事件参数.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        this.PropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    /// 触发属性变更事件.
    /// </summary>
    /// <param name="e">属性变更事件参数.</param>
    protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        this.PropertyChanging?.Invoke(this, e);
    }

    /// <summary>
    /// 触发属性变更事件.
    /// </summary>
    /// <param name="propertyName">属性名.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
        this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 触发属性变更事件.
    /// </summary>
    /// <param name="propertyName">属性名.</param>
    protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
        this.OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
    }

    /// <summary>
    /// 设置属性值.
    /// </summary>
    /// <typeparam name="T">属性类型.</typeparam>
    /// <param name="field">属性字段引用.</param>
    /// <param name="newValue">新值.</param>
    /// <param name="propertyName">属性名.</param>
    /// <returns>如果值发生改变则返回 true，否则返回 false.</returns>
    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        this.OnPropertyChanging(propertyName);
        field = newValue;
        this.OnPropertyChanged(propertyName);
        return true;
    }
}