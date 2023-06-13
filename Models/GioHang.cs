using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class GioHang
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string? UserId { get; set; } // Thêm trường UserId để liên kết với người dùng
    public int SoLuong { get; set; }
    // Các thuộc tính khác của giỏ hàng
    public int SanPhamId { get; set; }
}