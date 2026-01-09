using PlantMania.Web.Models;

namespace PlantMania.Web.ViewModels.Home;

public class HomeDashboardViewModel
{
    // Stats
    public int UsersCount { get; set; }
    public int PostsCount { get; set; }
    public int QuestionsCount { get; set; }
    public int AnswersCount { get; set; }

    // Blocks
    public List<PostMini> UnansweredQuestions { get; set; } = new();
    public List<ActivityItem> LatestActivity { get; set; } = new();
    public List<CategoryStat> PopularCategories { get; set; } = new();
}

public class PostMini
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public string CategoryName { get; set; } = "";
}

public class ActivityItem
{
    public string Kind { get; set; } = ""; // "Question" / "Post" / "Answer"
    public int? PostId { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
}

public class CategoryStat
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public int PostsCount { get; set; }
}
