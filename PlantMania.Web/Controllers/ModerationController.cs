using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantMania.Web.Data;

namespace PlantMania.Web.Controllers;

[Authorize(Roles = "Admin,Moderator")]
public class ModerationController : Controller
{
    private readonly ApplicationDbContext _db;

    public ModerationController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();

        post.IsLocked = !post.IsLocked;
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Forum", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePin(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();

        post.IsPinned = !post.IsPinned;
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Forum", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "Forum");
    }
}

