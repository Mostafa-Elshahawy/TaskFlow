using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedProjectMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectManagers",
                table: "ProjectManagers");

            migrationBuilder.RenameColumn(
                name: "MembersId",
                table: "ProjectMembers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AssignedProjectsId",
                table: "ProjectMembers",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_MembersId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_UserId");

            migrationBuilder.RenameColumn(
                name: "ManagersId",
                table: "ProjectManagers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ManagedProjectsId",
                table: "ProjectManagers",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectManagers_ManagersId",
                table: "ProjectManagers",
                newName: "IX_ProjectManagers_UserId");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ProjectMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "ProjectMembers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ProjectManagers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectManagers",
                table: "ProjectManagers",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectManagers_AspNetUsers_UserId",
                table: "ProjectManagers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectManagers_Projects_ProjectId",
                table: "ProjectManagers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_UserId",
                table: "ProjectMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectManagers_AspNetUsers_UserId",
                table: "ProjectManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectManagers_Projects_ProjectId",
                table: "ProjectManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_UserId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectManagers",
                table: "ProjectManagers");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjectManagers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProjectMembers",
                newName: "AssignedProjectsId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProjectMembers",
                newName: "MembersId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_MembersId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProjectManagers",
                newName: "ManagedProjectsId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProjectManagers",
                newName: "ManagersId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectManagers_UserId",
                table: "ProjectManagers",
                newName: "IX_ProjectManagers_ManagersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                columns: new[] { "AssignedProjectsId", "MembersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectManagers",
                table: "ProjectManagers",
                columns: new[] { "ManagedProjectsId", "ManagersId" });

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
        }
    }
}
