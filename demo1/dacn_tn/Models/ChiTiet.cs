using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace dacn.Models
{
    public partial class ChiTiet : DbContext
    {
        public ChiTiet()
            : base("name=ChiTieu")
        {
        }

        public virtual DbSet<DanhMuc> DanhMuc { get; set; }
        public virtual DbSet<GiaoDich> GiaoDich { get; set; }
        public virtual DbSet<NguoiDung> NguoiDung { get; set; }

        public virtual DbSet<NganSach> NganSach { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DanhMuc>()
                .HasMany(e => e.GiaoDich)
                .WithRequired(e => e.DanhMuc)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NguoiDung>()
                .HasMany(e => e.DanhMuc)
                .WithRequired(e => e.NguoiDung)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NguoiDung>()
                .HasMany(e => e.GiaoDich)
                .WithRequired(e => e.NguoiDung)
                .WillCascadeOnDelete(false);
        }
    }
}
