using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class zelfdefixvoorandereSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var stored_Procedure = @"CREATE OR ALTER PROCEDURE[dbo].[ChangeUserPassword] @loginName varchar(max), @newPassword varchar(max)
                                     AS
                                     BEGIN
                                     SET NOCOUNT ON;

                                     DECLARE @hash AS varbinary(max) = CONVERT(varbinary(max), HASHBYTES('SHA2_512', @newPassword), 2)
       
                                     UPDATE[dbo].[Users]       
                                     SET PasswordHash = @hash       
                                     WHERE LoginName = @loginName       

                                     SET NOCOUNT OFF
       

                                     SELECT       
                                     @loginName as Login_Name,
                                     @newPassword as Your_new_password
                                     END";
            migrationBuilder.Sql(stored_Procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
