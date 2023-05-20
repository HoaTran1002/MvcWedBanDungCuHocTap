using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcWedBanDungCuHocTap.Models
{
    public class ThuongHieu
    {
        [Key]
        public int Id { get; set; }
        public string? TenThuongHieuSP { get; set; }
    }
}