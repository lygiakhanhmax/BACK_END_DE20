using System;
using Microsoft.EntityFrameworkCore;

namespace QuanLyLichKham.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<BacSi> BacSis { get; set; }
        public DbSet<LichKham> LichKhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng BenhNhan
            modelBuilder.Entity<BenhNhan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TaiKhoan).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MatKhau).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SoDienThoai).IsRequired().HasMaxLength(15);
                entity.Property(e => e.DiaChiEmail).IsRequired().HasMaxLength(255);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
            });

            // Cấu hình bảng BacSi
            modelBuilder.Entity<BacSi>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HoTen).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ChuyenKhoa).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SoDienThoai).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            });

            // Cấu hình bảng LichKham và quan hệ
            modelBuilder.Entity<LichKham>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.BenhNhan)
                      .WithMany(e => e.LichKhams)
                      .HasForeignKey(e => e.BenhNhanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.BacSi)
                      .WithMany(e => e.LichKhams)
                      .HasForeignKey(e => e.BacSiId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed dữ liệu mẫu
            modelBuilder.Entity<BenhNhan>().HasData(
                new BenhNhan { Id = 1, TaiKhoan = "Nguyễn Văn A", MatKhau = "1234@aA", SoDienThoai = "0336382611", DiaChiEmail = "anv@gmail.com", NgayTao = DateTime.Now },
                new BenhNhan { Id = 2, TaiKhoan = "Nguyễn Văn B", MatKhau = "4321@aA", SoDienThoai = "0336382612", DiaChiEmail = "bnv@gmail.com", NgayTao = DateTime.Now }
            );

            modelBuilder.Entity<BacSi>().HasData(
                new BacSi { Id = 1, HoTen = "BS. Nguyễn Văn Hùng", ChuyenKhoa = "Nội khoa", SoDienThoai = "0901234567", Email = "hungnv@benhvien.com" },
                new BacSi { Id = 2, HoTen = "BS. Trần Thị Lan", ChuyenKhoa = "Nhi khoa", SoDienThoai = "0901234568", Email = "lantt@benhvien.com" }
            );
        }
    }
}
