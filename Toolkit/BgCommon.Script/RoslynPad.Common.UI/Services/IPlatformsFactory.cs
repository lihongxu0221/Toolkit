using RoslynPad.Build;

namespace RoslynPad.UI;

public interface IPlatformsFactory
{
    IReadOnlyList<ExecutionPlatform> GetExecutionPlatforms();

    string DotNetExecutable { get; }
}