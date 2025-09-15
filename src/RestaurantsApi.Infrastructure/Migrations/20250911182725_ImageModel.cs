using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantsApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Restaurants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Restaurants");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Role", "Username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "$2a$10$k4sOQ9MhxNGE4UJh7X0E.Oa0rsXZ4nFyWZs2YqrfUM2bGcOhtO9zC", 0, "admin" });
        }
    }
}
