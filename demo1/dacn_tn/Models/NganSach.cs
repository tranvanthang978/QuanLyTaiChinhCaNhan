using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dacn.Models
{
    public class NganSach
    {
        [Key]
        public int MaNganSach { get; set; }

        public int MaNguoiDung { get; set; }

        public int? MaDanhMuc { get; set; }



        [Column(TypeName = "decimal")]
        [Range(0, double.MaxValue)]
        public decimal SoTienGioiHan { get; set; }

        [NotMapped]
        public decimal DaChi { get; set; }
        [NotMapped]
        public decimal ConLai { get; set; }


        public int Thang { get; set; }
        public int Nam { get; set; }

        public DateTime NgayTao { get; set; }

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung NguoiDung { get; set; }

        [ForeignKey("MaDanhMuc")]
        public virtual DanhMuc DanhMuc { get; set; }
    }
}