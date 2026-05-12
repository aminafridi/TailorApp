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
}
