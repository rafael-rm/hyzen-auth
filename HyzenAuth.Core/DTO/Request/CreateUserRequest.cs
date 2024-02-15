﻿namespace HyzenAuth.Core.DTO.Request;

public record CreateUserRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}