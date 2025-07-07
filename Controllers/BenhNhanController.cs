using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyLichKham.Models;

namespace QuanLyLichKham.Controllers
{
    public class BenhNhanController : Controller
    {
        private readonly DatabaseContext _context;

        public BenhNhanController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: BenhNhan
        public async Task<IActionResult> Index()
        {
            var benhNhans = await _context.BenhNhans.ToListAsync();
            return View(benhNhans);
        }

        // GET: BenhNhan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BenhNhan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BenhNhan benhNhan)
        {
            try
            {
                // Loại bỏ validation cho Id và NgayTao
                ModelState.Remove("Id");
                ModelState.Remove("NgayTao");
                ModelState.Remove("LichKhams");

                if (ModelState.IsValid)
                {
                    // Kiểm tra trùng lặp tài khoản
                    var existingUser = await _context.BenhNhans
                        .FirstOrDefaultAsync(x => x.TaiKhoan == benhNhan.TaiKhoan);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại!");
                        return View(benhNhan);
                    }

                    // Kiểm tra trùng lặp email
                    var existingEmail = await _context.BenhNhans
                        .FirstOrDefaultAsync(x => x.DiaChiEmail == benhNhan.DiaChiEmail);

                    if (existingEmail != null)
                    {
                        ModelState.AddModelError("DiaChiEmail", "Email đã tồn tại!");
                        return View(benhNhan);
                    }

                    // Đặt ngày tạo
                    benhNhan.NgayTao = DateTime.Now;
                    benhNhan.Id = 0; // Đảm bảo Id = 0 để EF tự generate

                    _context.BenhNhans.Add(benhNhan);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm bệnh nhân thành công!";
                    return RedirectToAction(nameof(Index));
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

            return View(benhNhan);
        }

        // GET: BenhNhan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benhNhan = await _context.BenhNhans.FindAsync(id);
            if (benhNhan == null)
            {
                return NotFound();
            }
            return View(benhNhan);
        }

        // POST: BenhNhan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TaiKhoan,MatKhau,SoDienThoai,DiaChiEmail,NgayTao")] BenhNhan benhNhan)
        {
            if (id != benhNhan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng lặp tài khoản (trừ bản ghi hiện tại)
                    var existingUser = await _context.BenhNhans
                        .FirstOrDefaultAsync(x => x.TaiKhoan == benhNhan.TaiKhoan && x.Id != id);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại!");
                        return View(benhNhan);
                    }

                    // Kiểm tra trùng lặp email (trừ bản ghi hiện tại)
                    var existingEmail = await _context.BenhNhans
                        .FirstOrDefaultAsync(x => x.DiaChiEmail == benhNhan.DiaChiEmail && x.Id != id);

                    if (existingEmail != null)
                    {
                        ModelState.AddModelError("DiaChiEmail", "Email đã tồn tại!");
                        return View(benhNhan);
                    }

                    _context.Update(benhNhan);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật bệnh nhân thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenhNhanExists(benhNhan.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(benhNhan);
        }

        // GET: BenhNhan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benhNhan = await _context.BenhNhans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (benhNhan == null)
            {
                return NotFound();
            }

            return View(benhNhan);
        }

        // POST: BenhNhan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var benhNhan = await _context.BenhNhans.FindAsync(id);
                if (benhNhan != null)
                {
                    _context.BenhNhans.Remove(benhNhan);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Xóa bệnh nhân thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BenhNhanExists(int id)
        {
            return _context.BenhNhans.Any(e => e.Id == id);
        }
    }
}