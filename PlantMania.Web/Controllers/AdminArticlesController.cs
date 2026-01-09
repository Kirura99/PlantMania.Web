using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantMania.Web.Data;
using PlantMania.Web.Models;

namespace PlantMania.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminArticlesController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminArticlesController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: /AdminArticles
    public async Task<IActionResult> Index()
    {
        var items = await _db.Articles
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync();

        return View(items);
    }

    // GET: /AdminArticles/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var article = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id);
        if (article is null) return NotFound();

        return View(article);
    }

    // GET: /AdminArticles/Create
    public IActionResult Create()
    {
        return View(new Article());
    }

    // POST: /AdminArticles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Article model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Ensure slug is unique
        var slugExists = await _db.Articles.AnyAsync(a => a.Slug == model.Slug);
        if (slugExists)
        {
            ModelState.AddModelError(nameof(Article.Slug), "Slug must be unique.");
            return View(model);
        }

        model.CreatedAtUtc = DateTime.UtcNow;
        model.IsPublished = false;
        model.PublishedAtUtc = null;

        _db.Articles.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /AdminArticles/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return NotFound();

        return View(article);
    }

    // POST: /AdminArticles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Article model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        // Unique slug check (exclude current)
        var slugExists = await _db.Articles.AnyAsync(a => a.Slug == model.Slug && a.Id != model.Id);
        if (slugExists)
        {
            ModelState.AddModelError(nameof(Article.Slug), "Slug must be unique.");
            return View(model);
        }

        var entity = await _db.Articles.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Title = model.Title;
        entity.Summary = model.Summary;
        entity.Content = model.Content;
        entity.Slug = model.Slug;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /AdminArticles/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return NotFound();

        return View(article);
    }

    // POST: /AdminArticles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return NotFound();

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: /AdminArticles/TogglePublish/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return NotFound();

        article.IsPublished = !article.IsPublished;
        article.PublishedAtUtc = article.IsPublished ? DateTime.UtcNow : null;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
