namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeFavMovie : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "FavoriteMovie_ID", "dbo.Movies");
            DropIndex("dbo.AspNetUsers", new[] { "FavoriteMovie_ID" });
            AddColumn("dbo.AspNetUsers", "FavoriteMovie", c => c.String());
            DropColumn("dbo.AspNetUsers", "FavoriteMovie_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "FavoriteMovie_ID", c => c.Int());
            DropColumn("dbo.AspNetUsers", "FavoriteMovie");
            CreateIndex("dbo.AspNetUsers", "FavoriteMovie_ID");
            AddForeignKey("dbo.AspNetUsers", "FavoriteMovie_ID", "dbo.Movies", "ID");
        }
    }
}
