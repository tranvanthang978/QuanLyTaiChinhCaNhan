using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dacn.Models
{
    // Model cũ - dùng để hiển thị Chi tiết Danh mục (đã được nhóm và tính tổng)
    public class BaoCaoViewModel
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; }
        public string Loai { get; set; } // Thu / Chi
        public string TenDanhMuc { get; set; }
        public decimal TongTien { get; set; } // Tổng tiền cho danh mục đó (khi đã group)
        public DateTime NgayGD { get; set; } // Ngày giao dịch (khi chưa group)
    }

    // Model MỚI - dùng để hiển thị Tổng quan theo Thời gian (Daily/Monthly Trend)
    public class ThongKeNgayViewModel
    {
        // Ngày giao dịch (chỉ có Date, không có Time)
        public DateTime Ngay { get; set; }
        // Tổng tiền Thu trong ngày đó
        public decimal TongThu { get; set; }
        // Tổng tiền Chi trong ngày đó
        public decimal TongChi { get; set; }
    }
}
