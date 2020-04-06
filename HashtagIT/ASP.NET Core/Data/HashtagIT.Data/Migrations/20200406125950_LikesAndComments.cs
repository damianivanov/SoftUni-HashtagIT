namespace HashtagIT.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class LikesAndComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Likes",
                table: "Posts",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Posts");
        }
    }
}
