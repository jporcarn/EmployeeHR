using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeHR.EF.Migrations
{
    public partial class RowVersion_Trigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string commandText = @"
CREATE TRIGGER RowVersionConcurrencyTrigger
   ON  Employee
   AFTER INSERT,UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Employee
        SET Employee.RowVersion = GETDATE()
    FROM
        Employee
        INNER JOIN inserted ON Employee.ID = inserted.ID;
END
";
            migrationBuilder.Sql(commandText);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string commandText = @"
DROP TRIGGER IF EXISTS RowVersionConcurrencyTrigger]
";
            migrationBuilder.Sql(commandText);
        }
    }
}