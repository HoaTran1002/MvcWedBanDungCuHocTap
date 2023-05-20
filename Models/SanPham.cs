using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcWedBanDungCuHocTap.Models
{

    public class SanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? MaSP { get; set; }
        public string? TenSP { get; set; }
        public string? MoTaSP { get; set; }
        public string? XuatSu { get; set; }
        public int SoLuongNhap { get; set; }
        public int SoLuongTon { get; set; }
        public decimal? GiaNhap { get; set; }
        public decimal? GiaBan { get; set; }
        public DateTime NgayNhap { get; set; }
        public DateTime NgayBan { get; set; }
    }
}
