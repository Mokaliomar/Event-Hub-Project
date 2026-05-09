using System;
using System.ComponentModel.DataAnnotations.Schema;
using EventHubProject.Models.Enums;

namespace EventHubProject.Models;

[Table("Badge")]
public class Badge
{
    public int BadgeId { get; set; }
    public DateTime DateIssued { get; set; }
    public Tier Tier { get; set; }

    #region Attendee Connection (Principle)
    public Attendee Attendee { get; set; } = default!;
    [ForeignKey("Attendee")]
    public int AttendeeId { get; set; }
    #endregion
}

