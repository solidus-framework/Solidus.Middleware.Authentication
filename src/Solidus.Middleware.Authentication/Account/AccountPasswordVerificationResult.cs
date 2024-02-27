namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// Specifies the results for password verification.
/// </summary>
public enum AccountPasswordVerificationResult
{
    /// <summary>
    /// Indicates password verification failed.
    /// </summary>
    Failed = 0,

    /// <summary>
    /// Indicates password verification was successful.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Indicates password verification was successful however its using legacy hashing algorithm and should be rehashed.
    /// </summary>
    SuccessRehashNeeded = 2,
}
