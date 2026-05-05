using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TailorApp.Models;
using TailorApp.Repositories;

namespace TailorApp.Controllers;

[Authorize]
public class CustomersController(ICustomerRepository customerRepo, ISizeRepository sizeRepo) : Controller
{
    // GET: Customers?search=...&page=1&pageSize=50
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 50)
    {
        ViewBag.Search = search;
        var pagedResult = await customerRepo.GetPagedAsync(search, page, pageSize);
        return View(pagedResult);
    }

    // GET: Customers/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var customer = await customerRepo.GetByIdAsync(id);
        if (customer is null) return NotFound();

        var sizes = await sizeRepo.GetByCustomerIdAsync(id);
        ViewBag.Sizes = sizes.ToList();
        return View(customer);
    }

    // GET: Customers/Create
    public IActionResult Create() => View(new Customer { Status = true });

    // POST: Customers/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        if (!ModelState.IsValid) return View(model);
        int newId = await customerRepo.CreateAsync(model);
        TempData["Success"] = $"Customer '{model.CustomerName}' added successfully!";
        return RedirectToAction(nameof(Details), new { id = newId });
    }

    // GET: Customers/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await customerRepo.GetByIdAsync(id);
        if (customer is null) return NotFound();
        return View(customer);
    }

    // POST: Customers/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Customer model)
    {
        if (!ModelState.IsValid) return View(model);
        await customerRepo.UpdateAsync(model);
        TempData["Success"] = "Customer updated successfully!";
        return RedirectToAction(nameof(Details), new { id = model.CustomerID });
    }

    // POST: Customers/Delete/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await customerRepo.DeleteAsync(id);
        TempData["Success"] = "Customer deleted.";
        return RedirectToAction(nameof(Index));
    }
}
