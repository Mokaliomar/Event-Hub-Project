using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EventHubProject.Models;

[Table("ProfilePage")]
public class ProfilePage
{
    [Key]
    public int ProfilePageId { get; set; }
    public string? Title { get; set; }
    public string? Biography { get; set; }
    public string? Logo { get; set; }

    #region Account Connection (Dependent)
    public Account Account { get; set; } = default!;
    [ForeignKey(nameof(Account))]
    public int AccountId { get; set; }
    #endregion
}
