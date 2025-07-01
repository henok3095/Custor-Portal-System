using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustorPortalAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Projects_ProjectKey",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_creatorKey",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectKey",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_Projects_ProjectKey",
                table: "UserProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Project");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_creatorKey",
                table: "Project",
                newName: "IX_Project_creatorKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                column: "projectKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Project_ProjectKey",
                table: "Files",
                column: "ProjectKey",
                principalTable: "Project",
                principalColumn: "projectKey",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Users_creatorKey",
                table: "Projects",
                column: "creatorKey",
                principalTable: "Users",
                principalColumn: "UserKey",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Project_ProjectKey",
                table: "Tasks",
                column: "ProjectKey",
                principalTable: "Project",
                principalColumn: "projectKey",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_Project_ProjectKey",
                table: "UserProjects",
                column: "ProjectKey",
                principalTable: "Project",
                principalColumn: "projectKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Project_ProjectKey",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Users_creatorKey",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Project_ProjectKey",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_Project_ProjectKey",
                table: "UserProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Project");

            migrationBuilder.RenameIndex(
                name: "IX_Project_creatorKey",
                table: "Project",
                newName: "IX_Projects_creatorKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Project",
                column: "projectKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Projects_ProjectKey",
                table: "Files",
                column: "ProjectKey",
                principalTable: "Project",
                principalColumn: "projectKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_creatorKey",
                table: "Project",
                column: "creatorKey",
                principalTable: "Users",
                principalColumn: "UserKey",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectKey",
                table: "Tasks",
                column: "ProjectKey",
                principalTable: "Project",
                principalColumn: "projectKey",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_Projects_ProjectKey",
                table: "UserProjects",
                column: "ProjectKey",
                principalTable: "Projects",
                principalColumn: "projectKey",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
