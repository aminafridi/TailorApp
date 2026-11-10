using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using TailorApp.Models;
using TailorApp.Repositories;

namespace TailorApp.Controllers;

[Authorize]
public class SizesController(ISizeRepository sizeRepo, ICustomerRepository customerRepo) : Controller
{
    // GET: Sizes?name=...&regNo=...&mobile=...&page=1&pageSize=50
    // Each box searches its own field; filled boxes are combined.
    public async Task<IActionResult> Index(string? name, string? regNo, string? mobile, int page = 1, int pageSize = 50)
    {
        if (page < 1) page = 1;

        var regDigits = Digits(regNo);
        var mobileDigits = Digits(mobile);
        if (mobile?.TrimStart().StartsWith('+') == true && mobileDigits.StartsWith("92"))
            mobileDigits = "0" + mobileDigits[2..]; // +92 345... is stored as 0345...

        var filter = new SizeSearch
        {
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
            RegisterNo = int.TryParse(regDigits, out var reg) ? reg : null,
            Mobile = mobileDigits.Length > 0 ? mobileDigits : null
        };

        // Show the boxes exactly as typed
        ViewBag.Name = name?.Trim() ?? "";
        ViewBag.RegNo = regNo?.Trim() ?? "";
        ViewBag.Mobile = mobile?.Trim() ?? "";
        ViewBag.HasFilter = !filter.IsEmpty;

        var pagedResult = await sizeRepo.GetPagedAsync(filter, page, pageSize);
        return View(pagedResult);
    }

    // Numbers are stored with English digits and no separators, so digits typed on an
    // Urdu keyboard are converted and spaces, dashes and "+" are dropped.
    private static string Digits(string? text) =>
        string.IsNullOrWhiteSpace(text) ? "" : new string(text
            .Select(c => c is >= '۰' and <= '۹' ? (char)('0' + (c - '۰')) :
                         c is >= '٠' and <= '٩' ? (char)('0' + (c - '٠')) : c)
            .Where(char.IsAsciiDigit).ToArray());

    // Adds a form error when another record already uses the register number.
    private async Task CheckRegisterNoIsFreeAsync(int registerNo, int? exceptSizeId)
    {
        var owner = await sizeRepo.GetRegisterNoOwnerAsync(registerNo, exceptSizeId);
        if (owner is null) return;

        int next = await sizeRepo.GetNextRegisterNoAsync(0);
        ModelState.AddModelError(nameof(Size.RegisterNo),
            $"Register no {registerNo} is already used by {owner}. Next free number: {next}.");
    }

    // The stored procedures refuse a duplicate too (error 50010), in case two
    // people save the same number at the same moment.
    private static bool IsDuplicateRegisterNo(SqlException ex) => ex.Number == 50010;

    // GET: Sizes/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var size = await sizeRepo.GetByIdAsync(id);
        if (size is null) return NotFound();

        var customer = await customerRepo.GetByIdAsync(size.Customer_ID);
        ViewBag.CustomerMobile = customer?.MobileNo1 ?? customer?.MobileNo2;
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
        if (ModelState.IsValid)
            await CheckRegisterNoIsFreeAsync(model.RegisterNo, null);

        if (ModelState.IsValid)
        {
            try
            {
                await sizeRepo.CreateAsync(model);
            }
            catch (SqlException ex) when (IsDuplicateRegisterNo(ex))
            {
                await CheckRegisterNoIsFreeAsync(model.RegisterNo, null);
            }
        }

        if (!ModelState.IsValid)
        {
            var customer = await customerRepo.GetByIdAsync(model.Customer_ID);
            ViewBag.CustomerName = customer?.CustomerName;
            return View(model);
        }

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
        var existing = await sizeRepo.GetByIdAsync(model.SizeID);
        if (existing is null) return NotFound();

        // Some old records share a number from before this rule; they can still be
        // edited, and only a changed number has to be free.
        bool numberChanged = existing.RegisterNo != model.RegisterNo;
        if (ModelState.IsValid && numberChanged)
            await CheckRegisterNoIsFreeAsync(model.RegisterNo, model.SizeID);

        if (ModelState.IsValid)
        {
            try
            {
                await sizeRepo.UpdateAsync(model);
            }
            catch (SqlException ex) when (IsDuplicateRegisterNo(ex))
            {
                await CheckRegisterNoIsFreeAsync(model.RegisterNo, model.SizeID);
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.CustomerName = existing.CustomerName;
            return View(model);
        }

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
