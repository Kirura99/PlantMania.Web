using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantMania.Web.Data;
using PlantMania.Web.Models;

namespace PlantMania.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminTagsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminTagsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _db.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();

        return View(items);
    }

    public IActionResult Create() => View(new Tag());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tag model)
    {
        if (!ModelState.IsValid) return View(model);

        var slugExists = await _db.Tags.AnyAsync(x => x.Slug == model.Slug);
        if (slugExists)
        {
            ModelState.AddModelError(nameof(Tag.Slug), "Slug must be unique.");
            return View(model);
        }

        _db.Tags.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.Tags.FindAsync(id);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Tag model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var slugExists = await _db.Tags.AnyAsync(x => x.Slug == model.Slug && x.Id != model.Id);
        if (slugExists)
        {
            ModelState.AddModelError(nameof(Tag.Slug), "Slug must be unique.");
            return View(model);
        }

        var entity = await _db.Tags.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = model.Name;
        entity.Slug = model.Slug;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Tags.FindAsync(id);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _db.Tags.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Tags.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
