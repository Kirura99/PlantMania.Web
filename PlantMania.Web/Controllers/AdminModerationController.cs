using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlantMania.Web.Controllers;

[Authorize(Roles = "Admin,Moderator")]
public class AdminModerationController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
