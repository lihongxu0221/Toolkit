using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace RoslynPad.Build;

public interface IResultObject
{
    string? Value { get; }

    void WriteTo(StringBuilder builder);
}

public interface IResultWithLineNumber
{
    int? LineNumber { get; }

    int Column { get; }
}

public partial class ResultObject : ObservableObject, IResultObject, IResultWithLineNumber
{
    [ObservableProperty]
    private string? header = string.Empty;

    [ObservableProperty]
    private string? value = string.Empty;

    [ObservableProperty]
    private int? lineNumber;

    [ObservableProperty]
    private int column = 0;

    [ObservableProperty]
    private bool isExpanded = false;

    [ObservableProperty]
    private string? type = string.Empty;

    [ObservableProperty]
    private List<ResultObject>? children = null;

    public bool HasChildren => Children?.Count > 0;

    public override string ToString()
    {
        var builder = new StringBuilder();
        BuildStringRecursive(builder, 0);
        return builder.ToString();
    }

    public void WriteTo(StringBuilder builder)
    {
        BuildStringRecursive(builder, 0);
    }

    private void BuildStringRecursive(StringBuilder builder, int level)
    {
        for (var i = 0; i < level; i++)
        {
            _ = builder.Append("  ");
        }

        _ = builder.Append(Header);
        if (Header != null && Value != null)
        {
            _ = builder.Append(" = ");
        }

        _ = builder.Append(Value);
        _ = builder.AppendLine();
        if (Children != null)
        {
            foreach (ResultObject child in Children)
            {
                child.BuildStringRecursive(builder, level + 1);
            }
        }
    }
}

public class ExceptionResultObject : ResultObject
{
    [JsonPropertyName("m")]
    public string? Message { get; set; }
}

public class InputReadRequest
{
}

public class ProgressResultObject
{
    [JsonPropertyName("p")]
    public double? Progress { get; set; }
}

public sealed partial class CompilationErrorResultObject : ObservableObject, IResultObject, IResultWithLineNumber
{
    [ObservableProperty]
    private string? errorCode = string.Empty;

    [ObservableProperty]
    private string? severity = string.Empty;

    [ObservableProperty]
    private int? lineNumber;

    [ObservableProperty]
    private int column;

    [ObservableProperty]
    private string? message = string.Empty;

    public static CompilationErrorResultObject Create(string severity, string errorCode, string message, int line, int column) => new()
    {
        ErrorCode = errorCode,
        Severity = severity,
        Message = message,

        // 0 to 1-based
        LineNumber = line + 1,
        Column = column + 1,
    };

    public override string ToString() => $"{ErrorCode}: {Message}";

    string? IResultObject.Value => this.ToString();

    public void WriteTo(StringBuilder builder) => builder.Append(ToString());
}

public partial class RestoreResultObject : ObservableObject, IResultObject
{
    private readonly string? _value;

    public string Value => _value ?? Message;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private string severity = string.Empty;

    public RestoreResultObject(string message, string severity, string? value = null)
    {
        Message = message;
        Severity = severity;
        _value = value;
    }

    public void WriteTo(StringBuilder builder) => builder.Append(Value);
}
