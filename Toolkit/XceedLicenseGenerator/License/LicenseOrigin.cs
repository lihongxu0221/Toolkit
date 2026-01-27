namespace XceedLicenseGenerator;

internal enum LicenseOrigin
{
    TrialVersion,
    TrialRenew1,
    TrialRenew2,
    TrialRenew3,
    TrialRenew4,
    TrialRenew5,
    TrialRenew6,
    TrialRenew7,
    TrialRenew8,
    TrialRenew9,
    FirstTrialValue = 0,
    LastTrialValue = 9,
    CommanderReseller,
    CommanderDirect,
    WebSite,
    LockSmith,
    Columbo,
    WebSite41,
    CRM,
    FirstOrigin = 0,
    LastOrigin = 16,
    InvalidOrigin = 63
}