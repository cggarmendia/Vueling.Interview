namespace Vueling.DataAccess.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_db : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        From = c.String(nullable: false, maxLength: 128),
                        To = c.String(nullable: false, maxLength: 128),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.From, t.To });
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sku = c.String(nullable: false, maxLength: 60),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Sku, name: "IX_TransactionsSku");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Transactions", "IX_TransactionsSku");
            DropTable("dbo.Transactions");
            DropTable("dbo.Rates");
        }
    }
}
