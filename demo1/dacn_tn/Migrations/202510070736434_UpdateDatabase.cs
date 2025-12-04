namespace dacn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NganSaches", "GiaoDich_MaGiaoDich", c => c.Int());
            CreateIndex("dbo.NganSaches", "GiaoDich_MaGiaoDich");
            AddForeignKey("dbo.NganSaches", "GiaoDich_MaGiaoDich", "dbo.GiaoDich", "MaGiaoDich");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NganSaches", "GiaoDich_MaGiaoDich", "dbo.GiaoDich");
            DropIndex("dbo.NganSaches", new[] { "GiaoDich_MaGiaoDich" });
            DropColumn("dbo.NganSaches", "GiaoDich_MaGiaoDich");
        }
    }
}
