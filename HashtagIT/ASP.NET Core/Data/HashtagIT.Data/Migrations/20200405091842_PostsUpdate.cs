namespace HashtagIT.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class PostsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_HashtagSets_HashtagSetId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_HashtagSetId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "HashtagSetId",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "HashtagSet",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IGUserName",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashtagSet",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IGUserName",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "HashtagSetId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_HashtagSetId",
                table: "Posts",
                column: "HashtagSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_HashtagSets_HashtagSetId",
                table: "Posts",
                column: "HashtagSetId",
                principalTable: "HashtagSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
