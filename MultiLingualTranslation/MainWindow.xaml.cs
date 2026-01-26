using MultiLingualTranslation.Helpers;
using MultiLingualTranslation.Services;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MultiLingualTranslation;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        // 1. 配置参数（可改为从配置文件读取，更灵活）
        string originalResxPath = txtResPath.Text; // 原始.resx文件路径

        if (!File.Exists(originalResxPath))
        {
            System.Windows.MessageBox.Show("指定的原始.resx文件路径不存在，请检查后重试。", "文件不存在", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        string originalResxName = System.IO.Path.GetFileNameWithoutExtension(originalResxPath);
        string targetResxDirectory = System.IO.Path.GetDirectoryName(originalResxPath); // 目标.resx文件存放目录
        Dictionary<string, string> targetLangConfig = new Dictionary<string, string>
        {
            ["zh-CN"] = "zh",
            ["zh-TW"] = "cht",
            ["en-US"] = "en",
            ["vi"] = "vie",
        };

        try
        {
            // 2. 读取原始.resx文件，提取键值对
            var originalResourceDict = ResxHandler.ReadFrom(originalResxPath, true);
            if (this.txtTranslationTexts.Document.Blocks.Count > 0)
            {
                bool needReWrite = false;
                foreach (var block in this.txtTranslationTexts.Document.Blocks)
                {
                    TextRange tr = new TextRange(block.ContentStart, block.ContentEnd);
                    if (!string.IsNullOrEmpty(tr.Text) &&
                        !originalResourceDict.ContainsKey(tr.Text))
                    {
                        originalResourceDict.Add(tr.Text, tr.Text);
                        needReWrite = true;
                    }
                }

                if (needReWrite)
                {
                    ResxHandler.WriteTo(originalResxPath, originalResourceDict);
                }
            }

            var translationItems = new List<TranslationService.TranslateItem>();
            foreach (var translationItem in originalResourceDict)
            {
                translationItems.Add(new TranslationService.TranslateItem()
                {
                    Key = translationItem.Key,
                    Src = translationItem.Value,
                    Dst = string.Empty,
                });
            }

            Debug.WriteLine($"成功读取原始.resx文件，共获取 {originalResourceDict.Count} 个待翻译资源项");

            // 3. 初始化翻译服务
            using var httpClient = new HttpClient();
            var translationService = new TranslationService(httpClient);

            // 4. 遍历目标语言，批量翻译并生成.resx文件
            foreach (var (cultureCode, baiduLangCode) in targetLangConfig)
            {
                // 构造目标.resx文件路径（符合.NET资源文件命名规范）
                string targetResxFileName = $"{originalResxName}.{cultureCode}.resx";
                string targetResxPath = System.IO.Path.Combine(targetResxDirectory, targetResxFileName);

                // 构建临时列表
                var tempTranslationItems = new List<TranslationService.TranslateItem>(translationItems);

                // 如果资源文件已经存在，则过滤已存在的Key,和翻译不为空
                if (File.Exists(targetResxPath))
                {
                    // 读取已存在的目标资源文件，过滤掉空值的项，并构建Key-Value字典
                    var targetResourceDict = ResxHandler.ReadFrom(targetResxPath)
                                                        .Where(t => !string.IsNullOrEmpty(t.Value))
                                                        .ToDictionary(t => t.Key, t => t.Value);
                    foreach (var targetResourceItem in targetResourceDict)
                    {
                        // 查找已存在的Key，并从待翻译列表中移除
                        var existTranslationItem = tempTranslationItems.FirstOrDefault(t => t.Key == targetResourceItem.Key);
                        if (existTranslationItem != null && !string.IsNullOrEmpty(targetResourceItem.Value))
                        {
                            tempTranslationItems.Remove(existTranslationItem);
                        }
                    }
                }

                if (tempTranslationItems.Count > 0)
                {
                    var translatedResults = await translationService.TranslatePostAsync(
                        sourceLang: "auto", // 假设原始资源为中文，可改为"auto"自动识别
                        targetLang: baiduLangCode,
                        items: tempTranslationItems.ToArray()
                    );

                    tempTranslationItems.Clear();
                    tempTranslationItems = null;

                    // 可选：添加延时，避免触发接口限流（百度翻译API有调用频率限制）
                    await Task.Delay(100);

                    // 5. 生成目标语言.resx文件
                    ResxHandler.WriteTo(targetResxPath, translatedResults.ToDictionary(t => t.Key, t => t.Dst));
                }

                // // 构造目标.resx文件路径（符合.NET资源文件命名规范）
                // string targetResxFileName = $"{originalResxName}.{cultureCode}.resx";
                // string targetResxPath = System.IO.Path.Combine(targetResxDirectory, targetResxFileName);
                //
                // 批量翻译资源值
                // var translatedResourceDict = new Dictionary<string, string>();
                // foreach (var (key, value) in originalResourceDict)
                // {
                //     var translatedValue = await translationService.TranslateTextAsync(
                //         sourceText: value,
                //         sourceLang: "zh", // 假设原始资源为中文，可改为"auto"自动识别
                //         targetLang: baiduLangCode
                //     );
                //
                //     translatedResourceDict.Add(key, translatedValue);
                //
                //     // 可选：添加延时，避免触发接口限流（百度翻译API有调用频率限制）
                //     await Task.Delay(100);
                // }
                //
                // // 5. 生成目标语言.resx文件
                // ResxHandler.WriteTo(targetResxPath, translatedResourceDict);
            }

            Debug.WriteLine("所有目标语言.resx文件生成完成！");
            System.Windows.MessageBox.Show("所有目标语言.resx文件生成完成！");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"流程执行失败：{ex.Message}");
            System.Windows.MessageBox.Show($"流程执行失败：{ex.Message}");
        }
    }
}