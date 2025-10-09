// File: Controllers/TacGiaController.cs

using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    public class TacGiaController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: TacGia - Hiển thị danh sách tác giả
        public ActionResult Index()
        {
            // Chỉ lấy những tác giả chưa bị đánh dấu xóa
            var danhSachTacGia = db.TacGias.Where(tg => tg.DaXoa != true).ToList();
            return View(danhSachTacGia);
        }

        // POST: TacGia/Create - Tạo mới tác giả
        [HttpPost]
        public ActionResult Create(TacGia tacGia)
        {
            try
            {
                if (string.IsNullOrEmpty(tacGia.TenTacGia))
                {
                    return Json(new { success = false, message = "Tên tác giả không được để trống." });
                }

                tacGia.DaXoa = false;
                db.TacGias.Add(tacGia);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm tác giả thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: TacGia/GetTacGiaDetails/5 - Lấy thông tin chi tiết một tác giả
        [HttpGet]
        public ActionResult GetTacGiaDetails(int id)
        {
            var tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }
            // Trả về dữ liệu cần thiết
            var result = new
            {
                tacGia.MaTacGia,
                tacGia.TenTacGia,
                tacGia.TieuSu // Giả sử "Thông tin liên hệ" là TieuSu
            };
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        // POST: TacGia/Edit - Cập nhật thông tin tác giả
        [HttpPost]
        public ActionResult Edit(TacGia tacGia)
        {
            try
            {
                var existingTacGia = db.TacGias.Find(tacGia.MaTacGia);
                if (existingTacGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tác giả." });
                }

                existingTacGia.TenTacGia = tacGia.TenTacGia;
                existingTacGia.TieuSu = tacGia.TieuSu; // Giả sử "Thông tin liên hệ" là TieuSu

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: TacGia/Delete/5 - Xóa tác giả
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // Kiểm tra an toàn: Không cho xóa nếu tác giả này đã có sách
                bool hasBooks = db.Saches.Any(s => s.MaTacGia == id && s.DaXoa != true);
                if (hasBooks)
                {
                    return Json(new { success = false, message = "Không thể xóa tác giả này vì đã có sách của họ." });
                }

                var tacGia = db.TacGias.Find(id);
                if (tacGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tác giả." });
                }

                // Thực hiện xóa mềm
                tacGia.DaXoa = true;
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa tác giả thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
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