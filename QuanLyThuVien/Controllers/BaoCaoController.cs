// File: ~/Controllers/BaoCaoController.cs

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.ViewModels;

namespace QuanLyThuVien.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        public ActionResult Index()
        {
            var viewModel = new BaoCaoViewModel();
            var homNay = DateTime.Now;
            var dauThangNay = new DateTime(homNay.Year, homNay.Month, 1);
            var cuoiThangNay = dauThangNay.AddMonths(1).AddDays(-1);

            // === 1. TÍNH TOÁN CÁC CHỈ SỐ TỔNG QUAN (STATS CARDS) ===

            // Lượt mượn sách trong tháng (tính theo số cuốn trong ChiTietMuonTra)
            viewModel.LuotMuonTrongThang = db.ChiTietMuonTras
                .Count(ct => ct.MuonTra.NgayMuon >= dauThangNay && ct.MuonTra.NgayMuon <= cuoiThangNay);

            // Sách đang quá hạn
            viewModel.SachDangQuaHan = db.MuonTras
                .Count(mt => mt.NgayTraThucTe == null && mt.NgayHenTra < homNay);

            // Độc giả mới trong tháng
            viewModel.DocGiaMoiTrongThang = db.DocGias
                .Count(dg => dg.NgayTaoTaiKhoan >= dauThangNay && dg.NgayTaoTaiKhoan <= cuoiThangNay);

            // Sách mới được nhập (giả sử dùng NgayCapNhat)
            viewModel.SachMoiDuocNhap = db.Saches
                .Count(s => s.NgayCapNhat >= dauThangNay && s.NgayCapNhat <= cuoiThangNay);


            // === 2. LẤY DỮ LIỆU CHO BIỂU ĐỒ ===

            // Biểu đồ Lượt mượn sách (6 tháng gần nhất)
            for (int i = 5; i >= 0; i--)
            {
                var thangCanXet = homNay.AddMonths(-i);
                var dauThang = new DateTime(thangCanXet.Year, thangCanXet.Month, 1);
                var cuoiThang = dauThang.AddMonths(1).AddDays(-1);

                int luotMuon = db.ChiTietMuonTras
                    .Count(ct => ct.MuonTra.NgayMuon >= dauThang && ct.MuonTra.NgayMuon <= cuoiThang);

                viewModel.BieuDoLuotMuon_Nhan.Add($"Tháng {thangCanXet.Month}/{thangCanXet.Year}");
                viewModel.BieuDoLuotMuon_DuLieu.Add(luotMuon);
            }

            // Biểu đồ Cơ cấu Thể loại sách
            var duLieuTheLoai = db.TheLoais
                .Where(tl => tl.DaXoa != true && tl.PhanLoaiSaches.Any()) // Chỉ lấy thể loại có sách
                .Select(tl => new {
                    tl.TenTheLoai,
                    SoLuongSach = tl.PhanLoaiSaches.Count
                })
                .ToList();

            foreach (var item in duLieuTheLoai)
            {
                viewModel.BieuDoTheLoai_Nhan.Add(item.TenTheLoai);
                viewModel.BieuDoTheLoai_DuLieu.Add(item.SoLuongSach);
            }


            // === 3. LẤY DỮ LIỆU TOP 10 SÁCH ĐƯỢC MƯỢN NHIỀU NHẤT ===
            var topSach = db.ChiTietMuonTras
                .GroupBy(ct => ct.Sach) // Nhóm theo đối tượng Sách
                .Select(g => new
                {
                    Sach = g.Key,
                    LuotMuon = g.Count()
                })
                .OrderByDescending(x => x.LuotMuon)
                .Take(10)
                .ToList();

            int hang = 1;
            foreach (var item in topSach)
            {
                viewModel.Top10SachDuocMuon.Add(new TopSachViewModel
                {
                    Hang = hang++,
                    TenSach = item.Sach.TenSach,
                    TenTacGia = item.Sach.TacGia.TenTacGia, // Giả sử có quan hệ trực tiếp
                    LuotMuon = item.LuotMuon
                });
            }

            return View(viewModel);
        }
    }
}