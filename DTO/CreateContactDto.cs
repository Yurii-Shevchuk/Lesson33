using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration.Annotations;

namespace Lesson_33_MVC.DTO;

public class CreateContactDto // Data Transfer Object
{
    [Required]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string? LastName { get; set; }

    [RegularExpression(@"0\d{9}")]
    public string? Phone { get; set; }

    public string? Address { get; set; }

    [Ignore]
    public IFormFile? AvatarFile { get; set; }
}
