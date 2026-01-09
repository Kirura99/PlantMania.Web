using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlantMania.Web.ViewModels.Admin;

namespace PlantMania.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminUsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminUsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: /AdminUsers/Moderator
    public IActionResult Moderator()
    {
        return View(new AssignModeratorViewModel());
    }

    // POST: /AdminUsers/Moderator
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Moderator(AssignModeratorViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            model.ResultMessage = "User not found.";
            return View(model);
        }

        if (model.MakeModerator)
        {
            if (!await _userManager.IsInRoleAsync(user, "Moderator"))
                await _userManager.AddToRoleAsync(user, "Moderator");

            model.ResultMessage = "Moderator role assigned.";
        }
        else
        {
            if (await _userManager.IsInRoleAsync(user, "Moderator"))
                await _userManager.RemoveFromRoleAsync(user, "Moderator");

            model.ResultMessage = "Moderator role removed.";
        }

        return View(model);
    }
}
