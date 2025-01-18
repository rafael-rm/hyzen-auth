namespace Legacy.DTOs.Request.Auth;

public class RecoverPasswordRequest
{
    public string Email { get; set; }
    public string VerificationCode { get; set; }
    public string NewPassword { get; set; }
}