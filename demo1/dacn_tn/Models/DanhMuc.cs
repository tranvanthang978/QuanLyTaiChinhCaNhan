namespace dacn.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DanhMuc")]
    public partial class DanhMuc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DanhMuc()
        {
            GiaoDich = new HashSet<GiaoDich>();
        }

        [Key]
        [DisplayName("Mã danh mục")]
        public int MaDanhMuc { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Tên danh mục")]
        public string TenDanhMuc { get; set; }

        [StringLength(10)]
        [DisplayName("Loại")]
        public string Loai { get; set; }

        [DisplayName("Mã người dùng")]
        public int MaNguoiDung { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoDich> GiaoDich { get; set; }
    }
}
