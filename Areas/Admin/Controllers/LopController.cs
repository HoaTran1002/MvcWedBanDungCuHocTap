using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using Microsoft.EntityFrameworkCore;

using MvcWedBanSach.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;

namespace MvcWedBanSach.Areas.Admin.Controllers;

[Area("Admin")]
public class LopController : Controller{
    private readonly ILogger<LopController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly DbApplicationContext _context;

    public LopController(ILogger<LopController> logger, IWebHostEnvironment hostEnvironment, DbApplicationContext context)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _context = context;
    }


    public IActionResult Index()
    {
        var list = _context.Lops.ToList();
        return View(list);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PostCreate( string TenLop)
    {
        try
        {
            if (string.IsNullOrEmpty(TenLop))
                ModelState.AddModelError("TenLop", "Vui lòng nhập tên sản lớp");
            if (ModelState.IsValid)
            {
                Lop lop = new Lop();
                lop.DoiTuongSP = TenLop;
                await _context.Lops.AddAsync(lop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Lop");
            }
        }
        catch (Exception)
        {
            return BadRequest("Có lỗi xảy ra trong quá trình xử lý yêu cầu của bạn.");
        }

        return RedirectToAction("Create", "Lop");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var lop = await _context.Lops.FindAsync(id);
        if (lop == null)
        {
            return NotFound();
        }

        return View(lop);
    }

   [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var lop = await _context.Lops.FindAsync(id);
        if (lop == null)
        {
            return NotFound();
        }

        // Truyền mô hình LopEditViewModel với dữ liệu cần thiết
        var viewModel = new Lop
        {
            Id = lop.Id,
            DoiTuongSP = lop.DoiTuongSP
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Lop updatedLop)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingLop = await _context.Lops.FindAsync(updatedLop.Id);
                if (existingLop == null)
                {
                    return NotFound();
                }

                existingLop.DoiTuongSP = updatedLop.DoiTuongSP;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Lop");
            }
            catch (Exception)
            {
                return BadRequest(403);
            }
        }

        return View(updatedLop);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var lop = await _context.Lops.FindAsync(id);
        if (lop == null)
        {
            return NotFound();
        }

        return View(lop);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lop = await _context.Lops.FindAsync(id);
        if (lop == null)
        {
            return NotFound();
        }

        _context.Lops.Remove(lop);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Lop");
    }


}