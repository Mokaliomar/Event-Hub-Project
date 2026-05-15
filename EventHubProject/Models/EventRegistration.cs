using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace EventHubProject.Models;

[Table("EventRegistration")]
[PrimaryKey("EventId", "AttendeeId")]
public class EventRegistration
{
    #region Event Connection (Dependent)
    public Event Event { get; set; } = default!;
    [ForeignKey("Event")]
    public int EventId { get; set; }
    #endregion
    
    #region Attendee Connection (Dependent)
    public Attendee Attendee { get; set; } = default!;
    [ForeignKey("Attendee")]
    public int AttendeeId { get; set; }
    #endregion

    public string? ShortNote { get; set; }
    public DateTime RegistrationDate { get; set; }
}
