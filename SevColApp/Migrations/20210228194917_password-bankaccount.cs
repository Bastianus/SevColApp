using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class passwordbankaccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "BankAccounts",
                type: "varbinary(max)",
                nullable: true);

            var stored_Procedure1 = @"CREATE OR ALTER PROCEDURE[dbo].[ChangeUserPassword] @loginName nvarchar(max), @newPassword nvarchar(max)
                                     AS
                                     BEGIN
                                     SET NOCOUNT ON;

                                     DECLARE @hash AS varbinary(max) = CONVERT(varbinary(max), HASHBYTES('SHA2_512', '@newPassword'), 2)
       
                                     UPDATE[dbo].[Users]       
                                     SET PasswordHash = @hash       
                                     WHERE LoginName = @loginName       

                                     SET NOCOUNT OFF
       

                                     SELECT       
                                     @loginName as Login_Name,
                                     @newPassword as Your_new_password
                                     END";

            var stored_Procedure2 = @"CREATE OR ALTER PROCEDURE[dbo].[ChangeBankAccountPassword] @accountNumber nvarchar(max), @newPassword nvarchar(max)
                                     AS
                                     BEGIN
                                     SET NOCOUNT ON;

                                     DECLARE @hash AS varbinary(max) = CONVERT(varbinary(max), HASHBYTES('SHA2_512', '@newPassword'), 2)
       
                                     UPDATE[dbo].[BankAccount]       
                                     SET PasswordHash = @hash       
                                     WHERE AccountNumber = @accountNumber       

                                     SET NOCOUNT OFF
       

                                     SELECT       
                                     @accountNumber as Account_Number,
                                     @newPassword as Your_new_password
                                     END";
            migrationBuilder.Sql(stored_Procedure1);
            migrationBuilder.Sql(stored_Procedure2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "BankAccounts");
        }
    }
}
