using System.ComponentModel.DataAnnotations;

namespace PlantMania.Web.Models;

public class Article
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    // Used for SEO-friendly URL and searching
    [Required, StringLength(140)]
    public string Slug { get; set; } = string.Empty;

    public bool IsPublished { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }
}
