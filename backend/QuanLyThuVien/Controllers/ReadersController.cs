using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    public class ReadersController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // Action Index để hiển thị danh sách (giữ nguyên)
        public ActionResult Index()
        {
            var danhSachDocGia = db.DocGias.OrderBy(dg => dg.HoTen).ToList();
            return View(danhSachDocGia);
        }

        // [HttpPost] Action để TẠO MỚI độc giả
        [HttpPost]
        public ActionResult Create(DocGia docGia, string password_modal) // Thêm tham số password
        {
            // Kiểm tra xem Username hoặc Email đã tồn tại chưa
            if (db.DocGias.Any(d => d.Username == docGia.Username))
            {
                return Json(new { success = false, message = "Username đã tồn tại." });
            }
            if (db.DocGias.Any(d => d.Email == docGia.Email))
            {
                return Json(new { success = false, message = "Email đã được sử dụng." });
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Gán các giá trị mặc định
                    docGia.NgayTaoTaiKhoan = DateTime.Now;
                    docGia.DaXoa = false; // Mặc định là hoạt động

                    // Mã hóa mật khẩu nếu có nhập
                    if (!string.IsNullOrEmpty(password_modal))
                    {
                        docGia.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password_modal);
                    }

                    // Thêm độc giả mới vào DB
                    db.DocGias.Add(docGia);
                    db.SaveChanges(); // Lưu để lấy MaDocGia

                    // Tự động tạo thẻ mượn sách tương ứng
                    var newTheMuon = new TheMuonSach
                    {
                        MaDocGia = docGia.MaDocGia,
                        NgayDangKy = DateTime.Now,
                        NgayHetHan = DateTime.Now.AddYears(4),
                        TinhTrangThe = "Hoạt động"
                    };
                    db.TheMuonSaches.Add(newTheMuon);
                    db.SaveChanges();

                    transaction.Commit();
                    return Json(new { success = true, message = "Thêm độc giả mới thành công!" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
                }
            }
        }

        // Action để lấy thông tin chi tiết của một độc giả (dùng cho việc sửa)
        [HttpGet]
        public ActionResult GetReaderDetails(int id)
        {
            var docGia = db.DocGias
                         .Where(d => d.MaDocGia == id)
                         .Select(d => new {
                             d.MaDocGia,
                             d.HoTen,
                             d.Lop,
                             d.Email,
                             d.Username,
                             d.DaXoa
                         })
                         .FirstOrDefault();

            if (docGia == null)
            {
                return HttpNotFound();
            }
            return Json(new { success = true, data = docGia }, JsonRequestBehavior.AllowGet);
        }


        // [HttpPost] Action để CẬP NHẬT thông tin độc giả
        [HttpPost]
        public ActionResult Edit(DocGia docGia, string password_modal)
        {
            try
            {
                var existingReader = db.DocGias.Find(docGia.MaDocGia);
                if (existingReader == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy độc giả." });
                }

                // Cập nhật thông tin
                existingReader.HoTen = docGia.HoTen;
                existingReader.Lop = docGia.Lop;
                existingReader.Email = docGia.Email;
                // Không cho phép sửa Username
                // existingReader.Username = docGia.Username;
                existingReader.DaXoa = docGia.DaXoa;

                // Cập nhật mật khẩu nếu người dùng có nhập mật khẩu mới
                if (!string.IsNullOrEmpty(password_modal))
                {
                    existingReader.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password_modal);
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thông tin độc giả thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }


        // [HttpPost] Action để XÓA VĨNH VIỄN độc giả
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Bắt đầu một transaction để đảm bảo xóa cả độc giả và thẻ mượn
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Tìm độc giả cần xóa
                    var docGia = db.DocGias.Find(id);
                    if (docGia == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy độc giả." });
                    }

                    // 2. Kiểm tra xem độc giả có đang mượn sách không
                    // Đây là bước kiểm tra an toàn rất quan trọng
                    bool isBorrowing = db.MuonTras.Any(mt => mt.TheMuonSach.MaDocGia == id && mt.NgayTraThucTe == null);
                    if (isBorrowing)
                    {
                        return Json(new { success = false, message = "Không thể xóa độc giả này vì họ đang mượn sách!" });
                    }

                    // 3. Tìm và xóa tất cả thẻ mượn liên quan (nếu có)
                    var theMuonSach = db.TheMuonSaches.Where(tms => tms.MaDocGia == id).ToList();
                    if (theMuonSach.Any())
                    {
                        db.TheMuonSaches.RemoveRange(theMuonSach);
                    }

                    // 4. Xóa độc giả
                    db.DocGias.Remove(docGia);

                    // 5. Lưu tất cả thay đổi vào CSDL
                    db.SaveChanges();

                    // 6. Hoàn tất transaction
                    transaction.Commit();

                    return Json(new { success = true, message = "Xóa độc giả thành công!" });
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi, hủy bỏ mọi thay đổi
                    transaction.Rollback();
                    // Ghi log lỗi để debug (tùy chọn)
                    // Log.Error("Lỗi khi xóa độc giả: " + ex.Message);
                    return Json(new { success = false, message = "Đã xảy ra lỗi trong quá trình xóa." });
                }
            }
        }
    }
}