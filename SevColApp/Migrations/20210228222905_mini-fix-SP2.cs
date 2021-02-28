using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class minifixSP2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var stored_Procedure = @"CREATE OR ALTER PROCEDURE[dbo].[ChangeBankAccountPassword] @accountNumber varchar(max), @newPassword varchar(max)
                                     AS
                                     BEGIN
                                     SET NOCOUNT ON;

                                     DECLARE @hash AS varbinary(max) = CONVERT(varbinary(max), HASHBYTES('SHA2_512', '@newPassword'), 2)
       
                                     UPDATE[dbo].[BankAccounts]       
                                     SET PasswordHash = @hash       
                                     WHERE AccountNumber = @accountNumber       

                                     SET NOCOUNT OFF
       

                                     SELECT       
                                     @accountNumber as Account_Number,
                                     @newPassword as Your_new_password
                                     END";
            migrationBuilder.Sql(stored_Procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
