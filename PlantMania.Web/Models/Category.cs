using System.ComponentModel.DataAnnotations;

namespace PlantMania.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Slug { get; set; } = string.Empty;
}
