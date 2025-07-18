// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

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

[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Authority")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Authority.Models")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Authority.ViewModels")]
[assembly: XmlnsDefinition("http://www.sz-baigu.com/", "BgCommon.Authority.Views")]
[assembly: XmlnsPrefix("http://www.sz-baigu.com/", "bg")]