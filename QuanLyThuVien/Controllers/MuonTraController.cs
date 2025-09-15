using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
// Dòng này cho phép bạn sử dụng các class trong thư mục Models (như MuonTra, DocGia...)
using QuanLyThuVien.Models;

// Dòng này cho phép bạn sử dụng các class trong thư mục ViewModels (như MuonTraViewModel)
using QuanLyThuVien.ViewModels;
using System.Data.Entity; // BẮT BUỘC phải có dòng này để dùng .Include()

namespace QuanLyThuVien.Controllers
{
    public class MuonTraController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();
        // GET: MuonTra
        public ActionResult Index()
        {
            var danhSachPhieuMuon = db.MuonTras
                // .Include() giúp tải trước dữ liệu liên quan để tránh lỗi NullReferenceException.
                // Nó đi theo đường dẫn: MuonTra -> TheMuonSach -> DocGia
                .Include(mt => mt.TheMuonSach.DocGia)
                .Select(mt => new MuonTraViewModel
                {
                    MaPhieu = mt.MaMuonTra.ToString(),

                    // Lấy Tên Độc Giả thông qua các mối quan hệ (navigation properties)
                    // Entity Framework đã tự động tạo các thuộc tính này cho bạn từ CSDL
                    TenDocGia = mt.TheMuonSach.DocGia.HoTen,

                    NgayMuon = mt.NgayMuon,
                    NgayHenTra = mt.NgayHenTra,

                    // Logic tính toán Trạng Thái dựa trên NgayTraThucTe và NgayHenTra
                    TrangThai = (mt.NgayTraThucTe != null)
                                ? "Đã trả"
                                : (DateTime.Now > mt.NgayHenTra ? "Quá hạn" : "Đang mượn")
                })
                .OrderByDescending(p => p.NgayMuon) // Sắp xếp phiếu mới nhất lên trên
                .ToList();

            return View(danhSachPhieuMuon);
        }
    }
}