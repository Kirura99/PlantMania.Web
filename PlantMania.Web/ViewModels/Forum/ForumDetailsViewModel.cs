using System.ComponentModel.DataAnnotations;
using PlantMania.Web.Models;

namespace PlantMania.Web.ViewModels.Forum;

public class ForumDetailsViewModel
{
    public Post Post { get; set; } = null!;
    public string CategoryName { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public List<Answer> Answers { get; set; } = new();

    public bool CanAcceptAnswer { get; set; }
    public int? AcceptedAnswerId { get; set; }

    [Required]
    public string NewAnswerContent { get; set; } = "";
}
