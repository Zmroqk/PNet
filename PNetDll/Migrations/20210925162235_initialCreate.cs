using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PNetDll.Migrations
{
    public partial class initialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ips",
                columns: table => new
                {
                    IpId = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Hostname = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ips", x => x.IpId);
                });

            migrationBuilder.CreateTable(
                name: "Disconnects",
                columns: table => new
                {
                    DisconnectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConnectedIpIpId = table.Column<uint>(type: "INTEGER", nullable: true),
                    DisconnectDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReconnectDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disconnects", x => x.DisconnectId);
                    table.ForeignKey(
                        name: "FK_Disconnects_Ips_ConnectedIpIpId",
                        column: x => x.ConnectedIpIpId,
                        principalTable: "Ips",
                        principalColumn: "IpId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PingsData",
                columns: table => new
                {
                    PingDataModelId = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpId = table.Column<uint>(type: "INTEGER", nullable: true),
                    Ping = table.Column<int>(type: "INTEGER", nullable: false),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PingsData", x => x.PingDataModelId);
                    table.ForeignKey(
                        name: "FK_PingsData_Ips_IpId",
                        column: x => x.IpId,
                        principalTable: "Ips",
                        principalColumn: "IpId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disconnects_ConnectedIpIpId",
                table: "Disconnects",
                column: "ConnectedIpIpId");

            migrationBuilder.CreateIndex(
                name: "IX_Ips_IPAddress",
                table: "Ips",
                column: "IPAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PingsData_IpId",
                table: "PingsData",
                column: "IpId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disconnects");

            migrationBuilder.DropTable(
                name: "PingsData");

            migrationBuilder.DropTable(
                name: "Ips");
        }
    }
}
