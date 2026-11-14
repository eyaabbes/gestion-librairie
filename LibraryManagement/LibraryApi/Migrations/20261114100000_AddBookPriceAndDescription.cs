using LibraryApi.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryApi.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261114100000_AddBookPriceAndDescription")]
public sealed class AddBookPriceAndDescription : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Books",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "Price",
            table: "Books",
            type: "decimal(10,2)",
            precision: 10,
            scale: 2,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Description", table: "Books");
        migrationBuilder.DropColumn(name: "Price", table: "Books");
    }
}
