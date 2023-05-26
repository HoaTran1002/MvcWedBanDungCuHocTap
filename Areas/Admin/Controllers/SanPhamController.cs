using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using MvcWedBanSach.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;


namespace MvcWedBanSach.Areas.Admin.Controllers;
[Area("Admin")]
public class SanPhamController : Controller
{
    private readonly ILogger<SanPhamController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly DbApplicationContext _context;

    public SanPhamController(ILogger<SanPhamController> logger, IWebHostEnvironment hostEnvironment, DbApplicationContext context)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _context = context;
    }
    public IActionResult DanhSachSanPham()
    {

        return View();
    }
    public IActionResult ThemSanPham()
    {
        ViewBag.ListThuongHieu = _context.ThuongHieus.ToList<ThuongHieu>();
        ViewBag.ListTheLoai = _context.TheLoais.ToList<TheLoai>();
        ViewBag.ListLop = _context.Lops.ToList<Lop>();
        ViewBag.ListdanhMucSanPhams = _context.DanhMucSanPhams.ToList<DanhMucSanPham>();
        // Kiểm tra ModelState để xem có lỗi hay không
        if (!ModelState.IsValid)
        {
            // Lấy danh sách lỗi từ ModelState và lưu vào TempData
            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            ViewBag.errorMessages = errorMessages;

        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> PostThemSanPham(
    [Required(ErrorMessage = "Vui lòng nhập mã sản phẩm")] string MaSP,
    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")] string TenSP,
    string MoTaSP,
    string XuatSuSP,
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải là số nguyên không âm")] int SoLuongTon,
    [Range(0, double.MaxValue, ErrorMessage = "Giá nhập phải là số không âm")] decimal GiaNhapSP,
    [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải là số không âm")] decimal GiaBanSP,
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng nhập phải là số nguyên không âm")] int SoLuongNhap,
    [Range(0, int.MaxValue, ErrorMessage = "Thể loại sản phẩm không hợp lệ")] int TheLoaiSP,
    [Range(0, int.MaxValue, ErrorMessage = "Thương hiệu không hợp lệ")] int ThuongHieuSp,
    [Range(0, int.MaxValue, ErrorMessage = "Lớp sản phẩm không hợp lệ")] int LopSP,
    [Range(0, int.MaxValue, ErrorMessage = "Danh mục không hợp lệ")] int IdDanhMuc,
    HinhAnh hinhanh)
    {
        try
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(hinhanh.HinhAnhSP.FileName);
            string extension = Path.GetExtension(hinhanh.HinhAnhSP.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/img/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await hinhanh.HinhAnhSP.CopyToAsync(fileStream);
            }
            SanPham sanpham = new SanPham();
            sanpham.MaSP = MaSP;
            sanpham.TenSP = TenSP;
            sanpham.MoTaSP = MoTaSP;
            sanpham.XuatSu = XuatSuSP;
            sanpham.SoLuongNhap = SoLuongNhap;
            sanpham.SoLuongTon = SoLuongTon;
            sanpham.GiaNhap = GiaNhapSP;
            sanpham.GiaBan = GiaBanSP;

            sanpham.IdDanhMucSanPham = IdDanhMuc;
            await _context.SanPhams.AddAsync(sanpham);
            await _context.SaveChangesAsync();

            hinhanh.IdSP = sanpham.Id;
            // Insert records
            await _context.HinhAnhs.AddAsync(hinhanh);
            await _context.SaveChangesAsync();
            ChiTietSanPham ctsp = new ChiTietSanPham();
            ctsp.IdThuongHieu = ThuongHieuSp;
            ctsp.IdTheLoai = TheLoaiSP;
            ctsp.IdLop = LopSP;
            ctsp.IdSP = sanpham.Id;
            await _context.ChiTietSanPhams.AddAsync(ctsp);
            await _context.SaveChangesAsync();

            return RedirectToAction("DanhSachSanPham", "SanPham");
        }
        catch (System.Exception)
        {

            throw;
            return BadRequest(403);
        }
        // Trả về view với thông báo lỗi và giữ lại các giá trị đã nhập

        return RedirectToAction("DanhSachSanPham", "SanPham");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}