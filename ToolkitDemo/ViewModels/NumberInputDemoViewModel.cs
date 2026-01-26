using BgCommon.Prism.Wpf.MVVM;
using ToolkitDemo.Models;

namespace ToolkitDemo.ViewModels;

public partial class NumberInputDemoViewModel : ObservableObject
{
    //public NumberInputDemoViewModel(IContainerExtension container)
    //    : base(container)
    //{
    //}

    [ObservableProperty]
    private ObservableCollection<Person> persons = new ObservableCollection<Person>();

    public NumberInputDemoViewModel()
    {
        this.Persons.Add(new Person() { Name = "张三" });
        this.Persons.Add(new Person() { Name = "李四" });
        this.Persons.Add(new Person() { Name = "王五" });
    }

    public AllEditorTypes Instance => AllEditorTypes.Instance;

    [ObservableProperty]
    private double sampleValue = 66;

    [RelayCommand]
    private void OnValueChanged(object parameter)
    {
        Debug.WriteLine("OnValueChanged");
    }

    [RelayCommand]
    private void OnTextChanged(object parameter)
    {
        Debug.WriteLine("OnTextChanged");
    }

    [RelayCommand]
    private void OnPickUp()
    {
    }

    [RelayCommand]
    private void OnLocate()
    {
    }
}