namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class genreAndRatings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FavoriteGenre", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FavoriteGenre");
        }
    }
}
