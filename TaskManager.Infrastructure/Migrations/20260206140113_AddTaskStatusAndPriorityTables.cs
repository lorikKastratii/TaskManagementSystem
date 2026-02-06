using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskStatusAndPriorityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create TaskStatuses table
            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.Id);
                });

            // Step 2: Create TaskPriorities table
            migrationBuilder.CreateTable(
                name: "TaskPriorities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPriorities", x => x.Id);
                });

            // Step 3: Insert seed data for TaskStatuses
            migrationBuilder.InsertData(
                table: "TaskStatuses",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), DateTime.UtcNow, "Task is pending and not yet started", "Todo", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), DateTime.UtcNow, "Task is currently being worked on", "InProgress", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), DateTime.UtcNow, "Task has been completed", "Done", null }
                });

            // Step 4: Insert seed data for TaskPriorities
            migrationBuilder.InsertData(
                table: "TaskPriorities",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), DateTime.UtcNow, "Low priority task", 0, "Low", null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), DateTime.UtcNow, "Medium priority task", 1, "Medium", null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), DateTime.UtcNow, "High priority task", 2, "High", null }
                });

            // Step 5: Add nullable StatusId and PriorityId columns to Tasks table
            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PriorityId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            // Step 6: Migrate existing data from enum columns to FK columns
            // Status: 0 = Todo, 1 = InProgress, 2 = Done
            migrationBuilder.Sql(@"
                UPDATE Tasks 
                SET StatusId = CASE Status 
                    WHEN 0 THEN '11111111-1111-1111-1111-111111111111'  -- Todo
                    WHEN 1 THEN '22222222-2222-2222-2222-222222222222'  -- InProgress
                    WHEN 2 THEN '33333333-3333-3333-3333-333333333333'  -- Done
                    ELSE '11111111-1111-1111-1111-111111111111'          -- Default to Todo
                END
            ");

            // Priority: 0 = Low, 1 = Medium, 2 = High
            migrationBuilder.Sql(@"
                UPDATE Tasks 
                SET PriorityId = CASE Priority 
                    WHEN 0 THEN '44444444-4444-4444-4444-444444444444'  -- Low
                    WHEN 1 THEN '55555555-5555-5555-5555-555555555555'  -- Medium
                    WHEN 2 THEN '66666666-6666-6666-6666-666666666666'  -- High
                    ELSE '55555555-5555-5555-5555-555555555555'          -- Default to Medium
                END
            ");

            // Step 7: Make StatusId and PriorityId NOT NULL
            migrationBuilder.AlterColumn<Guid>(
                name: "StatusId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PriorityId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // Step 8: Drop old enum columns
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");

            // Step 9: Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PriorityId",
                table: "Tasks",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatuses_Name",
                table: "TaskStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskPriorities_Name",
                table: "TaskPriorities",
                column: "Name",
                unique: true);

            // Step 10: Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStatuses_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "TaskStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskPriorities_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "TaskPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskPriorities_PriorityId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatuses_StatusId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskPriorities");

            migrationBuilder.DropTable(
                name: "TaskStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PriorityId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
