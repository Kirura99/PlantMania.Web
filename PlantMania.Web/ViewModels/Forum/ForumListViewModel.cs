using PlantMania.Web.Models;

namespace PlantMania.Web.ViewModels.Forum;

public class ForumListViewModel
{
    public List<PostListItem> Items { get; set; } = new();

    public string? Q { get; set; }
    public int? CategoryId { get; set; }
    public PostType? Type { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class PostListItem
{
    public int Id { get; set; }
    public PostType Type { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public int AnswersCount { get; set; }
    public bool IsLocked { get; set; }
    public bool IsPinned { get; set; }
}
