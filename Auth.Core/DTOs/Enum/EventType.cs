namespace Auth.Core.DTOs.Enum;

public enum EventType
{
    // Eventos de criação e modificação de conta
    AccountCreated = 1,
    AccountUpdated = 2,
    AccountDeleted = 3,
    AccountActivated = 4,
    AccountDeactivated = 5,
    AccountPasswordChanged = 6,

    // Recuperação de conta
    PasswordRecoveryRequested = 7,
    PasswordRecovered = 8,
    PasswordRecoveryFailed = 9,

    // Login e Logout
    LoginSuccess = 10,
    LoginFailed = 11,
    Logout = 12,

    // Tentativas de segurança
    TwoFactorAuthenticationEnabled = 13,
    TwoFactorAuthenticationDisabled = 14,
    AccountLocked = 15,
    AccountUnlocked = 16,
    LoginBlockedDueToSuspiciousActivity = 17,
    LoginAttemptAfterLockout = 18,
    PasswordRecoveryAttemptAfterLockout = 19,

    // Emails de notificação
    EmailVerificationSent = 20,
    EmailVerified = 21,
    EmailVerificationFailed = 22,
    PasswordRecoveryRequestLimitReached = 23,
    AccountConfirmationResent = 24,

    // Outros eventos
    AccountProfileUpdated = 25,
    AccountTermsAccepted = 26,
    AccountPrivacySettingsUpdated = 27
}