using System.ComponentModel.DataAnnotations;

namespace PlantMania.Web.Models;

public class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }

    [Required, StringLength(400)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public string AuthorId { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
