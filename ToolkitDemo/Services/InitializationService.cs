using BgCommon.Prism.Wpf.Services.Implementation;

namespace ToolkitDemo.Services;

public class InitializationService : InitializationServiceBase
{
    public InitializationService(IContainerExtension container)
        : base(container) { }

    protected override Task<bool> OnInitializeAsync()
    {
        return base.OnInitializeAsync();
    }

    protected override Task<bool> OnLoadAsync(int startProgress)
    {
        return base.OnLoadAsync(startProgress);
    }
}