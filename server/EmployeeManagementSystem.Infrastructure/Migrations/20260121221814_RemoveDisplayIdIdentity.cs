using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDisplayIdIdentity : Migration
    {
        private static readonly string[] Tables = new[]
        {
            "Addresses", "Contacts", "Employments", "EmploymentSchools",
            "Items", "Persons", "Positions", "SalaryGrades", "Schools"
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                // Step 1: Drop the unique index
                migrationBuilder.Sql($"DROP INDEX IF EXISTS [IX_{table}_DisplayId] ON [{table}];");

                // Step 2: Add temp column
                migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [DisplayId_Temp] bigint NOT NULL DEFAULT 0;");

                // Step 3: Copy data
                migrationBuilder.Sql($"UPDATE [{table}] SET [DisplayId_Temp] = [DisplayId];");

                // Step 4: Drop old column
                migrationBuilder.Sql($"ALTER TABLE [{table}] DROP COLUMN [DisplayId];");

                // Step 5: Rename temp column
                migrationBuilder.Sql($"EXEC sp_rename '{table}.DisplayId_Temp', 'DisplayId', 'COLUMN';");

                // Step 6: Recreate unique index
                migrationBuilder.Sql($"CREATE UNIQUE INDEX [IX_{table}_DisplayId] ON [{table}] ([DisplayId]);");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverting to IDENTITY columns is complex and data-destructive
            // This is a one-way migration for production scenarios
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"DROP INDEX IF EXISTS [IX_{table}_DisplayId] ON [{table}];");
                migrationBuilder.Sql($"ALTER TABLE [{table}] ADD [DisplayId_Temp] bigint NOT NULL IDENTITY(1,1);");
                migrationBuilder.Sql($"ALTER TABLE [{table}] DROP COLUMN [DisplayId];");
                migrationBuilder.Sql($"EXEC sp_rename '{table}.DisplayId_Temp', 'DisplayId', 'COLUMN';");
                migrationBuilder.Sql($"CREATE UNIQUE INDEX [IX_{table}_DisplayId] ON [{table}] ([DisplayId]);");
            }
        }
    }
}
