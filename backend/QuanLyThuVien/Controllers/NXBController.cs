using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    public class NXBController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: NXB - Hiển thị danh sách
        public ActionResult Index()
        {
            var danhSachNXB = db.NhaXuatBans.Where(nxb => nxb.DaXoa != true).ToList();
            return View(danhSachNXB);
        }

        // POST: NXB/Create - Tạo mới NXB
        [HttpPost]
        public ActionResult Create(NhaXuatBan nxb)
        {
            try
            {
                if (string.IsNullOrEmpty(nxb.TenNXB))
                {
                    return Json(new { success = false, message = "Tên nhà xuất bản không được để trống." });
                }

                nxb.DaXoa = false;
                db.NhaXuatBans.Add(nxb);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm nhà xuất bản thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: NXB/GetNXBDetails/5 - Lấy thông tin chi tiết một NXB
        [HttpGet]
        public ActionResult GetNXBDetails(int id)
        {
            var nxb = db.NhaXuatBans.Find(id);
            if (nxb == null)
            {
                return HttpNotFound();
            }
            // Chỉ trả về các trường cần thiết để tránh lỗi circular reference
            var result = new
            {
                nxb.MaNXB,
                nxb.TenNXB,
                nxb.DiaChi,
                nxb.SoDienThoai,
                nxb.Email
            };
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        // POST: NXB/Edit - Cập nhật thông tin NXB
        [HttpPost]
        public ActionResult Edit(NhaXuatBan nxb)
        {
            try
            {
                var existingNXB = db.NhaXuatBans.Find(nxb.MaNXB);
                if (existingNXB == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhà xuất bản." });
                }

                existingNXB.TenNXB = nxb.TenNXB;
                existingNXB.DiaChi = nxb.DiaChi;
                existingNXB.SoDienThoai = nxb.SoDienThoai;
                existingNXB.Email = nxb.Email;

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: NXB/Delete/5 - Xóa NXB
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // Kiểm tra an toàn: Không cho xóa nếu NXB này đã có sách
                bool hasBooks = db.Saches.Any(s => s.MaNXB == id && s.DaXoa != true);
                if (hasBooks)
                {
                    return Json(new { success = false, message = "Không thể xóa NXB này vì đã có sách thuộc NXB." });
                }

                var nxb = db.NhaXuatBans.Find(id);
                if (nxb == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhà xuất bản." });
                }

                // Thực hiện xóa mềm
                nxb.DaXoa = true;
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa nhà xuất bản thành công!" });
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