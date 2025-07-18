using BgCommon.Prism.Wpf.Authority.Services.Implementation;

namespace ToolkitDemo.Services;

internal class ModuleService : ModuleServiceBase
{
    public ModuleService(IContainerExtension container)
        : base(container)
    {
    }

    public override Task<bool> InitializeAsync()
    {
        return Task.FromResult(true);
    }
}