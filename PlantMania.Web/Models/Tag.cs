using System.ComponentModel.DataAnnotations;

namespace PlantMania.Web.Models;

public class Tag
{
    public int Id { get; set; }

    [Required, StringLength(40)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Slug { get; set; } = string.Empty;
}
