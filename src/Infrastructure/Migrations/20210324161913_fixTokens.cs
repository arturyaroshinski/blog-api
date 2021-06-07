using Microsoft.EntityFrameworkCore.Migrations;

namespace Yaroshinski.Blog.Infrastructure.Migrations
{
    public partial class fixTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Authors_AuthorId",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "RefreshTokens",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Authors_AuthorId",
                table: "RefreshTokens",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Authors_AuthorId",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "RefreshTokens",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Authors_AuthorId",
                table: "RefreshTokens",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
