using System.ComponentModel.DataAnnotations;

namespace Lesson_33_MVC.Data.Models;

public class Avatar
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ImageType { get; set; }

    [MaxLength(1_000_000)]
    public byte[]? ImageData { get; set; }
}