using BgCommon;
using BgCommon.DependencyInjection;
using BgCommon.MVVM;
using BgControls.Data;
using BgControls.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using hc = HandyControl.Controls;

namespace BgControls.Controls;

/// <summary>
/// InputMessageBox.xaml 的交互逻辑
/// </summary>
[Registration(Registration.Dialog, typeof(InputMessageBoxViewModel))]
public partial class InputMessageBoxView : UserControl, IRegistration
{
    public InputMessageBoxView()
    {
        InitializeComponent();
    }
}

/// <summary>
/// InputMessageBoxViewModel.cs .
/// </summary>
public partial class InputMessageBoxViewModel : DialogViewModelBase
{
    private ObservableCollection<InputContent> contents = new ObservableCollection<InputContent>();

    /// <summary>
    /// Gets a value indicating whether gets 是否有子项
    /// </summary>
    public bool HasItems => Contents.Count > 0;

    /// <summary>
    /// Gets or sets 消息主体
    /// </summary>
    public ObservableCollection<InputContent> Contents
    {
        get => contents;
        set => SetProperty(ref contents, value);
    }

    public InputMessageBoxViewModel(IContainerExtension container)
        : base(container)
    {
    }

    protected override bool OnOK(object? paramter, ref IDialogParameters keys)
    {
        bool isValidSuccess = true;
        StringBuilder builder = new StringBuilder();
        RegexRule rule = new RegexRule();
        for (int i = 0; i < Contents.Count; i++)
        {
            InputContent item = Contents[i];

            if (item.Value == null ||
                (item.Value is string value && value.IsEmpty()))
            {
                _ = builder.AppendLine($"{item.Key}不能为空");
                isValidSuccess = false;
                continue;
            }

            if (item.TextType == TextType.Boolean)
            {
                continue;
            }

            rule.Type = item.TextType;
            ValidationResult vr = rule.Validate(item.ValueString, CultureInfo.CurrentUICulture);
            isValidSuccess = vr.IsValid;
            if (!vr.IsValid)
            {
                _ = builder.AppendLine($"[{item.Key}] {vr.ErrorContent}");
            }
        }

        if (!isValidSuccess)
        {
            _ = this.Warn(builder.ToString());
            _ = builder.Clear();
            return false;
        }

        keys.Add("Results", Contents.Select(t => new InputContent(t)).ToArray());
        return true;
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        Title = GetString("PlzInput");

        if (parameters.ContainsKey("Parameters"))
        {
            if (parameters["Parameters"] is InputContent[] inputItems && inputItems != null)
            {
                for (int i = 0; i < inputItems.Length; i++)
                {
                    var item = new InputContent(inputItems[i]);
                    item.UpdateUILayout();
                    Contents.Add(item);
                }
            }
        }
        else
        {
            var item = new InputContent();
            item.UpdateUILayout();
            Contents.Add(item);
        }
    }
}

/// <summary>
/// 输入参数,结果
/// </summary>
public class InputContent : ObservableObject
{
    private readonly int _index = 0;
    private readonly int _decimalPlaces = 0;
    private object? _value = null;
    private string _key = string.Empty;
    private TextType _textType = TextType.Common;
    private FrameworkElement? _element = null;

    /// <summary>
    /// Gets 索引
    /// </summary>
    public int Index => _index;

    /// <summary>
    /// Gets 小数点后面的位数
    /// </summary>
    public int DecimalPlaces => _decimalPlaces;

    /// <summary>
    /// Gets 输入类型
    /// </summary>
    public TextType TextType
    {
        get => _textType;
        private set => SetProperty(ref _textType, value);
    }

    public string Key
    {
        get => _key;
        private set => SetProperty(ref _key, value);
    }

    public object? Value
    {
        get
        {
            return _value;
        }

        set
        {
            _ = SetProperty(ref _value, value);
        }
    }

    public string ValueString
    {
        get
        {
            if (Value != null)
            {
                if (TextType == TextType.DateTime && Value is DateTime dateTime)
                {
                    return dateTime.ToString("yyyy-mm-dd HH:mm:ss");
                }

                return Value.ToString() ?? string.Empty;
            }

            return string.Empty;
        }
    }

    public bool? ValueBool
    {
        get
        {
            if (!string.IsNullOrEmpty(ValueString))
            {
                if (bool.TryParse(ValueString, out var tempValue))
                {
                    return tempValue;
                }
            }

            return null;
        }
    }

