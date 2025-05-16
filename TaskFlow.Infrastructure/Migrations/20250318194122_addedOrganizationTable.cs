using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedOrganizationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject_AspNetUsers_ManagersId",
                table: "ApplicationUserProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject_Projects_ManagedProjectsId",
                table: "ApplicationUserProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject1_AspNetUsers_MembersId",
                table: "ApplicationUserProject1");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProject1_Projects_AssignedProjectsId",
                table: "ApplicationUserProject1");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserProject1",
                table: "ApplicationUserProject1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserProject",
                table: "ApplicationUserProject");

            migrationBuilder.RenameTable(
                name: "ApplicationUserProject1",
                newName: "ProjectMembers");

            migrationBuilder.RenameTable(
                name: "ApplicationUserProject",
                newName: "ProjectManagers");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserProject1_MembersId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_MembersId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserProject_ManagersId",
                table: "ProjectManagers",
                newName: "IX_ProjectManagers_ManagersId");

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                columns: new[] { "AssignedProjectsId", "MembersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectManagers",
                table: "ProjectManagers",
                columns: new[] { "ManagedProjectsId", "ManagersId" });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMembers",
                columns: table => new
                {
                    MemberOrganizationsId = table.Column<int>(type: "int", nullable: false),
                    MembersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMembers", x => new { x.MemberOrganizationsId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_OrganizationMembers_AspNetUsers_MembersId",
                        column: x => x.MembersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationMembers_Organizations_MemberOrganizationsId",
                        column: x => x.MemberOrganizationsId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrganizationId",
                table: "Projects",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_MembersId",
                table: "OrganizationMembers",
                column: "MembersId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OwnerId",
                table: "Organizations",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectManagers_AspNetUsers_ManagersId",
                table: "ProjectManagers",
                column: "ManagersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectManagers_Projects_ManagedProjectsId",
                table: "ProjectManagers",
                column: "ManagedProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_MembersId",
                table: "ProjectMembers",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_AssignedProjectsId",
                table: "ProjectMembers",
                column: "AssignedProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Organizations_OrganizationId",
                table: "Projects",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectManagers_AspNetUsers_ManagersId",
                table: "ProjectManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectManagers_Projects_ManagedProjectsId",
                table: "ProjectManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_MembersId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_AssignedProjectsId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Organizations_OrganizationId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "OrganizationMembers");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OrganizationId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectManagers",
                table: "ProjectManagers");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "ProjectMembers",
                newName: "ApplicationUserProject1");

            migrationBuilder.RenameTable(
                name: "ProjectManagers",
                newName: "ApplicationUserProject");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_MembersId",
                table: "ApplicationUserProject1",
                newName: "IX_ApplicationUserProject1_MembersId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectManagers_ManagersId",
                table: "ApplicationUserProject",
                newName: "IX_ApplicationUserProject_ManagersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserProject1",
                table: "ApplicationUserProject1",
                columns: new[] { "AssignedProjectsId", "MembersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserProject",
                table: "ApplicationUserProject",
                columns: new[] { "ManagedProjectsId", "ManagersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProject_AspNetUsers_ManagersId",
                table: "ApplicationUserProject",
                column: "ManagersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProject_Projects_ManagedProjectsId",
                table: "ApplicationUserProject",
                column: "ManagedProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProject1_AspNetUsers_MembersId",
                table: "ApplicationUserProject1",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProject1_Projects_AssignedProjectsId",
                table: "ApplicationUserProject1",
                column: "AssignedProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
