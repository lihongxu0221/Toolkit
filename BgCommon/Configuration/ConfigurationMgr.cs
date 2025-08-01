namespace BgCommon.Configuration;

/// <summary>
/// 配置类，用于保存特定实体类型的设置。
/// </summary>
/// <typeparam name="TEntity">实体类型。</typeparam>
[Serializable]
public class ConfigurationMgr<TEntity> : ObservableObject
    where TEntity : class, new()
{
    /// <summary>
    /// 序列化方法枚举。
    /// </summary>
    public enum SerializeMethod
    {
        /// <summary>
        /// 使用二进制进行序列化。
        /// </summary>
        Bin,

        /// <summary>
        /// 使用XML进行序列化。
        /// </summary>
        Xml,

        /// <summary>
        /// 使用 System.Text.Json 进行序列化。
        /// </summary>
        Json
    }

    // 私有字段。
    [NonSerialized]
    private bool hasLoadAll = false;

    [NonSerialized]
    private string filePath = string.Empty;

    private TEntity? entity = new TEntity();

    [NonSerialized]
    private SerializeMethod method = SerializeMethod.Bin;

    /// <summary>
    /// Gets or sets 获取或设置配置的实体实例。
    /// </summary>
    [Browsable(true)]
    public TEntity? Entity
    {
        get { return entity; }
        set { _ = SetProperty(ref entity, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether 获取或设置一个值，该值指示是否已加载所有配置。
    /// </summary>
    [Browsable(false)]
    public bool HasLoadAll
    {
        get { return hasLoadAll; }
        set { _ = SetProperty(ref hasLoadAll, value); }
    }

    /// <summary>
    /// Gets 获取配置的序列化方法。
    /// </summary>
    [Browsable(false)]
    public SerializeMethod Method
    {
        get { return method; }
        private set { _ = SetProperty(ref method, value); }
    }

    /// <summary>
    /// Gets 获取配置文件的完整路径。
    /// </summary>
    [Browsable(false)]
    public string FilePath
    {
        get { return filePath; }
    }

    /// <summary>
    /// Gets 获取配置文件的根目录路径。
    /// </summary>
    [Browsable(false)]
    public string? RootDirPath
    {
        get { return Path.GetDirectoryName(filePath); }
    }

    /// <summary>
    /// Gets 获取配置文件的文件名。
    /// </summary>
    [Browsable(false)]
    public string FileName
    {
        get { return Path.GetFileName(filePath); }
    }

    /// <summary>
    /// Gets a value indicating whether 该值指示配置是否为空（即实体为null）。
    /// </summary>
    public bool IsEmpty => Entity == null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMgr{TEntity}"/> class.
    /// 使用此构造函数时，需要后续调用方法 <code>SaveToFile(entity, filePath)</code> 来设置并保存配置。
    /// </summary>
    /// <param name="method">序列化方法。</param>
    public ConfigurationMgr(SerializeMethod method = SerializeMethod.Bin)
    {
        this.Method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMgr{TEntity}"/> class.
    /// </summary>
    /// <param name="filePath">配置文件的路径。</param>
    /// <param name="method">序列化方法。</param>
    public ConfigurationMgr(string filePath, SerializeMethod method = SerializeMethod.Bin)
        : this(filePath, null, method)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationMgr{TEntity}"/> class.
    /// </summary>
    /// <param name="filePath">配置文件的路径。</param>
    /// <param name="entity">实体实例。</param>
    /// <param name="method">序列化方法。</param>
    public ConfigurationMgr(string filePath, TEntity? entity, SerializeMethod method = SerializeMethod.Bin)
    {
        this.Method = method;
        this.Entity = entity;
        this.SetFilePath(filePath);
    }

    /// <summary>
    /// 根据指定的序列化方法从文件加载配置。如果文件不存在，则会创建一个新的实体实例。
    /// </summary>
    /// <exception cref="NotSupportedException">当序列化方法不受支持时抛出。</exception>
    public void LoadFromFile()
    {
        if (!File.Exists(filePath))
        {
            Entity = new TEntity();
            return; // 文件不存在，直接返回新创建的实例。
        }

        // 根据序列化方法实现加载逻辑。
        switch (Method)
        {
            case SerializeMethod.Bin: // 加载二进制序列化。
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Entity = (TEntity?)formatter.Deserialize(fileStream);
                }

                break;
            case SerializeMethod.Xml: // 加载XML序列化。
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TEntity));
                    Entity = (TEntity?)serializer.Deserialize(fileStream);
                }

                break;

            case SerializeMethod.Json: // 加载JSON序列化。
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string json = reader.ReadToEnd();
                        Entity = JsonSerializer.Deserialize<TEntity>(json);
                    }
                }

                break;

            default:
                throw new NotSupportedException($"序列化方法 {Method} 不受支持。");
        }
    }

    /// <summary>
    /// 从文件异步加载配置。如果文件不存在，则会创建一个新的实体实例。
    /// </summary>
    /// <param name="cancellationToken">用于取消操作的令牌。</param>
    public async Task LoadFromFileAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            Entity = new TEntity();
            return;
        }

        switch (Method)
        {
            case SerializeMethod.Bin: // 加载二进制序列化。
                // BinaryFormatter 没有异步API，但我们可以在后台线程中执行以避免阻塞UI。
                // 为了简单起见，这里仍使用同步方式，实际应用可考虑 Task.Run。
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Entity = (TEntity?)formatter.Deserialize(fileStream);
                }

                break;
            case SerializeMethod.Xml: // XmlSerializer 没有内置的异步反序列化，但我们可以异步读取文件。
                string xmlContent = await File.ReadAllTextAsync(filePath, cancellationToken);
                using (var reader = new StringReader(xmlContent))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TEntity));
                    Entity = (TEntity?)serializer.Deserialize(reader);
                }

                break;
            case SerializeMethod.Json: // 使用 System.Text.Json 的异步API。
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Entity = await JsonSerializer.DeserializeAsync<TEntity>(fileStream, cancellationToken: cancellationToken);
                }

                break;
            default:
                throw new NotSupportedException($"序列化方法 {Method} 不受支持。");
        }
    }

    /// <summary>
    /// 根据指定的序列化方法将当前配置保存到文件。
    /// </summary>
    public void SaveToFile()
    {
        // 调用静态方法来执行实际的保存操作。
        Save(Entity, FilePath, Method);
    }

    /// <summary>
    /// 根据指定的序列化方法将当前配置异步保存到文件。
    /// </summary>
    /// <param name="cancellationToken">用于取消操作的令牌。</param>
    public Task SaveToFileAsync(CancellationToken cancellationToken = default)
    {
        return SaveAsync(Entity, FilePath, Method, cancellationToken);
    }

    /// <summary>
    /// 将指定的实体保存到文件，并更新当前实例的实体和文件路径。
    /// </summary>
    /// <param name="entity">要保存的实体实例。</param>
    /// <param name="filePath">文件的完整路径，如果为null则使用实例当前的路径。</param>
    /// <exception cref="ArgumentNullException">当实体或文件路径为空时抛出。</exception>
    /// <exception cref="NotSupportedException">当序列化方法不受支持时抛出。</exception>
    public void SaveToFile(TEntity? entity, string? filePath = null)
    {
        this.entity = entity;
        if (!string.IsNullOrEmpty(filePath) && FilePath != filePath)
        {
            SetFilePath(filePath);
        }

        if (Entity == null)
        {
            throw new ArgumentNullException("要保存的实体(Entity)不能为空。");
        }

        if (string.IsNullOrEmpty(FilePath))
        {
            throw new ArgumentNullException(nameof(FilePath), "文件路径不能为空或空字符串。");
        }

        // 根据序列化方法实现保存逻辑。
        switch (Method)
        {
            case SerializeMethod.Bin:
            {
                using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, Entity);
                }

                break;
            }

            case SerializeMethod.Xml:
            {
                // 保存XML序列化。
                using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TEntity));
                    // 将实体序列化到文件流。
                    serializer.Serialize(fileStream, Entity);
                }

                break;
            }

            case SerializeMethod.Json:
            {
                // 保存JSON序列化。
                using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    string json = JsonSerializer.Serialize(Entity, new JsonSerializerOptions { WriteIndented = true });

                    // 将JSON字符串写入文件。
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, leaveOpen: true))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }
                }

                break;
            }

            default:
            {
                throw new NotSupportedException($"序列化方法 {Method} 不受支持。");
            }
        }
    }

    /// <summary>
    /// 静态工具方法，用于将实体保存到指定的文件路径。
    /// </summary>
    /// <param name="entity">要保存的实体。</param>
    /// <param name="filePath">文件路径。</param>
    /// <param name="method">序列化方法。</param>
    public static void Save(TEntity? entity, string filePath, SerializeMethod method)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        // 确保目录存在。
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        switch (method)
        {
            case SerializeMethod.Bin:
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, entity);
                }

                break;
            case SerializeMethod.Xml:
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TEntity));
                    serializer.Serialize(fileStream, entity);
                }

                break;
            case SerializeMethod.Json:
                var options = new JsonSerializerOptions { WriteIndented = true }; // 美化输出，方便阅读。
                string json = JsonSerializer.Serialize(entity, options);
                File.WriteAllText(filePath, json);
                break;
            default:
                throw new NotSupportedException($"序列化方法 {method} 不受支持。");
        }
    }

    /// <summary>
    /// 静态工具方法，用于将实体异步保存到指定的文件路径。
    /// </summary>
    /// <param name="entity">要保存的实体。</param>
    /// <param name="filePath">文件路径。</param>
    /// <param name="method">序列化方法。</param>
    /// <param name="cancellationToken">用于取消操作的令牌。</param>
    public static async Task SaveAsync(TEntity? entity, string filePath, SerializeMethod method, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        // 确保目录存在。
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        switch (method)
        {
            case SerializeMethod.Bin:
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, entity);
                }

                break;
            case SerializeMethod.Xml:
                // XmlSerializer 没有内置的异步序列化，但我们可以异步写入文件。
                using (var memoryStream = new MemoryStream())
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TEntity));
                    serializer.Serialize(memoryStream, entity);
                    memoryStream.Position = 0;
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await memoryStream.CopyToAsync(fileStream, cancellationToken);
                    }
                }

                break;
            case SerializeMethod.Json:
                var options = new JsonSerializerOptions { WriteIndented = true };
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    await JsonSerializer.SerializeAsync(fileStream, entity, options, cancellationToken);
                }

                break;
            default:
                throw new NotSupportedException($"序列化方法 {method} 不受支持。");
        }
    }

    /// <summary>
    /// 设置配置文件的完整路径，并确保路径和目录有效。
    /// </summary>
    /// <param name="filePath">配置文件所在的完整路径。</param>
    private void SetFilePath(string filePath)
    {
        // 校验文件路径是否为空。
        ArgumentNullException.ThrowIfNullOrEmpty(filePath);
        string? directory = Path.GetDirectoryName(filePath);
        string? fileName = Path.GetFileName(filePath);

        // 文件名不能为空。
        ArgumentNullException.ThrowIfNullOrEmpty(fileName, nameof(filePath));

        if (string.IsNullOrEmpty(directory))
        {
            // 如果未指定目录，则默认为当前应用程序的基目录。
            directory = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(directory, fileName);
        }

        if (!Path.IsPathRooted(directory))
        {
            // 如果是相对路径，则转换为基于应用程序基目录的绝对路径。
            directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
            filePath = Path.Combine(directory, fileName);
        }

        // 确保目录存在。
        if (!Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        this.filePath = filePath;
    }
}