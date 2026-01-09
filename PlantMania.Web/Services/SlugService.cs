using System.Text.RegularExpressions;

namespace PlantMania.Web.Services;

public static class SlugService
{
    // Converts title to a URL-friendly slug
    public static string ToSlug(string input)
    {
        input = input.Trim().ToLowerInvariant();

        input = Regex.Replace(input, @"\s+", "-");
        input = Regex.Replace(input, @"[^a-z0-9\-]", "");
        input = Regex.Replace(input, @"-+", "-");

        return input.Trim('-');
    }
}
