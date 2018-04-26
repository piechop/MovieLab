namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class author : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reviews", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Reviews", new[] { "Author_Id" });
            AddColumn("dbo.Reviews", "Author", c => c.String());
            DropColumn("dbo.Reviews", "Author_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reviews", "Author_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Reviews", "Author");
            CreateIndex("dbo.Reviews", "Author_Id");
            AddForeignKey("dbo.Reviews", "Author_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
