namespace dacn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChiTietModel1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NganSaches", "GiaoDich_MaGiaoDich", "dbo.GiaoDich");
            DropIndex("dbo.NganSaches", new[] { "GiaoDich_MaGiaoDich" });
            DropColumn("dbo.NganSaches", "GiaoDich_MaGiaoDich");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NganSaches", "GiaoDich_MaGiaoDich", c => c.Int());
            CreateIndex("dbo.NganSaches", "GiaoDich_MaGiaoDich");
            AddForeignKey("dbo.NganSaches", "GiaoDich_MaGiaoDich", "dbo.GiaoDich", "MaGiaoDich");
        }
    }
}
