namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rat : DbMigration
    {
        public override void Up()
        {
            
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        Rating = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}
