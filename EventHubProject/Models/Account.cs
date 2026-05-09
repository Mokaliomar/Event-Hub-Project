using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHubProject.Models;

[Table("Account")]
public class Account
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }

    #region ProfilePage Connection (Principle)
    public ProfilePage ProfilePage { get; set; } = default!;
    #endregion

    #region Organizer Connection (Dependent)
    public Organizer Organizer { get; set; } = default!;
    [ForeignKey("Organizer")]
    public int OrganizerId { get; set; }
    #endregion
}
