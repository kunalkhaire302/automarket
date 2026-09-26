using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RewardsLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "referrals",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    referred_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_snapshot = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    qualifying_booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    qualified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referrals", x => x.id);
                    table.ForeignKey(
                        name: "FK_referrals_bookings_qualifying_booking_id",
                        column: x => x.qualifying_booking_id,
                        principalSchema: "automarket",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_referrals_users_referred_user_id",
                        column: x => x.referred_user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_referrals_users_referrer_id",
                        column: x => x.referrer_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wallet_transactions",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    event_key = table.Column<string>(type: "text", nullable: false),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_transactions", x => x.id);
                    table.CheckConstraint("wallet_nonzero_points", "points <> 0");
                    table.ForeignKey(
                        name: "FK_wallet_transactions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_referrals_qualifying_booking_id",
                schema: "automarket",
                table: "referrals",
                column: "qualifying_booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_referrals_referred_user_id",
                schema: "automarket",
                table: "referrals",
                column: "referred_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_referrals_referrer_id_status",
                schema: "automarket",
                table: "referrals",
                columns: new[] { "referrer_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transactions_event_key",
                schema: "automarket",
                table: "wallet_transactions",
                column: "event_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transactions_user_id_status_created_at",
                schema: "automarket",
                table: "wallet_transactions",
                columns: new[] { "user_id", "status", "created_at" });

            migrationBuilder.Sql("REVOKE ALL ON automarket.referrals, automarket.wallet_transactions FROM anon, authenticated;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "referrals",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "wallet_transactions",
                schema: "automarket");
        }
    }
}