    public int? ValueInt
    {
        get
        {
            if (!string.IsNullOrEmpty(ValueString))
            {
                if (int.TryParse(ValueString, out int tempValue))
                {
                    return tempValue;
                }
            }

            return null;
        }
    }

    public uint? ValueUInt
    {
        get
        {
            if (!string.IsNullOrEmpty(ValueString))
            {
                if (uint.TryParse(ValueString, out uint tempValue))
                {
                    return tempValue;
                }
            }

            return null;
        }
    }

    public double? ValueDouble
    {
        get
        {
            if (!string.IsNullOrEmpty(ValueString))
            {
                if (double.TryParse(ValueString, out double tempValue))
                {
                    return tempValue;
                }
            }

            return null;
        }
    }

    public float? ValueFloat
    {
        get
        {
            if (!string.IsNullOrEmpty(ValueString))
            {
                if (float.TryParse(ValueString, out float tempValue))
                {
                    return tempValue;
                }
            }

            return null;
        }
    }

    public short? ValueShort
    {
        get
        {
            if (!string.IsNullOrEmpty(ValueString))
            {
                if (short.TryParse(ValueString, out var tempValue))
                {
                    return tempValue;
                }
            }

            return null;
        }
    }

    public ushort? ValueUShort
    {
        get
        {
            if (!string.IsNullOrEmpty(ValueString))
            {
                if (ushort.TryParse(ValueString, out ushort tempValue))
                {
                    return tempValue;
                }
            }
            return null;
        }
    }

    public FrameworkElement? Element
    {
        get => _element;
        private set => SetProperty(ref _element, value);
    }

    public InputContent()
        : this(1, TextType.Common, string.Empty, 0)
    {
    }

    public InputContent(InputContent item)
    {
        _index = item.Index;
        _decimalPlaces = item.DecimalPlaces;
        Key = item.Key;
        TextType = item.TextType;
        Value = item.Value;
    }

    public InputContent(int index, TextType textType, object? content, int decimalPlaces = 0)
        : this(index, $"{Ioc.Instance.GetString("Parameter")}{index}", textType, content, decimalPlaces)
    {
    }

    public InputContent(int index, string key, TextType _textType, object? content, int decimalPlaces = 0)
    {
        this._index = index;
        this._decimalPlaces = decimalPlaces;
        Key = key;
        TextType = _textType;

        if (TextType == TextType.Double ||
            TextType == TextType.PDouble ||
            TextType == TextType.NDouble ||
            TextType == TextType.Float ||
            TextType == TextType.PFloat ||
            TextType == TextType.NFloat ||
            TextType == TextType.Int ||
            TextType == TextType.PInt ||
            TextType == TextType.NInt ||
            TextType == TextType.Short ||
            TextType == TextType.PShort ||
            TextType == TextType.NShort)
        {
            if (content is string str && double.TryParse(str, out double value))
            {
                Value = Math.Round(value, decimalPlaces);
            }
            else
            {
                Value = Convert.ChangeType(content, typeof(double));
            }
        }
        else
        {
            Value = content;
        }
    }

