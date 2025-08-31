using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedOrganizationRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_AspNetUsers_MembersId",
                table: "OrganizationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_Organizations_MemberOrganizationsId",
                table: "OrganizationMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationMembers",
                table: "OrganizationMembers");

            migrationBuilder.RenameColumn(
                name: "MembersId",
                table: "OrganizationMembers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "MemberOrganizationsId",
                table: "OrganizationMembers",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMembers_MembersId",
                table: "OrganizationMembers",
                newName: "IX_OrganizationMembers_UserId");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "OrganizationMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "OrganizationMembers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "OrganizationInvitations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationMembers",
                table: "OrganizationMembers",
                columns: new[] { "OrganizationId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_AspNetUsers_UserId",
                table: "OrganizationMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_Organizations_OrganizationId",
                table: "OrganizationMembers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_AspNetUsers_UserId",
                table: "OrganizationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_Organizations_OrganizationId",
                table: "OrganizationMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationMembers",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "OrganizationInvitations");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "OrganizationMembers",
                newName: "MemberOrganizationsId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OrganizationMembers",
                newName: "MembersId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMembers_UserId",
                table: "OrganizationMembers",
                newName: "IX_OrganizationMembers_MembersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationMembers",
                table: "OrganizationMembers",
                columns: new[] { "MemberOrganizationsId", "MembersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_AspNetUsers_MembersId",
                table: "OrganizationMembers",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_Organizations_MemberOrganizationsId",
                table: "OrganizationMembers",
                column: "MemberOrganizationsId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
