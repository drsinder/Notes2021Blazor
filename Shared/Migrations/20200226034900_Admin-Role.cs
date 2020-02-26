using Microsoft.EntityFrameworkCore.Migrations;

namespace Notes2021Blazor.Shared.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5d480cdb-0ed4-49e0-9e5a-450704c6008c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5c6aa3d-7993-49a8-a96a-a01274295f7a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d0b1adf9-019f-4f0a-bc0f-4271c117f287", "087218e4-fd71-4bf3-b3ad-2f4a754be0ba", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "39f4d1c2-d492-44fb-b5ad-a0c321ec058f", "9eea027d-1a6c-4451-88c6-154279806a8a", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3f9e415c-2a70-4696-9018-efc81c2e5470", "921e36fb-e2f0-4949-9976-a8a638421b15", "Guest", "GUEST" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39f4d1c2-d492-44fb-b5ad-a0c321ec058f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f9e415c-2a70-4696-9018-efc81c2e5470");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d0b1adf9-019f-4f0a-bc0f-4271c117f287");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5d480cdb-0ed4-49e0-9e5a-450704c6008c", "3d98d2fd-f97e-4e80-9e25-f5866cb720c9", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b5c6aa3d-7993-49a8-a96a-a01274295f7a", "e0b5237b-ea80-4c49-a352-e34353c2b101", "Admin", "ADMIN" });
        }
    }
}
