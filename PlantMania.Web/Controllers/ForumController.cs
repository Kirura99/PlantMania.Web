using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantMania.Web.Data;
using PlantMania.Web.Models;
using PlantMania.Web.Services;
using PlantMania.Web.ViewModels.Forum;

namespace PlantMania.Web.Controllers;

public class ForumController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public ForumController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET: /Forum
    public async Task<IActionResult> Index(string? q, int? categoryId, PostType? type, int page = 1)
    {
        const int pageSize = 10;

        var query = _db.Posts.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p => p.Title.Contains(term) || p.Content.Contains(term));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.IsPinned)
            .ThenByDescending(p => p.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostListItem
            {
                Id = p.Id,
                Type = p.Type,
                Title = p.Title,
                Slug = p.Slug,
                CreatedAtUtc = p.CreatedAtUtc,
                IsLocked = p.IsLocked,
                IsPinned = p.IsPinned,
                CategoryName = _db.Categories.Where(c => c.Id == p.CategoryId).Select(c => c.Name).FirstOrDefault() ?? "",
                AnswersCount = _db.Answers.Count(a => a.PostId == p.Id)
            })
            .ToListAsync();

        var vm = new ForumListViewModel
        {
            Items = items,
            Q = q,
            CategoryId = categoryId,
            Type = type,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        ViewBag.Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        return View(vm);
    }

    // GET: /Forum/Create
    [Authorize]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        return View(new ForumCreateViewModel { Type = PostType.Post });
    }

    // POST: /Forum/Create
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ForumCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Forbid();

        var baseSlug = SlugService.ToSlug(model.Title);
        var slug = baseSlug;
        var i = 2;
        while (await _db.Posts.AnyAsync(p => p.Slug == slug))
        {
            slug = $"{baseSlug}-{i}";
            i++;
        }

        var entity = new Post
        {
            Type = model.Type,
            Title = model.Title.Trim(),
            Content = model.Content.Trim(),
            CategoryId = model.CategoryId,
            AuthorId = userId,
            Slug = slug,
            CreatedAtUtc = DateTime.UtcNow,
            Status = model.Type == PostType.Question ? QuestionStatus.Open : null
        };

        _db.Posts.Add(entity);
        await _db.SaveChangesAsync();

        // Tags (optional)
        if (!string.IsNullOrWhiteSpace(model.TagsCsv))
        {
            var tags = model.TagsCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToLowerInvariant())
                .Distinct()
                .Take(8)
                .ToList();

            foreach (var t in tags)
            {
                var tagSlug = SlugService.ToSlug(t);
                var tag = await _db.Tags.FirstOrDefaultAsync(x => x.Slug == tagSlug);
                if (tag is null)
                {
                    tag = new Tag { Name = t, Slug = tagSlug };
                    _db.Tags.Add(tag);
                    await _db.SaveChangesAsync();
                }

                var exists = await _db.PostTags.AnyAsync(pt => pt.PostId == entity.Id && pt.TagId == tag.Id);
                if (!exists)
                {
                    _db.PostTags.Add(new PostTag { PostId = entity.Id, TagId = tag.Id });
                }
            }

            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    // GET: /Forum/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var post = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (post is null) return NotFound();

        var categoryName = await _db.Categories.Where(c => c.Id == post.CategoryId).Select(c => c.Name).FirstOrDefaultAsync() ?? "";

        var tags = await _db.PostTags
            .Where(pt => pt.PostId == id)
            .Join(_db.Tags, pt => pt.TagId, t => t.Id, (pt, t) => t.Name)
            .ToListAsync();

        var answers = await _db.Answers
            .AsNoTracking()
            .Where(a => a.PostId == id)
            .OrderBy(a => a.CreatedAtUtc)
            .ToListAsync();

        var userId = _userManager.GetUserId(User);

        var canAccept =
            post.Type == PostType.Question &&
            !string.IsNullOrWhiteSpace(userId) &&
            (post.AuthorId == userId || User.IsInRole("Admin") || User.IsInRole("Moderator"));

        var vm = new ForumDetailsViewModel
        {
            Post = post,
            CategoryName = categoryName,
            Tags = tags,
            Answers = answers,
            CanAcceptAnswer = canAccept,
            AcceptedAnswerId = post.AcceptedAnswerId
        };

        return View(vm);
    }

    // POST: /Forum/AddAnswer/5
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAnswer(int id, string newAnswerContent)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null) return NotFound();

        if (post.Type != PostType.Question) return BadRequest();
        if (post.IsLocked) return Forbid();

        var content = (newAnswerContent ?? "").Trim();
        if (content.Length == 0)
            return RedirectToAction(nameof(Details), new { id });

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Forbid();

        var answer = new Answer
        {
            PostId = id,
            AuthorId = userId,
            Content = content,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Answers.Add(answer);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }


    // POST: /Forum/AcceptAnswer?postId=5&answerId=12
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptAnswer(int postId, int answerId)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) return NotFound();

        if (post.Type != PostType.Question) return BadRequest();

        var answer = await _db.Answers.FirstOrDefaultAsync(a => a.Id == answerId && a.PostId == postId);
        if (answer is null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Forbid();

        var canAccept = post.AuthorId == userId || User.IsInRole("Admin") || User.IsInRole("Moderator");
        if (!canAccept) return Forbid();

        post.AcceptedAnswerId = answerId;
        post.Status = QuestionStatus.Solved;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = postId });
    }
}
