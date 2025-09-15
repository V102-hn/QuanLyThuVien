using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.ViewModels;

namespace QuanLyThuVien.Controllers
{
    public class CaiDatController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: CaiDat (Hiển thị form cài đặt)
        public ActionResult Index()
        {
            var viewModel = new CaiDatViewModel
            {
                // Đọc từng giá trị từ DB, chuyển đổi kiểu và gán vào ViewModel
                SoNgayMuonToiDa = int.Parse(db.CauHinhs.Find("SoNgayMuonToiDa").GiaTri),
                SoSachMuonToiDa = int.Parse(db.CauHinhs.Find("SoSachMuonToiDa").GiaTri),
                TienPhatMoiNgay = int.Parse(db.CauHinhs.Find("TienPhatMoiNgay").GiaTri),
                TenThuVien = db.CauHinhs.Find("TenThuVien").GiaTri,
                DiaChiThuVien = db.CauHinhs.Find("DiaChiThuVien").GiaTri
            };
            return View(viewModel);
        }

        // POST: CaiDat (Lưu các thay đổi)
        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm để bảo mật
        public ActionResult Index(CaiDatViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật từng giá trị trong DB từ ViewModel
                var soNgayMuon = db.CauHinhs.Find("SoNgayMuonToiDa");
                soNgayMuon.GiaTri = viewModel.SoNgayMuonToiDa.ToString();

                var soSachMuon = db.CauHinhs.Find("SoSachMuonToiDa");
                soSachMuon.GiaTri = viewModel.SoSachMuonToiDa.ToString();

                var tienPhat = db.CauHinhs.Find("TienPhatMoiNgay");
                tienPhat.GiaTri = viewModel.TienPhatMoiNgay.ToString();

                var tenThuVien = db.CauHinhs.Find("TenThuVien");
                tenThuVien.GiaTri = viewModel.TenThuVien;

                var diaChi = db.CauHinhs.Find("DiaChiThuVien");
                diaChi.GiaTri = viewModel.DiaChiThuVien;

                // Lưu tất cả thay đổi vào database
                db.SaveChanges();

                // Gửi thông báo thành công về View
                TempData["SuccessMessage"] = "Cập nhật cài đặt thành công!";

                return RedirectToAction("Index");
            }

            // Nếu model không hợp lệ, hiển thị lại form với các lỗi
            return View(viewModel);
        }
    }
}