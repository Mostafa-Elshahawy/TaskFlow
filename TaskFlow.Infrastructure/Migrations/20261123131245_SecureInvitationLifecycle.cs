using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecureInvitationLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationInvitations_OrganizationId",
                table: "OrganizationInvitations");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OrganizationInvitations",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAtUtc",
                table: "OrganizationInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedByUserId",
                table: "OrganizationInvitations",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAtUtc",
                table: "OrganizationInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAtUtc",
                table: "OrganizationInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OrganizationInvitations",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "OrganizationInvitations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailOutboxMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Recipient = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProtectedHtmlBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextAttemptAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOutboxMessages", x => x.Id);
                });

            migrationBuilder.Sql(
                """
                UPDATE [OrganizationInvitations]
                SET [InvitedEmail] = LEFT(UPPER(LTRIM(RTRIM([InvitedEmail]))), 256),
                    [ExpiresAtUtc] = DATEADD(hour, 72, [CreatedAtUtc]),
                    [AcceptedAtUtc] = CASE WHEN [Accepted] = 1 THEN [CreatedAtUtc] ELSE NULL END;

                ;WITH [RankedTokens] AS
                (
                    SELECT [Id], [Token],
                           ROW_NUMBER() OVER (PARTITION BY [Token] ORDER BY [Id]) AS [TokenRank]
                    FROM [OrganizationInvitations]
                )
                UPDATE [invitation]
                SET [TokenHash] = CONVERT(varchar(64), HASHBYTES(
                    'SHA2_256',
                    CASE WHEN [ranked].[TokenRank] = 1
                         THEN [ranked].[Token]
                         ELSE CONCAT([ranked].[Token], N':', CONVERT(nvarchar(20), [ranked].[Id]))
                    END), 2)
                FROM [OrganizationInvitations] AS [invitation]
                INNER JOIN [RankedTokens] AS [ranked] ON [ranked].[Id] = [invitation].[Id];

                ;WITH [RankedOutstanding] AS
                (
                    SELECT [Id],
                           ROW_NUMBER() OVER (
                               PARTITION BY [OrganizationId], [InvitedEmail]
                               ORDER BY [CreatedAtUtc] DESC, [Id] DESC) AS [InviteRank]
                    FROM [OrganizationInvitations]
                    WHERE [AcceptedAtUtc] IS NULL
                )
                UPDATE [invitation]
                SET [RevokedAtUtc] = [invitation].[CreatedAtUtc]
                FROM [OrganizationInvitations] AS [invitation]
                INNER JOIN [RankedOutstanding] AS [ranked] ON [ranked].[Id] = [invitation].[Id]
                WHERE [ranked].[InviteRank] > 1;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "InvitedEmail",
                table: "OrganizationInvitations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAtUtc",
                table: "OrganizationInvitations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "OrganizationInvitations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "OrganizationInvitations");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitations_OrganizationId_InvitedEmail",
                table: "OrganizationInvitations",
                columns: new[] { "OrganizationId", "InvitedEmail" },
                unique: true,
                filter: "[AcceptedAtUtc] IS NULL AND [RevokedAtUtc] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitations_TokenHash",
                table: "OrganizationInvitations",
                column: "TokenHash",
                unique: true,
                filter: "[TokenHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxMessages_ProcessedAtUtc_NextAttemptAtUtc",
                table: "EmailOutboxMessages",
                columns: new[] { "ProcessedAtUtc", "NextAttemptAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailOutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationInvitations_OrganizationId_InvitedEmail",
                table: "OrganizationInvitations");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationInvitations_TokenHash",
                table: "OrganizationInvitations");

            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "OrganizationInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "OrganizationInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE [OrganizationInvitations]
                SET [Accepted] = CASE WHEN [AcceptedAtUtc] IS NULL THEN 0 ELSE 1 END,
                    [Token] = CONCAT(N'legacy-', CONVERT(nvarchar(20), [Id]));
                """);

            migrationBuilder.DropColumn(
                name: "AcceptedAtUtc",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "AcceptedByUserId",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "ExpiresAtUtc",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "RevokedAtUtc",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "OrganizationInvitations");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "OrganizationInvitations",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "InvitedEmail",
                table: "OrganizationInvitations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitations_OrganizationId",
                table: "OrganizationInvitations",
                column: "OrganizationId");
        }
    }
}
