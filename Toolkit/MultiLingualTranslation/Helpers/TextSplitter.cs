namespace MultiLingualTranslation.Helpers;

public class TextSplitter
{
    /// <summary>
    /// 将字符串数组按Environment.NewLine拼接，确保每个拼接结果长度≤maxLength.
    /// </summary>
    /// <param name="maxLength">待拼接字符串最大长度.</param>
    /// <param name="sourceTexts">待拼接的字符串数组（可为空/空数组）.</param>
    /// <returns> 拆分后的拼接字符串列表.</returns>
    /// <exception cref="ArgumentOutOfRangeException">单个元素长度超过6000时抛出. </exception>
    public static List<string> SplitToMaxLength(int maxLength, params string[] sourceTexts)
    {
        // 初始化结果列表
        var result = new List<string>();

        // 处理空输入：直接返回空列表
        if (sourceTexts == null || sourceTexts.Length == 0)
        {
            return result;
        }

        // 获取换行符的长度（Windows是2(\r\n)，Linux/Mac是1(\n)）
        int newLineLength = Environment.NewLine.Length;

        // 当前批次的元素列表（用于临时存储待拼接的元素）
        var currentBatch = new List<string>();

        // 当前批次拼接后的总长度（初始为0）
        int currentTotalLength = 0;

        // 遍历每个源文本元素
        foreach (var text in sourceTexts)
        {
            // 处理null元素：视为空字符串
            string currentText = text ?? string.Empty;

            // 检查单个元素是否超过6000：若超过则无法拆分，抛出异常
            if (currentText.Length > maxLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sourceTexts),
                    "数组中存在单个元素长度超过6000，无法满足拼接后长度≤6000的要求");
            }

            // 计算添加当前元素后的总长度：
            // - 若当前批次已有元素，需额外加换行符长度
            // - 若当前批次为空，直接取当前元素长度
            int lengthToAdd = currentBatch.Count == 0
                ? currentText.Length
                : currentText.Length + newLineLength;

            // 判断添加后是否超过6000
            if (currentTotalLength + lengthToAdd > maxLength)
            {
                // 超过限制：保存当前批次，开始新批次
                if (currentBatch.Count > 0)
                {
                    result.Add(string.Join(Environment.NewLine, currentBatch));

                    // 重置当前批次
                    currentBatch.Clear();
                    currentTotalLength = 0;
                }
            }

            // 将当前元素加入批次，并更新总长度
            currentBatch.Add(currentText);
            currentTotalLength += lengthToAdd;
        }

        // 处理最后一个未保存的批次
        if (currentBatch.Count > 0)
        {
            result.Add(string.Join(Environment.NewLine, currentBatch));
        }

        return result;
    }
}