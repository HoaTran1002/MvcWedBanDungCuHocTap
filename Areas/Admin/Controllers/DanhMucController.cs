using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using Microsoft.EntityFrameworkCore;
using MvcWedBanSach.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;

namespace MvcWedBanSach.Areas.Admin.Controllers;

[Area("Admin")]
public class DanhMucController : Controller{
    private readonly ILogger<DanhMucController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly DbApplicationContext _context;

    public DanhMucController(ILogger<DanhMucController> logger, IWebHostEnvironment hostEnvironment, DbApplicationContext context)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _context = context;
    }


    public IActionResult Index()
    {
        var list = _context.DanhMucSanPhams.ToList();
        return View(list);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PostCreate(string TenDM)
    {
        try
        {
            if (string.IsNullOrEmpty(TenDM))
                ModelState.AddModelError("TenLop", "Vui lòng nhập tên sản lớp");
            if (ModelState.IsValid)
            {
                DanhMucSanPham danhMuc = new DanhMucSanPham();
                danhMuc.TenDM = TenDM;
                await _context.DanhMucSanPhams.AddAsync(danhMuc);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "DanhMuc");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("Có lỗi xảy ra trong quá trình xử lý yêu cầu của bạn.");
        }

        return RedirectToAction("Create", "DanhMuc");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var danhmuc = await _context.DanhMucSanPhams.FindAsync(id);
        if (danhmuc== null)
        {
            return NotFound();
        }

        return View(danhmuc);
    }

   [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var danhmuc = await _context.DanhMucSanPhams.FindAsync(id);
        if (danhmuc == null)
        {
            return NotFound();
        }

        // Truyền mô hình LopEditViewModel với dữ liệu cần thiết
        var viewModel = new DanhMucSanPham
        {
            Id = danhmuc.Id,
            TenDM = danhmuc.TenDM
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DanhMucSanPham updatedDanhMuc)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingDanhMuc = await _context.DanhMucSanPhams.FindAsync(updatedDanhMuc.Id);
                if (existingDanhMuc == null)
                {
                    return NotFound();
                }

                existingDanhMuc.TenDM = updatedDanhMuc.TenDM;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "DanhMuc");
            }
            catch (Exception)
            {
                return BadRequest(403);
            }
        }

        return View(updatedDanhMuc);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var danhmuc = await _context.DanhMucSanPhams.FindAsync(id);
        if (danhmuc == null)
        {
            return NotFound();
        }

        return View(danhmuc);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var danhmuc = await _context.DanhMucSanPhams.FindAsync(id);
        if (danhmuc == null)
        {
            return NotFound();
        }

        _context.DanhMucSanPhams.Remove(danhmuc);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "DanhMuc");
    }


}