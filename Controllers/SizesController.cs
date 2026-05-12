using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TailorApp.Models;
using TailorApp.Repositories;

namespace TailorApp.Controllers;

[Authorize]
public class SizesController(ISizeRepository sizeRepo, ICustomerRepository customerRepo) : Controller
{
    // GET: Sizes?search=...&page=1&pageSize=50&searchName=...&searchRegNo=...&searchMobile=...
    public async Task<IActionResult> Index(string? search, string? searchName, string? searchRegNo, string? searchMobile, int page = 1, int pageSize = 50)
    {
        ViewBag.Search = search;
        ViewBag.SearchName = searchName;
        ViewBag.SearchRegNo = searchRegNo;
        ViewBag.SearchMobile = searchMobile;
        var pagedResult = await sizeRepo.GetPagedAsync(search, page, pageSize, searchName, searchRegNo, searchMobile);
        return View(pagedResult);
    }

    // GET: Sizes/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var size = await sizeRepo.GetByIdAsync(id);
        if (size is null) return NotFound();
        return View(size);
    }

    // GET: Sizes/DetailsPartial/5
    public async Task<IActionResult> DetailsPartial(int id)
    {
        var size = await sizeRepo.GetByIdAsync(id);
        if (size == null) return NotFound();

        var customer = await customerRepo.GetByIdAsync(size.Customer_ID);
        if (customer != null)
        {
            ViewBag.CustomerMobile = customer.MobileNo1 ?? customer.MobileNo2;
            var sizes = (await sizeRepo.GetByCustomerIdAsync(size.Customer_ID)).OrderByDescending(x => x.SizeID).ToList();
            ViewBag.AllSizes = sizes;
        }

        return PartialView("_DetailsModal", size);
    }

    // GET: Sizes/LatestSizePartial?customerId=3
    [HttpGet]
    public async Task<IActionResult> LatestSizePartial(int customerId)
    {
        var customer = await customerRepo.GetByIdAsync(customerId);
        if (customer == null) return NotFound();

        var sizes = (await sizeRepo.GetByCustomerIdAsync(customerId)).OrderByDescending(x => x.SizeID).ToList();
        var latestSize = sizes.FirstOrDefault();

        if (latestSize == null)
        {
            ViewBag.CustomerName = customer.CustomerName;
            return PartialView("_NoSizesModal", customerId);
        }

        ViewBag.CustomerMobile = customer.MobileNo1 ?? customer.MobileNo2;
        ViewBag.AllSizes = sizes;

        return PartialView("_DetailsModal", latestSize);
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
            RegisterNo = nextRegNo
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
        return RedirectToAction("Index", "Sizes");
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
        return RedirectToAction("Index", "Sizes");
    }

    // POST: Sizes/Delete/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int customerId)
    {
        // If customerId isn't passed from the form, fetch the size to find it
        if (customerId == 0)
        {
            var size = await sizeRepo.GetByIdAsync(id);
            if (size != null) customerId = size.Customer_ID;
        }

        await sizeRepo.DeleteAsync(id);
        TempData["Success"] = "Measurement record deleted.";

        return RedirectToAction("Index", "Sizes");
    }
}
