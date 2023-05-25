using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanSach.Models;

namespace MvcWedBanSach.Areas.Admin.Controllers;
[Area("Admin")]
public class DashBoardController : Controller
{
    private readonly ILogger<DashBoardController> _logger;

    public DashBoardController(ILogger<DashBoardController> logger)
    {
        _logger = logger;

    }
    public IActionResult Index()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}