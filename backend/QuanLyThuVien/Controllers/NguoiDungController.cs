// File: Controllers/NguoiDungController.cs

using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: NguoiDung - Hiển thị danh sách
        public ActionResult Index()
        {
            var danhSachNguoiDung = db.ThuThus.ToList();
            return View(danhSachNguoiDung);
        }

        // POST: NguoiDung/Create - Tạo người dùng mới
        [HttpPost]
        public ActionResult Create(ThuThu user, string password_user) // password_user khớp với name trong form
        {
            try
            {
                // Kiểm tra dữ liệu trùng lặp
                if (db.ThuThus.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    return Json(new { success = false, message = "Tên đăng nhập đã tồn tại." });
                }
                if (string.IsNullOrEmpty(password_user))
                {
                    return Json(new { success = false, message = "Mật khẩu không được để trống khi tạo mới." });
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password_user);
                user.DaXoa = false; // Mặc định là Hoạt động

                db.ThuThus.Add(user);
                db.SaveChanges();

                return Json(new { success = true, message = "Thêm người dùng mới thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // GET: NguoiDung/GetUserDetails/5 - Lấy thông tin chi tiết một người dùng
        [HttpGet]
        public ActionResult GetUserDetails(int id)
        {
            var user = db.ThuThus.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            // Trả về dữ liệu cần thiết, không bao gồm PasswordHash
            var result = new
            {
                user.MaThuThu,
                user.Username,
                user.HoTen,
                user.Email,
                user.IsAdmin,
                user.DaXoa
            };
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        // POST: NguoiDung/Edit - Cập nhật thông tin người dùng
        [HttpPost]
        public ActionResult Edit(ThuThu user, string password_user)
        {
            try
            {
                var existingUser = db.ThuThus.Find(user.MaThuThu);
                if (existingUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                }

                // Cập nhật thông tin
                existingUser.HoTen = user.HoTen;
                existingUser.Email = user.Email;
                existingUser.IsAdmin = user.IsAdmin;
                existingUser.DaXoa = user.DaXoa;

                // Chỉ cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
                if (!string.IsNullOrEmpty(password_user))
                {
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password_user);
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thông tin người dùng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // POST: NguoiDung/Delete/5 - Xóa (mềm) người dùng
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var user = db.ThuThus.Find(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                }

                // Biện pháp an toàn: không cho phép xóa tài khoản admin
                if (user.IsAdmin == true)
                {
                    return Json(new { success = false, message = "Không thể xóa tài khoản có vai trò Admin." });
                }

                user.DaXoa = true; // Thực hiện xóa mềm
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa người dùng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}