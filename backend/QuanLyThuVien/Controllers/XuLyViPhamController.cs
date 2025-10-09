using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.ViewModels;
using System.Data.Entity;

namespace QuanLyThuVien.Controllers
{
    public class XuLyViPhamController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: XuLyViPham
        public ActionResult Index()
        {
            var danhSachViPham = db.ViPhams
                .Include(vp => vp.DocGia)
                .OrderByDescending(vp => vp.NgayViPham)
                .ToList() // Lấy dữ liệu về bộ nhớ
                .Select(vp => new ViPhamViewModel
                {
                    MaViPhamRaw = vp.MaViPham, // Lưu ID gốc
                    MaViPham = "VP" + vp.MaViPham.ToString("D3"),
                    TenDocGia = vp.DocGia?.HoTen,
                    LoaiViPham = vp.LoaiViPham,
                    SoTienPhat = vp.HinhThucPhat.HasValue ? vp.HinhThucPhat.Value.ToString("N0") + "đ" : "0đ",
                    NgayGhiNhan = vp.NgayViPham,
                    TrangThai = "đã thanh toán".Equals(vp.TrangThaiXuLy?.Trim(), StringComparison.OrdinalIgnoreCase)
                                ? "Đã thanh toán" : "Chưa thanh toán"
                })
                .ToList();

            var viewModel = new XuLyViPhamPageViewModel
            {
                DanhSachViPham = danhSachViPham,
                DanhSachDocGia = db.DocGias
                                   .Where(dg => dg.DaXoa == false)
                                   .Select(dg => new SelectListItem
                                   {
                                       Value = dg.MaDocGia.ToString(),
                                       Text = dg.MaDocGia + " - " + dg.HoTen
                                   }).ToList()
            };
            return View(viewModel);
        }

        // POST: XuLyViPham/Create
        [HttpPost]
        public ActionResult Create(ViPham viPham)
        {
            try
            {
                viPham.NgayViPham = DateTime.Now;
                viPham.TrangThaiXuLy = "Chưa thanh toán";

                db.ViPhams.Add(viPham);
                db.SaveChanges();
                return Json(new { success = true, message = "Lập phiếu phạt thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: XuLyViPham/MarkAsPaid/5
        [HttpPost]
        public ActionResult MarkAsPaid(int id)
        {
            try
            {
                var viPham = db.ViPhams.Find(id);
                if (viPham == null) return Json(new { success = false, message = "Không tìm thấy vi phạm." });

                viPham.TrangThaiXuLy = "đã thanh toán";
                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: XuLyViPham/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var viPham = db.ViPhams.Find(id);
                if (viPham == null) return Json(new { success = false, message = "Không tìm thấy vi phạm." });

                db.ViPhams.Remove(viPham);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa phiếu phạt thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        [HttpGet]
        public ActionResult GetViPhamDetails(int id)
        {
            var viPham = db.ViPhams.Find(id);
            if (viPham == null)
            {
                return HttpNotFound();
            }
            // Trả về dữ liệu cần thiết cho form sửa
            var result = new
            {
                viPham.MaViPham,
                viPham.MaDocGia,
                viPham.LoaiViPham,
                viPham.HinhThucPhat
            };
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        // POST: XuLyViPham/Edit
        [HttpPost]
        public ActionResult Edit(ViPham viPham)
        {
            try
            {
                var existingViPham = db.ViPhams.Find(viPham.MaViPham);
                if (existingViPham == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu phạt." });
                }

                // Cập nhật các trường được phép sửa
                existingViPham.MaDocGia = viPham.MaDocGia;
                existingViPham.LoaiViPham = viPham.LoaiViPham;
                existingViPham.HinhThucPhat = viPham.HinhThucPhat;
                // Ngày vi phạm và Trạng thái thường không được sửa

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật phiếu phạt thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}