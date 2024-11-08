﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using Auth.Core.DTOs.Enum;
using Auth.Core.Infrastructure;
using Hyzen.SDK.Exception;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Models;

[Table("verification_code")]
public class VerificationCode
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")]
    public int Id { get; set; }

    [Column("code", TypeName = "VARCHAR(12)"), MaxLength(12), Required]
    public string Code { get; set; }
    
    [Column("type", TypeName = "INT"), Required]
    public VerificationCodeType Type { get; set; }

    [Column("created_at", TypeName = "TIMESTAMP"), DatabaseGenerated(DatabaseGeneratedOption.Computed), Required]
    public DateTime CreatedAt { get; set; }

    [Column("expires_at", TypeName = "TIMESTAMP"), Required]
    public DateTime ExpiresAt { get; set; }

    [Column("used_at", TypeName = "TIMESTAMP")]
    public DateTime? UsedAt { get; set; }
    
    [ForeignKey("User"), Column("user_id", TypeName = "INT"), Required] public int UserId { get; set; }
    public User User { get; set; }
    
    private VerificationCode(string code, VerificationCodeType type, DateTime expiresAt, int userId)
    {
        Code = code;
        Type = type;
        ExpiresAt = expiresAt;
        UserId = userId;
    }
    
    public static async Task<VerificationCode> GetAsync(int userId, string code, VerificationCodeType type)
    {
        return await AuthContext.Get().VerificationCodesSet
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Code == code && s.Type == type);
    }
    
    public void Ensure(User user, VerificationCodeType expectedType)
    {
        if (expectedType != Type)
            throw new HException("This code is not valid for this operation.", ExceptionType.InvalidOperation);
        
        if (UsedAt.HasValue)
            throw new HException("This code has already been used.", ExceptionType.InvalidOperation);

        if (DateTime.Now > ExpiresAt)
            throw new HException("This code has expired.", ExceptionType.InvalidOperation);

        if (UserId != user.Id)
            throw new HException("This code is not valid for this user.", ExceptionType.InvalidOperation);
    }
    
    public void UseAsync()
    {
        UsedAt = DateTime.Now;
    }
    
    public static async Task<VerificationCode> CreateAsync(int userId, DateTime expiresAt, VerificationCodeType type)
    {
        var code = new VerificationCode(GenerateRandomCode(12), type, expiresAt, userId);
        
        await AuthContext.Get().VerificationCodesSet.AddAsync(code);
        return code;
    }
    
    public static string FormatVerificationCode(string code)
    {
        if (code.Length != 12)
        {
            throw new ArgumentException("O código de verificação deve ter 12 caracteres.");
        }

        return $"{code.Substring(0, 3)}-{code.Substring(3, 3)}-{code.Substring(6, 3)}-{code.Substring(9, 3)}";
    }
    
    private static string GenerateRandomCode(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
        }

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var code = new StringBuilder(length);

        using (var rng = RandomNumberGenerator.Create())
        {
            var buffer = new byte[length];

            for (var i = 0; i < length; i++)
            {
                rng.GetBytes(buffer, i, 1);
                var index = buffer[i] % chars.Length;
                code.Append(chars[index]);
            }
        }

        return code.ToString();
    }
}