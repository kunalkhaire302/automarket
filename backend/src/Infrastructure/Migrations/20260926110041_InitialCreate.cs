using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "automarket");

            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm WITH SCHEMA extensions;");
            migrationBuilder.Sql("REVOKE ALL ON SCHEMA automarket FROM PUBLIC, anon, authenticated;");

            migrationBuilder.CreateTable(
                name: "cities",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    service_radius_km = table.Column<double>(type: "double precision", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_profiles",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    version = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<string>(type: "text", nullable: false),
                    service_interval_km = table.Column<int>(type: "integer", nullable: false),
                    service_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    annual_consumables_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    older_vehicle_multiplier = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    high_maintenance_age = table.Column<int>(type: "integer", nullable: false),
                    high_maintenance_km = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenance_profiles", x => x.id);
                    table.CheckConstraint("maintenance_positive_costs", "service_interval_km > 0 AND service_cost >= 0 AND annual_consumables_cost >= 0 AND older_vehicle_multiplier >= 1");
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    referral_code = table.Column<string>(type: "text", nullable: false),
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_users_referrer_id",
                        column: x => x.referrer_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pricing_rules",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    body_type = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    multiplier = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    kind = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricing_rules", x => x.id);
                    table.CheckConstraint("pricing_rule_bounds", "multiplier BETWEEN 0.5 AND 1.5 AND ends_at > starts_at");
                    table.ForeignKey(
                        name: "FK_pricing_rules_cities_city_id",
                        column: x => x.city_id,
                        principalSchema: "automarket",
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_centers",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    kind = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_centers", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_centers_cities_city_id",
                        column: x => x.city_id,
                        principalSchema: "automarket",
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "text", nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cars",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: true),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    variant = table.Column<string>(type: "text", nullable: false),
                    manufacturing_year = table.Column<int>(type: "integer", nullable: false),
                    registration_year = table.Column<int>(type: "integer", nullable: false),
                    kilometers = table.Column<int>(type: "integer", nullable: false),
                    ownership_count = table.Column<int>(type: "integer", nullable: false),
                    fuel_type = table.Column<string>(type: "text", nullable: false),
                    transmission = table.Column<string>(type: "text", nullable: false),
                    body_type = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cars", x => x.id);
                    table.CheckConstraint("car_positive_price", "price > 0");
                    table.CheckConstraint("car_valid_usage", "kilometers >= 0 AND ownership_count >= 1");
                    table.ForeignKey(
                        name: "FK_cars_cities_city_id",
                        column: x => x.city_id,
                        principalSchema: "automarket",
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cars_users_seller_id",
                        column: x => x.seller_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification_preferences",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    in_app = table.Column<bool>(type: "boolean", nullable: false),
                    push = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_preferences", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_preferences_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    deep_link = table.Column<string>(type: "text", nullable: false),
                    event_key = table.Column<string>(type: "text", nullable: false),
                    read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_sessions",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_sessions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_center_id = table.Column<Guid>(type: "uuid", nullable: false),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    appointment_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.id);
                    table.CheckConstraint("appointment_duration", "ends_at > starts_at");
                    table.ForeignKey(
                        name: "FK_appointments_cars_car_id",
                        column: x => x.car_id,
                        principalSchema: "automarket",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_service_centers_service_center_id",
                        column: x => x.service_center_id,
                        principalSchema: "automarket",
                        principalTable: "service_centers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "car_images",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    storage_key = table.Column<string>(type: "text", nullable: false),
                    image_type = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_car_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_car_images_cars_car_id",
                        column: x => x.car_id,
                        principalSchema: "automarket",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "saved_cars",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saved_cars", x => x.id);
                    table.ForeignKey(
                        name: "FK_saved_cars_cars_car_id",
                        column: x => x.car_id,
                        principalSchema: "automarket",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_saved_cars_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seller_submissions",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    review_notes = table.Column<string>(type: "text", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seller_submissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_seller_submissions_cars_car_id",
                        column: x => x.car_id,
                        principalSchema: "automarket",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_seller_submissions_users_seller_id",
                        column: x => x.seller_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                schema: "automarket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_reference = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    idempotency_key = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.id);
                    table.ForeignKey(
                        name: "FK_bookings_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalSchema: "automarket",
                        principalTable: "appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_cars_car_id",
                        column: x => x.car_id,
                        principalSchema: "automarket",
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "automarket",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_car_id_starts_at",
                schema: "automarket",
                table: "appointments",
                columns: new[] { "car_id", "starts_at" },
                unique: true,
                filter: "\"status\" NOT IN ('CANCELLED', 'NO_SHOW')");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_service_center_id",
                schema: "automarket",
                table: "appointments",
                column: "service_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_user_id_starts_at",
                schema: "automarket",
                table: "appointments",
                columns: new[] { "user_id", "starts_at" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_actor_user_id",
                schema: "automarket",
                table: "audit_logs",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_appointment_id",
                schema: "automarket",
                table: "bookings",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_booking_reference",
                schema: "automarket",
                table: "bookings",
                column: "booking_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_car_id",
                schema: "automarket",
                table: "bookings",
                column: "car_id",
                unique: true,
                filter: "\"status\" NOT IN ('CANCELLED', 'EXPIRED')");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_user_id_idempotency_key",
                schema: "automarket",
                table: "bookings",
                columns: new[] { "user_id", "idempotency_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_car_images_car_id",
                schema: "automarket",
                table: "car_images",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_car_images_storage_key",
                schema: "automarket",
                table: "car_images",
                column: "storage_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cars_brand_model",
                schema: "automarket",
                table: "cars",
                columns: new[] { "brand", "model" });

            migrationBuilder.CreateIndex(
                name: "IX_cars_city_id",
                schema: "automarket",
                table: "cars",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_created_at",
                schema: "automarket",
                table: "cars",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_cars_seller_id",
                schema: "automarket",
                table: "cars",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_status_city_id_price",
                schema: "automarket",
                table: "cars",
                columns: new[] { "status", "city_id", "price" });

            migrationBuilder.CreateIndex(
                name: "IX_cities_name_state",
                schema: "automarket",
                table: "cities",
                columns: new[] { "name", "state" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_profiles_brand_model_version",
                schema: "automarket",
                table: "maintenance_profiles",
                columns: new[] { "brand", "model", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_preferences_user_id_category",
                schema: "automarket",
                table: "notification_preferences",
                columns: new[] { "user_id", "category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id_event_key",
                schema: "automarket",
                table: "notifications",
                columns: new[] { "user_id", "event_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id_read_at_created_at",
                schema: "automarket",
                table: "notifications",
                columns: new[] { "user_id", "read_at", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_pricing_rules_city_id",
                schema: "automarket",
                table: "pricing_rules",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_sessions_token_hash",
                schema: "automarket",
                table: "refresh_sessions",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_sessions_user_id",
                schema: "automarket",
                table: "refresh_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_saved_cars_car_id",
                schema: "automarket",
                table: "saved_cars",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_saved_cars_user_id_car_id_kind",
                schema: "automarket",
                table: "saved_cars",
                columns: new[] { "user_id", "car_id", "kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seller_submissions_car_id",
                schema: "automarket",
                table: "seller_submissions",
                column: "car_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seller_submissions_seller_id",
                schema: "automarket",
                table: "seller_submissions",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_centers_city_id",
                schema: "automarket",
                table: "service_centers",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "automarket",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_referral_code",
                schema: "automarket",
                table: "users",
                column: "referral_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_referrer_id",
                schema: "automarket",
                table: "users",
                column: "referrer_id");

            migrationBuilder.Sql("REVOKE ALL ON ALL TABLES IN SCHEMA automarket FROM anon, authenticated;");
            migrationBuilder.Sql("ALTER DEFAULT PRIVILEGES IN SCHEMA automarket REVOKE ALL ON TABLES FROM anon, authenticated;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "bookings",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "car_images",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "maintenance_profiles",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "notification_preferences",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "pricing_rules",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "refresh_sessions",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "saved_cars",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "seller_submissions",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "appointments",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "cars",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "service_centers",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "users",
                schema: "automarket");

            migrationBuilder.DropTable(
                name: "cities",
                schema: "automarket");
        }
    }
}
