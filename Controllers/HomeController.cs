using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TailorApp.Repositories;

namespace TailorApp.Controllers;

[Authorize]
public class HomeController(ICustomerRepository customerRepo) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await customerRepo.GetDashboardStatsAsync();
        return View(model);
    }
}
