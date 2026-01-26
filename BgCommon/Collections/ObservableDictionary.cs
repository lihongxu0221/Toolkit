namespace BgCommon.Collections;

[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class ObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    where TKey : notnull
{
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";
    private const string KeysName = "Keys";
    private const string ValuesName = "Values";

    private readonly IDictionary<TKey, TValue> dictionary;

    [field: NonSerialized]
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    [field: NonSerialized]
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableDictionary()
    {
        dictionary = new Dictionary<TKey, TValue>();
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
    {
        this.dictionary = new Dictionary<TKey, TValue>(dictionary);
    }

    public ObservableDictionary(IEqualityComparer<TKey> comparer)
    {
        dictionary = new Dictionary<TKey, TValue>(comparer);
    }

    public ObservableDictionary(int capacity)
    {
        dictionary = new Dictionary<TKey, TValue>(capacity);
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
    {
        this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
    }

    public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
        dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
    }

    public void Add(TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            throw new ArgumentException("An item with the same key has already been added.");
        }

        dictionary.Add(key, value);
        OnCollectionReset();
    }

    public bool Remove(TKey key)
    {
        if (!dictionary.TryGetValue(key, out TValue? _))
        {
            return false;
        }

        OnCollectionReset();
        return true;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return default;
        }

        set
        {
            bool keyExists = dictionary.ContainsKey(key);
            if (keyExists && EqualityComparer<TValue>.Default.Equals(dictionary[key], value))
            {
                return;
            }

            dictionary[key] = value;
            if (keyExists)
            {
                OnPropertyChanged(IndexerName);
                OnPropertyChanged(ValuesName);
            }
            else
            {
                OnCollectionReset();
            }
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        if (dictionary.Count > 0)
        {
            dictionary.Clear();
            OnCollectionReset();
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item))
        {
            OnCollectionReset();
            return true;
        }

        return false;
    }

    private void OnCollectionReset()
    {
        OnPropertyChanged(CountString, KeysName, ValuesName, IndexerName);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnPropertyChanged(params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            OnPropertyChanged(propertyName);
        }
    }

    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

    public ICollection<TKey> Keys => dictionary.Keys;

    public ICollection<TValue> Values => dictionary.Values;

    public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => dictionary.CopyTo(array, arrayIndex);

    public int Count => dictionary.Count;

    public bool IsReadOnly => dictionary.IsReadOnly;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}