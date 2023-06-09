using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcWedBanDungCuHocTap.Models
{
    public class HinhAnh
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("SanPham")]
        public int IdSP { get; set; }
        public string? HinhAnhSP { get; set; }
    }
}