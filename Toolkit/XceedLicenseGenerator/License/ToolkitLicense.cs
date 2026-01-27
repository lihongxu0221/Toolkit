namespace XceedLicenseGenerator;

internal class ToolkitLicense : XceedLicense
{
    public static XceedLicense CreateLicense(string licenseKey, Type licenserType)
    {
        var xceedLicense = new ToolkitLicense();
        xceedLicense.Initialize(licenseKey, licenserType);
        if (xceedLicense.LicenseKey == null || xceedLicense.LicenseKey.Length == 0)
        {
            return null;
        }
        return xceedLicense;
    }

    private ToolkitLicense()
    {
    }

    protected override void ThrowNoKeyInCodeAndRegistry()
    {
        ThrowException.ThrowLicenseException(this.LicenseeType, null, this.LicenseeType.FullName + ".LicenseKey property must be set to a valid license key in the code of your application before using this product. Please refer to the Licensing topic in the documentation for more information on this exception.");
    }

    protected override void ThrowKeyInvalid()
    {
        ThrowException.ThrowLicenseException(this.LicenseeType, null, "The license key used to license this Xceed product is invalid. " + this.LicenseeType.FullName + ".LicenseKey must be set to a valid license key before using this product. Please refer to the Licensing topic in the documentation for specific instructions on how to avoid this exception.");
    }

    protected override void ThrowKeyInvalidProductVersion()
    {
        ThrowException.ThrowLicenseException(this.LicenseeType, null, "License key version too old, it does not qualify to unlock this version of Xceed Toolkit Plus for WPF. Please upgrade your license to benefit from the latest bug fixes and/or features.");
    }

    protected override void ThrowTrialKeyExpired()
    {
        ThrowException.ThrowLicenseException(this.LicenseeType, null, "You did not unlock this control with a license key, therefore you have been using Xceed Toolkit Plus for WPF in trial mode which has expired. You must now set the " + this.LicenseeType.FullName + ".LicenseKey property with a valid license key. Please refer to the Licensing topic in the documentation for more information on this exception.");
    }

    protected override XceedLicense.ProductVersion[] AllowedVersions
    {
        get
        {
            XceedLicense.VersionNumber versionNumber = new XceedLicense.VersionNumber("5.1");
            return new XceedLicense.ProductVersion[]
            {
                    new XceedLicense.ProductVersion(LicenseProduct.XceedToolkitWPF, versionNumber, versionNumber)
            };
        }
    }

    protected override Type LicenseeType
    {
        get
        {
            return typeof(Licenser);
        }
    }
}