using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantMania.Web.Data;
using PlantMania.Web.Models;
using PlantMania.Web.ViewModels.Home;

namespace PlantMania.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        // Stats
        var usersCount = await _db.Users.CountAsync();
        var postsCount = await _db.Posts.CountAsync();
        var questionsCount = await _db.Posts.CountAsync(p => p.Type == PostType.Question);
        var answersCount = await _db.Answers.CountAsync();

        // Block 1: Unanswered questions
        var unanswered = await _db.Posts
            .AsNoTracking()
            .Where(p => p.Type == PostType.Question)
            .Where(p => !_db.Answers.Any(a => a.PostId == p.Id))
            .OrderByDescending(p => p.CreatedAtUtc)
            .Take(5)
            .Join(_db.Categories.AsNoTracking(),
                p => p.CategoryId,
                c => c.Id,
                (p, c) => new PostMini
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreatedAtUtc = p.CreatedAtUtc,
                    CategoryName = c.Name
                })
            .ToListAsync();

        // Block 2: Latest activity (posts + answers merged)
        var latestPosts = await _db.Posts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAtUtc)
            .Take(5)
            .Select(p => new ActivityItem
            {
                Kind = p.Type == PostType.Question ? "Question" : "Post",
                PostId = p.Id,
                Title = p.Title,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();

        var latestAnswers = await _db.Answers
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAtUtc)
            .Take(5)
            .Join(_db.Posts.AsNoTracking(),
                a => a.PostId,
                p => p.Id,
                (a, p) => new ActivityItem
                {
                    Kind = "Answer",
                    PostId = p.Id,
                    Title = p.Title,
                    CreatedAtUtc = a.CreatedAtUtc
                })
            .ToListAsync();

        var activity = latestPosts
            .Concat(latestAnswers)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(8)
            .ToList();

        // Block 3: Popular categories
        var popularCategories = await _db.Posts
            .AsNoTracking()
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(3)
            .Join(_db.Categories.AsNoTracking(),
                x => x.CategoryId,
                c => c.Id,
                (x, c) => new CategoryStat
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    PostsCount = x.Count
                })
            .ToListAsync();

        var vm = new HomeDashboardViewModel
        {
            UsersCount = usersCount,
            PostsCount = postsCount,
            QuestionsCount = questionsCount,
            AnswersCount = answersCount,
            UnansweredQuestions = unanswered,
            LatestActivity = activity,
            PopularCategories = popularCategories
        };

        return View(vm);
    }
}
