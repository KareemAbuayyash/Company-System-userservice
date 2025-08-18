using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveManagerIdFromDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Departments");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 7, 31, 10, 13, 4, 890, DateTimeKind.Utc).AddTicks(6999));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 7, 31, 10, 13, 4, 890, DateTimeKind.Utc).AddTicks(7001));

            migrationBuilder.UpdateData(
                table: "MainPageContents",
                keyColumn: "ContentId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 7, 31, 10, 13, 4, 890, DateTimeKind.Utc).AddTicks(7089));

            migrationBuilder.UpdateData(
                table: "MainPageContents",
                keyColumn: "ContentId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 7, 31, 10, 13, 4, 890, DateTimeKind.Utc).AddTicks(7091));

            migrationBuilder.UpdateData(
                table: "MainPageContents",
                keyColumn: "ContentId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 7, 31, 10, 13, 4, 890, DateTimeKind.Utc).AddTicks(7093));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ManagerId" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ManagerId" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "MainPageContents",
                keyColumn: "ContentId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MainPageContents",
                keyColumn: "ContentId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MainPageContents",
                keyColumn: "ContentId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId");
        }
    }
}
