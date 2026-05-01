using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiFlow.Migrations
{
    /// <inheritdoc />
    public partial class ServiciosPersonalizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Servicios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Servicios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Servicios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Emprendimientos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Emprendimientos");
        }
    }
}
