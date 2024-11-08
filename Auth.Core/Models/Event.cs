using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Core.DTOs.Enum;
using Auth.Core.Infrastructure;
using Hyzen.SDK.Exception;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Models;

[Table("events")]
public class Event
{
    private Event() { }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")] 
    public int Id { get; set; }
    
    [Column("user_id", TypeName = "INT"), ForeignKey("User"), Required] 
    public int UserId { get; set; }
    public User User { get; set; }
    
    [Column("event_type", TypeName = "INT"), Required] 
    public EventType EventType { get; set; }
    
    [Column("created_at", TypeName = "TIMESTAMP"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }
    
    [Column("description", TypeName = "TEXT"), MaxLength(32768)] 
    public string Description { get; set; }

    public Event(int userId, EventType eventType, string description = null)
    {
        UserId = userId;
        EventType = eventType;
        Description = description;
    }

    public static async Task<int> CountAsync(int userId, EventType eventType, DateTime? from = null, DateTime? to = null)
    {
        var query = AuthContext.Get().EventsSet
            .Where(s => s.UserId == userId && s.EventType == eventType);

        if (from.HasValue)
            query = query.Where(s => s.CreatedAt >= from);

        if (to.HasValue)
            query = query.Where(s => s.CreatedAt <= to);

        return await query.CountAsync();
    }
    
    public static async Task Register(int userId, EventType eventType, string description)
    {
        var @event = new Event(userId, eventType, description);
        await AuthContext.Get().EventsSet.AddAsync(@event);
    }
}
