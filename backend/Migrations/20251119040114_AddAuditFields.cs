using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Candidate"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobPostings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Full-time"),
                    SalaryMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalaryMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Open"),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPostings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "Industry", "Logo", "Name", "UpdatedBy", "UpdatedDate", "Website" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6314), "Leading technology company specializing in software development", "Technology", null, "Tech Corp", null, null, "https://techcorp.example.com" },
                    { 2, "System", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6317), "Premier financial services provider", "Finance", null, "Finance Solutions Ltd", null, null, "https://financesolutions.example.com" },
                    { 3, "System", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6320), "Healthcare technology innovator", "Healthcare", null, "Health Plus", null, null, "https://healthplus.example.com" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CompanyId", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6526), "admin@recruitment.com", "admin", true, "AQAAAAEAACcQAAAAEHashed_Password_Here", "Admin" },
                    { 2, 0, new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6529), "hr@recruitment.com", "hr_manager", true, "AQAAAAEAACcQAAAAEHashed_Password_Here", "HR" }
                });

            migrationBuilder.InsertData(
                table: "JobPostings",
                columns: new[] { "Id", "ClosingDate", "CompanyId", "CreatedBy", "CreatedDate", "Description", "EmploymentType", "Location", "PostedDate", "Requirements", "SalaryMax", "SalaryMin", "Status", "Title", "UpdatedBy", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { 1, null, 1, "System", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6491), "We are seeking an experienced .NET developer to join our team...", "Full-time", "Ho Chi Minh City", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6488), "5+ years of experience with .NET Core, C#, SQL Server, and Azure", 3500m, 2000m, "Open", "Senior .NET Developer", null, null, null },
                    { 2, null, 1, "System", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6497), "Join our dynamic team as a Frontend Developer...", "Full-time", "Hanoi", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6496), "3+ years of experience with React, TypeScript, and modern frontend tools", 2500m, 1500m, "Open", "Frontend React Developer", null, null, null },
                    { 3, null, 2, "System", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6501), "Seeking a detail-oriented Financial Analyst...", "Full-time", "Da Nang", new DateTime(2025, 11, 19, 4, 1, 14, 210, DateTimeKind.Utc).AddTicks(6500), "Bachelor's degree in Finance, 2+ years experience in financial analysis", 2000m, 1200m, "Open", "Financial Analyst", null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_PostedDate",
                table: "JobPostings",
                column: "PostedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Status",
                table: "JobPostings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_UserId",
                table: "JobPostings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                table: "Users",
                column: "FullName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobPostings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
