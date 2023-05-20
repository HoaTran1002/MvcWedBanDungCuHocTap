using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcWedBanDungCuHocTap.Models
{
    public class Lop
    {
        [Key]
        public int Id { get; set; }
        public string? DoiTuongSP { get; set; }
    }
}