using System.ComponentModel.DataAnnotations;

namespace QuanLyLichKham.Models
{
    public class LichKham
    {
        public int Id { get; set; }

        [Display(Name = "Bệnh nhân")]
        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        public int BenhNhanId { get; set; }

        [Display(Name = "Bác sĩ")]
        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int BacSiId { get; set; }

        [Display(Name = "Ngày khám")]
        [Required(ErrorMessage = "Vui lòng chọn ngày khám")]
        [DataType(DataType.Date)]
        public DateTime NgayKham { get; set; }

        [Display(Name = "Giờ khám")]
        [Required(ErrorMessage = "Vui lòng chọn giờ khám")]
        [DataType(DataType.Time)]
        public TimeSpan GioKham { get; set; }

        [Display(Name = "Triệu chứng")]
        [StringLength(1000, ErrorMessage = "Triệu chứng không được vượt quá 1000 ký tự")]
        public string? TrieuChung { get; set; }

        [Display(Name = "Trạng thái")]
        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string TrangThai { get; set; } = "Chờ khám";

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        public BenhNhan? BenhNhan { get; set; }
        public BacSi? BacSi { get; set; }
    }
}