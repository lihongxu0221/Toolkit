namespace BgCommon.Script.Models;

[Serializable]
public partial class OutputParam : ObservableObject
{
    private string name = string.Empty;
    private Type valueType = typeof(string);
    private object? value;

    public string Name
    {
        get => this.name;
        set => SetProperty(ref this.name, value);
    }

    public Type ValueType
    {
        get => this.valueType;
        set => SetProperty(ref this.valueType, value);
    }

    public object? Value
    {
        get => this.value;
        set => SetProperty(ref this.value, value);
    }
}