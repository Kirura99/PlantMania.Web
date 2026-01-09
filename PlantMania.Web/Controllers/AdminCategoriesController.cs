using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantMania.Web.Data;
using PlantMania.Web.Models;

namespace PlantMania.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminCategoriesController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminCategoriesController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: /AdminCategories
    public async Task<IActionResult> Index()
    {
        var items = await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(items);
    }

    // GET: /AdminCategories/Create
    public IActionResult Create()
    {
        return View(new Category());
    }

    // POST: /AdminCategories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var slugExists = await _db.Categories.AnyAsync(x => x.Slug == model.Slug);
        if (slugExists)
        {
            ModelState.AddModelError(nameof(Category.Slug), "Slug must be unique.");
            return View(model);
        }

        _db.Categories.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /AdminCategories/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.Categories.FindAsync(id);
        if (entity is null) return NotFound();

        return View(entity);
    }

    // POST: /AdminCategories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var slugExists = await _db.Categories.AnyAsync(x => x.Slug == model.Slug && x.Id != model.Id);
        if (slugExists)
        {
            ModelState.AddModelError(nameof(Category.Slug), "Slug must be unique.");
            return View(model);
        }

        var entity = await _db.Categories.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = model.Name;
        entity.Slug = model.Slug;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /AdminCategories/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Categories.FindAsync(id);
        if (entity is null) return NotFound();

        return View(entity);
    }

    // POST: /AdminCategories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _db.Categories.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
