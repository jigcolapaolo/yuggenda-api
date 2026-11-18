using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yuggenda.Infrastructure.Persistence.Migrations
{
    public partial class AddAppointmentOverlapConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Impide citas activas superpuestas para el mismo profesional.
            migrationBuilder.Sql("""
                ALTER TABLE "Appointments"
                ADD CONSTRAINT "EX_appointments_member_time_overlap"
                EXCLUDE USING gist (
                    "BusinessMemberId" WITH =,
                    tstzrange("StartTime", "EndTime", '[)') WITH &&
                )
                WHERE ("Status" IN (0, 1));
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Elimina la restricción al revertir la migración.
            migrationBuilder.Sql("""
                ALTER TABLE "Appointments"
                DROP CONSTRAINT "EX_appointments_member_time_overlap";
                """);
        }
    }
}