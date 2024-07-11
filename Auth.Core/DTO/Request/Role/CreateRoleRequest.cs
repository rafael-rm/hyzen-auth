﻿using System.ComponentModel.DataAnnotations;

namespace Auth.Core.DTO.Request.Role;

public class CreateRoleRequest
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }
}