using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcWedBanDungCuHocTap.Models
{
    public class DanhMucSanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? TenDM { get; set; }
        public string? MoTaDM { get; set; }
        public int? SoLuongLoaiSP { get; set; }
        [ForeignKey("SanPham")]
        public int IdSP { get; set; }
    }
}
