using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.ViewModels;
using System.Data.Entity;

namespace QuanLyThuVien.Controllers
{
    public class MuonTraController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: MuonTra
        public ActionResult Index()
        {
            // 1. Lấy danh sách phiếu mượn (sử dụng logic cũ của bạn)
            var danhSachPhieuMuon = db.MuonTras
                .Include(mt => mt.TheMuonSach.DocGia)
                .Where(mt => mt.DaXoa == false)
                .Select(mt => new MuonTraViewModel
                {
                    MaPhieu = mt.MaMuonTra.ToString(),
                    TenDocGia = mt.TheMuonSach.DocGia.HoTen,
                    NgayMuon = mt.NgayMuon,
                    NgayHenTra = mt.NgayHenTra,
                    TrangThai = (mt.NgayTraThucTe != null)
                                ? "Đã trả"
                                : (DateTime.Now > mt.NgayHenTra ? "Quá hạn" : "Đang mượn")
                })
                .OrderByDescending(p => p.NgayMuon)
                .ToList();

            // 2. Tạo ViewModel "bao bọc" để truyền ra View
            var viewModel = new QuanLyMuonTraViewModel
            {
                DanhSachPhieuMuon = danhSachPhieuMuon,

                // 3. Lấy dữ liệu cho các dropdown trong modal
                DanhSachDocGia = db.DocGias
                                   .Where(dg => dg.DaXoa == false)
                                   .Select(dg => new SelectListItem
                                   {
                                       Value = dg.MaDocGia.ToString(),
                                       Text = dg.MaDocGia + " - " + dg.HoTen
                                   }).ToList(),

                DanhSachSach = db.Saches
                                 .Where(s => s.DaXoa == false && s.SoLuongTon > 0)
                                 .Select(s => new SelectListItem
                                 {
                                     Value = s.MaSach.ToString(),
                                     Text = s.MaSach + " - " + s.TenSach
                                 }).ToList()
            };

            return View(viewModel);
        }

        // POST: MuonTra/Create
        [HttpPost]
        public ActionResult Create(CreateMuonTraDto data)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (data == null || data.SachIds == null || !data.SachIds.Any())
                    {
                        return Json(new { success = false, message = "Dữ liệu không hợp lệ hoặc chưa chọn sách." });
                    }

                    var theMuon = db.TheMuonSaches.FirstOrDefault(t => t.MaDocGia == data.MaDocGia);
                    if (theMuon == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy thẻ mượn của độc giả." });
                    }

                    int maThuThu = 1; // Giả sử thủ thư đăng nhập có ID = 1

                    var phieuMuon = new MuonTra
                    {
                        MaTheMuon = theMuon.MaTheMuon,
                        MaThuThu = maThuThu,
                        NgayMuon = data.NgayMuon,
                        NgayHenTra = data.NgayHenTra,
                        TrangThai = "Đang mượn",
                        DaXoa = false
                    };
                    db.MuonTras.Add(phieuMuon);
                    db.SaveChanges();

                    foreach (var sachId in data.SachIds)
                    {
                        var sach = db.Saches.Find(sachId);
                        if (sach == null || sach.SoLuongTon <= 0)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = $"Sách với mã {sachId} không hợp lệ hoặc đã hết." });
                        }

                        var chiTiet = new ChiTietMuonTra
                        {
                            MaMuonTra = phieuMuon.MaMuonTra,
                            MaSach = sachId,
                            SoLuongMuon = 1,
                            GhiChu = data.GhiChu
                        };
                        db.ChiTietMuonTras.Add(chiTiet);

                        sach.SoLuongTon -= 1;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true, message = "Tạo phiếu mượn thành công!" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
                }
            }
        }
        [HttpGet]
        public ActionResult GetMuonTraDetails(int id)
        {
            try
            {
                // Lấy thông tin phiếu mượn chính
                var phieuMuon = db.MuonTras
                    .Where(mt => mt.MaMuonTra == id)
                    .Select(mt => new {
                        mt.MaMuonTra,
                        mt.TheMuonSach.MaDocGia,
                        mt.NgayMuon,
                        mt.NgayHenTra
                    })
                    .FirstOrDefault();

                if (phieuMuon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu mượn." }, JsonRequestBehavior.AllowGet);
                }

                // Lấy danh sách sách đã mượn và ghi chú
                var chiTiet = db.ChiTietMuonTras
                    .Where(ct => ct.MaMuonTra == id)
                    .Select(ct => new {
                        TenSachDisplay = ct.Sach.MaSach + " - " + ct.Sach.TenSach,
                        ct.GhiChu
                    })
                    .ToList();

                var ghiChuChung = chiTiet.FirstOrDefault()?.GhiChu;

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        phieuMuon = phieuMuon,
                        chiTietSach = chiTiet,
                        ghiChu = ghiChuChung
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult MarkAsReturned(int id)
        {
            try
            {
                var phieuMuon = db.MuonTras.Find(id);
                if (phieuMuon == null || phieuMuon.NgayTraThucTe != null)
                {
                    return Json(new { success = false, message = "Phiếu mượn không hợp lệ hoặc đã được trả." });
                }

                phieuMuon.NgayTraThucTe = DateTime.Now;
                phieuMuon.TrangThai = "Đã trả";

                // Hoàn trả lại số lượng sách vào kho
                var chiTietMuon = db.ChiTietMuonTras.Where(ct => ct.MaMuonTra == id);
                foreach (var item in chiTietMuon)
                {
                    var sach = db.Saches.Find(item.MaSach);
                    if (sach != null) sach.SoLuongTon += item.SoLuongMuon;
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Xác nhận trả sách thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var phieuMuon = db.MuonTras.Find(id);
                    if (phieuMuon == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy phiếu mượn." });
                    }

                    var chiTietMuonList = db.ChiTietMuonTras.Where(ct => ct.MaMuonTra == id).ToList();

                    // Chỉ hoàn sách nếu phiếu chưa được trả
                    if (phieuMuon.NgayTraThucTe == null)
                    {
                        foreach (var item in chiTietMuonList)
                        {
                            var sach = db.Saches.Find(item.MaSach);
                            if (sach != null) sach.SoLuongTon += item.SoLuongMuon;
                        }
                    }

                    db.ChiTietMuonTras.RemoveRange(chiTietMuonList); // Xóa chi tiết mượn
                    db.MuonTras.Remove(phieuMuon); // Xóa phiếu mượn

                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true, message = "Xóa phiếu mượn thành công!" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(EditMuonTraDto data)
        {
            // Kiểm tra dữ liệu đầu vào cơ bản
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                var phieuMuon = db.MuonTras.Find(data.MaMuonTra);
                if (phieuMuon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu mượn." });
                }

                // Cập nhật các trường được phép sửa
                phieuMuon.NgayHenTra = data.NgayHenTra;

                // Cập nhật Ghi Chú cho tất cả các sách trong phiếu mượn đó
                var chiTietList = db.ChiTietMuonTras.Where(ct => ct.MaMuonTra == data.MaMuonTra).ToList();
                foreach (var item in chiTietList)
                {
                    item.GhiChu = data.GhiChu;
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật phiếu mượn thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}