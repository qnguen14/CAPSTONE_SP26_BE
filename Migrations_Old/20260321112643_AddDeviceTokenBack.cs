using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceTokenBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                CREATE TABLE IF NOT EXISTS ""AgroTempV1"".""DeviceToken"" (
                    id uuid NOT NULL,
                    user_id uuid NOT NULL,
                    expo_push_token text NOT NULL,
                    platform integer NOT NULL,
                    is_active boolean NOT NULL,
                    created_at timestamp with time zone NOT NULL,
                    last_used_at timestamp with time zone NOT NULL,
                    device_name character varying(256),
                    CONSTRAINT ""PK_DeviceToken"" PRIMARY KEY (id),
                    CONSTRAINT ""FK_DeviceToken_User_user_id"" FOREIGN KEY (user_id)
                        REFERENCES ""AgroTempV1"".""User"" (id) ON DELETE CASCADE
                );
                ");

            migrationBuilder.Sql(
                @"
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_DeviceToken_user_id_expo_push_token""
                ON ""AgroTempV1"".""DeviceToken"" (user_id, expo_push_token);
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"AgroTempV1\".\"DeviceToken\";");
        }
    }
}
