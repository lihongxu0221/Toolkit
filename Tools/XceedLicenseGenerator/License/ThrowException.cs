namespace XceedLicenseGenerator;

internal class ThrowException
{
    private ThrowException()
    {
    }

    public static void ThrowArgumentException(string message, string paramName, Exception innerExcept)
    {
        throw new ArgumentException(message, paramName, innerExcept);
    }

    public static void ThrowArgumentOutOfRangeException(string paramName, object value, string message)
    {
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    public static void ThrowLicenseException(Type? type, object? instance, string message)
    {
        throw new Exception(message);
    }
}