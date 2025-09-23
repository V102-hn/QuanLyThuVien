// Controllers/AccountController.cs
using QuanLyThuVien.Models;
using QuanLyThuVien.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThuVien.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        [HttpPost]
        public JsonResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra xem Username đã tồn tại chưa
                if (db.DocGias.Any(d => d.Username == model.Username))
                {
                    return Json(new { success = false, message = "Tên đăng nhập đã tồn tại." });
                }

                // 2. Tạo đối tượng DocGia mới
                var docGia = new DocGia
                {
                    HoTen = model.HoTen,
                    Username = model.Username,
                    // 3. Băm mật khẩu
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    NgayTaoTaiKhoan = DateTime.Now,
                    DaXoa = false
                };

                // 4. Lưu vào CSDL
                db.DocGias.Add(docGia);
                db.SaveChanges();

                return Json(new { success = true, message = "Đăng ký tài khoản thành công!" });
            }

            // Trả về lỗi nếu dữ liệu không hợp lệ
            string errorMessages = string.Join("\n", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            return Json(new { success = false, message = errorMessages });
        }

        [HttpPost]
        public JsonResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Tìm độc giả bằng Username
                var docGia = db.DocGias.FirstOrDefault(d => d.Username == model.Username && d.DaXoa != true);

                if (docGia != null)
                {
                    // 2. So sánh mật khẩu đã băm với mật khẩu người dùng nhập
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, docGia.PasswordHash);
                    if (isPasswordValid)
                    {
                        // 3. Đăng nhập thành công, lưu thông tin vào Session
                        Session["UserInfo"] = docGia; // Lưu cả đối tượng DocGia
                        Session["Username"] = docGia.Username;
                        Session["UserFullName"] = docGia.HoTen;

                        // Chuyển hướng tới trang quản trị hoặc trang cá nhân
                        // Thay đổi URL nếu cần
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                    }
                }
            }

            // Đăng nhập thất bại
            return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
        }

        public ActionResult Logout()
        {
            Session.Clear(); // Xóa tất cả session
            Session.Abandon();
            return RedirectToAction("Index", "Home"); // Về trang chủ
        }
        // Dùng để tạo hash một cách nhanh chóng, sau khi dùng xong có thể xóa đi
        public ActionResult GenerateHash(string password = "123456")
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            // Trả về chuỗi hash dưới dạng văn bản thuần
            return Content(hashedPassword);
        }
    }
}