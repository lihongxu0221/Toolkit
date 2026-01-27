using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(

    // where theme specific resource dictionaries are located
    // (used if a resource is not found in the page,
    // or application resource dictionaries)
    ResourceDictionaryLocation.None,

    // where the generic resource dictionary is located
    // (used if a resource is not found in the page,
    // app, or any theme specific resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: InternalsVisibleTo("BgControls")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Collections")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Collections.Generic")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Configuration")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Core")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Core.Models")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Text.Json.Converters")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Text.Json")]
[assembly: XmlnsPrefix("http://www.sz-baigu.com/", "bg")]