using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien.ViewModels
{
    public class CreateMuonTraDto
    {
        public int MaDocGia { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime NgayHenTra { get; set; }
        public string GhiChu { get; set; }
        public List<int> SachIds { get; set; }
    }
}