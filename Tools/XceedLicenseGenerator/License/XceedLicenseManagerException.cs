namespace XceedLicenseGenerator;

internal class XceedLicenseManagerException : Exception
{
    public XceedLicenseManagerException()
        : base("Error reading the license key")
    {
    }

    public XceedLicenseManagerException(string message)
        : base(message)
    {
    }

    public XceedLicenseManagerException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public XceedLicenseManagerException(LicenseErrorCode errorCode)
        : base(errorCode.ToString())
    {
    }
}