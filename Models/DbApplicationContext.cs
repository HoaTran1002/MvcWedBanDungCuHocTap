using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MvcWedBanDungCuHocTap.Models
{
    public class DbApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public DbApplicationContext(DbContextOptions options) : base(options) { }

        public DbSet<HinhAnh>? HinhAnhs { get; set; }
        public DbSet<SanPham>? SanPhams { get; set; }
        public DbSet<DanhMucSanPham>? DanhMucSanPhams { get; set; }
        public DbSet<ChiTietSanPham>? ChiTietSanPhams { get; set; }
        public DbSet<Lop>? Lops { get; set; }
        public DbSet<TheLoai>? TheLoais { get; set; }
        public DbSet<ThuongHieu>? ThuongHieus { get; set; }
        public DbSet<GioHang>? GioHangs { get; set; }

    }
}