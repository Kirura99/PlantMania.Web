using System.ComponentModel.DataAnnotations;
using PlantMania.Web.Models;

namespace PlantMania.Web.ViewModels.Forum;

public class ForumCreateViewModel
{
    [Required]
    public PostType Type { get; set; }

    [Required, StringLength(140)]
    public string Title { get; set; } = "";

    [Required]
    public string Content { get; set; } = "";

    [Required]
    public int CategoryId { get; set; }

    // Simple tags input: "orchid, pests, soil"
    public string? TagsCsv { get; set; }
}
