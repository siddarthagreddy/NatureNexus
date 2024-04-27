using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatureNexus.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Parks",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    parkCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ParkActivities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    activityID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    parkID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkActivities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParkActivities_Activities_activityID",
                        column: x => x.activityID,
                        principalTable: "Activities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParkActivities_Parks_parkID",
                        column: x => x.parkID,
                        principalTable: "Parks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "StateParks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stateID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    parkID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateParks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StateParks_Parks_parkID",
                        column: x => x.parkID,
                        principalTable: "Parks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StateParks_States_stateID",
                        column: x => x.stateID,
                        principalTable: "States",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ParkTopics",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    topicID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    parkID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkTopics", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParkTopics_Parks_parkID",
                        column: x => x.parkID,
                        principalTable: "Parks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParkTopics_Topics_topicID",
                        column: x => x.topicID,
                        principalTable: "Topics",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkActivities_activityID",
                table: "ParkActivities",
                column: "activityID");

            migrationBuilder.CreateIndex(
                name: "IX_ParkActivities_parkID",
                table: "ParkActivities",
                column: "parkID");

            migrationBuilder.CreateIndex(
                name: "IX_ParkTopics_parkID",
                table: "ParkTopics",
                column: "parkID");

            migrationBuilder.CreateIndex(
                name: "IX_ParkTopics_topicID",
                table: "ParkTopics",
                column: "topicID");

            migrationBuilder.CreateIndex(
                name: "IX_StateParks_parkID",
                table: "StateParks",
                column: "parkID");

            migrationBuilder.CreateIndex(
                name: "IX_StateParks_stateID",
                table: "StateParks",
                column: "stateID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkActivities");

            migrationBuilder.DropTable(
                name: "ParkTopics");

            migrationBuilder.DropTable(
                name: "StateParks");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Parks");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
