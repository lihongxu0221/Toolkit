using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script.CodeDom")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script.Converters")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script.Converters.Formatting")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script.Extras")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script.Services")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script.ViewModels")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Script.Views")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "RoslynPad")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "RoslynPad.Build")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "RoslynPad.UI")]
[assembly: XmlnsPrefix("http://www.sz-baigu.com/", "bg")]
