using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lesson_33_MVC.Data.Models;

public class Contact
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [Required]
    [RegularExpression(@"0\d{9}")]
    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? AvatarId { get; set; }

    [ForeignKey(nameof(AvatarId))]
    public Avatar? Avatar { get; set; }
}
