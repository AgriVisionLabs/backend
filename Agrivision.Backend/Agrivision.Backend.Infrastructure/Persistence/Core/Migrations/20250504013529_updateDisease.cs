using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
<<<<<<< HEAD:Agrivision.Backend/Agrivision.Backend.Infrastructure/Persistence/Core/Migrations/20250430062118_MinorChangesToIrrigationUnit.cs
    public partial class MinorChangesToIrrigationUnit : Migration
=======
    public partial class updateDisease : Migration
>>>>>>> .....:Agrivision.Backend/Agrivision.Backend.Infrastructure/Persistence/Core/Migrations/20250504013529_updateDisease.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
<<<<<<< HEAD:Agrivision.Backend/Agrivision.Backend.Infrastructure/Persistence/Core/Migrations/20250430062118_MinorChangesToIrrigationUnit.cs
                name: "IsOn",
                table: "IrrigationUnits",
=======
                name: "Is_Safe",
                table: "Diseases",
>>>>>>> .....:Agrivision.Backend/Agrivision.Backend.Infrastructure/Persistence/Core/Migrations/20250504013529_updateDisease.cs
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
<<<<<<< HEAD:Agrivision.Backend/Agrivision.Backend.Infrastructure/Persistence/Core/Migrations/20250430062118_MinorChangesToIrrigationUnit.cs
                name: "IsOn",
                table: "IrrigationUnits");
=======
                name: "Is_Safe",
                table: "Diseases");
>>>>>>> .....:Agrivision.Backend/Agrivision.Backend.Infrastructure/Persistence/Core/Migrations/20250504013529_updateDisease.cs
        }
    }
}
