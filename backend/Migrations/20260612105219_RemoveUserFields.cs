using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NzolaNet.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "Nacionalidade",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Utilizadores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Utilizadores",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Utilizadores",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidade",
                table: "Utilizadores",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "Utilizadores",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
