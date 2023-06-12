using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using MvcWedBanSach.Models;

namespace MvcWedBanSach.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DbApplicationContext _context;
    private static List<DanhMucSanPham> danhMucSanPhams;
    public HomeController(ILogger<HomeController> logger, DbApplicationContext context)
    {
        _logger = logger;
        _context = context;
        danhMucSanPhams = _context.DanhMucSanPhams.ToList<DanhMucSanPham>();

    }
    public IActionResult Index()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        var query = (from a in _context.SanPhams
                     join b in _context.HinhAnhs on a.Id equals b.IdSP
                     select new { SanPham = a, HinhAnh = b }).ToList();
        ViewBag.ListSp = query;
        return View();
    }
    [Authorize]
    public IActionResult Cart()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        return View();
    }
    [Authorize]
    public IActionResult Checkout()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        return View();
    }
    public IActionResult Contact()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        return View();
    }

    public IActionResult Search(string keysearch)
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        var products = (from a in _context.SanPhams
                     join b in _context.HinhAnhs on a.Id equals b.IdSP
                     select new { SanPham = a, HinhAnh = b })
            .Where(p => p.SanPham.TenSP.Contains(keysearch))
            .ToList();
            ViewBag.keysearch = keysearch;
            ViewBag.ListSp =products;
        return View();
    }

    public IActionResult Detail(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var query = (   from a in _context.SanPhams
                        join b in _context.HinhAnhs on a.Id equals b.IdSP
                        join c in _context.ChiTietSanPhams on a.Id equals c.IdSP
                        join d in _context.ThuongHieus on c.IdThuongHieu equals d.Id
                        join e in _context.TheLoais on c.IdTheLoai equals e.Id
                        join f in _context.Lops on c.IdLop equals f.Id
                        join g in _context.DanhMucSanPhams on a.IdDanhMucSanPham equals g.Id
                        where a.Id == id 
                        select new { SanPham = a, HinhAnh = b, ThuongHieu = d, TheLoai = e, Lop = f, DanhMuc = g }).FirstOrDefault();
        ViewBag.SanPham = query;
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        return View();
    }
    public IActionResult Shop()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;

        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
