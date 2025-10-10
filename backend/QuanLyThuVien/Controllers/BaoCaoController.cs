// File: ~/Controllers/BaoCaoController.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml; // Thư viện EPPlus
using OfficeOpenXml.Style; // Dùng để style cho file Excel
using QuanLyThuVien.Models;
using QuanLyThuVien.ViewModels;

namespace QuanLyThuVien.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // Action Index đã được nâng cấp với bộ lọc
        public ActionResult Index(string filter = "month") // Mặc định là lọc theo tháng
        {
            var viewModel = new BaoCaoViewModel();
            var homNay = DateTime.Now;
            DateTime batDau, ketThuc;

            // Xác định khoảng thời gian bắt đầu và kết thúc dựa trên bộ lọc
            switch (filter)
            {
                case "week":
                    int diff = (7 + (homNay.DayOfWeek - DayOfWeek.Monday)) % 7;
                    batDau = homNay.AddDays(-1 * diff).Date;
                    ketThuc = batDau.AddDays(6);
                    break;
                case "year":
                    batDau = new DateTime(homNay.Year, 1, 1);
                    ketThuc = new DateTime(homNay.Year, 12, 31);
                    break;
                case "month":
                default:
                    batDau = new DateTime(homNay.Year, homNay.Month, 1);
                    ketThuc = batDau.AddMonths(1).AddDays(-1);
                    break;
            }

            // === 1. TÍNH TOÁN CÁC CHỈ SỐ TỔNG QUAN (DÙNG KHOẢNG THỜI GIAN ĐỘNG) ===
            viewModel.LuotMuonTrongThang = db.ChiTietMuonTras.Count(ct => ct.MuonTra.NgayMuon >= batDau && ct.MuonTra.NgayMuon <= ketThuc);
            viewModel.SachDangQuaHan = db.MuonTras.Count(mt => mt.NgayTraThucTe == null && mt.NgayHenTra < homNay); // Quá hạn là tổng thể, không theo filter
            viewModel.DocGiaMoiTrongThang = db.DocGias.Count(dg => dg.NgayTaoTaiKhoan >= batDau && dg.NgayTaoTaiKhoan <= ketThuc);
            viewModel.SachMoiDuocNhap = db.Saches.Count(s => s.NgayCapNhat >= batDau && s.NgayCapNhat <= ketThuc);

            // === 2. BIỂU ĐỒ VÀ TOP 10 (thường giữ cố định, không thay đổi theo filter ngắn hạn) ===
            // (Giữ nguyên logic tính toán biểu đồ và Top 10 của bạn)

            // Biểu đồ Lượt mượn 6 tháng
            for (int i = 5; i >= 0; i--)
            {
                var thangCanXet = homNay.AddMonths(-i);
                var dauThang = new DateTime(thangCanXet.Year, thangCanXet.Month, 1);
                var cuoiThang = dauThang.AddMonths(1).AddDays(-1);
                int luotMuon = db.ChiTietMuonTras.Count(ct => ct.MuonTra.NgayMuon >= dauThang && ct.MuonTra.NgayMuon <= cuoiThang);
                viewModel.BieuDoLuotMuon_Nhan.Add($"T{thangCanXet.Month}");
                viewModel.BieuDoLuotMuon_DuLieu.Add(luotMuon);
            }

            // Biểu đồ Thể loại
            var duLieuTheLoai = db.TheLoais
                .Where(tl => tl.DaXoa != true && tl.PhanLoaiSaches.Any())
                .Select(tl => new { tl.TenTheLoai, SoLuongSach = tl.PhanLoaiSaches.Count() }).ToList();
            viewModel.BieuDoTheLoai_Nhan = duLieuTheLoai.Select(x => x.TenTheLoai).ToList();
            viewModel.BieuDoTheLoai_DuLieu = duLieuTheLoai.Select(x => x.SoLuongSach).ToList();

            // Top 10 Sách
            var topSach = db.ChiTietMuonTras
                .GroupBy(ct => ct.Sach)
                .Select(g => new { Sach = g.Key, LuotMuon = g.Count() })
                .OrderByDescending(x => x.LuotMuon).Take(10).ToList();
            int hang = 1;
            viewModel.Top10SachDuocMuon = topSach.Select(item => new TopSachViewModel
            {
                Hang = hang++,
                TenSach = item.Sach.TenSach,
                TenTacGia = item.Sach.TacGia.TenTacGia,
                LuotMuon = item.LuotMuon
            }).ToList();

            return View(viewModel);
        }

        // ACTION MỚI: XUẤT BÁO CÁO RA EXCEL
        // --- THAY THẾ TOÀN BỘ ACTION EXPORTTOEXCEL BẰNG PHIÊN BẢN NÀY ---

        public ActionResult ExportToExcel(string filter = "month")
        {
            // === LẤY DỮ LIỆU (TƯƠNG TỰ ACTION INDEX) ===
            var homNay = DateTime.Now;
            DateTime batDau, ketThuc;
            string reportPeriod = "Tháng Này"; // Tên khoảng thời gian báo cáo

            switch (filter)
            {
                case "week":
                    int diff = (7 + (homNay.DayOfWeek - DayOfWeek.Monday)) % 7;
                    batDau = homNay.AddDays(-1 * diff).Date;
                    ketThuc = batDau.AddDays(6);
                    reportPeriod = "Tuần Này";
                    break;
                case "year":
                    batDau = new DateTime(homNay.Year, 1, 1);
                    ketThuc = new DateTime(homNay.Year, 12, 31);
                    reportPeriod = $"Năm {homNay.Year}";
                    break;
                case "month":
                default:
                    batDau = new DateTime(homNay.Year, homNay.Month, 1);
                    ketThuc = batDau.AddMonths(1).AddDays(-1);
                    reportPeriod = $"Tháng {homNay.Month}/{homNay.Year}";
                    break;
            }

            int luotMuon = db.ChiTietMuonTras.Count(ct => ct.MuonTra.NgayMuon >= batDau && ct.MuonTra.NgayMuon <= ketThuc);
            int sachQuaHan = db.MuonTras.Count(mt => mt.NgayTraThucTe == null && mt.NgayHenTra < homNay);
            int docGiaMoi = db.DocGias.Count(dg => dg.NgayTaoTaiKhoan >= batDau && dg.NgayTaoTaiKhoan <= ketThuc);
            var topSachData = db.ChiTietMuonTras.GroupBy(ct => ct.Sach)
                .Select(g => new { Sach = g.Key, LuotMuon = g.Count() })
                .OrderByDescending(x => x.LuotMuon).Take(10).ToList();

            // --- Bắt đầu tạo file Excel ---
            // Không cần set LicenseContext vì chúng ta đã hạ cấp thư viện
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoThongKe");

                // --- ĐỊNH DẠNG VÀ THÊM DỮ LIỆU ---

                // 1. Tiêu đề chính
                worksheet.Cells["A1"].Value = "BÁO CÁO THỐNG KÊ THƯ VIỆN";
                worksheet.Cells["A1:D1"].Merge = true;
                worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                worksheet.Cells["A1:D1"].Style.Font.Size = 18;
                worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2"].Value = $"Kỳ báo cáo: {reportPeriod}";
                worksheet.Cells["A2:D2"].Merge = true;
                worksheet.Cells["A2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:D2"].Style.Font.Italic = true;

                // 2. Các chỉ số tổng quan
                worksheet.Cells["A4"].Value = "Thống Kê Tổng Quan";
                worksheet.Cells["A4:B4"].Merge = true;
                worksheet.Cells["A4:B4"].Style.Font.Bold = true;

                worksheet.Cells["A5"].Value = "Lượt mượn sách:";
                worksheet.Cells["B5"].Value = luotMuon;
                worksheet.Cells["A6"].Value = "Sách đang quá hạn:";
                worksheet.Cells["B6"].Value = sachQuaHan;
                worksheet.Cells["A7"].Value = "Độc giả mới:";
                worksheet.Cells["B7"].Value = docGiaMoi;
                worksheet.Cells["A5:A7"].Style.Font.Bold = true;

                // 3. Bảng Top 10 sách
                int startRowForTopList = 9; // Bắt đầu bảng ở dòng 9
                worksheet.Cells[startRowForTopList - 1, 1].Value = "Top 10 Sách Mượn Nhiều Nhất";
                worksheet.Cells[startRowForTopList - 1, 1, startRowForTopList - 1, 4].Merge = true;
                worksheet.Cells[startRowForTopList - 1, 1, startRowForTopList - 1, 4].Style.Font.Bold = true;

                worksheet.Cells[startRowForTopList, 1].Value = "Hạng";
                worksheet.Cells[startRowForTopList, 2].Value = "Tên Sách";
                worksheet.Cells[startRowForTopList, 3].Value = "Tác Giả";
                worksheet.Cells[startRowForTopList, 4].Value = "Lượt Mượn";
                using (var range = worksheet.Cells[startRowForTopList, 1, startRowForTopList, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Đổ dữ liệu Top 10
                int currentRow = startRowForTopList + 1;
                int hang = 1;
                foreach (var item in topSachData)
                {
                    worksheet.Cells[currentRow, 1].Value = hang++;
                    worksheet.Cells[currentRow, 2].Value = item.Sach.TenSach;
                    worksheet.Cells[currentRow, 3].Value = item.Sach.TacGia?.TenTacGia; // Thêm '?' để tránh lỗi nếu Tác giả là null
                    worksheet.Cells[currentRow, 4].Value = item.LuotMuon;
                    currentRow++;
                }

                // Tự động căn chỉnh độ rộng các cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Chuyển file Excel thành dạng byte array để tải về
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCaoThongKe_{DateTime.Now:ddMMyyyy}.xlsx");
            }
        }
    }
}