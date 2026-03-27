using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AgroTempV2");

            migrationBuilder.CreateTable(
                name: "Blacklisted_Tokens",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_id = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    blacklisted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklisted_Tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Job_Category",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PayOS_Order",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_code = table.Column<long>(type: "bigint", nullable: false),
                    total_amount = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    payment_link_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    qr_code = table.Column<string>(type: "text", nullable: true),
                    checkout_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    amount_paid = table.Column<long>(type: "bigint", nullable: false),
                    amount_remaining = table.Column<long>(type: "bigint", nullable: false),
                    bin = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    account_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    account_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    return_url = table.Column<string>(type: "text", nullable: true),
                    cancel_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    canceled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "text", nullable: true),
                    last_transaction_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    buyer_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    buyer_company_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    buyer_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    buyer_phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    buyer_address = table.Column<string>(type: "text", nullable: true),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    buyer_not_get_invoice = table.Column<bool>(type: "boolean", nullable: true),
                    tax_percentage = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOS_Order", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PayOS_Webhook_Log",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_code = table.Column<long>(type: "bigint", nullable: true),
                    reference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    raw_payload = table.Column<string>(type: "text", nullable: false),
                    is_payload_parsed = table.Column<bool>(type: "boolean", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    received_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOS_Webhook_Log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    verification_token = table.Column<string>(type: "text", nullable: true),
                    verification_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PayOS_Invoice",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    invoice_number = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    issued_timestamp = table.Column<long>(type: "bigint", nullable: true),
                    issued_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    transaction_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reservation_code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    code_of_tax = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOS_Invoice", x => x.id);
                    table.ForeignKey(
                        name: "FK_PayOS_Invoice_PayOS_Order_order_id",
                        column: x => x.order_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "PayOS_Order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayOS_Order_Item",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    tax_percentage = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOS_Order_Item", x => x.id);
                    table.ForeignKey(
                        name: "FK_PayOS_Order_Item_PayOS_Order_order_id",
                        column: x => x.order_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "PayOS_Order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayOS_Transaction",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    account_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    transaction_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    virtual_account_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    virtual_account_number = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    counter_account_bank_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    counter_account_bank_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    counter_account_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    counter_account_number = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOS_Transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_PayOS_Transaction_PayOS_Order_order_id",
                        column: x => x.order_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "PayOS_Order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chat_Message",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_content = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat_Message", x => x.id);
                    table.ForeignKey(
                        name: "FK_Chat_Message_User_recipient_id",
                        column: x => x.recipient_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chat_Message_User_sender_id",
                        column: x => x.sender_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Device_Token",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expo_push_token = table.Column<string>(type: "text", nullable: false),
                    platform = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    device_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device_Token", x => x.id);
                    table.ForeignKey(
                        name: "FK_Device_Token_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Farmer",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contact_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    total_jobs_posted = table.Column<int>(type: "integer", nullable: false),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmer", x => x.id);
                    table.ForeignKey(
                        name: "FK_Farmer_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    related_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notification_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    locked_balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.id);
                    table.ForeignKey(
                        name: "FK_Wallet_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    age_range = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    primary_location = table.Column<string>(type: "text", nullable: false),
                    travel_radius_km_preference = table.Column<double>(type: "double precision", nullable: true),
                    experience_level = table.Column<int>(type: "integer", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    availability_schedule = table.Column<string>(type: "text", nullable: false),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Farm",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farmer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    farm_type = table.Column<int>(type: "integer", nullable: false),
                    livestock_count = table.Column<int>(type: "integer", nullable: true),
                    area_size = table.Column<decimal>(type: "numeric", nullable: true),
                    location_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    image_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farm", x => x.id);
                    table.ForeignKey(
                        name: "FK_Farm_Farmer_farmer_id",
                        column: x => x.farmer_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Farmer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_code = table.Column<long>(type: "bigint", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    paymnetLink_id = table.Column<string>(type: "text", nullable: false),
                    checkout_id = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Payment_Wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Wallet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Withdrawal_Requests",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    bank_account_number = table.Column<string>(type: "text", nullable: false),
                    bank_name = table.Column<string>(type: "text", nullable: false),
                    account_holder_name = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawal_Requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_Withdrawal_Requests_Wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Wallet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker_Skill",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_level = table.Column<int>(type: "integer", nullable: false),
                    years_experience = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker_Skill", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_Skill_Skill_skill_id",
                        column: x => x.skill_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Skill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Worker_Skill_Worker_worker_id",
                        column: x => x.worker_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Worker",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job_Post",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farmer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    selected_days = table.Column<List<DateTime>>(type: "timestamp with time zone[]", nullable: false),
                    workers_needed = table.Column<int>(type: "integer", nullable: false),
                    workers_accepted = table.Column<int>(type: "integer", nullable: false),
                    job_type = table.Column<int>(type: "integer", nullable: false),
                    wage_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    required_skills = table.Column<string>(type: "text", nullable: false),
                    requirements = table.Column<List<string>>(type: "text[]", nullable: false),
                    privileges = table.Column<List<string>>(type: "text[]", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_urgent = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Post", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Post_Farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Post_Farmer_farmer_id",
                        column: x => x.farmer_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Farmer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Post_Job_Category_job_category_id",
                        column: x => x.job_category_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dispute_Reports",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farmer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    worker_id = table.Column<Guid>(type: "uuid", nullable: true),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dispute_type = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    evidence_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    admin_note = table.Column<string>(type: "text", nullable: true),
                    resolved_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispute_Reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_Dispute_Reports_Farmer_farmer_id",
                        column: x => x.farmer_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Farmer",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Dispute_Reports_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dispute_Reports_User_resolved_by_id",
                        column: x => x.resolved_by_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Dispute_Reports_Worker_worker_id",
                        column: x => x.worker_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Worker",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Job_Application",
                schema: "AgroTempV2",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    cover_letter = table.Column<string>(type: "text", nullable: true),
                    applied_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    response_message = table.Column<string>(type: "text", nullable: true),
                    work_dates = table.Column<List<DateTime>>(type: "timestamp with time zone[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Application", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_Application_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Application_Worker_worker_id",
                        column: x => x.worker_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Worker",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job_Skill_Requirement",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    required_level = table.Column<int>(type: "integer", nullable: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Skill_Requirement", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Skill_Requirement_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Skill_Requirement_Skill_skill_id",
                        column: x => x.skill_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Skill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rater_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ratee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating_score = table.Column<int>(type: "integer", nullable: false),
                    review_text = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.id);
                    table.ForeignKey(
                        name: "FK_Rating_Job_Post_job_id",
                        column: x => x.job_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rating_User_ratee_id",
                        column: x => x.ratee_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_User_rater_id",
                        column: x => x.rater_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Job_Detail",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    work_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    worker_description = table.Column<string>(type: "text", nullable: true),
                    farmer_feedback = table.Column<string>(type: "text", nullable: true),
                    farmer_approved_percent = table.Column<int>(type: "integer", nullable: true),
                    job_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    worker_payment_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    refund_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Detail_Job_Application_job_application_id",
                        column: x => x.job_application_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Detail_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Detail_Worker_worker_id",
                        column: x => x.worker_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Worker",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_attachment",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cloudinary_public_id = table.Column<string>(type: "text", nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    format = table.Column<string>(type: "text", nullable: true),
                    file_size = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_attachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_attachment_Job_Detail_job_detail_id",
                        column: x => x.job_detail_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallet_Transactions",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    reference_code = table.Column<string>(type: "text", nullable: false),
                    desription = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet_Transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Wallet_Transactions_Job_Detail_job_detail_id",
                        column: x => x.job_detail_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wallet_Transactions_Wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Wallet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker_Session",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    check_in_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    check_in_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    check_out_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_out_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    total_hours_worked = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker_Session", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_Session_Job_Detail_job_detail_id",
                        column: x => x.job_detail_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Message_recipient_id",
                schema: "AgroTempV2",
                table: "Chat_Message",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Message_sender_id",
                schema: "AgroTempV2",
                table: "Chat_Message",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_Device_Token_user_id_expo_push_token",
                schema: "AgroTempV2",
                table: "Device_Token",
                columns: new[] { "user_id", "expo_push_token" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_Reports_farmer_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports",
                column: "farmer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_Reports_job_post_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_Reports_resolved_by_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports",
                column: "resolved_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_Reports_worker_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports",
                column: "worker_id");

            migrationBuilder.CreateIndex(
                name: "IX_Farm_farmer_id",
                schema: "AgroTempV2",
                table: "Farm",
                column: "farmer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Farmer_user_id",
                schema: "AgroTempV2",
                table: "Farmer",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job_Application_job_post_id",
                schema: "AgroTempV2",
                table: "Job_Application",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Application_worker_id",
                schema: "AgroTempV2",
                table: "Job_Application",
                column: "worker_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_attachment_job_detail_id",
                schema: "AgroTempV2",
                table: "job_attachment",
                column: "job_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Detail_job_application_id",
                schema: "AgroTempV2",
                table: "Job_Detail",
                column: "job_application_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Detail_job_post_id",
                schema: "AgroTempV2",
                table: "Job_Detail",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Detail_worker_id",
                schema: "AgroTempV2",
                table: "Job_Detail",
                column: "worker_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Post_farm_id",
                schema: "AgroTempV2",
                table: "Job_Post",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Post_farmer_id",
                schema: "AgroTempV2",
                table: "Job_Post",
                column: "farmer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Post_job_category_id",
                schema: "AgroTempV2",
                table: "Job_Post",
                column: "job_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Skill_Requirement_job_post_id",
                schema: "AgroTempV2",
                table: "Job_Skill_Requirement",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Skill_Requirement_skill_id",
                schema: "AgroTempV2",
                table: "Job_Skill_Requirement",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_user_id",
                schema: "AgroTempV2",
                table: "Notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_wallet_id",
                schema: "AgroTempV2",
                table: "Payment",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Invoice_order_id",
                schema: "AgroTempV2",
                table: "PayOS_Invoice",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Order_order_code",
                schema: "AgroTempV2",
                table: "PayOS_Order",
                column: "order_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Order_payment_link_id",
                schema: "AgroTempV2",
                table: "PayOS_Order",
                column: "payment_link_id");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Order_Item_order_id",
                schema: "AgroTempV2",
                table: "PayOS_Order_Item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Transaction_order_id",
                schema: "AgroTempV2",
                table: "PayOS_Transaction",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Webhook_Log_order_code",
                schema: "AgroTempV2",
                table: "PayOS_Webhook_Log",
                column: "order_code");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Webhook_Log_received_at",
                schema: "AgroTempV2",
                table: "PayOS_Webhook_Log",
                column: "received_at");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Webhook_Log_reference",
                schema: "AgroTempV2",
                table: "PayOS_Webhook_Log",
                column: "reference");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_job_id",
                schema: "AgroTempV2",
                table: "Rating",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_ratee_id",
                schema: "AgroTempV2",
                table: "Rating",
                column: "ratee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_rater_id",
                schema: "AgroTempV2",
                table: "Rating",
                column: "rater_id");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_user_id",
                schema: "AgroTempV2",
                table: "Wallet",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_Transactions_job_detail_id",
                schema: "AgroTempV2",
                table: "Wallet_Transactions",
                column: "job_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_Transactions_wallet_id",
                schema: "AgroTempV2",
                table: "Wallet_Transactions",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawal_Requests_wallet_id",
                schema: "AgroTempV2",
                table: "Withdrawal_Requests",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_user_id",
                schema: "AgroTempV2",
                table: "Worker",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Session_job_detail_id",
                schema: "AgroTempV2",
                table: "Worker_Session",
                column: "job_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Skill_skill_id",
                schema: "AgroTempV2",
                table: "Worker_Skill",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Skill_worker_id",
                schema: "AgroTempV2",
                table: "Worker_Skill",
                column: "worker_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blacklisted_Tokens",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Chat_Message",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Device_Token",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Dispute_Reports",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "job_attachment",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Job_Skill_Requirement",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "PayOS_Invoice",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "PayOS_Order_Item",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "PayOS_Transaction",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "PayOS_Webhook_Log",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Rating",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Wallet_Transactions",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Withdrawal_Requests",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Worker_Session",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Worker_Skill",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "PayOS_Order",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Wallet",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Job_Detail",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Skill",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Job_Application",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Job_Post",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Worker",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Farm",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Job_Category",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "Farmer",
                schema: "AgroTempV2");

            migrationBuilder.DropTable(
                name: "User",
                schema: "AgroTempV2");
        }
    }
}
