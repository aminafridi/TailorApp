using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TailorApp.Models;
using TailorApp.Repositories;

namespace TailorApp.Controllers;

[Authorize]
public class SizesController(ISizeRepository sizeRepo, ICustomerRepository customerRepo) : Controller
{
    // GET: Sizes/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var size = await sizeRepo.GetByIdAsync(id);
        if (size is null) return NotFound();
        return View(size);
    }

    // GET: Sizes/Create?customerId=3
    public async Task<IActionResult> Create(int customerId)
    {
        var customer = await customerRepo.GetByIdAsync(customerId);
        if (customer is null) return NotFound();

        int nextRegNo = await sizeRepo.GetNextRegisterNoAsync(customerId);
        ViewBag.CustomerName = customer.CustomerName;

        return View(new Size
        {
            Customer_ID = customerId,
            RegisterNo = nextRegNo,
            BazoType = 1,
            CalarType = 1,
            GheraType = 1
        });
    }

    // POST: Sizes/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Size model)
    {
        if (!ModelState.IsValid)
        {
            var customer = await customerRepo.GetByIdAsync(model.Customer_ID);
            ViewBag.CustomerName = customer?.CustomerName;
            return View(model);
        }

        await sizeRepo.CreateAsync(model);
        TempData["Success"] = $"Measurement record #{model.RegisterNo} added!";
        return RedirectToAction("Details", "Customers", new { id = model.Customer_ID });
    }

    // GET: Sizes/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var size = await sizeRepo.GetByIdAsync(id);
        if (size is null) return NotFound();
        ViewBag.CustomerName = size.CustomerName;
        return View(size);
    }

    // POST: Sizes/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Size model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CustomerName = model.CustomerName;
            return View(model);
        }

        await sizeRepo.UpdateAsync(model);
        TempData["Success"] = "Measurement updated successfully!";
        return RedirectToAction("Details", "Customers", new { id = model.Customer_ID });
    }

    // POST: Sizes/Delete/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int customerId)
    {
        await sizeRepo.DeleteAsync(id);
        TempData["Success"] = "Measurement record deleted.";
        return RedirectToAction("Details", "Customers", new { id = customerId });
    }
}
