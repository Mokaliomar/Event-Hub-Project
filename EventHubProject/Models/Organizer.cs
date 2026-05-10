using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHubProject.Models;

[Table("Organizer")]
public class Organizer
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    #region Account Connection (Principle)
    public Account Account { get; set; } = default!;
    #endregion
    
    #region Event Connection (Principle)
    public ICollection<Event> Events { get; set; } = new HashSet<Event>();
    #endregion
}
