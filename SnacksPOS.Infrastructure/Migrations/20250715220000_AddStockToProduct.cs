using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnacksPOS.Infrastructure.Migrations;

public partial class AddStockToProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Stock",
            table: "Products",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Stock",
            table: "Products");
    }
}
