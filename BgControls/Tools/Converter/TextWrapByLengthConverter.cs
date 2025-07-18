namespace BgControls.Tools.Converter;

/// <summary>
/// 一个值转换器，它接收一个字符串，并根据指定的长度插入换行符。
/// </summary>
public class TextWrapByLengthConverter : IValueConverter
{
    /// <summary>
    /// 将字符串按指定长度分块，并用换行符连接。
    /// </summary>
    /// <param name="value">绑定的源数据，应为字符串。</param>
    /// <param name="targetType">目标属性的类型。</param>
    /// <param name="parameter">每行允许的最大字符数。如果未提供，默认为4。</param>
    /// <param name="culture">要使用的区域性。</param>
    /// <returns>处理后带有换行符的字符串。</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 1. 数据校验
        if (value is string text)
        {
            // 2. 获取参数（每行字符数）
            int chunkSize = 4; // 默认值为4
            if (parameter != null && int.TryParse(parameter.ToString(), out int parsedSize))
            {
                chunkSize = parsedSize;
            }

            // 如果文本长度小于或等于分块大小，则无需处理
            if (text.Length <= chunkSize)
            {
                return text;
            }
            else if (text.Length < 8)
            {
                // 计算分割点。使用 Math.Ceiling 确保第一行更长或等长，视觉上更稳定。
                // 例如：5个字 -> 3 + 2； 6个字 -> 3 + 3； 7个字 -> 4 + 3
                int splitIndex = (int)Math.Ceiling(text.Length / 2.0);

                string firstLine = text.Substring(0, splitIndex);
                string secondLine = text.Substring(splitIndex);
                return $"{firstLine}{Environment.NewLine}{secondLine}";
            }

            // 3. 核心逻辑：插入换行符
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i += chunkSize)
            {
                // 如果不是第一行，则在前面添加换行符
                if (i > 0)
                {
                    _ = sb.Append(Environment.NewLine); // 使用 Environment.NewLine 更具平台通用性
                }

                // 取出当前块的子字符串
                int lengthToTake = Math.Min(chunkSize, text.Length - i);
                _ = sb.Append(text.Substring(i, lengthToTake));
            }

            return sb.ToString();
        }

        // --- 如果传入的 'value' 不是字符串 (例如它是 null, Image, 或其他UIElement) ---
        // 返回 DependencyProperty.UnsetValue. 
        // 这会告诉WPF绑定引擎：“转换失败，请不要设置目标属性(TextBlock.Text)的值”。
        return DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// 不实现反向转换。
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 此场景不需要反向转换
        throw new NotImplementedException();
    }
}
