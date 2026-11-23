using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskFlow.Infrastructure.Persistence;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDBContext))]
[Migration("20261115195500_BackfillProjectCreatorMemberships")]
public partial class BackfillProjectCreatorMemberships : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            INSERT INTO ProjectMembers (ProjectId, UserId, AddedAt, Id)
            SELECT p.Id, p.CreatedByUserId, GETUTCDATE(), 0
            FROM Projects p
            INNER JOIN OrganizationMembers om
                ON om.OrganizationId = p.OrganizationId
                AND om.UserId = p.CreatedByUserId
            WHERE p.isDeleted = 0
              AND NOT EXISTS (
                  SELECT 1
                  FROM ProjectMembers pm
                  WHERE pm.ProjectId = p.Id
                    AND pm.UserId = p.CreatedByUserId
              );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Data backfills are intentionally not reversed because existing and inserted memberships
        // cannot be distinguished safely after the migration has run.
    }
}
