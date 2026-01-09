using System.ComponentModel.DataAnnotations;

namespace PlantMania.Web.Models;

public enum PostType
{
    Post = 0,
    Question = 1
}

public enum QuestionStatus
{
    Open = 0,
    Solved = 1,
    Closed = 2
}

public class Post
{
    public int Id { get; set; }

    [Required]
    public PostType Type { get; set; }

    [Required, StringLength(140)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Slug { get; set; } = string.Empty;

    // Author (Identity)
    [Required]
    public string AuthorId { get; set; } = string.Empty;

    // Category
    public int CategoryId { get; set; }

    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }

    // Q&A fields (only for Question type)
    public QuestionStatus? Status { get; set; }
    public int? AcceptedAnswerId { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
