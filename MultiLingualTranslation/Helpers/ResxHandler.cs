using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Resources;

namespace MultiLingualTranslation.Helpers;

public static class ResxHandler
{
    /// <summary>
    /// 读取原始.resx文件，提取用户自定义资源键值对.
    /// </summary>
    /// <param name="resxFilePath">原始.resx文件完整路径（如：D:\Projects\Resources.resx）.</param>
    /// <param name="isKeyFillEmptyValue">当value为空时，使用Key填充.</param>
    /// <returns>资源键值对字典.</returns>
    public static Dictionary<string, string> ReadFrom(string resxFilePath, bool isKeyFillEmptyValue = false)
    {
        var resourceDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // 校验文件是否存在
        if (!File.Exists(resxFilePath))
        {
            // throw new FileNotFoundException("原始.resx文件不存在", resxFilePath);
            return resourceDict;
        }

        // 使用ResXResourceReader读取.resx文件
        using (var resxReader = new ResXResourceReader(resxFilePath))
        {
            // 启用元数据解析，确保正确读取键值对
            resxReader.UseResXDataNodes = true;

            // 遍历所有资源项
            foreach (DictionaryEntry entry in resxReader)
            {
                var resxNode = entry.Value as ResXDataNode;
                if (resxNode == null)
                {
                    continue;
                }

                // 过滤系统配置项（以"$"开头的项，如$this、$root，无需翻译）
                if (resxNode.Name.StartsWith('$') || string.IsNullOrEmpty(resxNode.Name))
                {
                    continue;
                }

                // 提取资源值（忽略空值资源）
                var resourceValue = resxNode.GetValue((ITypeResolutionService)null)?.ToString() ?? string.Empty;
                if (!resourceDict.ContainsKey(resxNode.Name))
                {
                    if (isKeyFillEmptyValue && string.IsNullOrEmpty(resourceValue))
                    {
                        resourceValue = resxNode.Name;
                    }

                    resourceDict.Add(resxNode.Name, resourceValue);
                }
            }
        }

        return resourceDict;
    }

    /// <summary>
    /// 生成目标语言.resx文件，写入翻译后的键值对.
    /// </summary>
    /// <param name="targetResxFilePath">目标.resx文件完整路径（如：D:\Projects\Resources.en-US.resx）.</param>
    /// <param name="translatedResourceDict">翻译后的资源键值对字典.</param>/// <summary>
    /// 生成目标语言.resx文件。
    /// 逻辑：
    /// 1. 如果目标文件已存在 Key 且 Value 不为空，则保留原值；
    /// 2. 如果目标文件已存在 Key 但 Value 为空，则更新为新翻译值；
    /// 3. 如果目标文件不存在该 Key，则新增.
    /// </summary>
    public static void WriteTo(string targetResxFilePath, Dictionary<string, string> translatedResourceDict)
    {
        // 1. 读取目标文件现有的内容（如果不存在则返回空字典）
        var existingContent = ReadFrom(targetResxFilePath);

        // 2. 准备最终要写入的字典
        // 先以现有的内容为基础，这样可以保留所有旧的 Key
        var finalContent = new Dictionary<string, string>(existingContent, StringComparer.OrdinalIgnoreCase);

        int addedCount = 0;
        int updatedCount = 0;

        // 3. 遍历新翻译的内容进行合并
        foreach (var item in translatedResourceDict)
        {
            string key = item.Key;
            string newValue = item.Value;

            if (finalContent.TryGetValue(key, out var existingValue))
            {
                // 情况：Key 已存在
                // 只有当原有 Value 为空（Null/Empty/空格）时，才更新它
                if (string.IsNullOrWhiteSpace(existingValue))
                {
                    finalContent[key] = newValue;
                    updatedCount++;
                }
            }
            else
            {
                // 情况：Key 不存在，直接添加
                finalContent.Add(key, newValue);
                addedCount++;
            }
        }

        // 4. 执行物理写入
        var targetDirectory = Path.GetDirectoryName(targetResxFilePath);
        if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        using (var resxWriter = new ResXResourceWriter(targetResxFilePath))
        {
            foreach (var kvp in finalContent)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    continue;
                }

                var resxDataNode = new ResXDataNode(kvp.Key, kvp.Value);

                // // 逻辑：只有新增或更新过的项才打上自动翻译的注释
                // // 判断依据：该 Key 是否在原始翻译字典里，且最终值等于翻译值
                // if (translatedResourceDict.ContainsKey(kvp.Key) && kvp.Value == translatedResourceDict[kvp.Key])
                // {
                //     // 注意：如果原文件里本来就有的且值没变，通常不改动它的注释
                //     // 这里简单处理：只要是来自百度翻译的补全，就加注释
                //     if (!existingContent.ContainsKey(kvp.Key) || string.IsNullOrWhiteSpace(existingContent[kvp.Key]))
                //     {
                //         resxDataNode.Comment = "自动翻译补全（百度翻译API）";
                //     }
                // }
                resxWriter.AddResource(resxDataNode);
            }

            resxWriter.Generate();
        }

        Console.WriteLine($"[{Path.GetFileName(targetResxFilePath)}] 处理完毕: 新增 {addedCount} 条, 填补空值 {updatedCount} 条.");
    }
}