namespace Legacy.DTOs.Enum;

public enum EventType
{
    // Login e Logout
    LoginSuccess = 1,
    LoginFailed = 2,
    LoginAttemptLimitReached = 3,
    LoginAttemptAfterLockout = 4,
    
    // Eventos de criação e modificação de conta
    AccountCreated = 5,
    AccountPasswordChanged = 6,

    // Recuperação de conta
    PasswordRecoveryRequested = 7,
    PasswordRecovered = 8,
    PasswordRecoveryFailed = 9,
    PasswordRecoveryAttemptAfterLockout = 10,
    PasswordRecoveryRequestLimitReached = 11,
}