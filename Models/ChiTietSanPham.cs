using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcWedBanDungCuHocTap.Models
{
    public class ChiTietSanPham
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TheLoai")]
        public int IdTheLoai { get; set; }
        [ForeignKey("ThuongHieu")]
        public int IdThuongHieu { get; set; }

        [ForeignKey("SanPham")]
        public int IdSP { get; set; }
        [ForeignKey("Lop")]
        public int IdLop { get; set; }

    }
}
