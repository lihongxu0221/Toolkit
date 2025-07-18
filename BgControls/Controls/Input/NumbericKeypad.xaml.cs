using BgCommon;
using BgControls.Controls.Datas;
using Standard;

namespace BgControls.Controls;

/// <summary>
/// NumericKeypad.xaml 的交互逻辑
/// </summary>
public partial class NumericKeypad : UserControl
{
    public static readonly DependencyProperty IsFloatProperty =
        DependencyProperty.Register("IsFloat", typeof(bool), typeof(NumericKeypad), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private string _operand1 = string.Empty;
    private string _operand2 = string.Empty;
    private string _operator = string.Empty;
    private bool isNewEntry = true;

    public event EventHandler<double>? ValueConfirmed;

    public event EventHandler<string>? DisplayTextChanged;

    public NumericKeypad()
    {
        InitializeComponent();
    }

    public bool IsFloat
    {
        get { return (bool)GetValue(IsFloatProperty); }
        set { SetValue(IsFloatProperty, value); }
    }

    public int CaretIndex
    {
        get { return txtDisplay.CaretIndex; }
        set { txtDisplay.CaretIndex = value; }
    }

    public void SetInitialValue(double? value, bool isFloat)
    {
        this.SetCurrentValue(IsFloatProperty, isFloat);

        if (value == null)
        {
            this.Reset();
            return;
        }

        isNewEntry = true;
        txtDisplay.Text = value.ToString();
    }

    public void Reset()
    {
        txtDisplay.Text = "0";
        _operand1 = string.Empty;
        _operand2 = string.Empty;
        _operator = string.Empty;
        isNewEntry = true;
        CaretIndex = txtDisplay.Text.Length;
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button == null)
        {
            return;
        }

        string? content = button.Content.ToString();

        if (char.IsDigit(content[0]))
        {
            HandleDigit(content);
        }
        else
        {
            switch (content)
            {
                case ".":
                    if (!isNewEntry && !txtDisplay.Text.Contains("."))
                    {
                        txtDisplay.Text += ".";
                    }

                    break;

                case "←":
                case "→":
                case "↑":
                case "↓":
                    HandleMoveOperator(content);
                    break;

                case "+":
                case "-":
                case "*":
                case "/":
                    HandleOperator(content);
                    break;
                case "=":
                    HandleEquals();
                    break;
                case "C":
                    Reset();
                    break;
                case "Del":
                    HandleBackspace();
                    break;
            }
        }

        DisplayTextChanged?.Invoke(this, txtDisplay.Text);
    }

    private void HandleDigit(string digit)
    {
        if (isNewEntry)
        {
            txtDisplay.Text = digit;
            isNewEntry = false;
        }
        else
        {
            txtDisplay.Text += digit;
        }
    }

    private void HandleMoveOperator(string op)
    {

    }

    private void HandleOperator(string op)
    {
        if (!string.IsNullOrEmpty(_operator))
        {
            HandleEquals(); // 计算之前的结果
        }

        _operand1 = txtDisplay.Text;
        _operator = op;
        isNewEntry = true;
    }

    private void HandleEquals()
    {
        if (string.IsNullOrEmpty(_operator))
        {
            _operand1 = txtDisplay.Text;
            double val1 = Convert.ToDouble(_operand1);
            ValueConfirmed?.Invoke(this, val1);
            return;
        }

        if (string.IsNullOrEmpty(_operand2))
        {
            _operand2 = txtDisplay.Text;
        }

        try
        {
            double val1 = Convert.ToDouble(_operand1);
            double val2 = Convert.ToDouble(_operand2);
            double result = 0;

            switch (_operator)
            {
                case "+": result = val1 + val2; break;
                case "-": result = val1 - val2; break;
                case "*": result = val1 * val2; break;
                case "/":
                    if (val2 == 0)
                    {
                        _ = Ioc.Instance.Error("Cannot divide by zero.");
                        Reset();
                        return;
                    }

                    result = val1 / val2;
                    break;
            }

            txtDisplay.Text = result.ToString();
            ValueConfirmed?.Invoke(this, result);
        }
        catch (Exception ex)
        {
            _ = Ioc.Instance.Error("Invalid operation: " + ex.Message);
            Reset();
        }
        finally
        {
            _operator = string.Empty;
            isNewEntry = true;
            _operand1 = txtDisplay.Text; // 允许连续计算
            _operand2 = string.Empty;
        }
    }

    private void HandleBackspace()
    {
        if (!isNewEntry && txtDisplay.Text.Length > 0)
        {
            txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
            if (string.IsNullOrEmpty(txtDisplay.Text))
            {
                txtDisplay.Text = "0";
                isNewEntry = true;
            }
        }
    }
}