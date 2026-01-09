using System.ComponentModel.DataAnnotations;

namespace PlantMania.Web.ViewModels.Admin;

public class AssignModeratorViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool MakeModerator { get; set; } = true;

    public string? ResultMessage { get; set; }
}
