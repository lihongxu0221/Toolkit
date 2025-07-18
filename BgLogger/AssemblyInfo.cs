using System.Windows;
using System.Windows.Markup;

[assembly:ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page,
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
    //(used if a resource is not found in the page,
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgLogger")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgLogger.Logging")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgLogger.Logging.Models")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgLogger.Logging.Services")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgLogger.Logging.Views")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgLogger.Logging.ViewModels")]
[assembly: XmlnsPrefix("http://www.sz-baigu.com/", "bg")]