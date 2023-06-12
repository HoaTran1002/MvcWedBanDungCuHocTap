using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using MvcWedBanSach.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

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
    [HttpGet]
    public IActionResult DanhSachSanPham()
    {
        var query = (from a in _context.SanPhams
                     join b in _context.HinhAnhs on a.Id equals b.IdSP
                     select new { SanPham = a, HinhAnh = b }).ToList();
        ViewBag.ListSp = query;
        return View();
    }
    [HttpGet]
    public IActionResult XemChiTiet()
    {

        return View();
    }
    [HttpGet("/Admin/SanPham/SuaSanPham/{Id}")]
    public async Task<IActionResult> SuaSanPham([FromRoute] int Id)
    {
        // Sử dụng ID để thực hiện các xử lý khác, ví dụ:
        return View();
    }
    [HttpGet]
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
    IFormFile hinhanh)
    {
        try
        {

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

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(hinhanh.FileName);

            string extension = Path.GetExtension(hinhanh.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/img/", fileName);
            string stringUrl = Path.Combine("img/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await hinhanh.CopyToAsync(fileStream);
            }
            HinhAnh hinhanhsp = new HinhAnh();
            hinhanhsp.IdSP = sanpham.Id;
            hinhanhsp.HinhAnhSP = stringUrl;

            await _context.HinhAnhs.AddAsync(hinhanhsp);
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
        }
        // Trả về view với thông báo lỗi và giữ lại các giá trị đã nhập
        return BadRequest(403);
    }
    [HttpPost]
    public async Task<IActionResult> XoaSanPham(int Id)
    {
        try
        {

            //xoá sản phẩm 
            var SanPhamRemove = _context.SanPhams.SingleOrDefault(sp => sp.Id == Id);
            if (SanPhamRemove != null)
            {
                _context.SanPhams.Remove(SanPhamRemove);
                _context.SaveChangesAsync();
            }
            //xoá hình ảnh
            HinhAnh anhD = _context.HinhAnhs.First(ha => ha.IdSP == Id);
            _context.HinhAnhs.Where(ha => ha.IdSP == Id).ExecuteDelete();
            _context.SaveChangesAsync();



            //xoá hình ảnh sản phẩm
            await _context.SaveChangesAsync();
            return RedirectToAction("DanhSachSanPham", "SanPham");
        }
        catch (System.Exception)
        {
            throw;
        }
        // Trả về view với thông báo lỗi và giữ lại các giá trị đã nhập
        return BadRequest(403);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [HttpGet]
    public async Task<IActionResult> EditSP(int id)
    {
        var ChiTietSP = await _context.ChiTietSanPhams.FirstOrDefaultAsync(c => c.IdSP == id);
        var SanPham = await _context.SanPhams.FindAsync(id);
        ViewBag.ListLop = await _context.Lops.ToListAsync();
        ViewBag.ListThuongHieu = await _context.ThuongHieus.ToListAsync();
        ViewBag.ListTheLoai = await _context.TheLoais.ToListAsync();
        ViewBag.danhMucSanPhams = await _context.DanhMucSanPhams.ToListAsync();
        if (ChiTietSP == null || SanPham == null)
        {
            return NotFound();
        }

        ViewBag.ChiTietSanPham = ChiTietSP;
        return View(SanPham);
    }

    [HttpPost]
    public async Task<IActionResult> EditSP(SanPham updatedSanPhams, ChiTietSanPham ctsp)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingSanPham = await _context.SanPhams.FindAsync(updatedSanPhams.Id);
                var existingCTSP = await _context.ChiTietSanPhams.FirstOrDefaultAsync(r => r.IdSP == existingSanPham.Id);

                if (existingSanPham == null || existingCTSP == null)
                {
                    return NotFound();
                }

                existingSanPham.MaSP = updatedSanPhams.MaSP;
                existingSanPham.TenSP = updatedSanPhams.TenSP;
                existingSanPham.MoTaSP = updatedSanPhams.MoTaSP;
                existingSanPham.GiaBan = updatedSanPhams.GiaBan;
                existingSanPham.GiaNhap = updatedSanPhams.GiaNhap;
                existingSanPham.SoLuongNhap = updatedSanPhams.SoLuongNhap;
                existingSanPham.SoLuongTon = updatedSanPhams.SoLuongTon;
                existingSanPham.XuatSu = updatedSanPhams.XuatSu;

                existingCTSP.IdLop = ctsp.IdLop;
                existingCTSP.IdTheLoai = ctsp.IdTheLoai;
                existingCTSP.IdThuongHieu = ctsp.IdThuongHieu;


                await _context.SaveChangesAsync();
                return RedirectToAction("DanhSachSanPham", "SanPham");
            }
            catch (Exception)
            {

                return BadRequest("Lỗi");

            }
        }

        return RedirectToAction("DanhSachSanPham", "SanPham");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var ChiTietSP = await _context.ChiTietSanPhams.FirstOrDefaultAsync(c => c.IdSP == id);
        var SanPham = await _context.SanPhams.FindAsync(id);
        ViewBag.ListLop = await _context.Lops.ToListAsync();
        ViewBag.ListThuongHieu = await _context.ThuongHieus.ToListAsync();
        ViewBag.ListTheLoai = await _context.TheLoais.ToListAsync();
        ViewBag.danhMucSanPhams = await _context.DanhMucSanPhams.ToListAsync();
        if (ChiTietSP == null || SanPham == null)
        {
            return NotFound();
        }

        ViewBag.ChiTietSanPham = ChiTietSP;
        return View(SanPham);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sanpham = await _context.SanPhams.FindAsync(id);
        var ChiTietSP = await _context.ChiTietSanPhams.FirstOrDefaultAsync(c => c.IdSP == id);
        if (sanpham == null)
        {
            return NotFound();
        }

        _context.SanPhams.Remove(sanpham);
        _context.ChiTietSanPhams.Remove(ChiTietSP);
        await _context.SaveChangesAsync();

        return RedirectToAction("DanhSachSanPham", "SanPham");
    }
}