namespace BgControls.Controls;

/// <summary>
/// NumericKeypad.xaml 的交互逻辑
/// </summary>
public partial class NumericKeypad : UserControl
{
    private string operand1 = string.Empty;
    private string operand2 = string.Empty;
    private string operatorChar = string.Empty;
    private bool isNewEntry = true;

    private double maximum = double.MaxValue;
    private double minimum = double.MinValue;
    private double? initialValue = null;
    private int? decimalPlaces = null;

    public NumericKeypad()
    {
        InitializeComponent();
    }

    public int CaretIndex
    {
        get { return txtDisplay.CaretIndex; }
        set { txtDisplay.CaretIndex = value; }
    }

    public event EventHandler<double>? ValueConfirmed;

    public event EventHandler<string>? DisplayTextChanged;

    /// <summary>
    /// 设置小键盘的初始值和输入约束.这是与 NumberInput 交互的入口.
    /// </summary>
    /// <param name="value">初始值</param>
    /// <param name="maximum">允许的最大值</param>
    /// <param name="minimum">允许的最小值</param>
    /// <param name="decimalPlaces">允许的小数位数</param>
    public void SetInitialValue(double? value, double maximum, double minimum, int? decimalPlaces)
    {
        this.maximum = maximum;
        this.minimum = minimum;
        this.decimalPlaces = decimalPlaces;

        // 根据约束更新UI（启用/禁用按钮）
        this.UpdateConstraintUI();

        this.btnDecimal.IsEnabled = decimalPlaces.HasValue && decimalPlaces > 0;
        this.btnNegative.IsEnabled = this.minimum < 0;

        // 设置初始显示文本
        if (value == null)
        {
            this.Reset();
            return;
        }

        initialValue = value;
        isNewEntry = true;
        txtDisplay.Text = ToString(value);
        CaretIndex = txtDisplay.Text?.Length ?? 0;
    }

    public void Reset()
    {
        txtDisplay.Text = ToString();
        operand1 = string.Empty;
        operand2 = string.Empty;
        operatorChar = string.Empty;
        isNewEntry = true;
        CaretIndex = txtDisplay.Text.Length;
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button == null)
        {
            return;
        }

        string content = button.Content.ToString() ?? string.Empty;

        if (button.Content is TextBlock { Text: string value })
        {
            content = value;
        }

        if (char.IsDigit(content[0]))
        {
            HandleDigit(content);
        }
        else
        {
            switch (content)
            {
                case ".":
                    HandleDecimalPoint();
                    break;

                case "-":
                    HandleNegativeSign();
                    break;

                case "←":
                case "→":
                case "↑":
                case "↓":
                    HandleMoveOperator(content);
                    break;

                // case "+":
                // case "-":
                // case "*":
                // case "/":
                //    HandleOperator(content);
                //    break;

                case "C":
                    Reset();
                    break;

                case "":
                    HandleBackspace();
                    break;

                case "OK":
                    HandleEquals(true);
                    break;
            }
        }

