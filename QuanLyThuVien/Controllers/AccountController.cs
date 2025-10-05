// Controllers/AccountController.cs
using QuanLyThuVien.Models;
using QuanLyThuVien.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyThuVien.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại." });
            }
            model.Email = model.Username;

            // Kiểm tra xem Username hoặc Email đã tồn tại chưa
            if (db.DocGias.Any(dg => dg.Username == model.Username))
            {
                return Json(new { success = false, message = "Tên đăng nhập đã tồn tại." });
            }
            if (db.DocGias.Any(dg => dg.Email == model.Email))
            {
                return Json(new { success = false, message = "Email đã được sử dụng." });
            }

            // Bắt đầu transaction để đảm bảo tạo độc giả và thẻ mượn cùng lúc
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // BƯỚC 1: TẠO MỚI ĐỘC GIẢ
                    var newDocGia = new DocGia
                    {
                        HoTen = model.HoTen,
                        Email = model.Email,
                        Username = model.Username,
                        // === THAY ĐỔI DUY NHẤT LÀ Ở ĐÂY ===
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        NgayTaoTaiKhoan = DateTime.Now,
                        DaXoa = false,
                    };

                    db.DocGias.Add(newDocGia);
                    db.SaveChanges();

                    // BƯỚC 2: TỰ ĐỘNG TẠO THẺ MƯỢN SÁCH TƯƠNG ỨNG
                    var newTheMuon = new TheMuonSach
                    {
                        MaDocGia = newDocGia.MaDocGia,
                        NgayDangKy = DateTime.Now,
                        NgayHetHan = DateTime.Now.AddYears(4),
                        TinhTrangThe = "Hoạt động"
                    };

                    db.TheMuonSaches.Add(newTheMuon);
                    db.SaveChanges();

                    transaction.Commit();

                    return Json(new { success = true, message = "Đăng ký tài khoản thành công! Bạn có thể đăng nhập ngay bây giờ." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Lỗi hệ thống, không thể đăng ký tài khoản. " + ex.Message });
                }
            }
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
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
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