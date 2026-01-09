using System.ComponentModel.DataAnnotations;

namespace PlantMania.Web.Models;

public class Answer
{
    public int Id { get; set; }

    public int PostId { get; set; } // Question Id

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public string AuthorId { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
