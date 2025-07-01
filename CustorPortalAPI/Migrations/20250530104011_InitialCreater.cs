using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustorPortalAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreater : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Files",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Is_current",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Users",
                newName: "Is_Active");

            migrationBuilder.RenameColumn(
                name: "Updated_at",
                table: "Users",
                newName: "Updated_At");

            migrationBuilder.RenameColumn(
                name: "Password_hash",
                table: "Users",
                newName: "Password_Hash");

            migrationBuilder.RenameColumn(
                name: "Last_name",
                table: "Users",
                newName: "Last_Name");

            migrationBuilder.RenameColumn(
                name: "First_name",
                table: "Users",
                newName: "First_Name");

            migrationBuilder.RenameColumn(
                name: "Created_at",
                table: "Users",
                newName: "Created_At");

            migrationBuilder.RenameColumn(
                name: "deadline",
                table: "Tasks",
                newName: "Deadline");

            migrationBuilder.RenameColumn(
                name: "creatorKey",
                table: "Tasks",
                newName: "CreatorKey");

            migrationBuilder.RenameColumn(
                name: "File_type",
                table: "Files",
                newName: "FileType");

            migrationBuilder.RenameColumn(
                name: "File_path",
                table: "Files",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "File_Name",
                table: "Files",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "Created_at",
                table: "Files",
                newName: "UploadedAt");

            migrationBuilder.AlterColumn<bool>(
                name: "Is_Active",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "Files",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectKeyNavigationprojectKey",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UploaderKeyNavigationUserKey",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FileKey",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    projectKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.projectKey);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_ProjectKeyNavigationprojectKey",
                table: "Files",
                column: "ProjectKeyNavigationprojectKey");

            migrationBuilder.CreateIndex(
                name: "IX_Files_UploaderKeyNavigationUserKey",
                table: "Files",
                column: "UploaderKeyNavigationUserKey");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_FileKey",
                table: "Comments",
                column: "FileKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Files_DocumentId",
                table: "Comments",
                column: "DocumentId",
                principalTable: "Files",
                principalColumn: "FileKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Files_FileKey",
                table: "Comments",
                column: "FileKey",
                principalTable: "Files",
                principalColumn: "FileKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Project_ProjectKeyNavigationprojectKey",
                table: "Files",
                column: "ProjectKeyNavigationprojectKey",
                principalTable: "Project",
                principalColumn: "projectKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_UploaderKeyNavigationUserKey",
                table: "Files",
                column: "UploaderKeyNavigationUserKey",
                principalTable: "Users",
                principalColumn: "UserKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Files_DocumentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Files_FileKey",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Project_ProjectKeyNavigationprojectKey",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_UploaderKeyNavigationUserKey",
                table: "Files");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Files_ProjectKeyNavigationprojectKey",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_UploaderKeyNavigationUserKey",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Comments_FileKey",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ProjectKeyNavigationprojectKey",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UploaderKeyNavigationUserKey",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FileKey",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "Updated_At",
                table: "Users",
                newName: "Updated_at");

            migrationBuilder.RenameColumn(
                name: "Password_Hash",
                table: "Users",
                newName: "Password_hash");

            migrationBuilder.RenameColumn(
                name: "Last_Name",
                table: "Users",
                newName: "Last_name");

            migrationBuilder.RenameColumn(
                name: "Is_Active",
                table: "Users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "First_Name",
                table: "Users",
                newName: "First_name");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "Users",
                newName: "Created_at");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "Tasks",
                newName: "deadline");

            migrationBuilder.RenameColumn(
                name: "CreatorKey",
                table: "Tasks",
                newName: "creatorKey");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "Files",
                newName: "Created_at");

            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "Files",
                newName: "File_type");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Files",
                newName: "File_path");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Files",
                newName: "File_Name");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Is_current",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Files",
                table: "Comments",
                column: "DocumentId",
                principalTable: "Files",
                principalColumn: "FileKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks",
                table: "Comments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskKey");
        }
    }
}
