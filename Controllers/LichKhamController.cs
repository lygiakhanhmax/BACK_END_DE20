using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyLichKham.Models;

namespace QuanLyLichKham.Controllers
{
    public class LichKhamController : Controller
    {
        private readonly DatabaseContext _context;

        public LichKhamController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: LichKham
        public async Task<IActionResult> Index()
        {
            var lichKhams = await _context.LichKhams
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .OrderByDescending(l => l.NgayTao)
                .ToListAsync();
            return View(lichKhams);
        }

        // GET: LichKham/Create
        public async Task<IActionResult> Create()
        {
            ViewData["BenhNhanId"] = new SelectList(await _context.BenhNhans.ToListAsync(), "Id", "TaiKhoan");
            ViewData["BacSiId"] = new SelectList(await _context.BacSis.ToListAsync(), "Id", "HoTen");
            return View();
        }

        // POST: LichKham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichKham lichKham)
        {
            try
            {
                // Loại bỏ validation cho các trường không cần thiết
                ModelState.Remove("Id");
                ModelState.Remove("NgayTao");
                ModelState.Remove("TrangThai");
                ModelState.Remove("BenhNhan");
                ModelState.Remove("BacSi");

                if (ModelState.IsValid)
                {
                    // Kiểm tra xem bệnh nhân và bác sĩ có tồn tại không
                    var benhNhanExists = await _context.BenhNhans.AnyAsync(b => b.Id == lichKham.BenhNhanId);
                    var bacSiExists = await _context.BacSis.AnyAsync(b => b.Id == lichKham.BacSiId);

                    if (!benhNhanExists)
                    {
                        ModelState.AddModelError("BenhNhanId", "Bệnh nhân được chọn không tồn tại!");
                    }

                    if (!bacSiExists)
                    {
                        ModelState.AddModelError("BacSiId", "Bác sĩ được chọn không tồn tại!");
                    }

                    // Kiểm tra ngày khám không được trong quá khứ
                    if (lichKham.NgayKham.Date < DateTime.Now.Date)
                    {
                        ModelState.AddModelError("NgayKham", "Ngày khám không được trong quá khứ!");
                    }

                    // Kiểm tra trùng lịch khám
                    var lichKhamTrung = await _context.LichKhams
                        .AnyAsync(l => l.BacSiId == lichKham.BacSiId &&
                                      l.NgayKham.Date == lichKham.NgayKham.Date &&
                                      l.GioKham == lichKham.GioKham &&
                                      l.TrangThai != "Đã hủy");

                    if (lichKhamTrung)
                    {
                        ModelState.AddModelError("GioKham", "Bác sĩ đã có lịch khám vào thời gian này!");
                    }

                    if (ModelState.IsValid)
                    {
                        lichKham.TrangThai = "Chờ khám";
                        lichKham.NgayTao = DateTime.Now;
                        lichKham.Id = 0; // Đảm bảo EF tự generate ID

                        _context.LichKhams.Add(lichKham);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Tạo lịch khám thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    // Debug: In ra các lỗi validation
                    foreach (var modelError in ModelState)
                    {
                        if (modelError.Value.Errors.Count > 0)
                        {
                            foreach (var error in modelError.Value.Errors)
                            {
                                Console.WriteLine($"Field: {modelError.Key}, Error: {error.ErrorMessage}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            }

            // Load lại data cho dropdown nếu có lỗi
            ViewData["BenhNhanId"] = new SelectList(await _context.BenhNhans.ToListAsync(), "Id", "TaiKhoan", lichKham.BenhNhanId);
            ViewData["BacSiId"] = new SelectList(await _context.BacSis.ToListAsync(), "Id", "HoTen", lichKham.BacSiId);
            return View(lichKham);
        }

        // GET: LichKham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichKham = await _context.LichKhams.FindAsync(id);
            if (lichKham == null)
            {
                return NotFound();
            }
            ViewData["BenhNhanId"] = new SelectList(await _context.BenhNhans.ToListAsync(), "Id", "TaiKhoan", lichKham.BenhNhanId);
            ViewData["BacSiId"] = new SelectList(await _context.BacSis.ToListAsync(), "Id", "HoTen", lichKham.BacSiId);
            return View(lichKham);
        }

        // POST: LichKham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LichKham lichKham)
        {
            if (id != lichKham.Id)
            {
                return NotFound();
            }

            try
            {
                // Loại bỏ validation cho navigation properties
                ModelState.Remove("BenhNhan");
                ModelState.Remove("BacSi");

                if (ModelState.IsValid)
                {
                    // Kiểm tra xem bệnh nhân và bác sĩ có tồn tại không
                    var benhNhanExists = await _context.BenhNhans.AnyAsync(b => b.Id == lichKham.BenhNhanId);
                    var bacSiExists = await _context.BacSis.AnyAsync(b => b.Id == lichKham.BacSiId);

                    if (!benhNhanExists)
                    {
                        ModelState.AddModelError("BenhNhanId", "Bệnh nhân được chọn không tồn tại!");
                    }

                    if (!bacSiExists)
                    {
                        ModelState.AddModelError("BacSiId", "Bác sĩ được chọn không tồn tại!");
                    }

                    // Kiểm tra trùng lịch khám (trừ bản ghi hiện tại)
                    var lichKhamTrung = await _context.LichKhams
                        .AnyAsync(l => l.BacSiId == lichKham.BacSiId &&
                                      l.NgayKham.Date == lichKham.NgayKham.Date &&
                                      l.GioKham == lichKham.GioKham &&
                                      l.TrangThai != "Đã hủy" &&
                                      l.Id != id);

                    if (lichKhamTrung)
                    {
                        ModelState.AddModelError("GioKham", "Bác sĩ đã có lịch khám vào thời gian này!");
                    }

                    if (ModelState.IsValid)
                    {
                        _context.Update(lichKham);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Cập nhật lịch khám thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LichKhamExists(lichKham.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            ViewData["BenhNhanId"] = new SelectList(await _context.BenhNhans.ToListAsync(), "Id", "TaiKhoan", lichKham.BenhNhanId);
            ViewData["BacSiId"] = new SelectList(await _context.BacSis.ToListAsync(), "Id", "HoTen", lichKham.BacSiId);
            return View(lichKham);
        }

        // GET: LichKham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichKham = await _context.LichKhams
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lichKham == null)
            {
                return NotFound();
            }

            return View(lichKham);
        }

        // POST: LichKham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var lichKham = await _context.LichKhams.FindAsync(id);
                if (lichKham != null)
                {
                    _context.LichKhams.Remove(lichKham);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Xóa lịch khám thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LichKhamExists(int id)
        {
            return _context.LichKhams.Any(e => e.Id == id);
        }
    }
}