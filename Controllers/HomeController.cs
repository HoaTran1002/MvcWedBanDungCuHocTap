using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcWedBanDungCuHocTap.Models;
using MvcWedBanSach.Models;

namespace MvcWedBanSach.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DbApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private static List<DanhMucSanPham> danhMucSanPhams;


    private List<SanPhamHinhAnh> sanphamsearchpaging;


    public HomeController(ILogger<HomeController> logger, DbApplicationContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        danhMucSanPhams = _context.DanhMucSanPhams.ToList<DanhMucSanPham>();


    }

    public async Task<IActionResult> Index()

    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        var query = (from a in _context.SanPhams
                     join b in _context.HinhAnhs on a.Id equals b.IdSP
                     select new { SanPham = a, HinhAnh = b }).ToList();
        ViewBag.ListSp = query;
        var queryDM = (from danhMuc in _context.DanhMucSanPhams
                       join sanPham in _context.SanPhams on danhMuc.Id equals sanPham.IdDanhMucSanPham into sanPhamGroup
                       select new
                       {
                           MaDanhMuc = danhMuc.Id,
                           TenDanhMuc = danhMuc.TenDM,
                           SoLuongSanPham = sanPhamGroup.Count()
                       }).ToList();
        ViewBag.listDM = queryDM;



        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }



        return View();
    }
    [Authorize]
    public async Task<IActionResult> Cart()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        var currentUser = await _userManager.GetUserAsync(User);
        // ViewBag.user = currentUser.UserName;

        // Lấy danh sách sản phẩm trong giỏ hàng của người dùng hiện tại
        var cartProducts = _context.GioHangs
            .Where(giohang => giohang.UserId == currentUser.Id)
            .ToList();

        List<CartProductDTO> cartList = new List<CartProductDTO>();

        // Duyệt qua danh sách sản phẩm trong giỏ hàng và thực hiện join với bảng SanPham
        foreach (var cartProduct in cartProducts)
        {
            // Lấy thông tin sản phẩm từ bảng SanPham
            var product = _context.SanPhams.FirstOrDefault(p => p.Id == cartProduct.SanPhamId);

            if (product != null)
            {
                // Tạo đối tượng CartProductDTO và gán các thông tin cần thiết
                var cartProductDTO = new CartProductDTO
                {
                    SanPhamId = cartProduct.SanPhamId,
                    SoLuong = cartProduct.SoLuong,
                    TenSP = product.TenSP,
                    GiaBan = product.GiaBan,
                    TongTien = product.GiaBan * cartProduct.SoLuong
                };

                cartList.Add(cartProductDTO);
            }
        }
        ViewBag.carts = cartList;
        // Tính toán tổng tiền
        decimal tongTien = cartList.Sum(item => item.TongTien) ?? 0;
        ViewBag.TongTien = tongTien;

        // Tính toán thanh toán tổng (ví dụ)
        decimal phiVanChuyen = 10000; // Giả sử phí vận chuyển là 10,000 đ
        if (tongTien <= 0)
        {
            phiVanChuyen = 0;
        }
        decimal thanhToanTong = tongTien + phiVanChuyen;
        ViewBag.ThanhToanTong = thanhToanTong;
        ViewBag.VanChuyen = phiVanChuyen;




        ViewBag.NumberCart = cartProducts.Count - 1;


        ViewBag.NumberCart = cartProducts.Count - 1;

        return View(cartList);
    }

    [Authorize]
    public async Task<IActionResult> AddCart(int id)
    {
        // Lấy thông tin người dùng hiện tại
        ApplicationUser user = await _userManager.GetUserAsync(User);
        // Kiểm tra xem id sản phẩm đã có trong giỏ hàng của người dùng chưa
        var existingCartItem = _context.GioHangs.FirstOrDefault(giohang => giohang.UserId == user.Id && giohang.SanPhamId == id);

        if (existingCartItem != null)
        {
            // Đã có id sản phẩm trong giỏ hàng, chuyển hướng sang trang giỏ hàng
            return RedirectToAction("Cart");
        }
        if (user != null)
        {
            // Thêm sản phẩm vào giỏ hàng
            GioHang giohang = new GioHang();
            giohang.SanPhamId = id;
            giohang.UserId = user.Id; // UserId của người dùng hiện tại
            giohang.SoLuong = 1;

            await _context.GioHangs.AddAsync(giohang);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Cart");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteCart(int? SanPhamId)
    {


        // Lấy thông tin người dùng hiện tại
        var currentUser = await _userManager.GetUserAsync(User);

        // Lấy sản phẩm trong giỏ hàng của người dùng có productId nhận được
        var cartProduct = _context.GioHangs.FirstOrDefault(giohang => giohang.UserId == currentUser.Id && giohang.SanPhamId == SanPhamId);

        if (cartProduct != null)
        {
            // Xóa sản phẩm khỏi giỏ hàng
            _context.GioHangs.Remove(cartProduct);
            _context.SaveChanges();
        }

        // Chuyển hướng đến trang giỏ hàng
        return RedirectToAction("Cart");
    }

    [Authorize]


    public async Task<IActionResult> DecreaCart(int SanPhamId)
    {
        SanPhamId++;
        // Lấy thông tin người dùng hiện tại
        ApplicationUser user = await _userManager.GetUserAsync(User);

        // Truy xuất giỏ hàng của người dùng từ nguồn dữ liệu (ví dụ: cơ sở dữ liệu)
        var cart = _context.GioHangs.SingleOrDefault(c => c.UserId == user.Id && c.SanPhamId == SanPhamId);

        // Kiểm tra xem sản phẩm có nằm trong giỏ hàng của người dùng hay không
        if (cart != null)
        {
            // Giảm số lượng sản phẩm trong giỏ hàng
            if (cart.SoLuong > 1)
            {
                cart.SoLuong--;
                _context.SaveChanges();
            }
            else
            {
                // Nếu số lượng sản phẩm là 1, xóa sản phẩm khỏi giỏ hàng
                _context.GioHangs.Remove(cart);
                _context.SaveChanges();
            }
        }

        // Chuyển hướng về trang giỏ hàng
        return RedirectToAction("Cart");
    }

    [Authorize]

    public async Task<IActionResult> IncreaCart(int SanPhamId)
    {
        SanPhamId++;

        // Lấy thông tin người dùng hiện tại
        ApplicationUser user = await _userManager.GetUserAsync(User);

        ViewBag.user = user.UserName;
        // Truy xuất giỏ hàng của người dùng từ nguồn dữ liệu (ví dụ: cơ sở dữ liệu)
        var cart = _context.GioHangs.SingleOrDefault(c => c.UserId == user.Id && c.SanPhamId == SanPhamId);

        // Kiểm tra xem sản phẩm có nằm trong giỏ hàng của người dùng hay không
        if (cart != null)
        {
            // Tăng số lượng sản phẩm trong giỏ hàng
            cart.SoLuong++;
            _context.SaveChanges();
        }

        // Chuyển hướng về trang giỏ hàng
        return RedirectToAction("Cart", "Home");
    }


    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }

        return View();
    }
    public async Task<IActionResult> Contact()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Search(string keysearch, int p)

    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        int ItemInPage = 8;
        int page = p >= 1 ? p : 1;

        // int page = 1;
        int productInPage = (page - 1) * ItemInPage;

        var products = (from a in _context.SanPhams
                        join b in _context.HinhAnhs on a.Id equals b.IdSP
                        select new { SanPham = a, HinhAnh = b })
                        .Where(p => p.SanPham.TenSP.Contains(keysearch))
                        .Skip(productInPage).Take(ItemInPage)
                        .ToList();

        var productsItem = (from a in _context.SanPhams
                            join b in _context.HinhAnhs on a.Id equals b.IdSP
                            select new { SanPham = a, HinhAnh = b })
                        .Where(p => p.SanPham.TenSP.Contains(keysearch))
                        .ToList().Count;
        var TotlaProducts = products.Count;
        int TotalPage = (int)Math.Ceiling((double)productsItem / ItemInPage);


        ViewBag.CurrentPage = page;
        ViewBag.ItemInPage = ItemInPage;
        ViewBag.numberPage = TotalPage;
        ViewBag.keysearch = keysearch;
        ViewBag.ListSp = products;



        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }
        return View();
    }



    [HttpGet]
    public async Task<IActionResult> ListProductType(int id, int p)

    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        int ItemInPage = 8;
        int page = p >= 1 ? p : 1;

        int productInPage = (page - 1) * ItemInPage;


        var products = (from a in _context.SanPhams
                        join b in _context.HinhAnhs on a.Id equals b.IdSP
                        select new { SanPham = a, HinhAnh = b })
                        .Where(p => p.SanPham.IdDanhMucSanPham == id)
                        .Skip(productInPage).Take(ItemInPage)
                        .ToList();

        var productsItem = (from a in _context.SanPhams
                            join b in _context.HinhAnhs on a.Id equals b.IdSP
                            select new { SanPham = a, HinhAnh = b })
                        .Where(p => p.SanPham.IdDanhMucSanPham == id)
                        .ToList().Count;
        int TotalPage = (int)Math.Ceiling((double)productsItem / ItemInPage);
        ViewBag.IdType = id;

        var TotlaProducts = products.Count;

        ViewBag.CurrentPage = page;
        ViewBag.ItemInPage = ItemInPage;
        ViewBag.numberPage = TotalPage;
        ViewBag.ListSp = products;

        ViewBag.TotlaProducts = productsItem;






        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }

        return View();
    }
    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var query = (from a in _context.SanPhams
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





        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }
        return View();
    }
    public async Task<IActionResult> Shop()
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        var products = (from a in _context.SanPhams
                        join b in _context.HinhAnhs on a.Id equals b.IdSP
                        select new { SanPham = a, HinhAnh = b })
            .ToList();
        ViewBag.ListSp = products;


        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ShopSearch(string keysearch)
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        var products = (from a in _context.SanPhams
                        join b in _context.HinhAnhs on a.Id equals b.IdSP
                        select new { SanPham = a, HinhAnh = b })
            .Where(p => p.SanPham.TenSP.Contains(keysearch))
            .ToList();
        ViewBag.ListSp = products;
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }

        return View("Shop");
    }

    [HttpGet]
    public async Task<IActionResult> Shop(int p)
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        var TotlaProducts = _context.SanPhams.ToList().Count;
        int ItemInPage = 6;
        int page = p >= 1 ? p : 1;

        int TotalPage = (int)Math.Ceiling((double)TotlaProducts / ItemInPage);
        int productInPage = (page - 1) * ItemInPage;


        var products = (from a in _context.SanPhams
                        join b in _context.HinhAnhs on a.Id equals b.IdSP
                        select new { SanPham = a, HinhAnh = b })
                        .Skip(productInPage).Take(ItemInPage)
                        .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.ItemInPage = ItemInPage;
        ViewBag.numberPage = TotalPage;
        ViewBag.ListSp = products;


        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            ViewBag.NumberCart = 0;
        }
        else
        {

            var cartProducts = _context.GioHangs
                .Where(giohang => giohang.UserId == currentUser.Id)
                .ToList().Count;
            if (cartProducts == 0)
            {
                cartProducts = 1;
            }
            ViewBag.NumberCart = cartProducts - 1;
        }
        return View("Shop");
    }
    [HttpPost]
    public async Task<IActionResult> ShopFilter(string priceFilter)
    {
        ViewBag.ListdanhMucSanPhams = danhMucSanPhams;
        decimal price1, price2;

        switch (priceFilter)
        {
            case "1":
                price1 = 0;
                price2 = 50000;
                break;
            case "2":
                price1 = 50000;
                price2 = 200000;
                break;
            case "3":
                price1 = 200000;
                price2 = 500000;
                break;
            case "4":
                price1 = 500000;
                price2 = 1000000000;
                break;
            default:
                price1 = 0;
                price2 = 1000000000;
                break;
        }

        var products = (from a in _context.SanPhams
                        join b in _context.HinhAnhs on a.Id equals b.IdSP
                        select new { SanPham = a, HinhAnh = b })
                        .Where(p => p.SanPham.GiaBan.GetValueOrDefault() >= (decimal)price1 && p.SanPham.GiaBan.GetValueOrDefault() <= (decimal)price2)
                        .ToList();

        ViewBag.ListSp = products;
        var currentUser = await _userManager.GetUserAsync(User);
        var cartProducts = _context.GioHangs
                    .Where(giohang => giohang.UserId == currentUser.Id)
                    .ToList().Count;

        if (cartProducts == 0)
        {
            cartProducts = 1;
        }
        ViewBag.NumberCart = cartProducts - 1;

        return View("Shop");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

