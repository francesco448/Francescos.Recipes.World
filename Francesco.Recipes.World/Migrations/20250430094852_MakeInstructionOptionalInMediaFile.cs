using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Francesco.Recipes.World.Migrations
{
    /// <inheritdoc />
    public partial class MakeInstructionOptionalInMediaFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Instructions_InstructionId",
                table: "MediaFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "InstructionId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Instructions_InstructionId",
                table: "MediaFiles",
                column: "InstructionId",
                principalTable: "Instructions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Instructions_InstructionId",
                table: "MediaFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "InstructionId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Instructions_InstructionId",
                table: "MediaFiles",
                column: "InstructionId",
                principalTable: "Instructions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
