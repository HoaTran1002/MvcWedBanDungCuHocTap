using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcWedBanDungCuHocTap.Models
{
    public class TheLoai
    {
        [Key]
        public int Id { get; set; }
        public string? TenTheLoai { get; set; }
    }
}