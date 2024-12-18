using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarWars.Model.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHARACTERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NAME = table.Column<string>(type: "TEXT", nullable: false),
                    FACTION = table.Column<string>(type: "TEXT", nullable: false),
                    SPECIES = table.Column<string>(type: "TEXT", nullable: false),
                    HOMEWORLD = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHARACTERS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHARACTERS");
        }
    }
}
