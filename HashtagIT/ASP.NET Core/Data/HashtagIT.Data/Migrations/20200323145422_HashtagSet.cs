namespace HashtagIT.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class HashtagSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "HashtagSets");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "HashtagSets",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "HashtagSets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "HashtagSets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
