using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using Microsoft.EntityFrameworkCore;

using MvcWedBanSach.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;

namespace MvcWedBanSach.Areas.Admin.Controllers;

[Area("Admin")]
public class ThuongHieuController : Controller{
    private readonly ILogger<ThuongHieuController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly DbApplicationContext _context;

    public ThuongHieuController(ILogger<ThuongHieuController> logger, IWebHostEnvironment hostEnvironment, DbApplicationContext context)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _context = context;
    }


    public IActionResult Index()
    {
        var list = _context.ThuongHieus.ToList();
        return View(list);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PostCreate( string TenThuongHieu)
    {
        try
        {
            if (string.IsNullOrEmpty(TenThuongHieu))
                ModelState.AddModelError("TenThuongHieu", "Vui lòng nhập tên sản lớp");
            if (ModelState.IsValid)
            {
                ThuongHieu thuonghieu = new ThuongHieu();
                thuonghieu.TenThuongHieuSP = TenThuongHieu;
                await _context.ThuongHieus.AddAsync(thuonghieu);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ThuongHieu");
            }
        }
        catch (Exception)
        {
            return BadRequest("Có lỗi xảy ra trong quá trình xử lý yêu cầu của bạn.");
        }

        return RedirectToAction("Create", "ThuongHieu");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var thuonghieu = await _context.ThuongHieus.FindAsync(id);
        if (thuonghieu== null)
        {
            return NotFound();
        }

        return View(thuonghieu);
    }

   [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var thuonghieu = await _context.ThuongHieus.FindAsync(id);
        if (thuonghieu == null)
        {
            return NotFound();
        }

        var viewModel = new ThuongHieu
        {
            Id = thuonghieu.Id,
            TenThuongHieuSP = thuonghieu.TenThuongHieuSP
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ThuongHieu updatedThuongHieu)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingThuongHieu = await _context.ThuongHieus.FindAsync(updatedThuongHieu.Id);
                if (existingThuongHieu == null)
                {
                    return NotFound();
                }

                existingThuongHieu.TenThuongHieuSP = updatedThuongHieu.TenThuongHieuSP;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ThuongHieu");
            }
            catch (Exception)
            {
                return BadRequest(403);
            }
        }

        return View(updatedThuongHieu);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var thuonghieu = await _context.ThuongHieus.FindAsync(id);
        if (thuonghieu == null)
        {
            return NotFound();
        }

        return View(thuonghieu);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var thuonghieu = await _context.ThuongHieus.FindAsync(id);
        if (thuonghieu == null)
        {
            return NotFound();
        }

        _context.ThuongHieus.Remove(thuonghieu);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "ThuongHieu");
    }


}