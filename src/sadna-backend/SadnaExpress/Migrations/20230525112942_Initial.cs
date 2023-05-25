using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SadnaExpress.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bids",
                columns: table => new
                {
                    BidId = table.Column<Guid>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    StoreID = table.Column<Guid>(nullable: false),
                    ItemID = table.Column<Guid>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    OpenBid = table.Column<bool>(nullable: false),
                    DecisionDB = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bids", x => x.BidId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    InventoryID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemID);
                });

            migrationBuilder.CreateTable(
                name: "macs",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    mac = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_macs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreID = table.Column<Guid>(nullable: false),
                    StoreName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    PurchasePolicyCounter = table.Column<int>(nullable: false),
                    DiscountPolicyCounter = table.Column<int>(nullable: false),
                    StoreRating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreID);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    BidsDB = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    LoggedIn = table.Column<bool>(nullable: true),
                    SecurityQuestionsDB = table.Column<string>(nullable: true),
                    DirectSupervisorDB = table.Column<string>(nullable: true),
                    AppointDB = table.Column<string>(nullable: true),
                    PermissionDB = table.Column<string>(nullable: true),
                    BidsOffersDB = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ItemForOrders",
                columns: table => new
                {
                    ItemForOrderId = table.Column<Guid>(nullable: false),
                    ItemID = table.Column<Guid>(nullable: false),
                    StoreID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    UserEmail = table.Column<string>(nullable: true),
                    StoreName = table.Column<string>(nullable: true),
                    OrderID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemForOrders", x => x.ItemForOrderId);
                    table.ForeignKey(
                        name: "FK_ItemForOrders_orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    InventoryID = table.Column<Guid>(nullable: false),
                    StoreID = table.Column<Guid>(nullable: false),
                    Items_quantityDB = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.InventoryID);
                    table.ForeignKey(
                        name: "FK_Inventories_Stores_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Stores",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewID = table.Column<Guid>(nullable: false),
                    StoreID = table.Column<Guid>(nullable: true),
                    ItemID = table.Column<Guid>(nullable: true),
                    ReviewText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Reviews_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Stores_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Stores",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(nullable: false),
                    SentTo = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    SentFrom = table.Column<Guid>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Read = table.Column<bool>(nullable: false),
                    MemberUserId = table.Column<Guid>(nullable: true),
                    MemberUserId1 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notification_users_MemberUserId",
                        column: x => x.MemberUserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notification_users_MemberUserId1",
                        column: x => x.MemberUserId1,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shoppingCarts",
                columns: table => new
                {
                    ShoppingCartId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shoppingCarts", x => x.ShoppingCartId);
                    table.ForeignKey(
                        name: "FK_shoppingCarts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shoppingBaskets",
                columns: table => new
                {
                    ShoppingBasketId = table.Column<Guid>(nullable: false),
                    ShoppingCartId = table.Column<Guid>(nullable: false),
                    StoreID = table.Column<Guid>(nullable: false),
                    ItemInBasketDB = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shoppingBaskets", x => x.ShoppingBasketId);
                    table.ForeignKey(
                        name: "FK_shoppingBaskets_shoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "shoppingCarts",
                        principalColumn: "ShoppingCartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_StoreID",
                table: "Inventories",
                column: "StoreID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemForOrders_OrderID",
                table: "ItemForOrders",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MemberUserId",
                table: "Notification",
                column: "MemberUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MemberUserId1",
                table: "Notification",
                column: "MemberUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ItemID",
                table: "Reviews",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StoreID",
                table: "Reviews",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingBaskets_ShoppingCartId",
                table: "shoppingBaskets",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingCarts_UserId",
                table: "shoppingCarts",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bids");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "ItemForOrders");

            migrationBuilder.DropTable(
                name: "macs");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "shoppingBaskets");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "shoppingCarts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
