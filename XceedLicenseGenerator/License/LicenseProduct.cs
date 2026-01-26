using System.ComponentModel;

namespace XceedLicenseGenerator;

internal enum LicenseProduct
{
    [Description("Xceed Zip Compression Library")]
    XceedZip = 1,
    [Description("Xceed Zip Compression Library with SFX")]
    XceedZipSfx,
    [Description("Xceed Backup Library")]
    XceedBackup,
    [Description("Xceed Winsock Library")]
    XceedWinsock,
    [Description("Xceed FTP Library")]
    XceedFtp,
    [Description("Xceed Data Compression Library")]
    XceedCompress,
    [Description("Xceed Binary Encoding Library")]
    XceedEncode,
    [Description("Xceed Encryption Library")]
    XceedEncrypt,
    [Description("Xceed FTP Library For BizTalk Server")]
    XceedFtpBiz,
    [Description("Xceed Zip for .NET and .NET Standard")]
    XceedZipNET,
    [Description("Abale Zip")]
    AbaleZip,
    [Description("Xceed Grid for WinForms")]
    XceedGridNET,
    [Description("Xceed Streaming Compression for .NET and .NET Standard")]
    XceedCompressNET,
    [Description("Xceed Zip for .NET Compact Framework")]
    XceedZipNETCF,
    [Obsolete("This product was merged into Xceed Zip for .NET CF.")]
    [Description("Xceed Streaming Compression for .NET Compact Framework")]
    XceedCompressNETCF,
    [Description("Xceed SmartUI")]
    XceedSmartUI,
    [Description("Xceed SmartUI for WinForms")]
    XceedSmartUINET,
    [Description("Xceed FTP for .NET and .NET Standard")]
    XceedFtpNET,
    [Description("Xceed FTP for .NET Compact Framework")]
    XceedFtpNETCF,
    [Description("Xceed Chart for WinForms")]
    XceedChartNET,
    [Description("Xceed Docking Windows for WinForms")]
    XceedDockingWindowsNET,
    [Description("Xceed Chart for ASP.NET")]
    XceedChartASPNET,
    [Description("Xceed Input Validator for WinForms")]
    XceedInputValidatorNET,
    [Description("Xceed DeployReady")]
    XceedDeployReady,
    [Description("Xceed Editors for WinForms")]
    XceedEditorsNET,
    [Description("Xceed ZipNoAES for .NET and .NET Standard")]
    XceedZipNoAES,
    [Description("Xceed Tar for .NET and .NET Standard")]
    XceedTarNET,
    [Description("Xceed DataGrid for WPF (Free Edition)")]
    XceedDataGridFreeWPFNET,
    [Description("Xceed DataGrid Pro for WPF")]
    XceedDataGridProWPFNET,
    [Description("Xceed Workflow Activities for .NET")]
    XceedWorkflowActivitiesNET,
    [Description("Xceed Synchronization for .NET and .NET Standard")]
    XceedSynchronizationNET,
    [Description("Xceed Zip Compression Library (AMD64/EM64T port)")]
    XceedZipX64,
    [Description("Xceed Zip Compression Library (Itanium port)")]
    XceedZipIA64,
    [Description("Xceed Zip for .NET  and .NET Standard with Self-Extractor")]
    XceedZipNETSfx,
    [Description("Xceed Real-Time Zip for .NET and .NET Standard")]
    XceedRealTimeZipNET,
    [Description("Xceed Real-Time Zip for .NET Compact Framework")]
    XceedRealTimeZipNETCF,
    [Description("Xceed Upload for Silverlight")]
    XceedUploadSilverlight,
    [Description("Xceed 3D Views for WPF")]
    Xceed3DViewsWPFNET,
    [Description("Xceed Real-Time Zip for Silverlight")]
    XceedRealTimeZipSilverlight,
    [Description("Xceed Pro Themes for WPF")]
    XceedProThemesForWPF,
    [Description("Xceed Office 2007 Themes for WPF")]
    XceedOffice2007ThemesWPF,
    [Description("Xceed Glass Theme for WPF")]
    XceedGlassThemeWPF,
    [Description("Xceed Media Theme for WPF")]
    XceedMediaThemeWPF,
    [Description("Xceed Live Explorer Theme for WPF")]
    XceedLiveExplorerThemeWPF,
    [Description("Xceed Windows7 Theme for WPF")]
    XceedWindows7ThemeWPF,
    [Description("Xceed DataGrid for Silverlight")]
    XceedDataGridSilverlight,
    [Description("Xceed Ultimate ListBox for Silverlight")]
    XceedListBoxSilverlight,
    [Description("Xceed Fluent Assertions for .NET")]
    XceedFluentAssertionsNET,
    [Description("Xceed Upload for Windows Phone")]
    XceedUploadPhone,
    [Description("Xceed Ultimate ListBox for WPF")]
    XceedListBoxWPF,
    [Description("Xceed Blendables for WPF")]
    XceedBlendablesWPF,
    [Description("Xceed SFtp for .NET and .NET Standard")]
    XceedSFtpNET,
    [Description("Xceed Toolkit Plus for WPF")]
    XceedToolkitWPF,
    [Description("Xceed Real-Time Zip for Xamarin")]
    XceedRealTimeZipXamarin,
    [Description("Xceed Zip for Xamarin")]
    XceedZipXamarin,
    [Description("Xceed Ftp for Xamarin")]
    XceedFtpXamarin,
    [Description("Xceed SFtp for Xamarin")]
    XceedSFtpXamarin,
    [Description("Xceed Words for .NET and .NET 5+")]
    XceedWordsNET,
    [Description("Xceed PDF Creator for .NET, .NET Standard and .NET 5")]
    XceedPDFCreatorNET,
    [Description("Xceed DataGrid for JavaScript")]
    XceedDataGridJavaScript,
    [Description("Xceed Workbooks for .NET and .NET 5+")]
    XceedWorkbooksNET,
    [Description("Xceed Mail for .NET and .NET 5+")]
    XceedMailNET,
    [Description("Xceed Toolkit for .NET MAUI")]
    XceedToolkitMAUI,
    FirstProduct = 1,
    LastProduct = 63,
    InvalidProduct = 0
}