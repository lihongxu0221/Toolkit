using BgCommon.Helpers;
using System.Runtime.Serialization;

namespace BgCommon.Script.Models;

/// <summary>
/// 脚本文件实体类.
/// </summary>
[Serializable]
public partial class ScriptFile : ObservableObject
{
    private const string EncryptionPrefix = "baigu";

    private Guid id = Guid.NewGuid();
    private string sourceCode = string.Empty;
    private string password = string.Empty;
    private string referencedAssemblies = string.Empty;
    private string namespaces = string.Empty;
    private string targetType = string.Empty;
    private string targetMethod = string.Empty;
    private bool isEnable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptFile"/> class.
    /// </summary>
    public ScriptFile()
    {
    }

    /// <summary>
    /// Gets or sets 唯一标识符.
    /// </summary>
    public Guid Id
    {
        get => this.id;
        set => this.SetProperty(ref this.id, value);
    }

    /// <summary>
    /// Gets or sets 脚本源码内容.
    /// </summary>
    public string SourceCode
    {
        get => this.sourceCode;
        set => this.SetProperty(ref this.sourceCode, value);
    }

    /// <summary>
    /// Gets or sets 脚本加密密码.
    /// </summary>
    public string Password
    {
        get => this.password;
        set => this.SetProperty(ref this.password, value);
    }

    /// <summary>
    /// Gets or sets 引用程序集列表.
    /// </summary>
    public List<string> ReferencedAssemblies
    {
        get
        {
            // 将分号分隔的字符串转换为列表
            List<string> assemblyList = this.referencedAssemblies.Split(';').ToList();
            for (int i = 0; i < assemblyList.Count; i++)
            {
                // 去除首尾空格
                assemblyList[i] = assemblyList[i].Trim();
            }

            // 移除空字符串项
            assemblyList.Remove(string.Empty);
            return assemblyList;
        }

        set
        {
            var assemblyBuilder = new StringBuilder();
            foreach (string item in value)
            {
                string trimmedAssembly = item.Trim();
                if (!string.IsNullOrEmpty(trimmedAssembly))
                {
                    assemblyBuilder.Append(trimmedAssembly);
                    assemblyBuilder.Append(';');
                }
            }

            // 如果有内容，移除最后一个分号
            if (value.Count > 0 && assemblyBuilder.Length > 0)
            {
                assemblyBuilder.Length--;
            }

            string joinedAssemblies = assemblyBuilder.ToString();
            if (this.referencedAssemblies != joinedAssemblies)
            {
                this.referencedAssemblies = joinedAssemblies;
                this.OnPropertyChanged(nameof(this.ReferencedAssemblies));
            }
        }
    }

    /// <summary>
    /// Gets or sets 导入的命名空间列表.
    /// </summary>
    public List<string> Namespaces
    {
        get
        {
            // 将内部存储的字符串拆分为列表
            List<string> namespaceList = this.namespaces.Split(';').ToList();
            for (int i = 0; i < namespaceList.Count; i++)
            {
                namespaceList[i] = namespaceList[i].Trim();
            }

            namespaceList.Remove(string.Empty);
            return namespaceList;
        }

        set
        {
            var namespaceBuilder = new StringBuilder();
            foreach (string item in value)
            {
                string trimmedNamespace = item.Trim();
                if (!string.IsNullOrEmpty(trimmedNamespace))
                {
                    namespaceBuilder.Append(trimmedNamespace);
                    namespaceBuilder.Append(';');
                }
            }

            // 移除末尾的分号分隔符
            if (value.Count > 0 && namespaceBuilder.Length > 0)
            {
                namespaceBuilder.Length--;
            }

            string joinedNamespaces = namespaceBuilder.ToString();
            if (this.namespaces != joinedNamespaces)
            {
                this.namespaces = joinedNamespaces;
                this.OnPropertyChanged(nameof(this.Namespaces));
            }
        }
    }

    /// <summary>
    /// Gets or sets 与此实例关联的目标对象的类型名称.
    /// </summary>
    public string TargetType
    {
        get => this.targetType;
        set => this.SetProperty(ref this.targetType, value);
    }

    /// <summary>
    /// Gets or sets 操作目标的方法名称.
    /// </summary>
    public string TargetMethod
    {
        get => this.targetMethod;
        set => this.SetProperty(ref this.targetMethod, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 脚本是否启用.
    /// </summary>
    public bool IsEnable
    {
        get => this.isEnable;
        set => this.SetProperty(ref this.isEnable, value);
    }

    /// <summary>
    /// 序列化前执行：对 SourceCode 进行加密处理.
    /// </summary>
    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        if (string.IsNullOrEmpty(SourceCode) || string.IsNullOrEmpty(Password))
        {
            return; // 无内容或无密码，无需加密
        }

        try
        {
            // 1. SourceCode 和 Password 按位与操作
            string sourceCodeString = SourceCode.Parse(Password);

            // 2. 拼接 Password + "\r\n" + SourceCodeString
            string contentToEncrypt = $"{Password}\r\n{sourceCodeString}";

            // 3. 加密
            string encryptedContent = contentToEncrypt.Encrypt(EncryptionPrefix);

            // 4. 替换 SourceCode 为加密后的值（序列化时保存这个加密值）
            this.sourceCode = encryptedContent;
        }
        catch (Exception ex)
        {
            throw new SerializationException("序列化时加密 SourceCode 失败", ex);
        }
    }

    /// <summary>
    /// 反序列化后执行：还原 SourceCode 的原始值.
    /// </summary>
    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (string.IsNullOrEmpty(sourceCode) || string.IsNullOrEmpty(Password))
        {
            return; // 无加密内容或无密码，无需解密
        }

        try
        {
            // 1. 解密
            string decryptedContent = sourceCode.Decrypt(EncryptionPrefix);

            // 2. 拆分 Password 和 SourceCodeString（按 \r\n 分割）
            string[] parts = decryptedContent.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (parts.Length != 2 || parts[0] != Password)
            {
                throw new InvalidDataException("解密后密码不匹配，无法还原 SourceCode");
            }

            string sourceCodeString = parts[1];

            // 3. 按位与的反向操作还原原始 SourceCode
            string originalSourceCode = sourceCodeString.Parse(Password);

            // 4. 写回原始 SourceCode
            this.sourceCode = originalSourceCode;
        }
        catch (Exception ex)
        {
            throw new SerializationException("反序列化时解密 SourceCode 失败", ex);
        }
    }
}