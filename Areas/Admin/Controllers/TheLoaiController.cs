using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using Microsoft.EntityFrameworkCore;

using MvcWedBanSach.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;

namespace MvcWedBanSach.Areas.Admin.Controllers;

[Area("Admin")]
public class TheLoaiController : Controller
{
    private readonly ILogger<TheLoaiController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly DbApplicationContext _context;

    public TheLoaiController(ILogger<TheLoaiController> logger, IWebHostEnvironment hostEnvironment, DbApplicationContext context)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _context = context;
    }


    public IActionResult Index()
    {
        var list = _context.TheLoais.ToList();
        return View(list);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PostCreate(string TenTheLoai)
    {
        try
        {
            if (string.IsNullOrEmpty(TenTheLoai))
                ModelState.AddModelError("TenLop", "Vui lòng nhập tên sản lớp");
            if (ModelState.IsValid)
            {
                TheLoai TheLoais = new TheLoai();
                TheLoais.TenTheLoai = TenTheLoai;
                await _context.TheLoais.AddAsync(TheLoais);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "TheLoai");
            }
        }
        catch (Exception)
        {
            return BadRequest("Có lỗi xảy ra trong quá trình xử lý yêu cầu của bạn.");
        }

        return RedirectToAction("Create", "TheLoai");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var TheLoais = await _context.TheLoais.FindAsync(id);
        if (TheLoais == null)
        {
            return NotFound();
        }

        return View(TheLoais);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var TheLoais = await _context.TheLoais.FindAsync(id);
        if (TheLoais == null)
        {
            return NotFound();
        }

        // Truyền mô hình LopEditViewModel với dữ liệu cần thiết
        var viewModel = new TheLoai
        {
            Id = TheLoais.Id,
            TenTheLoai = TheLoais.TenTheLoai
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TheLoai updatedTheLoais)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingTheLoais = await _context.TheLoais.FindAsync(updatedTheLoais.Id);
                if (existingTheLoais == null)
                {
                    return NotFound();
                }

                existingTheLoais.TenTheLoai = updatedTheLoais.TenTheLoai;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "TheLoai");
            }
            catch (Exception)
            {
                return BadRequest(403);
            }
        }

        return View(updatedTheLoais);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var TheLoais = await _context.TheLoais.FindAsync(id);
        if (TheLoais == null)
        {
            return NotFound();
        }

        return View(TheLoais);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var TheLoais = await _context.TheLoais.FindAsync(id);
        if (TheLoais == null)
        {
            return NotFound();
        }

        _context.TheLoais.Remove(TheLoais);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "TheLoai");
    }


}