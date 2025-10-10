using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien.ViewModels
{
    public class MuonTraViewModel
    {
        public string MaPhieu { get; set; } 
        public string TenDocGia { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime NgayHenTra { get; set; }
        public string TrangThai { get; set; }
    }
}