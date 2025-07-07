using System.ComponentModel.DataAnnotations;

namespace QuanLyLichKham.Models
{
    public class BacSi
    {
        public int Id { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; }

        [Display(Name = "Chuyên khoa")]
        [Required(ErrorMessage = "Vui lòng nhập chuyên khoa")]
        public string ChuyenKhoa { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public ICollection<LichKham> LichKhams { get; set; }
    }
}