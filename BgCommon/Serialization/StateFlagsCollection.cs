namespace BgCommon.Serialization;

/// <summary>
/// 状态标志位集合，实现ICollection和IEnumerable接口
/// </summary>
public class StateFlagsCollection : ICollection, IEnumerable
{
    private Hashtable _sf;

    /// <summary>
    /// 通过状态名称索引获取对应的标志位值
    /// </summary>
    /// <param name="statename">要查询的状态名称（自动忽略"Sf"前缀）</param>
    /// <returns>
    /// 匹配的标志位值，未找到返回0
    /// </returns>
    public long this[string statename]
    {
        get
        {
            if (!string.IsNullOrEmpty(statename))
            {
                // 自动处理"Sf"前缀的字段名
                var key = statename.StartsWith("Sf") ? statename : "Sf" + statename;
                return _sf[key] as long? ?? 0L;
            }
            return 0L;
        }
    }

    /// <summary>
    /// 获取所有状态名称列表（自动去除"Sf"前缀）
    /// </summary>
    public string[] Names
    {
        get
        {
            var names = new string[_sf.Count];
            var index = 0;
            foreach (string key in _sf.Keys)
            {
                names[index++] = key;
            }
            return names;
        }
    }

    /// <summary>
    /// 获取所有标志位值的数组
    /// </summary>
    public long[] Flags
    {
        get
        {
            var flags = new long[_sf.Count];
            var index = 0;
            foreach (long value in _sf.Values)
            {
                flags[index++] = value;
            }
            return flags;
        }
    }

    // 以下实现ICollection接口成员
    public bool IsSynchronized => _sf.IsSynchronized;
    public int Count => _sf.Count;
    public object SyncRoot => _sf.SyncRoot;

    /// <summary>
    /// 根据类型反射构建状态标志集合
    /// </summary>
    /// <param name="objType">
    /// 需要提取状态标志的目标类型，自动扫描以"Sf"开头的静态long类型字段
    /// </param>
    public StateFlagsCollection(Type objType)
    {
        // 优化哈希表初始容量
        _sf = new Hashtable(23);

        // 反射获取所有符合条件的静态字段
        var fields = objType.GetFields(
            BindingFlags.Static |
            BindingFlags.Public | 
            BindingFlags.FlattenHierarchy);

        foreach (var field in fields)
        {
            if (field.Name.StartsWith("Sf") &&
                field.FieldType == typeof(long))
            {
                // 去除字段名前缀"Sf"作为键名
                var key = field.Name.Substring(2);
                _sf[key] = (long)field.GetValue(null);
            }
        }
    }

    /// <summary>
    /// 将元素复制到指定数组
    /// </summary>
    public void CopyTo(Array array, int index)
    {
        _sf.CopyTo(array, index);
    }

    /// <summary>
    /// 获取集合的枚举器
    /// </summary>
    public IEnumerator GetEnumerator()
    {
        return _sf.GetEnumerator();
    }
}