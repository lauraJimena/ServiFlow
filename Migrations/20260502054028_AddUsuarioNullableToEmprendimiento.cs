using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioNullableToEmprendimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Emprendimientos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emprendimientos_UsuarioId",
                table: "Emprendimientos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprendimientos_Usuarios_UsuarioId",
                table: "Emprendimientos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprendimientos_Usuarios_UsuarioId",
                table: "Emprendimientos");

            migrationBuilder.DropIndex(
                name: "IX_Emprendimientos_UsuarioId",
                table: "Emprendimientos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Emprendimientos");
        }
    }
}
