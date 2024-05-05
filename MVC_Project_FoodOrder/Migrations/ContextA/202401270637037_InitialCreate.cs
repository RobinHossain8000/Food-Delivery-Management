namespace MVC_Project_FoodOrder.Migrations.ContextA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.FoodItems",
                c => new
                    {
                        FoodItemId = c.Int(nullable: false, identity: true),
                        FoodName = c.String(nullable: false, maxLength: 50),
                        UnitPrice = c.Decimal(nullable: false, storeType: "money"),
                        Available = c.Boolean(nullable: false),
                        Picture = c.String(nullable: false, maxLength: 50),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FoodItemId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false, storeType: "date"),
                        Quantity = c.Int(nullable: false),
                        OrderType = c.Int(nullable: false),
                        FoodItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.FoodItems", t => t.FoodItemId, cascadeDelete: true)
                .Index(t => t.FoodItemId);
            /////////////////////////////////////////////////
            CreateStoredProcedure("InsertFoodItem", c => new {
                FoodName = c.String(maxLength: 50),
                UnitPrice = c.Decimal(),
                Available = c.Boolean(),
                Picture = c.String(maxLength: 50),
                CategoryId = c.Int(),

            }, @"INSERT INTO FoodItems (FoodName, UnitPrice, Available, Picture,CategoryId)
	                    VALUES (@FoodName, @UnitPrice, @Available, @Picture, @CategoryId)
	                    SELECT SCOPE_IDENTITY() AS FoodItemId
                    RETURN ");
            CreateStoredProcedure("UpdateFoodItem", c => new
            {
                FoodItemId = c.Int(),
                FoodName = c.String(maxLength: 50),
                UnitPrice = c.Decimal(),
                Available = c.Boolean(),
                Picture = c.String(maxLength: 50),
                CategoryId = c.Int(),

            }, @"UPDATE FoodItems SET FoodName = @FoodName, UnitPrice=@UnitPrice, Available=@Available, Picture=@Picture,CategoryId=@CategoryId
                    WHERE FoodItemId = @FoodItemId
                RETURN");

            CreateStoredProcedure("DeleteFoodItem", c => new
            {
                FoodItemId = c.Int()

            }, @"DELETE FROM FoodItems
                WHERE FoodItemId = @FoodItemId
            RETURN");
            CreateStoredProcedure("DeleteOrderByFoodItemId", c => new
            {
                FoodItemId = c.Int()

            }, @"DELETE FROM Orders
                WHERE FoodItemId = @FoodItemId
            RETURN");
            CreateStoredProcedure("InsertOrder", c => new
            {
                OrderDate = c.DateTime(),
                Quantity = c.Int(),
                OrderType = c.Int(),
                FoodItemId = c.Int()

            }, @"INSERT INTO Orders (OrderDate, Quantity, OrderType, FoodItemId)
	            VALUES (@OrderDate, @Quantity, @OrderType, @FoodItemId)
	            SELECT SCOPE_IDENTITY() as OrderId
            RETURN");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "FoodItemId", "dbo.FoodItems");
            DropForeignKey("dbo.FoodItems", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Orders", new[] { "FoodItemId" });
            DropIndex("dbo.FoodItems", new[] { "CategoryId" });
            DropTable("dbo.Orders");
            DropTable("dbo.FoodItems");
            DropTable("dbo.Categories");
            ///////////////////////////
            DropStoredProcedure("InsertFoodItem");
            DropStoredProcedure("UpdateFoodItem");
            DropStoredProcedure("DeleteFoodItem");
            DropStoredProcedure("DeleteOrderByFoodItemId");
            DropStoredProcedure("InsertOrder");
        }
    }
}
