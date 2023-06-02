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
    public IActionResult Detail()
    {
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
