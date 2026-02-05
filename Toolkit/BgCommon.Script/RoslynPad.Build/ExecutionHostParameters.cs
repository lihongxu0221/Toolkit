using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoslynPad.Build;

internal class ExecutionHostParameters
{
    public string BuildPath { get; }

    public string NuGetConfigPath { get; }

    public ImmutableArray<string> Imports { get; set; }

    public ImmutableHashSet<string> DisabledDiagnostics { get; }

    public string WorkingDirectory { get; set; }

    public SourceCodeKind SourceCodeKind { get; set; }

    public bool CheckOverflow { get; }

    public bool AllowUnsafe { get; }

    public ExecutionHostParameters(
        string buildPath,
        string nuGetConfigPath,
        ImmutableArray<string> imports,
        ImmutableHashSet<string> disabledDiagnostics,
        string workingDirectory,
        SourceCodeKind sourceCodeKind,
        bool checkOverflow = false,
        bool allowUnsafe = true)
    {
        BuildPath = buildPath;
        NuGetConfigPath = nuGetConfigPath;
        Imports = imports;
        DisabledDiagnostics = disabledDiagnostics;
        WorkingDirectory = workingDirectory;
        SourceCodeKind = sourceCodeKind;
        CheckOverflow = checkOverflow;
        AllowUnsafe = allowUnsafe;
    }
}
