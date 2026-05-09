using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHubProject.Models;

[Table("Event")]
public class Event
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? DetailedDescription { get; set; }
    public int AttendeesAllowed { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    #region Self-referencing relationship for sessions
    // Principle Side
    public ICollection<Event> Sessions { get; set; } = new HashSet<Event>();
    
    // Dependent Side
    public Event? AnnualConference { get; set; }
    [ForeignKey("AnnualConference")]
    public int? AnnualConferenceId { get; set; }


    #endregion

    #region Organizer Connection (Dependent)
    public Organizer? Organizer { get; set; } = default!;
    public int? OrganizerId { get; set; }
    #endregion

    #region EventRegistration Connection (Principle)
    public ICollection<EventRegistration> EventRegistrations { get; set; } = new HashSet<EventRegistration>();
    #endregion
}