        DisplayTextChanged?.Invoke(this, txtDisplay.Text);
    }

    private string ToString(double? value = null)
    {
        double tempValue = 0;
        if (value.HasValue)
        {
            tempValue = value.Value;
        }

        if (this.decimalPlaces.HasValue)
        {
            return tempValue.ToString($"F{decimalPlaces.Value}", CultureInfo.InvariantCulture);
        }

        return tempValue.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 处理数字输入，并实时检查范围.
    /// </summary>
    private void HandleDigit(string digit)
    {
        string prospectiveText;
        if (isNewEntry || txtDisplay.Text == "0")
        {
            prospectiveText = txtDisplay.Text.StartsWith("-") ? "-" + digit : digit;
            isNewEntry = false;
        }
        else
        {
            prospectiveText = txtDisplay.Text + digit;
        }

        // 实时检查小数位数
        if (decimalPlaces.HasValue && prospectiveText.Contains('.'))
        {
            var decimalPart = prospectiveText.Substring(prospectiveText.IndexOf('.') + 1);
            if (decimalPart.Length > decimalPlaces.Value)
            {
                // 超出小数位限制，输入无效
                return;
            }
        }

        // 实时预览检查是否会超出范围
        if (double.TryParse(prospectiveText, NumberStyles.Any, CultureInfo.InvariantCulture, out double prospectiveValue))
        {
            // 这里只检查是否超出最大值，因为输入过程中可能暂时小于最小值（例如输入负数时）
            if (prospectiveValue > maximum)
            {
                // 超出最大值，输入无效
                Trace.TraceInformation($"Input cannot exceed maximum value: {maximum}");
                return;
            }
        }

        txtDisplay.Text = prospectiveText;
    }

    /// <summary>
    /// 处理小数点输入.
    /// </summary>
    private void HandleDecimalPoint()
    {
        if (isNewEntry)
        {
            txtDisplay.Text = "0.";
            isNewEntry = false;
        }
        else if (!txtDisplay.Text.Contains("."))
        {
            txtDisplay.Text += ".";
        }
    }

    /// <summary>
    /// 处理负号输入.
    /// </summary>
    private void HandleNegativeSign()
    {
        if (txtDisplay.Text.StartsWith("-"))
        {
            // 移除负号
            txtDisplay.Text = txtDisplay.Text.Substring(1);
        }
        else
        {
            // 添加负号，但要检查添加后是否会小于最小值
            if (double.TryParse(txtDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double currentValue))
            {
                if (-currentValue >= minimum)
                {
                    txtDisplay.Text = "-" + txtDisplay.Text;
                }
                else
                {
                    Trace.TraceInformation($"Input cannot be less than minimum value: {minimum}");
                }
            }
        }
    }

    /// <summary>
    /// 处理退格.
    /// </summary>
    private void HandleBackspace()
    {
        if (isNewEntry && initialValue != null)
        {
            isNewEntry = false;
        }

        if (!isNewEntry && txtDisplay.Text.Length > 0)
        {
            txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
            if (string.IsNullOrEmpty(txtDisplay.Text) || txtDisplay.Text == "-")
            {
                txtDisplay.Text = "0";
                isNewEntry = true;
            }
        }
    }

    private void HandleMoveOperator(string op)
    {
    }

    private void HandleOperator(string op)
    {
        if (!string.IsNullOrEmpty(operatorChar))
        {
            HandleEquals(false); // 计算之前的结果
        }

        operand1 = txtDisplay.Text;
        operatorChar = op;
        isNewEntry = true;
    }

    /// <summary>
    /// 处理等于（或确认）操作。
    /// </summary>
    /// <param name="finalConfirmation">指示这是否是一次最终的用户确认操作。</param>
    private void HandleEquals(bool finalConfirmation)
    {
        double result = 0;
        if (string.IsNullOrEmpty(operatorChar))
        {
            operand1 = txtDisplay.Text;

            // 没有操作符，直接解析当前显示的值
            if (!double.TryParse(operand1, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                Trace.TraceError("Invalid number format.");
                Reset();
                return;
            }

            // double val1 = Convert.ToDouble(_operand1);
            // ValueConfirmed?.Invoke(this, val1);
            // return;
        }
        else
        {
            // 有操作符，执行计算
            if (!isNewEntry)
            {
                operand2 = txtDisplay.Text;
            }

            // 尝试计算结果
            if (!TryCalculate(out result))
            {
                return; // 计算失败，已在方法内处理
            }
        }

        // 在最终确认前，将结果限制在范围内
        result = Math.Max(minimum, Math.Min(maximum, result));

        // 如果是最终确认，则触发事件并可能关闭
        if (finalConfirmation)
        {
            ValueConfirmed?.Invoke(this, result);
        }

        // 更新显示并重置计算状态
        txtDisplay.Text = ToString(result);
        ResetCalculationState();
    }

    /// <summary>
    /// 计算完成后重置操作数和操作符状态.
    /// </summary>
    /// <param name="result">计算结果.</param>
    /// <returns>返回是否计算完成.</returns>
    private bool TryCalculate(out double result)
    {
        result = 0;
        double val1, val2;
        if (!double.TryParse(operand1, NumberStyles.Any, CultureInfo.InvariantCulture, out val1) ||
            !double.TryParse(operand2, NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
        {
            Trace.TraceError("Invalid number format for calculation.");
            Reset();
            return false;
        }

        switch (operatorChar)
        {
            case "+": result = val1 + val2; break;
            case "-": result = val1 - val2; break;
            case "*": result = val1 * val2; break;
            case "/":
                if (val2 == 0)
                {
                    Trace.TraceError("Cannot divide by zero.");
                    Reset();
                    return false;
                }

                result = val1 / val2;
                break;
        }

        return true;
    }

    private void ResetCalculationState()
    {
        operatorChar = string.Empty;
        isNewEntry = true;
        operand1 = txtDisplay.Text; // 允许连续计算
        operand2 = string.Empty;
    }

    /// <summary>
    /// 根据当前的约束条件，更新UI元素（如按钮）的可用状态。
    /// </summary>
    private void UpdateConstraintUI()
    {
        // 假设您的小数点按钮在XAML中有名为 "btnDecimal"
        if (this.FindName("btnDecimal") is Button decimalButton)
        {
            // 如果不允许小数 (DecimalPlaces 为 0 或 null)，则禁用小数点按钮
            decimalButton.IsEnabled = decimalPlaces.HasValue && decimalPlaces.Value > 0;
        }

        // 假设您的负号按钮在XAML中有名为 "btnNegative"
        if (this.FindName("btnNegative") is Button negativeButton)
        {
            // 如果允许的最小值大于等于0，则禁用负号按钮
            negativeButton.IsEnabled = minimum < 0;
        }
    }
}