﻿namespace Auth.Application.DTOs.Response;

public class GroupResponse
{
    public Guid Guid { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> Roles { get; set; } = [];
}