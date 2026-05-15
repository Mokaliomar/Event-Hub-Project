using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHubProject.Models;

[Table("Organizer")]
public class Organizer
{
    public int Id { get; set; }
    public required string Name { get; set; }

    #region Account Connection (Principle)
    public Account Account { get; set; } = default!;
    #endregion

    #region Event Connection (Principle)
    public ICollection<Event> Events { get; set; } = new HashSet<Event>();
    #endregion
}
