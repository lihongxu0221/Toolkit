using BgCommon.Prism.Wpf.Controls.Models;
using HandyControl.Controls;

namespace BgCommon.Prism.Wpf.Controls.Behaviors;

public static class InputContentBehavior
{
    // 1. 定义附加属性
    public static readonly DependencyProperty ConfigurationProperty =
        DependencyProperty.RegisterAttached("Configuration", typeof(InputContent), typeof(InputContentBehavior), new PropertyMetadata(null, OnConfigurationChanged));

    // Getter
    public static InputContent GetConfiguration(DependencyObject obj)
    {
        return (InputContent)obj.GetValue(ConfigurationProperty);
    }

    // Setter
    public static void SetConfiguration(DependencyObject obj, InputContent value)
    {
        obj.SetValue(ConfigurationProperty, value);
    }

    // 2. 属性变化时的回调方法
    private static void OnConfigurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // 确保附加的对象是 NumericUpDown，并且新的配置值不为空
        if (d is not NumericUpDown numericUpDown || e.NewValue is not InputContent config)
        {
            return;
        }

        // 3. 在这里执行所有复杂的配置逻辑
        // 注意：此时 DataTemplate 中的其他绑定（如Value）可能还未生效，
        // 所以我们直接从 config 对象获取信息。

        // 设置小数位数格式化
        numericUpDown.ValueFormat = $"N{config.DecimalPlaces}";

        // 根据 TextType 设置最大/最小值
        if (config.TextType.IsPositive())
        {
            numericUpDown.Minimum = 0;
        }
        else if (config.TextType.IsNegative())
        {
            // 对于负数，HandyControl 的 NumericUpDown 可能需要设置 Maximum 为 0
            // 而不是 Minimum。请根据控件具体行为调整。
            numericUpDown.Maximum = 0;
        }
        else
        {
            // 如果不是 P/N 类型，清除可能存在的限制
            numericUpDown.Minimum = double.MinValue;
            numericUpDown.Maximum = double.MaxValue;
        }
    }
}