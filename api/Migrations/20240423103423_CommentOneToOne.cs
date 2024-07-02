using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class CommentOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0dc7774a-0aaa-496b-90ed-e71113a3c851");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "325cce2a-034a-4ccf-b6f3-58fc14fa475d");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7d63015c-7fb0-46bd-ac6f-e23998747019", null, "Admin", "ADMIN" },
                    { "a267c3d7-2b25-4a52-bc10-6c5ad3fdd1c3", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AppUserId",
                table: "Comments",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_AppUserId",
                table: "Comments",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_AppUserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AppUserId",
                table: "Comments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7d63015c-7fb0-46bd-ac6f-e23998747019");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a267c3d7-2b25-4a52-bc10-6c5ad3fdd1c3");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Comments");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0dc7774a-0aaa-496b-90ed-e71113a3c851", null, "User", "USER" },
                    { "325cce2a-034a-4ccf-b6f3-58fc14fa475d", null, "Admin", "ADMIN" }
                });
        }
    }
}
