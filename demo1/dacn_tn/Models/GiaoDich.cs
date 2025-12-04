namespace dacn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiaoDich")]
    public partial class GiaoDich
    {
        [Key]
        [DisplayName("Mã giao dịch")]
        public int MaGiaoDich { get; set; }

        [StringLength(255)]
        [DisplayName("Mô tả")]
        public string MoTa { get; set; }

        
        [Required, DataType(DataType.DateTime)]
        // Dùng định dạng ISO yyyy-MM-ddTHH:mm để tương thích với HTML type="datetime-local"
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Thời gian")]
        public DateTime NgayGD { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [DisplayName("Số tiền")]
        public decimal SoTien { get; set; }

        [DisplayName("Mã danh mục")]
        public int MaDanhMuc { get; set; }

        [DisplayName("Mã người dùng")]
        public int MaNguoiDung { get; set; }

        public virtual DanhMuc DanhMuc { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
    }
}