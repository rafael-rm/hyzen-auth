namespace Auth.Core.DTOs.Request.User;

public class SearchUserRequest
{
    public List<Guid> Ids { get; set; }
    public List<string> Emails { get; set; }
    public bool IncludeInactive { get; set; }
}