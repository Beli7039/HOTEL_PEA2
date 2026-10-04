using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL_PEA2.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCargoAdministrativo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CargoAdministrativo",
                table: "Reserva",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CargoAdministrativo",
                table: "Reserva");
        }
    }
}
