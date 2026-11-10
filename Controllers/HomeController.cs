using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TailorApp.Repositories;

namespace TailorApp.Controllers;

[Authorize]
public class HomeController(ICustomerRepository customerRepo) : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Sizes");
    }

    // Target of app.UseExceptionHandler("/Home/Error")
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
