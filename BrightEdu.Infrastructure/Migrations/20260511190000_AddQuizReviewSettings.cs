using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightEdu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizReviewSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowMistakesAfterAttempt",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnlyWrongAnswers",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowMistakesAfterAttempt",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ShowOnlyWrongAnswers",
                table: "Quizzes");
        }
    }
}
