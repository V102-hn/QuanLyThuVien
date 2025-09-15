using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models;      // Thêm dòng này
using QuanLyThuVien.ViewModels; // Thêm dòng này
using System.Data.Entity;         // Thêm dòng này để dùng .Include()
namespace QuanLyThuVien.Controllers
{
    public class XuLyViPhamController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();
        // GET: XuLyViPham
        public ActionResult Index()
        {
            // === GIAI ĐOẠN 1: LẤY DỮ LIỆU THÔ TỪ DATABASE ===
            // Chọn ra các trường cần thiết và thực thi câu lệnh SQL với .ToList()
            var danhSachTho = db.ViPhams
                .Include(vp => vp.DocGia)
                .Select(vp => new // Sử dụng một kiểu dữ liệu tạm thời (anonymous type)
        {
                    vp.MaViPham,
                    vp.DocGia.HoTen,
                    vp.LoaiViPham,
                    vp.HinhThucPhat,
                    vp.NgayViPham,
                    vp.TrangThaiXuLy // Chỉ lấy chuỗi gốc
        })
                .ToList(); // <-- Điểm quan trọng: thực thi SQL và mang dữ liệu về bộ nhớ

            // === GIAI ĐOẠN 2: XỬ LÝ VÀ ĐỊNH DẠNG TRONG BỘ NHỚ ===
            // Bây giờ 'danhSachTho' là một List, chúng ta có thể dùng LINQ-to-Objects
            // và mọi tính năng của C#
            var danhSachViPham = danhSachTho
                .Select(vp => new ViPhamViewModel
                {
                    MaViPham = "VP" + vp.MaViPham.ToString("D3"),
                    TenDocGia = vp.HoTen,
                    LoaiViPham = vp.LoaiViPham,
                    SoTienPhat = vp.HinhThucPhat.HasValue ? vp.HinhThucPhat.Value.ToString("N0") + "đ" : "0đ",
                    NgayGhiNhan = vp.NgayViPham,

            // Đoạn code này bây giờ chạy trong bộ nhớ nên hoàn toàn hợp lệ
            TrangThai = "đã thanh toán".Equals(vp.TrangThaiXuLy?.Trim(), StringComparison.OrdinalIgnoreCase)
                                ? "Đã thanh toán"
                                : "Chưa thanh toán"
                })
                .OrderByDescending(vp => vp.NgayGhiNhan)
                .ToList();

            return View(danhSachViPham);
        }
    }
}