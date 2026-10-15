using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartMarketing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "marketing");

            migrationBuilder.CreateTable(
                name: "AppUsers",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthSubject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    LimitsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                    table.CheckConstraint("CK_Plans_Currency", "[Currency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'");
                    table.CheckConstraint("CK_Plans_LimitsJson", "ISJSON([LimitsJson]) = 1");
                    table.CheckConstraint("CK_Plans_MonthlyPrice", "[MonthlyPrice] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                schema: "marketing",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupportsPublishing = table.Column<bool>(type: "bit", nullable: false),
                    SupportsMessaging = table.Column<bool>(type: "bit", nullable: false),
                    SupportsCompetitorTracking = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Code);
                    table.CheckConstraint("CK_Platforms_Code", "[Code] COLLATE Latin1_General_BIN NOT LIKE '%[^a-z0-9_]%'");
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                    table.CheckConstraint("CK_Tenants_Name", "LEN(TRIM([Name])) > 0");
                    table.CheckConstraint("CK_Tenants_Status", "[Status] IN (N'Active', N'Suspended', N'Closed')");
                });

            migrationBuilder.CreateTable(
                name: "PlatformDomains",
                schema: "marketing",
                columns: table => new
                {
                    Domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlatformCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformDomains", x => x.Domain);
                    table.ForeignKey(
                        name: "FK_PlatformDomains_Platforms_PlatformCode",
                        column: x => x.PlatformCode,
                        principalSchema: "marketing",
                        principalTable: "Platforms",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DefaultCurrency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                    table.UniqueConstraint("AK_Businesses_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.CheckConstraint("CK_Businesses_DefaultCurrency", "[DefaultCurrency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'");
                    table.CheckConstraint("CK_Businesses_Name", "LEN(TRIM([Name])) > 0");
                    table.CheckConstraint("CK_Businesses_Status", "[Status] IN (N'Draft', N'Active', N'Paused', N'Closed')");
                    table.ForeignKey(
                        name: "FK_Businesses_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "marketing",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PeriodStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PeriodEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.CheckConstraint("CK_Subscriptions_Period", "[PeriodEnd] > [PeriodStart]");
                    table.CheckConstraint("CK_Subscriptions_Status", "[Status] IN (N'Trialing', N'Active', N'PastDue', N'Canceled', N'Expired')");
                    table.ForeignKey(
                        name: "FK_Subscriptions_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "marketing",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "marketing",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantMemberships",
                schema: "marketing",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantMemberships", x => new { x.TenantId, x.UserId });
                    table.CheckConstraint("CK_TenantMemberships_Role", "[Role] IN (N'Owner', N'Admin', N'Editor', N'Reviewer', N'Viewer')");
                    table.CheckConstraint("CK_TenantMemberships_Status", "[Status] IN (N'Invited', N'Active', N'Disabled')");
                    table.ForeignKey(
                        name: "FK_TenantMemberships_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "marketing",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantMemberships_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "marketing",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AiExecutions",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PromptVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    InputSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    OutputSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InputTokens = table.Column<int>(type: "int", nullable: true),
                    OutputTokens = table.Column<int>(type: "int", nullable: true),
                    EstimatedCostUsd = table.Column<decimal>(type: "decimal(14,6)", precision: 14, scale: 6, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiExecutions", x => x.Id);
                    table.UniqueConstraint("AK_AiExecutions_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_AiExecutions_Cost", "[EstimatedCostUsd] IS NULL OR [EstimatedCostUsd] >= 0");
                    table.CheckConstraint("CK_AiExecutions_InputSnapshotJson", "ISJSON([InputSnapshotJson]) = 1");
                    table.CheckConstraint("CK_AiExecutions_OutputSnapshotJson", "ISJSON([OutputSnapshotJson]) = 1");
                    table.CheckConstraint("CK_AiExecutions_Status", "[Status] IN (N'Queued', N'Running', N'Succeeded', N'Failed')");
                    table.CheckConstraint("CK_AiExecutions_TaskType", "[TaskType] IN (N'Strategy', N'Copy', N'Creative', N'Review', N'Optimization', N'CompetitorAnalysis')");
                    table.CheckConstraint("CK_AiExecutions_Timing", "[CompletedAt] IS NULL OR [StartedAt] IS NULL OR [CompletedAt] >= [StartedAt]");
                    table.CheckConstraint("CK_AiExecutions_Tokens", "([InputTokens] IS NULL OR [InputTokens] >= 0) AND ([OutputTokens] IS NULL OR [OutputTokens] >= 0)");
                    table.ForeignKey(
                        name: "FK_AiExecutions_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AudienceSegments",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TargetingRulesJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudienceSegments", x => x.Id);
                    table.UniqueConstraint("AK_AudienceSegments_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_AudienceSegments_TargetingRulesJson", "ISJSON([TargetingRulesJson]) = 1");
                    table.ForeignKey(
                        name: "FK_AudienceSegments_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                    table.CheckConstraint("CK_AuditEvents_DetailsJson", "ISJSON([DetailsJson]) = 1");
                    table.ForeignKey(
                        name: "FK_AuditEvents_AppUsers_ActorUserId",
                        column: x => x.ActorUserId,
                        principalSchema: "marketing",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditEvents_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditEvents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "marketing",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrandProfiles",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Dialect = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmojiLevel = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    Guidelines = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ColorPaletteJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'[]'"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandProfiles", x => x.BusinessId);
                    table.CheckConstraint("CK_BrandProfiles_ColorPaletteJson", "ISJSON([ColorPaletteJson]) = 1");
                    table.CheckConstraint("CK_BrandProfiles_EmojiLevel", "[EmojiLevel] BETWEEN 0 AND 3");
                    table.ForeignKey(
                        name: "FK_BrandProfiles_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessHours",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    SlotNo = table.Column<byte>(type: "tinyint", nullable: false),
                    OpensAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    ClosesAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    CrossesMidnight = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHours", x => x.Id);
                    table.CheckConstraint("CK_BusinessHours_Overnight", "[CrossesMidnight] = 0 OR [ClosesAt] <= [OpensAt]");
                    table.CheckConstraint("CK_BusinessHours_SameDay", "[CrossesMidnight] = 1 OR [ClosesAt] > [OpensAt]");
                    table.CheckConstraint("CK_BusinessHours_SlotNo", "[SlotNo] > 0");
                    table.CheckConstraint("CK_BusinessHours_Weekday", "[Weekday] BETWEEN 0 AND 6");
                    table.ForeignKey(
                        name: "FK_BusinessHours_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OfferPrice = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    StartsAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndsAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.UniqueConstraint("AK_Offers_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_Offers_Currency", "[Currency] IS NULL OR [Currency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'");
                    table.CheckConstraint("CK_Offers_DiscountPercent", "[DiscountPercent] IS NULL OR ([DiscountPercent] > 0 AND [DiscountPercent] <= 100)");
                    table.CheckConstraint("CK_Offers_OfferPrice", "[OfferPrice] IS NULL OR [OfferPrice] >= 0");
                    table.CheckConstraint("CK_Offers_Period", "[EndsAt] > [StartsAt]");
                    table.CheckConstraint("CK_Offers_PriceCurrency", "([OfferPrice] IS NULL AND [Currency] IS NULL) OR ([OfferPrice] IS NOT NULL AND [Currency] IS NOT NULL)");
                    table.CheckConstraint("CK_Offers_Status", "[Status] IN (N'Draft', N'Active', N'Paused', N'Ended')");
                    table.ForeignKey(
                        name: "FK_Offers_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.UniqueConstraint("AK_Products_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_Products_Currency", "[Currency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'");
                    table.CheckConstraint("CK_Products_Name", "LEN(TRIM([Name])) > 0");
                    table.CheckConstraint("CK_Products_Price", "[Price] >= 0");
                    table.ForeignKey(
                        name: "FK_Products_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SocialAccounts",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ExternalAccountId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Handle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CredentialRef = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialAccounts", x => x.Id);
                    table.UniqueConstraint("AK_SocialAccounts_BusinessId_Id_PlatformCode", x => new { x.BusinessId, x.Id, x.PlatformCode });
                    table.CheckConstraint("CK_SocialAccounts_Status", "[Status] IN (N'Connected', N'Expired', N'Disconnected')");
                    table.ForeignKey(
                        name: "FK_SocialAccounts_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SocialAccounts_Platforms_PlatformCode",
                        column: x => x.PlatformCode,
                        principalSchema: "marketing",
                        principalTable: "Platforms",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AutomationPolicies",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutomationEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaxPostsPerDay = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)3),
                    PublishWindowStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    PublishWindowEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    MaxDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MonthlyAdBudget = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: true),
                    BudgetCurrency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: true),
                    RequireSeparateReviewer = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationPolicies", x => x.BusinessId);
                    table.CheckConstraint("CK_AutomationPolicies_Budget", "[MonthlyAdBudget] IS NULL OR [MonthlyAdBudget] >= 0");
                    table.CheckConstraint("CK_AutomationPolicies_BudgetCurrency", "([MonthlyAdBudget] IS NULL AND [BudgetCurrency] IS NULL) OR ([MonthlyAdBudget] IS NOT NULL AND [BudgetCurrency] IS NOT NULL)");
                    table.CheckConstraint("CK_AutomationPolicies_MaxDiscount", "[MaxDiscountPercent] IS NULL OR [MaxDiscountPercent] BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_AutomationPolicies_MaxPostsPerDay", "[MaxPostsPerDay] > 0");
                    table.CheckConstraint("CK_AutomationPolicies_Window", "([PublishWindowStart] IS NULL AND [PublishWindowEnd] IS NULL) OR ([PublishWindowStart] IS NOT NULL AND [PublishWindowEnd] IS NOT NULL)");
                    table.CheckConstraint("CK_AutomationPolicies_WindowOrder", "[PublishWindowStart] IS NULL OR [PublishWindowEnd] > [PublishWindowStart]");
                    table.ForeignKey(
                        name: "FK_AutomationPolicies_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomationPolicies_TenantMemberships_TenantId_UpdatedBy",
                        columns: x => new { x.TenantId, x.UpdatedBy },
                        principalSchema: "marketing",
                        principalTable: "TenantMemberships",
                        principalColumns: new[] { "TenantId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Objective = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GoalStatement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TargetMetric = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TargetValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: true),
                    BudgetCurrency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: true),
                    StartsAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndsAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.UniqueConstraint("AK_Campaigns_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_Campaigns_Budget", "[Budget] IS NULL OR [Budget] >= 0");
                    table.CheckConstraint("CK_Campaigns_BudgetCurrency", "([Budget] IS NULL AND [BudgetCurrency] IS NULL) OR ([Budget] IS NOT NULL AND [BudgetCurrency] IS NOT NULL)");
                    table.CheckConstraint("CK_Campaigns_Objective", "[Objective] IN (N'Orders', N'Messages', N'Visits', N'Awareness', N'ProductPromotion', N'Followers')");
                    table.CheckConstraint("CK_Campaigns_Period", "[StartsAt] IS NULL OR [EndsAt] IS NULL OR [EndsAt] > [StartsAt]");
                    table.CheckConstraint("CK_Campaigns_Status", "[Status] IN (N'Draft', N'Planning', N'Ready', N'Active', N'Paused', N'Completed', N'Canceled')");
                    table.CheckConstraint("CK_Campaigns_TargetValue", "[TargetValue] IS NULL OR [TargetValue] >= 0");
                    table.ForeignKey(
                        name: "FK_Campaigns_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_TenantMemberships_TenantId_CreatedBy",
                        columns: x => new { x.TenantId, x.CreatedBy },
                        principalSchema: "marketing",
                        principalTable: "TenantMemberships",
                        principalColumns: new[] { "TenantId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackedSources",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    NormalizedUrl = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    Host = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PlatformCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Handle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LastScannedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AddedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedSources", x => x.Id);
                    table.UniqueConstraint("AK_TrackedSources_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_TrackedSources_Kind", "[Kind] IN (N'Competitor', N'Professional', N'Inspiration')");
                    table.CheckConstraint("CK_TrackedSources_Status", "[Status] IN (N'Pending', N'Active', N'Paused', N'Failed')");
                    table.CheckConstraint("CK_TrackedSources_Url", "[Url] LIKE 'http%://%'");
                    table.ForeignKey(
                        name: "FK_TrackedSources_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackedSources_Platforms_PlatformCode",
                        column: x => x.PlatformCode,
                        principalSchema: "marketing",
                        principalTable: "Platforms",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackedSources_TenantMemberships_TenantId_AddedBy",
                        columns: x => new { x.TenantId, x.AddedBy },
                        principalSchema: "marketing",
                        principalTable: "TenantMemberships",
                        principalColumns: new[] { "TenantId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StrategyRuns",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WindowStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WindowEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AlgorithmVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    InputSummaryJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    AiExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyRuns", x => x.Id);
                    table.UniqueConstraint("AK_StrategyRuns_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_StrategyRuns_InputSummaryJson", "ISJSON([InputSummaryJson]) = 1");
                    table.CheckConstraint("CK_StrategyRuns_Status", "[Status] IN (N'Queued', N'Running', N'Completed', N'Failed')");
                    table.CheckConstraint("CK_StrategyRuns_Window", "[WindowEnd] > [WindowStart]");
                    table.ForeignKey(
                        name: "FK_StrategyRuns_AiExecutions_BusinessId_AiExecutionId",
                        columns: x => new { x.BusinessId, x.AiExecutionId },
                        principalSchema: "marketing",
                        principalTable: "AiExecutions",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategyRuns_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsageEvents",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AiExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Units = table.Column<decimal>(type: "decimal(14,4)", precision: 14, scale: 4, nullable: false),
                    CreditDelta = table.Column<decimal>(type: "decimal(14,4)", precision: 14, scale: 4, nullable: false),
                    EstimatedCostUsd = table.Column<decimal>(type: "decimal(14,6)", precision: 14, scale: 6, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageEvents", x => x.Id);
                    table.CheckConstraint("CK_UsageEvents_Cost", "[EstimatedCostUsd] >= 0");
                    table.CheckConstraint("CK_UsageEvents_EventType", "[EventType] IN (N'CopyGeneration', N'ImageGeneration', N'VideoGeneration', N'Publish', N'CompetitorScan', N'CreditGrant', N'CreditAdjustment')");
                    table.CheckConstraint("CK_UsageEvents_ExecutionNeedsBusiness", "[AiExecutionId] IS NULL OR [BusinessId] IS NOT NULL");
                    table.CheckConstraint("CK_UsageEvents_Units", "[Units] >= 0");
                    table.ForeignKey(
                        name: "FK_UsageEvents_AiExecutions_BusinessId_AiExecutionId",
                        columns: x => new { x.BusinessId, x.AiExecutionId },
                        principalSchema: "marketing",
                        principalTable: "AiExecutions",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsageEvents_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsageEvents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "marketing",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerContacts",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudienceSegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhoneE164 = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    WhatsAppOptIn = table.Column<bool>(type: "bit", nullable: false),
                    OptInSource = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OptedInAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OptedOutAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerContacts", x => x.Id);
                    table.UniqueConstraint("AK_CustomerContacts_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_CustomerContacts_OptIn", "[WhatsAppOptIn] = 0 OR ([OptInSource] IS NOT NULL AND [OptedInAt] IS NOT NULL)");
                    table.CheckConstraint("CK_CustomerContacts_OptInSource", "[OptInSource] IN (N'InStore', N'WebsiteForm', N'WhatsAppReply', N'ImportWithConsent')");
                    table.CheckConstraint("CK_CustomerContacts_PhoneE164", "[PhoneE164] LIKE '+[1-9]%' AND [PhoneE164] NOT LIKE '+%[^0-9]%' AND LEN([PhoneE164]) BETWEEN 8 AND 16");
                    table.ForeignKey(
                        name: "FK_CustomerContacts_AudienceSegments_BusinessId_AudienceSegmentId",
                        columns: x => new { x.BusinessId, x.AudienceSegmentId },
                        principalSchema: "marketing",
                        principalTable: "AudienceSegments",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerContacts_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrandTerms",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Term = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rule = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandTerms", x => x.Id);
                    table.CheckConstraint("CK_BrandTerms_Rule", "[Rule] IN (N'Preferred', N'Prohibited')");
                    table.CheckConstraint("CK_BrandTerms_Term", "LEN(TRIM([Term])) > 0");
                    table.ForeignKey(
                        name: "FK_BrandTerms_BrandProfiles_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "marketing",
                        principalTable: "BrandProfiles",
                        principalColumn: "BusinessId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandTerms_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaAssets",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Kind = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ObjectKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ByteSize = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAssets", x => x.Id);
                    table.UniqueConstraint("AK_MediaAssets_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_MediaAssets_ByteSize", "[ByteSize] >= 0");
                    table.CheckConstraint("CK_MediaAssets_Kind", "[Kind] IN (N'Logo', N'ProductPhoto', N'BrandImage', N'GeneratedImage', N'Video', N'Document')");
                    table.ForeignKey(
                        name: "FK_MediaAssets_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediaAssets_Products_BusinessId_ProductId",
                        columns: x => new { x.BusinessId, x.ProductId },
                        principalSchema: "marketing",
                        principalTable: "Products",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WhatsAppTemplates",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProviderStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BodyPreview = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppTemplates", x => x.Id);
                    table.UniqueConstraint("AK_WhatsAppTemplates_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_WhatsAppTemplates_Category", "[Category] IN (N'Marketing', N'Utility')");
                    table.CheckConstraint("CK_WhatsAppTemplates_PlatformCode", "[PlatformCode] = N'whatsapp'");
                    table.CheckConstraint("CK_WhatsAppTemplates_ProviderStatus", "[ProviderStatus] IN (N'Pending', N'Approved', N'Rejected', N'Paused')");
                    table.ForeignKey(
                        name: "FK_WhatsAppTemplates_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WhatsAppTemplates_SocialAccounts_BusinessId_SocialAccountId_PlatformCode",
                        columns: x => new { x.BusinessId, x.SocialAccountId, x.PlatformCode },
                        principalSchema: "marketing",
                        principalTable: "SocialAccounts",
                        principalColumns: new[] { "BusinessId", "Id", "PlatformCode" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AutoApplyRules",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoApplyRules", x => new { x.BusinessId, x.ActionType });
                    table.CheckConstraint("CK_AutoApplyRules_ActionType", "[ActionType] IN (N'NewContent', N'ReviseCopy', N'ChangeFormat', N'ChangeChannel', N'Reschedule', N'Pause')");
                    table.ForeignKey(
                        name: "FK_AutoApplyRules_AutomationPolicies_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "marketing",
                        principalTable: "AutomationPolicies",
                        principalColumn: "BusinessId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AutomationPolicyPlatforms",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationPolicyPlatforms", x => new { x.BusinessId, x.PlatformCode });
                    table.ForeignKey(
                        name: "FK_AutomationPolicyPlatforms_AutomationPolicies_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "marketing",
                        principalTable: "AutomationPolicies",
                        principalColumn: "BusinessId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomationPolicyPlatforms_Platforms_PlatformCode",
                        column: x => x.PlatformCode,
                        principalSchema: "marketing",
                        principalTable: "Platforms",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignAudiences",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudienceSegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignAudiences", x => new { x.BusinessId, x.CampaignId, x.AudienceSegmentId });
                    table.ForeignKey(
                        name: "FK_CampaignAudiences_AudienceSegments_BusinessId_AudienceSegmentId",
                        columns: x => new { x.BusinessId, x.AudienceSegmentId },
                        principalSchema: "marketing",
                        principalTable: "AudienceSegments",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAudiences_Campaigns_BusinessId_CampaignId",
                        columns: x => new { x.BusinessId, x.CampaignId },
                        principalSchema: "marketing",
                        principalTable: "Campaigns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignOffers",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignOffers", x => new { x.BusinessId, x.CampaignId, x.OfferId });
                    table.ForeignKey(
                        name: "FK_CampaignOffers_Campaigns_BusinessId_CampaignId",
                        columns: x => new { x.BusinessId, x.CampaignId },
                        principalSchema: "marketing",
                        principalTable: "Campaigns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignOffers_Offers_BusinessId_OfferId",
                        columns: x => new { x.BusinessId, x.OfferId },
                        principalSchema: "marketing",
                        principalTable: "Offers",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignPlans",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNo = table.Column<int>(type: "int", nullable: false),
                    PlanJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    GeneratedByExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignPlans", x => x.Id);
                    table.CheckConstraint("CK_CampaignPlans_PlanJson", "ISJSON([PlanJson]) = 1");
                    table.CheckConstraint("CK_CampaignPlans_VersionNo", "[VersionNo] > 0");
                    table.ForeignKey(
                        name: "FK_CampaignPlans_AiExecutions_BusinessId_GeneratedByExecutionId",
                        columns: x => new { x.BusinessId, x.GeneratedByExecutionId },
                        principalSchema: "marketing",
                        principalTable: "AiExecutions",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignPlans_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignPlans_Campaigns_BusinessId_CampaignId",
                        columns: x => new { x.BusinessId, x.CampaignId },
                        principalSchema: "marketing",
                        principalTable: "Campaigns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignProducts",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignProducts", x => new { x.BusinessId, x.CampaignId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CampaignProducts_Campaigns_BusinessId_CampaignId",
                        columns: x => new { x.BusinessId, x.CampaignId },
                        principalSchema: "marketing",
                        principalTable: "Campaigns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignProducts_Products_BusinessId_ProductId",
                        columns: x => new { x.BusinessId, x.ProductId },
                        principalSchema: "marketing",
                        principalTable: "Products",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SourcePosts",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackedSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalPostId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PostedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Format = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    HookType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CaptionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashtagsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'[]'"),
                    LikesCount = table.Column<int>(type: "int", nullable: true),
                    CommentsCount = table.Column<int>(type: "int", nullable: true),
                    SharesCount = table.Column<int>(type: "int", nullable: true),
                    ViewsCount = table.Column<long>(type: "bigint", nullable: true),
                    EngagementRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourcePosts", x => x.Id);
                    table.UniqueConstraint("AK_SourcePosts_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_SourcePosts_Counts", "([LikesCount] IS NULL OR [LikesCount] >= 0) AND ([CommentsCount] IS NULL OR [CommentsCount] >= 0) AND ([SharesCount] IS NULL OR [SharesCount] >= 0) AND ([ViewsCount] IS NULL OR [ViewsCount] >= 0)");
                    table.CheckConstraint("CK_SourcePosts_EngagementRate", "[EngagementRate] IS NULL OR [EngagementRate] >= 0");
                    table.CheckConstraint("CK_SourcePosts_Format", "[Format] IN (N'Text', N'SingleImage', N'Carousel', N'Video', N'Reel', N'Story', N'Live')");
                    table.CheckConstraint("CK_SourcePosts_HashtagsJson", "ISJSON([HashtagsJson]) = 1");
                    table.CheckConstraint("CK_SourcePosts_HookType", "[HookType] IN (N'Question', N'Offer', N'Story', N'SocialProof', N'Urgency', N'Informative', N'Humor', N'Tutorial')");
                    table.ForeignKey(
                        name: "FK_SourcePosts_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SourcePosts_TrackedSources_BusinessId_TrackedSourceId",
                        columns: x => new { x.BusinessId, x.TrackedSourceId },
                        principalSchema: "marketing",
                        principalTable: "TrackedSources",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompetitorInsights",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackedSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StrategyRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InsightType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    Confidence = table.Column<decimal>(type: "decimal(4,3)", precision: 4, scale: 3, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitorInsights", x => x.Id);
                    table.UniqueConstraint("AK_CompetitorInsights_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_CompetitorInsights_Confidence", "[Confidence] IS NULL OR [Confidence] BETWEEN 0 AND 1");
                    table.CheckConstraint("CK_CompetitorInsights_DetailsJson", "ISJSON([DetailsJson]) = 1");
                    table.CheckConstraint("CK_CompetitorInsights_InsightType", "[InsightType] IN (N'PostingTime', N'Format', N'Hook', N'Hashtags', N'Offer', N'Tone', N'Frequency', N'Engagement')");
                    table.ForeignKey(
                        name: "FK_CompetitorInsights_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompetitorInsights_StrategyRuns_BusinessId_StrategyRunId",
                        columns: x => new { x.BusinessId, x.StrategyRunId },
                        principalSchema: "marketing",
                        principalTable: "StrategyRuns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompetitorInsights_TrackedSources_BusinessId_TrackedSourceId",
                        columns: x => new { x.BusinessId, x.TrackedSourceId },
                        principalSchema: "marketing",
                        principalTable: "TrackedSources",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StrategyRecommendations",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StrategyRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProposedPayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    ProposedPublishAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Rationale = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(4,3)", precision: 4, scale: 3, nullable: true),
                    DecisionStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AppliedAutomatically = table.Column<bool>(type: "bit", nullable: false),
                    DecidedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AppliedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyRecommendations", x => x.Id);
                    table.UniqueConstraint("AK_StrategyRecommendations_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_StrategyRecommendations_ActionType", "[ActionType] IN (N'NewContent', N'ReviseCopy', N'ChangeFormat', N'ChangeChannel', N'Reschedule', N'Pause')");
                    table.CheckConstraint("CK_StrategyRecommendations_AppliedAt", "([DecisionStatus] = N'Applied' AND [AppliedAt] IS NOT NULL) OR ([DecisionStatus] <> N'Applied' AND [AppliedAt] IS NULL)");
                    table.CheckConstraint("CK_StrategyRecommendations_AutoApplied", "[AppliedAutomatically] = 0 OR ([DecisionStatus] = N'Applied' AND [DecidedBy] IS NULL)");
                    table.CheckConstraint("CK_StrategyRecommendations_Confidence", "[Confidence] IS NULL OR [Confidence] BETWEEN 0 AND 1");
                    table.CheckConstraint("CK_StrategyRecommendations_Decider", "([DecidedBy] IS NULL AND [DecidedAt] IS NULL) OR ([DecidedBy] IS NOT NULL AND [DecidedAt] IS NOT NULL)");
                    table.CheckConstraint("CK_StrategyRecommendations_DecisionStatus", "[DecisionStatus] IN (N'Pending', N'Accepted', N'Rejected', N'Applied')");
                    table.CheckConstraint("CK_StrategyRecommendations_HumanDecision", "[DecisionStatus] = N'Pending' OR [AppliedAutomatically] = 1 OR [DecidedBy] IS NOT NULL");
                    table.CheckConstraint("CK_StrategyRecommendations_ProposedPayloadJson", "ISJSON([ProposedPayloadJson]) = 1");
                    table.ForeignKey(
                        name: "FK_StrategyRecommendations_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategyRecommendations_Campaigns_BusinessId_CampaignId",
                        columns: x => new { x.BusinessId, x.CampaignId },
                        principalSchema: "marketing",
                        principalTable: "Campaigns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategyRecommendations_StrategyRuns_BusinessId_StrategyRunId",
                        columns: x => new { x.BusinessId, x.StrategyRunId },
                        principalSchema: "marketing",
                        principalTable: "StrategyRuns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategyRecommendations_TenantMemberships_TenantId_DecidedBy",
                        columns: x => new { x.TenantId, x.DecidedBy },
                        principalSchema: "marketing",
                        principalTable: "TenantMemberships",
                        principalColumns: new[] { "TenantId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeSources",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SourceUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ExtractedText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeSources", x => x.Id);
                    table.CheckConstraint("CK_KnowledgeSources_AssetOrUrl", "[AssetId] IS NOT NULL OR [SourceUrl] IS NOT NULL");
                    table.CheckConstraint("CK_KnowledgeSources_SourceType", "[SourceType] IN (N'Menu', N'Guidelines', N'Website', N'HistoricalPost', N'Other')");
                    table.CheckConstraint("CK_KnowledgeSources_Status", "[Status] IN (N'Pending', N'Processed', N'Failed', N'Disabled')");
                    table.ForeignKey(
                        name: "FK_KnowledgeSources_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnowledgeSources_MediaAssets_BusinessId_AssetId",
                        columns: x => new { x.BusinessId, x.AssetId },
                        principalSchema: "marketing",
                        principalTable: "MediaAssets",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentItems",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PlannedPublishAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    VariantGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VariantLabel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SourceRecommendationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InspiredBySourcePostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentItems", x => x.Id);
                    table.UniqueConstraint("AK_ContentItems_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.UniqueConstraint("AK_ContentItems_BusinessId_Id_PlatformCode", x => new { x.BusinessId, x.Id, x.PlatformCode });
                    table.CheckConstraint("CK_ContentItems_ContentType", "[ContentType] IN (N'Post', N'Story', N'Reel', N'Short', N'AdCopy', N'WhatsAppMessage')");
                    table.CheckConstraint("CK_ContentItems_Status", "[Status] IN (N'Draft', N'InReview', N'Approved', N'Scheduled', N'Published', N'Canceled')");
                    table.CheckConstraint("CK_ContentItems_Variant", "([VariantGroupId] IS NULL AND [VariantLabel] IS NULL) OR ([VariantGroupId] IS NOT NULL AND [VariantLabel] IS NOT NULL)");
                    table.CheckConstraint("CK_ContentItems_WhatsAppType", "([PlatformCode] = N'whatsapp' AND [ContentType] = N'WhatsAppMessage') OR ([PlatformCode] <> N'whatsapp' AND [ContentType] <> N'WhatsAppMessage')");
                    table.ForeignKey(
                        name: "FK_ContentItems_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentItems_Campaigns_BusinessId_CampaignId",
                        columns: x => new { x.BusinessId, x.CampaignId },
                        principalSchema: "marketing",
                        principalTable: "Campaigns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentItems_Platforms_PlatformCode",
                        column: x => x.PlatformCode,
                        principalSchema: "marketing",
                        principalTable: "Platforms",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentItems_SourcePosts_BusinessId_InspiredBySourcePostId",
                        columns: x => new { x.BusinessId, x.InspiredBySourcePostId },
                        principalSchema: "marketing",
                        principalTable: "SourcePosts",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentItems_StrategyRecommendations_BusinessId_SourceRecommendationId",
                        columns: x => new { x.BusinessId, x.SourceRecommendationId },
                        principalSchema: "marketing",
                        principalTable: "StrategyRecommendations",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentRevisions",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionNo = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    HookType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CallToAction = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IncludesOffer = table.Column<bool>(type: "bit", nullable: false),
                    WhatsAppTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreativeDetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    FactsSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    BrandSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    GeneratedByExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceRecommendationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentRevisions", x => x.Id);
                    table.UniqueConstraint("AK_ContentRevisions_BusinessId_ContentItemId_Id", x => new { x.BusinessId, x.ContentItemId, x.Id });
                    table.UniqueConstraint("AK_ContentRevisions_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_ContentRevisions_Author", "[GeneratedByExecutionId] IS NOT NULL OR [CreatedBy] IS NOT NULL");
                    table.CheckConstraint("CK_ContentRevisions_BrandSnapshotJson", "ISJSON([BrandSnapshotJson]) = 1");
                    table.CheckConstraint("CK_ContentRevisions_CallToAction", "[CallToAction] IN (N'OrderNow', N'SendMessage', N'VisitStore', N'LearnMore', N'Follow', N'None')");
                    table.CheckConstraint("CK_ContentRevisions_CreativeDetailsJson", "ISJSON([CreativeDetailsJson]) = 1");
                    table.CheckConstraint("CK_ContentRevisions_FactsSnapshotJson", "ISJSON([FactsSnapshotJson]) = 1");
                    table.CheckConstraint("CK_ContentRevisions_Format", "[Format] IN (N'Text', N'SingleImage', N'Carousel', N'Video', N'Reel', N'Story', N'Live')");
                    table.CheckConstraint("CK_ContentRevisions_HookType", "[HookType] IN (N'Question', N'Offer', N'Story', N'SocialProof', N'Urgency', N'Informative', N'Humor', N'Tutorial')");
                    table.CheckConstraint("CK_ContentRevisions_RevisionNo", "[RevisionNo] > 0");
                    table.ForeignKey(
                        name: "FK_ContentRevisions_AiExecutions_BusinessId_GeneratedByExecutionId",
                        columns: x => new { x.BusinessId, x.GeneratedByExecutionId },
                        principalSchema: "marketing",
                        principalTable: "AiExecutions",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentRevisions_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentRevisions_ContentItems_BusinessId_ContentItemId",
                        columns: x => new { x.BusinessId, x.ContentItemId },
                        principalSchema: "marketing",
                        principalTable: "ContentItems",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentRevisions_StrategyRecommendations_BusinessId_SourceRecommendationId",
                        columns: x => new { x.BusinessId, x.SourceRecommendationId },
                        principalSchema: "marketing",
                        principalTable: "StrategyRecommendations",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentRevisions_TenantMemberships_TenantId_CreatedBy",
                        columns: x => new { x.TenantId, x.CreatedBy },
                        principalSchema: "marketing",
                        principalTable: "TenantMemberships",
                        principalColumns: new[] { "TenantId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentRevisions_WhatsAppTemplates_BusinessId_WhatsAppTemplateId",
                        columns: x => new { x.BusinessId, x.WhatsAppTemplateId },
                        principalSchema: "marketing",
                        principalTable: "WhatsAppTemplates",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentReviews",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentReviews", x => x.Id);
                    table.UniqueConstraint("AK_ContentReviews_BusinessId_ContentItemId_RevisionId_Id", x => new { x.BusinessId, x.ContentItemId, x.RevisionId, x.Id });
                    table.CheckConstraint("CK_ContentReviews_Decision", "[Decision] IN (N'Approved', N'Rejected', N'ChangesRequested')");
                    table.ForeignKey(
                        name: "FK_ContentReviews_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentReviews_ContentRevisions_BusinessId_ContentItemId_RevisionId",
                        columns: x => new { x.BusinessId, x.ContentItemId, x.RevisionId },
                        principalSchema: "marketing",
                        principalTable: "ContentRevisions",
                        principalColumns: new[] { "BusinessId", "ContentItemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentReviews_TenantMemberships_TenantId_ReviewerId",
                        columns: x => new { x.TenantId, x.ReviewerId },
                        principalSchema: "marketing",
                        principalTable: "TenantMemberships",
                        principalColumns: new[] { "TenantId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentRevisionAssets",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentRevisionAssets", x => new { x.BusinessId, x.RevisionId, x.AssetId });
                    table.ForeignKey(
                        name: "FK_ContentRevisionAssets_ContentRevisions_BusinessId_RevisionId",
                        columns: x => new { x.BusinessId, x.RevisionId },
                        principalSchema: "marketing",
                        principalTable: "ContentRevisions",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentRevisionAssets_MediaAssets_BusinessId_AssetId",
                        columns: x => new { x.BusinessId, x.AssetId },
                        principalSchema: "marketing",
                        principalTable: "MediaAssets",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovedContentRevisions",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedContentRevisions", x => new { x.BusinessId, x.ContentItemId, x.RevisionId });
                    table.ForeignKey(
                        name: "FK_ApprovedContentRevisions_ContentReviews_BusinessId_ContentItemId_RevisionId_ReviewId",
                        columns: x => new { x.BusinessId, x.ContentItemId, x.RevisionId, x.ReviewId },
                        principalSchema: "marketing",
                        principalTable: "ContentReviews",
                        principalColumns: new[] { "BusinessId", "ContentItemId", "RevisionId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRevocations",
                schema: "marketing",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevokedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRevocations", x => new { x.BusinessId, x.ContentItemId, x.RevisionId });
                    table.ForeignKey(
                        name: "FK_ApprovalRevocations_ApprovedContentRevisions_BusinessId_ContentItemId_RevisionId",
                        columns: x => new { x.BusinessId, x.ContentItemId, x.RevisionId },
                        principalSchema: "marketing",
                        principalTable: "ApprovedContentRevisions",
                        principalColumns: new[] { "BusinessId", "ContentItemId", "RevisionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalRevocations_TenantMemberships_TenantId_RevokedBy",
                        columns: x => new { x.TenantId, x.RevokedBy },
                        principalSchema: "marketing",
                        principalTable: "TenantMemberships",
                        principalColumns: new[] { "TenantId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PublicationJobs",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SourceRecommendationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationJobs", x => x.Id);
                    table.UniqueConstraint("AK_PublicationJobs_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.UniqueConstraint("AK_PublicationJobs_BusinessId_Id_SocialAccountId", x => new { x.BusinessId, x.Id, x.SocialAccountId });
                    table.CheckConstraint("CK_PublicationJobs_AttemptCount", "[AttemptCount] >= 0");
                    table.CheckConstraint("CK_PublicationJobs_Status", "[Status] IN (N'Queued', N'Running', N'Succeeded', N'Retry', N'Failed', N'Canceled')");
                    table.ForeignKey(
                        name: "FK_PublicationJobs_ApprovedContentRevisions_BusinessId_ContentItemId_RevisionId",
                        columns: x => new { x.BusinessId, x.ContentItemId, x.RevisionId },
                        principalSchema: "marketing",
                        principalTable: "ApprovedContentRevisions",
                        principalColumns: new[] { "BusinessId", "ContentItemId", "RevisionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicationJobs_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicationJobs_ContentItems_BusinessId_ContentItemId_PlatformCode",
                        columns: x => new { x.BusinessId, x.ContentItemId, x.PlatformCode },
                        principalSchema: "marketing",
                        principalTable: "ContentItems",
                        principalColumns: new[] { "BusinessId", "Id", "PlatformCode" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicationJobs_SocialAccounts_BusinessId_SocialAccountId_PlatformCode",
                        columns: x => new { x.BusinessId, x.SocialAccountId, x.PlatformCode },
                        principalSchema: "marketing",
                        principalTable: "SocialAccounts",
                        principalColumns: new[] { "BusinessId", "Id", "PlatformCode" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicationJobs_StrategyRecommendations_BusinessId_SourceRecommendationId",
                        columns: x => new { x.BusinessId, x.SourceRecommendationId },
                        principalSchema: "marketing",
                        principalTable: "StrategyRecommendations",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageDeliveries",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicationJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ExternalMessageId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDeliveries", x => x.Id);
                    table.CheckConstraint("CK_MessageDeliveries_Status", "[Status] IN (N'Queued', N'Sent', N'Delivered', N'Read', N'Failed')");
                    table.ForeignKey(
                        name: "FK_MessageDeliveries_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageDeliveries_CustomerContacts_BusinessId_ContactId",
                        columns: x => new { x.BusinessId, x.ContactId },
                        principalSchema: "marketing",
                        principalTable: "CustomerContacts",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageDeliveries_PublicationJobs_BusinessId_PublicationJobId",
                        columns: x => new { x.BusinessId, x.PublicationJobId },
                        principalSchema: "marketing",
                        principalTable: "PublicationJobs",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Publications",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicationJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalPostId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExternalUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publications", x => x.Id);
                    table.UniqueConstraint("AK_Publications_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Publications_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Publications_PublicationJobs_BusinessId_PublicationJobId_SocialAccountId",
                        columns: x => new { x.BusinessId, x.PublicationJobId, x.SocialAccountId },
                        principalSchema: "marketing",
                        principalTable: "PublicationJobs",
                        principalColumns: new[] { "BusinessId", "Id", "SocialAccountId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConversionEvents",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ExternalEventId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevenueAmount = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: true),
                    RevenueCurrency = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: true),
                    AttributionNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionEvents", x => x.Id);
                    table.CheckConstraint("CK_ConversionEvents_EventType", "[EventType] IN (N'LinkClick', N'Message', N'Lead', N'Order', N'StoreVisit')");
                    table.CheckConstraint("CK_ConversionEvents_Revenue", "[RevenueAmount] IS NULL OR [RevenueAmount] >= 0");
                    table.CheckConstraint("CK_ConversionEvents_RevenueCurrency", "([RevenueAmount] IS NULL AND [RevenueCurrency] IS NULL) OR ([RevenueAmount] IS NOT NULL AND [RevenueCurrency] IS NOT NULL)");
                    table.CheckConstraint("CK_ConversionEvents_Source", "[Source] IN (N'TrackedLink', N'Integration', N'Manual', N'Coupon')");
                    table.ForeignKey(
                        name: "FK_ConversionEvents_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConversionEvents_Campaigns_BusinessId_CampaignId",
                        columns: x => new { x.BusinessId, x.CampaignId },
                        principalSchema: "marketing",
                        principalTable: "Campaigns",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConversionEvents_Publications_BusinessId_PublicationId",
                        columns: x => new { x.BusinessId, x.PublicationId },
                        principalSchema: "marketing",
                        principalTable: "Publications",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetricObservations",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetricCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MetricValue = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    ObservedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RawPayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricObservations", x => x.Id);
                    table.UniqueConstraint("AK_MetricObservations_BusinessId_Id", x => new { x.BusinessId, x.Id });
                    table.CheckConstraint("CK_MetricObservations_MetricCode", "[MetricCode] IN (N'Impressions', N'Reach', N'Likes', N'Comments', N'Shares', N'Saves', N'LinkClicks', N'ProfileVisits', N'MessagesStarted', N'VideoViews', N'WatchTimeSeconds', N'Follows', N'Delivered', N'Read', N'Replies')");
                    table.CheckConstraint("CK_MetricObservations_MetricValue", "[MetricValue] >= 0");
                    table.CheckConstraint("CK_MetricObservations_RawPayloadJson", "ISJSON([RawPayloadJson]) = 1");
                    table.CheckConstraint("CK_MetricObservations_Source", "[Source] IN (N'PlatformApi', N'Webhook', N'Manual')");
                    table.ForeignKey(
                        name: "FK_MetricObservations_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MetricObservations_Publications_BusinessId_PublicationId",
                        columns: x => new { x.BusinessId, x.PublicationId },
                        principalSchema: "marketing",
                        principalTable: "Publications",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecommendationEvidence",
                schema: "marketing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecommendationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetricObservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourcePostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompetitorInsightId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Interpretation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationEvidence", x => x.Id);
                    table.CheckConstraint("CK_RecommendationEvidence_ExactlyOne", "(CASE WHEN [MetricObservationId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [SourcePostId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [CompetitorInsightId] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_RecommendationEvidence_Businesses_TenantId_BusinessId",
                        columns: x => new { x.TenantId, x.BusinessId },
                        principalSchema: "marketing",
                        principalTable: "Businesses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecommendationEvidence_CompetitorInsights_BusinessId_CompetitorInsightId",
                        columns: x => new { x.BusinessId, x.CompetitorInsightId },
                        principalSchema: "marketing",
                        principalTable: "CompetitorInsights",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecommendationEvidence_MetricObservations_BusinessId_MetricObservationId",
                        columns: x => new { x.BusinessId, x.MetricObservationId },
                        principalSchema: "marketing",
                        principalTable: "MetricObservations",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecommendationEvidence_SourcePosts_BusinessId_SourcePostId",
                        columns: x => new { x.BusinessId, x.SourcePostId },
                        principalSchema: "marketing",
                        principalTable: "SourcePosts",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecommendationEvidence_StrategyRecommendations_BusinessId_RecommendationId",
                        columns: x => new { x.BusinessId, x.RecommendationId },
                        principalSchema: "marketing",
                        principalTable: "StrategyRecommendations",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "marketing",
                table: "Platforms",
                columns: new[] { "Code", "DisplayName", "IsActive", "SupportsCompetitorTracking", "SupportsMessaging", "SupportsPublishing" },
                values: new object[,]
                {
                    { "facebook", "Facebook", true, true, true, true },
                    { "instagram", "Instagram", true, true, true, true },
                    { "linkedin", "LinkedIn", true, true, false, true },
                    { "snapchat", "Snapchat", true, true, false, true },
                    { "tiktok", "TikTok", true, true, false, true },
                    { "website", "Website", true, true, false, false },
                    { "whatsapp", "WhatsApp", true, false, true, true },
                    { "x", "X (Twitter)", true, true, false, true },
                    { "youtube", "YouTube", true, true, false, true }
                });

            migrationBuilder.InsertData(
                schema: "marketing",
                table: "PlatformDomains",
                columns: new[] { "Domain", "PlatformCode" },
                values: new object[,]
                {
                    { "facebook.com", "facebook" },
                    { "fb.com", "facebook" },
                    { "fb.me", "facebook" },
                    { "instagr.am", "instagram" },
                    { "instagram.com", "instagram" },
                    { "linkedin.com", "linkedin" },
                    { "snapchat.com", "snapchat" },
                    { "tiktok.com", "tiktok" },
                    { "twitter.com", "x" },
                    { "wa.me", "whatsapp" },
                    { "whatsapp.com", "whatsapp" },
                    { "x.com", "x" },
                    { "youtu.be", "youtube" },
                    { "youtube.com", "youtube" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiExecutions_BusinessId_TaskType_CreatedAt",
                schema: "marketing",
                table: "AiExecutions",
                columns: new[] { "BusinessId", "TaskType", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AiExecutions_TenantId_BusinessId",
                schema: "marketing",
                table: "AiExecutions",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRevocations_TenantId_RevokedBy",
                schema: "marketing",
                table: "ApprovalRevocations",
                columns: new[] { "TenantId", "RevokedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedContentRevisions_BusinessId_ContentItemId_RevisionId_ReviewId",
                schema: "marketing",
                table: "ApprovedContentRevisions",
                columns: new[] { "BusinessId", "ContentItemId", "RevisionId", "ReviewId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_AuthSubject",
                schema: "marketing",
                table: "AppUsers",
                column: "AuthSubject",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Email",
                schema: "marketing",
                table: "AppUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudienceSegments_BusinessId_Name",
                schema: "marketing",
                table: "AudienceSegments",
                columns: new[] { "BusinessId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudienceSegments_TenantId_BusinessId",
                schema: "marketing",
                table: "AudienceSegments",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_ActorUserId",
                schema: "marketing",
                table: "AuditEvents",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TenantId_BusinessId",
                schema: "marketing",
                table: "AuditEvents",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TenantId_CreatedAt",
                schema: "marketing",
                table: "AuditEvents",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomationPolicies_TenantId_BusinessId",
                schema: "marketing",
                table: "AutomationPolicies",
                columns: new[] { "TenantId", "BusinessId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutomationPolicies_TenantId_UpdatedBy",
                schema: "marketing",
                table: "AutomationPolicies",
                columns: new[] { "TenantId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomationPolicyPlatforms_PlatformCode",
                schema: "marketing",
                table: "AutomationPolicyPlatforms",
                column: "PlatformCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandProfiles_TenantId_BusinessId",
                schema: "marketing",
                table: "BrandProfiles",
                columns: new[] { "TenantId", "BusinessId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandTerms_BusinessId_Rule_Term",
                schema: "marketing",
                table: "BrandTerms",
                columns: new[] { "BusinessId", "Rule", "Term" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandTerms_TenantId_BusinessId",
                schema: "marketing",
                table: "BrandTerms",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_TenantId_Status",
                schema: "marketing",
                table: "Businesses",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHours_BusinessId_Weekday_SlotNo",
                schema: "marketing",
                table: "BusinessHours",
                columns: new[] { "BusinessId", "Weekday", "SlotNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHours_TenantId_BusinessId",
                schema: "marketing",
                table: "BusinessHours",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAudiences_BusinessId_AudienceSegmentId",
                schema: "marketing",
                table: "CampaignAudiences",
                columns: new[] { "BusinessId", "AudienceSegmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignOffers_BusinessId_OfferId",
                schema: "marketing",
                table: "CampaignOffers",
                columns: new[] { "BusinessId", "OfferId" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPlans_BusinessId_CampaignId_VersionNo",
                schema: "marketing",
                table: "CampaignPlans",
                columns: new[] { "BusinessId", "CampaignId", "VersionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPlans_BusinessId_GeneratedByExecutionId",
                schema: "marketing",
                table: "CampaignPlans",
                columns: new[] { "BusinessId", "GeneratedByExecutionId" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPlans_TenantId_BusinessId",
                schema: "marketing",
                table: "CampaignPlans",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignProducts_BusinessId_ProductId",
                schema: "marketing",
                table: "CampaignProducts",
                columns: new[] { "BusinessId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_BusinessId_Status_CreatedAt",
                schema: "marketing",
                table: "Campaigns",
                columns: new[] { "BusinessId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_TenantId_BusinessId",
                schema: "marketing",
                table: "Campaigns",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_TenantId_CreatedBy",
                schema: "marketing",
                table: "Campaigns",
                columns: new[] { "TenantId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitorInsights_BusinessId_InsightType_CreatedAt",
                schema: "marketing",
                table: "CompetitorInsights",
                columns: new[] { "BusinessId", "InsightType", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitorInsights_BusinessId_StrategyRunId",
                schema: "marketing",
                table: "CompetitorInsights",
                columns: new[] { "BusinessId", "StrategyRunId" });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitorInsights_BusinessId_TrackedSourceId",
                schema: "marketing",
                table: "CompetitorInsights",
                columns: new[] { "BusinessId", "TrackedSourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitorInsights_TenantId_BusinessId",
                schema: "marketing",
                table: "CompetitorInsights",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_BusinessId_CampaignId_Status",
                schema: "marketing",
                table: "ContentItems",
                columns: new[] { "BusinessId", "CampaignId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_BusinessId_InspiredBySourcePostId",
                schema: "marketing",
                table: "ContentItems",
                columns: new[] { "BusinessId", "InspiredBySourcePostId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_BusinessId_SourceRecommendationId",
                schema: "marketing",
                table: "ContentItems",
                columns: new[] { "BusinessId", "SourceRecommendationId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_BusinessId_VariantGroupId_VariantLabel",
                schema: "marketing",
                table: "ContentItems",
                columns: new[] { "BusinessId", "VariantGroupId", "VariantLabel" },
                unique: true,
                filter: "[VariantGroupId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_PlatformCode",
                schema: "marketing",
                table: "ContentItems",
                column: "PlatformCode");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_TenantId_BusinessId",
                schema: "marketing",
                table: "ContentItems",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentReviews_TenantId_BusinessId",
                schema: "marketing",
                table: "ContentReviews",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentReviews_TenantId_ReviewerId",
                schema: "marketing",
                table: "ContentReviews",
                columns: new[] { "TenantId", "ReviewerId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisionAssets_BusinessId_AssetId",
                schema: "marketing",
                table: "ContentRevisionAssets",
                columns: new[] { "BusinessId", "AssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_BusinessId_ContentItemId_RevisionNo",
                schema: "marketing",
                table: "ContentRevisions",
                columns: new[] { "BusinessId", "ContentItemId", "RevisionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_BusinessId_GeneratedByExecutionId",
                schema: "marketing",
                table: "ContentRevisions",
                columns: new[] { "BusinessId", "GeneratedByExecutionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_BusinessId_SourceRecommendationId",
                schema: "marketing",
                table: "ContentRevisions",
                columns: new[] { "BusinessId", "SourceRecommendationId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_BusinessId_WhatsAppTemplateId",
                schema: "marketing",
                table: "ContentRevisions",
                columns: new[] { "BusinessId", "WhatsAppTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_TenantId_BusinessId",
                schema: "marketing",
                table: "ContentRevisions",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_TenantId_CreatedBy",
                schema: "marketing",
                table: "ContentRevisions",
                columns: new[] { "TenantId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionEvents_BusinessId_CampaignId",
                schema: "marketing",
                table: "ConversionEvents",
                columns: new[] { "BusinessId", "CampaignId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionEvents_BusinessId_PublicationId_EventType",
                schema: "marketing",
                table: "ConversionEvents",
                columns: new[] { "BusinessId", "PublicationId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionEvents_BusinessId_Source_ExternalEventId",
                schema: "marketing",
                table: "ConversionEvents",
                columns: new[] { "BusinessId", "Source", "ExternalEventId" },
                unique: true,
                filter: "[ExternalEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionEvents_TenantId_BusinessId",
                schema: "marketing",
                table: "ConversionEvents",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_BusinessId_AudienceSegmentId",
                schema: "marketing",
                table: "CustomerContacts",
                columns: new[] { "BusinessId", "AudienceSegmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_BusinessId_PhoneE164",
                schema: "marketing",
                table: "CustomerContacts",
                columns: new[] { "BusinessId", "PhoneE164" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_TenantId_BusinessId",
                schema: "marketing",
                table: "CustomerContacts",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_BusinessId_AssetId",
                schema: "marketing",
                table: "KnowledgeSources",
                columns: new[] { "BusinessId", "AssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeSources_TenantId_BusinessId",
                schema: "marketing",
                table: "KnowledgeSources",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_BusinessId_ObjectKey",
                schema: "marketing",
                table: "MediaAssets",
                columns: new[] { "BusinessId", "ObjectKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_BusinessId_ProductId",
                schema: "marketing",
                table: "MediaAssets",
                columns: new[] { "BusinessId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_TenantId_BusinessId",
                schema: "marketing",
                table: "MediaAssets",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageDeliveries_BusinessId_ContactId",
                schema: "marketing",
                table: "MessageDeliveries",
                columns: new[] { "BusinessId", "ContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageDeliveries_BusinessId_PublicationJobId_ContactId",
                schema: "marketing",
                table: "MessageDeliveries",
                columns: new[] { "BusinessId", "PublicationJobId", "ContactId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageDeliveries_TenantId_BusinessId",
                schema: "marketing",
                table: "MessageDeliveries",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_MetricObservations_BusinessId_PublicationId_MetricCode_ObservedAt",
                schema: "marketing",
                table: "MetricObservations",
                columns: new[] { "BusinessId", "PublicationId", "MetricCode", "ObservedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetricObservations_TenantId_BusinessId",
                schema: "marketing",
                table: "MetricObservations",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TenantId_BusinessId",
                schema: "marketing",
                table: "Offers",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_Plans_Code",
                schema: "marketing",
                table: "Plans",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformDomains_PlatformCode",
                schema: "marketing",
                table: "PlatformDomains",
                column: "PlatformCode");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BusinessId_IsActive",
                schema: "marketing",
                table: "Products",
                columns: new[] { "BusinessId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BusinessId_Sku",
                schema: "marketing",
                table: "Products",
                columns: new[] { "BusinessId", "Sku" },
                unique: true,
                filter: "[Sku] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_BusinessId",
                schema: "marketing",
                table: "Products",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_BusinessId_ContentItemId_PlatformCode",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "BusinessId", "ContentItemId", "PlatformCode" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_BusinessId_ContentItemId_RevisionId",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "BusinessId", "ContentItemId", "RevisionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_BusinessId_ScheduledAt",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "BusinessId", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_BusinessId_SocialAccountId_IdempotencyKey",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "BusinessId", "SocialAccountId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_BusinessId_SocialAccountId_PlatformCode",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "BusinessId", "SocialAccountId", "PlatformCode" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_BusinessId_SourceRecommendationId",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "BusinessId", "SourceRecommendationId" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_Due",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "Status", "ScheduledAt" },
                filter: "[Status] IN (N'Queued', N'Retry')");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationJobs_TenantId_BusinessId",
                schema: "marketing",
                table: "PublicationJobs",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_Publications_BusinessId_PublicationJobId_SocialAccountId",
                schema: "marketing",
                table: "Publications",
                columns: new[] { "BusinessId", "PublicationJobId", "SocialAccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publications_BusinessId_SocialAccountId_ExternalPostId",
                schema: "marketing",
                table: "Publications",
                columns: new[] { "BusinessId", "SocialAccountId", "ExternalPostId" },
                unique: true,
                filter: "[ExternalPostId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_TenantId_BusinessId",
                schema: "marketing",
                table: "Publications",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationEvidence_BusinessId_CompetitorInsightId",
                schema: "marketing",
                table: "RecommendationEvidence",
                columns: new[] { "BusinessId", "CompetitorInsightId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationEvidence_BusinessId_MetricObservationId",
                schema: "marketing",
                table: "RecommendationEvidence",
                columns: new[] { "BusinessId", "MetricObservationId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationEvidence_BusinessId_RecommendationId",
                schema: "marketing",
                table: "RecommendationEvidence",
                columns: new[] { "BusinessId", "RecommendationId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationEvidence_BusinessId_SourcePostId",
                schema: "marketing",
                table: "RecommendationEvidence",
                columns: new[] { "BusinessId", "SourcePostId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationEvidence_TenantId_BusinessId",
                schema: "marketing",
                table: "RecommendationEvidence",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_PlatformCode",
                schema: "marketing",
                table: "SocialAccounts",
                column: "PlatformCode");

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_TenantId_BusinessId",
                schema: "marketing",
                table: "SocialAccounts",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_TenantId_PlatformCode_ExternalAccountId",
                schema: "marketing",
                table: "SocialAccounts",
                columns: new[] { "TenantId", "PlatformCode", "ExternalAccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SourcePosts_BusinessId_TrackedSourceId_ExternalPostId",
                schema: "marketing",
                table: "SourcePosts",
                columns: new[] { "BusinessId", "TrackedSourceId", "ExternalPostId" },
                unique: true,
                filter: "[ExternalPostId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SourcePosts_BusinessId_TrackedSourceId_PostedAt",
                schema: "marketing",
                table: "SourcePosts",
                columns: new[] { "BusinessId", "TrackedSourceId", "PostedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SourcePosts_TenantId_BusinessId",
                schema: "marketing",
                table: "SourcePosts",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRecommendations_BusinessId_CampaignId",
                schema: "marketing",
                table: "StrategyRecommendations",
                columns: new[] { "BusinessId", "CampaignId" });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRecommendations_BusinessId_StrategyRunId",
                schema: "marketing",
                table: "StrategyRecommendations",
                columns: new[] { "BusinessId", "StrategyRunId" });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRecommendations_Pending",
                schema: "marketing",
                table: "StrategyRecommendations",
                columns: new[] { "BusinessId", "CreatedAt" },
                filter: "[DecisionStatus] = N'Pending'");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRecommendations_TenantId_BusinessId",
                schema: "marketing",
                table: "StrategyRecommendations",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRecommendations_TenantId_DecidedBy",
                schema: "marketing",
                table: "StrategyRecommendations",
                columns: new[] { "TenantId", "DecidedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRuns_BusinessId_AiExecutionId",
                schema: "marketing",
                table: "StrategyRuns",
                columns: new[] { "BusinessId", "AiExecutionId" });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRuns_BusinessId_WindowEnd",
                schema: "marketing",
                table: "StrategyRuns",
                columns: new[] { "BusinessId", "WindowEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRuns_TenantId_BusinessId",
                schema: "marketing",
                table: "StrategyRuns",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                schema: "marketing",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ProviderReference",
                schema: "marketing",
                table: "Subscriptions",
                column: "ProviderReference",
                unique: true,
                filter: "[ProviderReference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Subscriptions_OneCurrentPerTenant",
                schema: "marketing",
                table: "Subscriptions",
                column: "TenantId",
                unique: true,
                filter: "[Status] IN (N'Trialing', N'Active')");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_UserId",
                schema: "marketing",
                table: "TenantMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedSources_BusinessId_Kind_Status",
                schema: "marketing",
                table: "TrackedSources",
                columns: new[] { "BusinessId", "Kind", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedSources_BusinessId_NormalizedUrl",
                schema: "marketing",
                table: "TrackedSources",
                columns: new[] { "BusinessId", "NormalizedUrl" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedSources_PlatformCode",
                schema: "marketing",
                table: "TrackedSources",
                column: "PlatformCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedSources_TenantId_AddedBy",
                schema: "marketing",
                table: "TrackedSources",
                columns: new[] { "TenantId", "AddedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedSources_TenantId_BusinessId",
                schema: "marketing",
                table: "TrackedSources",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_UsageEvents_BusinessId_AiExecutionId",
                schema: "marketing",
                table: "UsageEvents",
                columns: new[] { "BusinessId", "AiExecutionId" });

            migrationBuilder.CreateIndex(
                name: "IX_UsageEvents_TenantId_BusinessId",
                schema: "marketing",
                table: "UsageEvents",
                columns: new[] { "TenantId", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_UsageEvents_TenantId_IdempotencyKey",
                schema: "marketing",
                table: "UsageEvents",
                columns: new[] { "TenantId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsageEvents_TenantId_OccurredAt",
                schema: "marketing",
                table: "UsageEvents",
                columns: new[] { "TenantId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppTemplates_BusinessId_SocialAccountId_PlatformCode",
                schema: "marketing",
                table: "WhatsAppTemplates",
                columns: new[] { "BusinessId", "SocialAccountId", "PlatformCode" });

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppTemplates_BusinessId_SocialAccountId_TemplateName_LanguageCode",
                schema: "marketing",
                table: "WhatsAppTemplates",
                columns: new[] { "BusinessId", "SocialAccountId", "TemplateName", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppTemplates_TenantId_BusinessId",
                schema: "marketing",
                table: "WhatsAppTemplates",
                columns: new[] { "TenantId", "BusinessId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRevocations",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "AuditEvents",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "AutoApplyRules",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "AutomationPolicyPlatforms",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "BrandTerms",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "BusinessHours",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "CampaignAudiences",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "CampaignOffers",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "CampaignPlans",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "CampaignProducts",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "ContentRevisionAssets",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "ConversionEvents",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "KnowledgeSources",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "MessageDeliveries",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "PlatformDomains",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "RecommendationEvidence",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "UsageEvents",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "AutomationPolicies",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "BrandProfiles",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Offers",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "MediaAssets",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "CustomerContacts",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "CompetitorInsights",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "MetricObservations",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "AudienceSegments",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Publications",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "PublicationJobs",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "ApprovedContentRevisions",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "ContentReviews",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "ContentRevisions",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "ContentItems",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "WhatsAppTemplates",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "SourcePosts",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "StrategyRecommendations",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "SocialAccounts",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "TrackedSources",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Campaigns",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "StrategyRuns",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Platforms",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "TenantMemberships",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "AiExecutions",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "AppUsers",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Businesses",
                schema: "marketing");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "marketing");
        }
    }
}
