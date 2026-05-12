using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddEmprendedorToEmprendimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmprendedorId",
                table: "Emprendimientos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Emprendedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Ubicacion = table.Column<string>(type: "TEXT", nullable: true),
                    HorarioAtencion = table.Column<string>(type: "TEXT", nullable: true),
                    InstagramUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprendedor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprendimientos_EmprendedorId",
                table: "Emprendimientos",
                column: "EmprendedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprendimientos_Emprendedor_EmprendedorId",
                table: "Emprendimientos",
                column: "EmprendedorId",
                principalTable: "Emprendedor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprendimientos_Emprendedor_EmprendedorId",
                table: "Emprendimientos");

            migrationBuilder.DropTable(
                name: "Emprendedor");

            migrationBuilder.DropIndex(
                name: "IX_Emprendimientos_EmprendedorId",
                table: "Emprendimientos");

            migrationBuilder.DropColumn(
                name: "EmprendedorId",
                table: "Emprendimientos");
        }
    }
}
