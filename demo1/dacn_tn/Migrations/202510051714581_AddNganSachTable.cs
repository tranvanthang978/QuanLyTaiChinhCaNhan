namespace dacn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNganSachTable : DbMigration
    {
        public override void Up()
        {
            
            
            CreateTable(
                "dbo.NganSaches",
                c => new
                    {
                        MaNganSach = c.Int(nullable: false, identity: true),
                        MaNguoiDung = c.Int(nullable: false),
                        MaDanhMuc = c.Int(),
                        SoTienGioiHan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Thang = c.Int(nullable: false),
                        Nam = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MaNganSach)
                .ForeignKey("dbo.DanhMuc", t => t.MaDanhMuc)
                .ForeignKey("dbo.NguoiDung", t => t.MaNguoiDung, cascadeDelete: true)
                .Index(t => t.MaNguoiDung)
                .Index(t => t.MaDanhMuc);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NganSaches", "MaNguoiDung", "dbo.NguoiDung");
            DropForeignKey("dbo.NganSaches", "MaDanhMuc", "dbo.DanhMuc");
            DropForeignKey("dbo.GiaoDich", "MaDanhMuc", "dbo.DanhMuc");
            DropForeignKey("dbo.GiaoDich", "MaNguoiDung", "dbo.NguoiDung");
            DropForeignKey("dbo.DanhMuc", "MaNguoiDung", "dbo.NguoiDung");
            DropIndex("dbo.NganSaches", new[] { "MaDanhMuc" });
            DropIndex("dbo.NganSaches", new[] { "MaNguoiDung" });
            DropIndex("dbo.GiaoDich", new[] { "MaNguoiDung" });
            DropIndex("dbo.GiaoDich", new[] { "MaDanhMuc" });
            DropIndex("dbo.DanhMuc", new[] { "MaNguoiDung" });
            DropTable("dbo.NganSaches");
            DropTable("dbo.NguoiDung");
            DropTable("dbo.GiaoDich");
            DropTable("dbo.DanhMuc");
        }
    }
}
