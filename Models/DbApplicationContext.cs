using Microsoft.EntityFrameworkCore;

namespace MvcWedBanDungCuHocTap.Models
{
    public class DbApplicationContext : DbContext
    {
        public DbApplicationContext(DbContextOptions options) : base(options) { }

        public DbSet<HinhAnh>? HinhAnhs { get; set; }
        public DbSet<SanPham>? SanPhams { get; set; }
        public DbSet<DanhMucSanPham>? DanhMucSanPhams { get; set; }
    }
}