namespace RoslynPad.Editor;

/// <summary>
/// 通用属性工具类，用于注册和操作依赖属性.
/// </summary>
public static class CommonProperty
{
    /// <summary>
    /// 注册一个样式化属性.
    /// </summary>
    /// <typeparam name="TOwner">属性所有者的类型.</typeparam>
    /// <typeparam name="TValue">属性值的类型接口.</typeparam>
    /// <param name="name">属性的名称.</param>
    /// <param name="defaultValue">属性的默认值.</param>
    /// <param name="options">属性的配置选项.</param>
    /// <param name="onChanged">属性值变更时的回调动作.</param>
    /// <returns>返回封装后的样式化属性对象.</returns>
    public static StyledProperty<TValue> Register<TOwner, TValue>(
        string name,
        TValue defaultValue = default!,
        PropertyOptions options = PropertyOptions.None,
        Action<TOwner, CommonPropertyChangedArgs<TValue>>? onChanged = null)
        where TOwner : DependencyObject
    {
        // 初始化框架属性元数据选项.
        var frameworkMetadataOptions = FrameworkPropertyMetadataOptions.None;

        // 根据传入的配置位标志设置元数据选项.
        if (options.Has(PropertyOptions.AffectsRender))
        {
            frameworkMetadataOptions |= FrameworkPropertyMetadataOptions.AffectsRender;
        }

        if (options.Has(PropertyOptions.AffectsArrange))
        {
            frameworkMetadataOptions |= FrameworkPropertyMetadataOptions.AffectsArrange;
        }

        if (options.Has(PropertyOptions.AffectsMeasure))
        {
            frameworkMetadataOptions |= FrameworkPropertyMetadataOptions.AffectsMeasure;
        }

        if (options.Has(PropertyOptions.Inherits))
        {
            frameworkMetadataOptions |= FrameworkPropertyMetadataOptions.Inherits;
        }

        if (options.Has(PropertyOptions.BindsTwoWay))
        {
            frameworkMetadataOptions |= FrameworkPropertyMetadataOptions.BindsTwoWayByDefault;
        }

        // 包装变更回调，将原始的 DependencyObject 转换为特定的所有者类型.
        var propertyChangedCallback = onChanged != null
            ? new PropertyChangedCallback((dependencyObject, eventArgs) =>
                onChanged((TOwner)dependencyObject, new CommonPropertyChangedArgs<TValue>((TValue)eventArgs.OldValue, (TValue)eventArgs.NewValue)))
            : null;

        // 创建框架属性元数据.
        var frameworkMetadata = new FrameworkPropertyMetadata(defaultValue, frameworkMetadataOptions, propertyChangedCallback);

        // 注册底层的依赖属性.
        var dependencyProperty = DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), frameworkMetadata);

        return new StyledProperty<TValue>(dependencyProperty);
    }

    /// <summary>
    /// 获取指定对象的样式化属性值.
    /// </summary>
    /// <typeparam name="TValue">属性值的类型.</typeparam>
    /// <param name="dependencyObject">目标依赖对象.</param>
    /// <param name="styledProperty">样式化属性实例.</param>
    /// <returns>返回属性当前的值.</returns>
    public static TValue GetValue<TValue>(this DependencyObject dependencyObject, StyledProperty<TValue> styledProperty)
    {
        // 调用底层接口获取值并进行类型转换.
        return (TValue)dependencyObject.GetValue(styledProperty.Property);
    }

    /// <summary>
    /// 设置指定对象的样式化属性值.
    /// </summary>
    /// <typeparam name="TValue">属性值的类型.</typeparam>
    /// <param name="dependencyObject">目标依赖对象.</param>
    /// <param name="styledProperty">样式化属性实例.</param>
    /// <param name="value">要设置的新值.</param>
    public static void SetValue<TValue>(this DependencyObject dependencyObject, StyledProperty<TValue> styledProperty, TValue value)
    {
        // 调用底层接口设置属性值.
        dependencyObject.SetValue(styledProperty.Property, value);
    }
}

/// <summary>
/// 封装了 <see cref="DependencyProperty"/> 的泛型样式化属性类.
/// </summary>
/// <typeparam name="TValue">属性值的类型.</typeparam>
public sealed class StyledProperty<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StyledProperty{TValue}"/> class.
    /// </summary>
    /// <param name="property">底层依赖属性对象.</param>
    public StyledProperty(DependencyProperty property)
    {
        this.Property = property;
    }

    /// <summary>
    /// Gets 关联的依赖属性.
    /// </summary>
    public DependencyProperty Property { get; }

    /// <summary>
    /// 定义从样式化属性到依赖属性的隐式转换.
    /// </summary>
    /// <param name="styledProperty">样式化属性实例.</param>
    public static implicit operator DependencyProperty(StyledProperty<TValue> styledProperty) => styledProperty.Property;

    /// <summary>
    /// 为该属性添加新的所有者类型并返回新的样式化属性实例.
    /// </summary>
    /// <typeparam name="TOwner">新的所有者类型.</typeparam>
    /// <returns>返回添加所有者后的属性对象.</returns>
    public StyledProperty<TValue> AddOwner<TOwner>()
    {
        // 调用底层 AddOwner 并包装返回结果.
        return new StyledProperty<TValue>(this.Property.AddOwner(typeof(TOwner)));
    }

    /// <summary>
    /// Gets 属性值的类型.
    /// </summary>
    public Type PropertyType => this.Property.PropertyType;
}