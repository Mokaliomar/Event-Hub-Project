using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventHubProject.Enums;

namespace EventHubProject.Models;

[Table("Account")]
public class Account
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public Role Role { get; set; }

    #region ProfilePage Connection (Principle)
    public ProfilePage ProfilePage { get; set; } = default!;
    #endregion

    #region Organizer Connection (Dependent)
    public Organizer Organizer { get; set; } = default!;
    [ForeignKey("Organizer")]
    public int OrganizerId { get; set; }
    #endregion
}