    /// <summary>
    /// 更新布局控件
    /// </summary>
    internal void UpdateUILayout()
    {
        DependencyProperty? bindingProperty = null;
        switch (TextType)
        {
            case TextType.Common:
            case TextType.Phone:
            case TextType.Mail:
            case TextType.Url:
            case TextType.Ip:
            {
                Element = new TextBox();
                Element.Style = Application.Current.TryFindResource("TextBoxExtend") as Style;
                bindingProperty = TextBox.TextProperty;
                break;
            }

            case TextType.Int:
            case TextType.NInt:
            case TextType.PInt:
            case TextType.Short:
            case TextType.NShort:
            case TextType.PShort:
            case TextType.Float:
            case TextType.NFloat:
            case TextType.PFloat:
            case TextType.Double:
            case TextType.PDouble:
            case TextType.NDouble:
            {
                Element = new hc.NumericUpDown();
                Element.Style = Application.Current.TryFindResource("NumericUpDownExtend") as Style;
                bindingProperty = hc.NumericUpDown.ValueProperty;
                ((hc.NumericUpDown)Element).Value = (double)Convert.ChangeType(Value, typeof(double));
                ((hc.NumericUpDown)Element).ValueFormat = $"N{DecimalPlaces}";
                if (TextType == TextType.PInt || TextType == TextType.PShort ||
                    TextType == TextType.PFloat || TextType == TextType.PDouble)
                {
                    ((hc.NumericUpDown)Element).Minimum = 0;
                }
                else if (TextType == TextType.NInt || TextType == TextType.NShort ||
                         TextType == TextType.NFloat || TextType == TextType.NDouble)
                {
                    ((hc.NumericUpDown)Element).Maximum = 0;
                }

                break;
            }

            case TextType.DateTime:
            {
                Element = new hc.DateTimePicker();
                Element.Style = Application.Current.TryFindResource("DateTimePickerExtend") as Style;
                bindingProperty = hc.DateTimePicker.SelectedDateTimeProperty;
                break;
            }

            case TextType.Boolean:
            {
                Element = new CheckBox();
                Element.Style = Application.Current.TryFindResource("CheckBoxExtend") as Style;
                bindingProperty = CheckBox.IsCheckedProperty;
                break;
            }
        }

        if (Element != null)
        {
            Element.Margin = new Thickness(5);
            Element.VerticalAlignment = VerticalAlignment.Center;
            Element.HorizontalAlignment = HorizontalAlignment.Center;
            Element.Width = 420;

            hc.InfoElement.SetTitleWidth(Element, new GridLength(120));
            hc.InfoElement.SetNecessary(Element, true);
            hc.InfoElement.SetTitle(Element, Key);
            hc.InfoElement.SetTitlePlacement(Element, HandyControl.Data.TitlePlacementType.Left);
            hc.InfoElement.SetPlaceholder(Element, Ioc.Instance.GetString("PlzInput"));

            Binding binding = new Binding
            {
                Source = this,       // 绑定源
                Path = new PropertyPath(nameof(Value)), // 绑定路径
                Mode = BindingMode.TwoWay,       // 双向绑定
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            _ = Element.SetBinding(bindingProperty, binding);
        }
    }

    public override string ToString()
    {
        return $"{Key} {ValueString}";
    }
}

/// <summary>
/// MessageBox 扩展方法类
/// </summary>
public static partial class MessageBoxViewExtension
{
    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  TextType.Common:<br/>
    /// 2.  TextType.Phone:<br/>
    /// 3.  TextType.Mail:<br/>
    /// 4.  TextType.Url:<br/>
    /// 5.  TextType.Ip:<br/>
    /// 6.  TextType.Double <br/>
    /// 7.  TextType.PDouble<br/>
    /// 8.  TextType.NDouble<br/>
    /// 9.  TextType.Int <br/>
    /// 10. TextType.PInt<br/>
    /// 11. TextType.NInt<br/>
    /// 12. TextType.DateTime<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="inputParams">输入参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInput(this Ioc ioc, params InputContent[] inputParams)
    {
        return ioc.DialogService?.ShowInputDialog(inputParams);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  TextType.Common:<br/>
    /// 2.  TextType.Phone:<br/>
    /// 3.  TextType.Mail:<br/>
    /// 4.  TextType.Url:<br/>
    /// 5.  TextType.Ip:<br/>
    /// 6.  TextType.Double <br/>
    /// 7.  TextType.PDouble<br/>
    /// 8.  TextType.NDouble<br/>
    /// 9.  TextType.Int <br/>
    /// 10. TextType.PInt<br/>
    /// 11. TextType.NInt<br/>
    /// 12. TextType.DateTime<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="inputParams">输入参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputAsync(this Ioc ioc, params InputContent[] inputParams)
    {
        return await ioc.DialogService?.ShowInputDialogAsync(inputParams);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInput(this Ioc ioc, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        string[] array = new string[length];
        Array.Fill(array, string.Empty);
        return ioc?.ShowInput(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputInt(this Ioc ioc, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        int[] array = new int[length];
        Array.Fill(array, 0);
        return ioc?.ShowInputInt(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUInt(this Ioc ioc, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        uint[] array = new uint[length];
        Array.Fill(array, uint.MinValue);
        return ioc?.ShowInputUInt(array);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputDouble(this Ioc ioc, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return ioc?.ShowInputDouble(decimalPlaces, array);
    }

    /// <summary>
    /// 显示非负 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUDouble(this Ioc ioc, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return ioc?.ShowInputUDouble(decimalPlaces, array);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInput(this Ioc ioc, params string[] contents)
    {
        return ioc.DialogService!.ShowInputDialog(TextType.Common, 0, contents);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputInt(this Ioc ioc, params int[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.Int, 0, contents);
    }

    /// <summary>
    /// 显示uint输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUInt(this Ioc ioc, params uint[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.PInt, 0, contents);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputDouble(this Ioc ioc, int decimalPlaces, params double[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.Double, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示 udouble 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUDouble(this Ioc ioc, int decimalPlaces, params double[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.PDouble, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示 short 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputShort(this Ioc ioc, params short[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.Short, 0, contents);
    }

    /// <summary>
    /// 显示 ushort 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUShort(this Ioc ioc, params ushort[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.PShort, 0, contents);
    }

    /// <summary>
    /// 显示 float 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputFloat(this Ioc ioc, int decimalPlaces, params float[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.Float, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示 ufloat 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUFloat(this Ioc ioc, int decimalPlaces, params float[] contents)
    {
        return ioc?.DialogService?.ShowInputDialog(TextType.PFloat, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputAsync(this Ioc ioc, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        string[] array = new string[length];
        Array.Fill(array, string.Empty);
        return await ioc.ShowInputAsync(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputIntAsync(this Ioc ioc, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        int[] array = new int[length];
        Array.Fill(array, 0);
        return await ioc.ShowInputIntAsync(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUIntAsync(this Ioc ioc, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        uint[] array = new uint[length];
        Array.Fill(array, uint.MinValue);
        return await ioc.ShowInputUIntAsync(array);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputDoubleAsync(this Ioc ioc, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return await ioc.ShowInputDoubleAsync(decimalPlaces, array);
    }

    /// <summary>
    /// 显示非负 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUDoubleAsync(this Ioc ioc, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return await ioc.ShowInputUDoubleAsync(decimalPlaces, array);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputAsync(this Ioc ioc, params string[] contents)
    {
        return await ioc.DialogService!.ShowInputDialogAsync(TextType.Common, 0, contents);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputIntAsync(this Ioc ioc, params int[] contents)
    {
        return await ioc.DialogService!.ShowInputDialogAsync(TextType.Int, 0, contents);
    }

    /// <summary>
    /// 显示uint输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUIntAsync(this Ioc ioc, params uint[] contents)
    {
        return await ioc.DialogService!.ShowInputDialogAsync(TextType.PInt, 0, contents);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputDoubleAsync(this Ioc ioc, int decimalPlaces, params double[] contents)
    {
        return await ioc.DialogService!.ShowInputDialogAsync(TextType.Double, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示 udouble 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUDoubleAsync(this Ioc ioc, int decimalPlaces, params double[] contents)
    {
        return await ioc.DialogService!.ShowInputDialogAsync(TextType.PDouble, decimalPlaces, contents);
    }


    /// <summary>
    /// 显示 short 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputShortAsync(this Ioc ioc, params short[] contents)
    {
        return await ioc?.DialogService?.ShowInputDialogAsync(TextType.Short, 0, contents);
    }

    /// <summary>
    /// 显示 ushort 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUShortAsync(this Ioc ioc, params ushort[] contents)
    {
        return await ioc?.DialogService?.ShowInputDialogAsync(TextType.PShort, 0, contents);
    }

    /// <summary>
    /// 显示 float 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputFloatAsync(this Ioc ioc, int decimalPlaces, params float[] contents)
    {
        return await ioc?.DialogService?.ShowInputDialogAsync(TextType.Float, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示 ufloat 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="ioc">容器</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUFloatAsync(this Ioc ioc, int decimalPlaces, params float[] contents)
    {
        return await ioc?.DialogService?.ShowInputDialogAsync(TextType.PFloat, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  TextType.Common:<br/>
    /// 2.  TextType.Phone:<br/>
    /// 3.  TextType.Mail:<br/>
    /// 4.  TextType.Url:<br/>
    /// 5.  TextType.Ip:<br/>
    /// 6.  TextType.Double <br/>
    /// 7.  TextType.PDouble<br/>
    /// 8.  TextType.NDouble<br/>
    /// 9.  TextType.Int <br/>
    /// 10. TextType.PInt<br/>
    /// 11. TextType.NInt<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="inputParams">输入参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInput(this ViewModelBase vm, params InputContent[] inputParams)
    {
        return vm.DialogService?.ShowInputDialog(inputParams);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  TextType.Common:<br/>
    /// 2.  TextType.Phone:<br/>
    /// 3.  TextType.Mail:<br/>
    /// 4.  TextType.Url:<br/>
    /// 5.  TextType.Ip:<br/>
    /// 6.  TextType.Double <br/>
    /// 7.  TextType.PDouble<br/>
    /// 8.  TextType.NDouble<br/>
    /// 9.  TextType.Int <br/>
    /// 10. TextType.PInt<br/>
    /// 11. TextType.NInt<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="inputParams">输入参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputAsync(this ViewModelBase vm, params InputContent[] inputParams)
    {
        return await vm.DialogService?.ShowInputDialogAsync(inputParams);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInput(this ViewModelBase vm, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        string[] array = new string[length];
        Array.Fill(array, string.Empty);
        return vm.ShowInput(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputInt(this ViewModelBase vm, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        int[] array = new int[length];
        Array.Fill(array, 0);
        return vm.ShowInputInt(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUInt(this ViewModelBase vm, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        uint[] array = new uint[length];
        Array.Fill(array, uint.MinValue);
        return vm.ShowInputUInt(array);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputDouble(this ViewModelBase vm, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return vm.ShowInputDouble(decimalPlaces, array);
    }

    /// <summary>
    /// 显示非负 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUDouble(this ViewModelBase vm, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return vm.ShowInputUDouble(decimalPlaces, array);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInput(this ViewModelBase vm, params string[] contents)
    {
        return vm.DialogService!.ShowInputDialog(TextType.Common, 0, contents);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputInt(this ViewModelBase vm, params int[] contents)
    {
        return vm.DialogService?.ShowInputDialog(TextType.Int, 0, contents);
    }

    /// <summary>
    /// 显示uint输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUInt(this ViewModelBase vm, params uint[] contents)
    {
        return vm.DialogService?.ShowInputDialog(TextType.PInt, 0, contents);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputDouble(this ViewModelBase vm, int decimalPlaces, params double[] contents)
    {
        return vm.DialogService?.ShowInputDialog(TextType.Double, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示 udouble 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static InputContent[]? ShowInputUDouble(this ViewModelBase vm, int decimalPlaces, params double[] contents)
    {
        return vm.DialogService?.ShowInputDialog(TextType.PDouble, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputAsync(this ViewModelBase vm, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        string[] array = new string[length];
        Array.Fill(array, string.Empty);
        return await vm.ShowInputAsync(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputIntAsync(this ViewModelBase vm, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        int[] array = new int[length];
        Array.Fill(array, 0);
        return await vm.ShowInputIntAsync(array);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUIntAsync(this ViewModelBase vm, int itemCount = 1)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        uint[] array = new uint[length];
        Array.Fill(array, uint.MinValue);
        return await vm.ShowInputUIntAsync(array);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputDoubleAsync(this ViewModelBase vm, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return await vm.ShowInputDoubleAsync(decimalPlaces, array);
    }

    /// <summary>
    /// 显示非负 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="itemCount">参数数量</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUDoubleAsync(this ViewModelBase vm, int itemCount, int decimalPlaces = 3)
    {
        int length = itemCount <= 0 ? 1 : itemCount;
        double[] array = new double[length];
        Array.Fill(array, 0);
        return await vm.ShowInputUDoubleAsync(decimalPlaces, array);
    }

    /// <summary>
    /// 显示输入对话框，支持以下类型<br/>
    /// 1.  Common:<br/>
    /// 2.  Phone:<br/>
    /// 3.  Mail:<br/>
    /// 4.  Url:<br/>
    /// 5.  Ip:<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputAsync(this ViewModelBase vm, params string[] contents)
    {
        return await vm.DialogService!.ShowInputDialogAsync(TextType.Common, 0, contents);
    }

    /// <summary>
    /// 显示int输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputIntAsync(this ViewModelBase vm, params int[] contents)
    {
        return await vm.DialogService!.ShowInputDialogAsync(TextType.Int, 0, contents);
    }

    /// <summary>
    /// 显示uint输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUIntAsync(this ViewModelBase vm, params uint[] contents)
    {
        return await vm.DialogService!.ShowInputDialogAsync(TextType.PInt, 0, contents);
    }

    /// <summary>
    /// 显示 double 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputDoubleAsync(this ViewModelBase vm, int decimalPlaces, params double[] contents)
    {
        return await vm.DialogService!.ShowInputDialogAsync(TextType.Double, decimalPlaces, contents);
    }

    /// <summary>
    /// 显示 udouble 输入对话框，支持以下类型<br/>
    /// </summary>
    /// <param name="vm">ViewModel实例</param>
    /// <param name="decimalPlaces">小数点位数</param>
    /// <param name="contents">预设的参数列表</param>
    /// <returns>成功：返回输入的参数 失败：返回 null</returns>
    public static async Task<InputContent[]?> ShowInputUDoubleAsync(this ViewModelBase vm, int decimalPlaces, params double[] contents)
    {
        return await vm.DialogService!.ShowInputDialogAsync(TextType.PDouble, decimalPlaces, contents);
    }

    private static InputContent[]? ShowInputDialog<T>(this IDialogService dialogService, TextType textType = TextType.Common, int decimalPlaces = 3, params T[] contents)
    {
        InputContent[]? results = null;
        IDialogResult? result = null;
        try
        {

            dialogService.ShowDialog(
                nameof(InputMessageBoxView),
                ret => result = ret,
                keys => keys.Configure(textType, decimalPlaces, contents)
            );

            if (result != null)
            {
                if (result.Result == ButtonResult.OK &&
                    result.Parameters.TryGetValue("Results", out InputContent[]? resultArray) && resultArray != null)
                {
                    results = resultArray;
                    return results;
                }

                if (result.Exception != null)
                {
                    throw result.Exception;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }

        return results;
    }

    private static async Task<InputContent[]?> ShowInputDialogAsync<T>(this IDialogService dialogService, TextType textType = TextType.Common, int decimalPlaces = 3, params T[] contents)
    {
        InputContent[]? results = null;
        IDialogResult? result = null;
        try
        {
            if (dialogService == null)
            {
                return results;
            }

            result = await dialogService.ShowDialogAsync(
                nameof(InputMessageBoxView),
                keys => keys.Configure(textType, decimalPlaces, contents)
                ).ConfigureAwait(true);

            if (result != null)
            {
                if (result.Result == ButtonResult.OK &&
                    result.Parameters.TryGetValue("Results", out InputContent[]? resultArray) && resultArray != null)
                {
                    results = resultArray;
                    return results;
                }

                if (result.Exception != null)
                {
                    throw result.Exception;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }

        return results;
    }

    private static InputContent[]? ShowInputDialog(this IDialogService dialogService, params InputContent[] inputParams)
    {
        InputContent[]? results = null;
        IDialogResult? result = null;
        try
        {

            dialogService.ShowDialog(
                nameof(InputMessageBoxView),
                ret => result = ret,
                keys => keys.Configure(inputParams)
            );

            if (result != null)
            {
                if (result.Result == ButtonResult.OK &&
                    result.Parameters.TryGetValue("Results", out InputContent[]? resultArray) && resultArray != null)
                {
                    results = resultArray;
                    return results;
                }

                if (result.Exception != null)
                {
                    throw result.Exception;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }

        return results;
    }

    private static async Task<InputContent[]?> ShowInputDialogAsync(this IDialogService dialogService, params InputContent[] inputParams)
    {
        InputContent[]? results = null;
        IDialogResult? result = null;
        try
        {
            if (dialogService == null)
            {
                return results;
            }

            result = await dialogService.ShowDialogAsync(
                nameof(InputMessageBoxView),
                config: keys => keys.Configure(inputParams)
                ).ConfigureAwait(true);

            if (result != null)
            {
                if (result.Result == ButtonResult.OK &&
                    result.Parameters.TryGetValue("Results", out InputContent[]? resultArray) && resultArray != null)
                {
                    results = resultArray;
                    return results;
                }

                if (result.Exception != null)
                {
                    throw result.Exception;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }

        return results;
    }

    private static IDialogParameters Configure<T>(this IDialogParameters parameters, TextType textType, int decimalPlaces, params T[] contents)
    {
        InputContent[] inputParams = new InputContent[contents.Length];
        for (int i = 0; i < contents.Length; i++)
        {
            inputParams[i] = new InputContent(
                i + 1,
                $"{Ioc.Instance.GetString("Parameter")}{i + 1}",
                textType,
                contents[i],
                decimalPlaces);
        }

        return parameters.Configure(inputParams);
    }

    private static IDialogParameters Configure(this IDialogParameters parameters, params InputContent[] contents)
    {
        InputContent?[] inputParams = contents;
        if (contents.Length == 0)
        {
            inputParams = new InputContent[1]
            {
                new InputContent()
            };
        }

        parameters.Add("Parameters", inputParams);
        return parameters;
    }
}
