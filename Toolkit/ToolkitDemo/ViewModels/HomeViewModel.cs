using BgCommon.Localization;
using BgCommon.Prism.Wpf.MVVM;
using ToolkitDemo.Models;

namespace ToolkitDemo.ViewModels;

/// <summary>
/// 首页ViewModel类.
/// </summary>
public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty]
    private AllEditorTypes selectedObject = new AllEditorTypes();

    [ObservableProperty]
    private ObservableCollection<Person> persons = new ObservableCollection<Person>();

    public HomeViewModel(IContainerExtension container)
        : base(container)
    {
        this.SelectedObject = AllEditorTypes.Instance;
        this.Persons.Add(new Person() { Name = "张三" });
        this.Persons.Add(new Person() { Name = "李四" });
        this.Persons.Add(new Person() { Name = "王五" });
    }

    /// <summary>
    /// 启动.
    /// </summary>
    [RelayCommand]
    private void OnStart()
    {
        LocalizationProxy.SetLocalizationCulture(new CultureInfo("en-US"));
    }

    /// <summary>
    /// 停止.
    /// </summary>
    [RelayCommand]
    private void OnStop()
    {
        LocalizationProxy.SetLocalizationCulture(new CultureInfo("zh-CN"));
    }

    [RelayCommand]
    private void OnPickUp()
    {
        Debug.WriteLine("PickUp command executed.");
    }

    [RelayCommand]
    private void OnLocate()
    {
        Debug.WriteLine("Locate command executed.");
    }
}