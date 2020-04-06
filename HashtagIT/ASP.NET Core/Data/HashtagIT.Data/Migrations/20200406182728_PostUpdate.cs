namespace HashtagIT.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class PostUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostPhoto",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostPhoto",
                table: "Posts");
        }
    }
}
