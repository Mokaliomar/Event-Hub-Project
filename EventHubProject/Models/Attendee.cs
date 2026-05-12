using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.EntityFrameworkCore;

namespace EventHubProject.Models;
[Owned]
public class Address
{
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string PostalCode { get; set; }
}
[Table("Attendee")]
public class Attendee
{
    public int AttendeeId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }

    #region Event Registration Connection (Principle)
    public ICollection<EventRegistration> EventRegistrations { get; set; } = new HashSet<EventRegistration>();
    #endregion

    #region Badge Connection (Principle)
    public Badge? Badge { get; set; }
    #endregion
}
