IF OBJECT_ID(N'[marketing].[__EFMigrationsHistory]') IS NULL
BEGIN
    IF SCHEMA_ID(N'marketing') IS NULL EXEC(N'CREATE SCHEMA [marketing];');
    CREATE TABLE [marketing].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF SCHEMA_ID(N'marketing') IS NULL EXEC(N'CREATE SCHEMA [marketing];');

CREATE TABLE [marketing].[AppUsers] (
    [Id] uniqueidentifier NOT NULL,
    [AuthSubject] nvarchar(200) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [FullName] nvarchar(200) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_AppUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [marketing].[Plans] (
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [MonthlyPrice] decimal(12,2) NOT NULL,
    [Currency] nchar(3) NOT NULL,
    [LimitsJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Plans] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Plans_Currency] CHECK ([Currency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'),
    CONSTRAINT [CK_Plans_LimitsJson] CHECK (ISJSON([LimitsJson]) = 1),
    CONSTRAINT [CK_Plans_MonthlyPrice] CHECK ([MonthlyPrice] >= 0)
);

CREATE TABLE [marketing].[Platforms] (
    [Code] nvarchar(30) NOT NULL,
    [DisplayName] nvarchar(100) NOT NULL,
    [SupportsPublishing] bit NOT NULL,
    [SupportsMessaging] bit NOT NULL,
    [SupportsCompetitorTracking] bit NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Platforms] PRIMARY KEY ([Code]),
    CONSTRAINT [CK_Platforms_Code] CHECK ([Code] COLLATE Latin1_General_BIN NOT LIKE '%[^a-z0-9_]%')
);

CREATE TABLE [marketing].[Tenants] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Tenants_Name] CHECK (LEN(TRIM([Name])) > 0),
    CONSTRAINT [CK_Tenants_Status] CHECK ([Status] IN (N'Active', N'Suspended', N'Closed'))
);

CREATE TABLE [marketing].[PlatformDomains] (
    [Domain] nvarchar(100) NOT NULL,
    [PlatformCode] nvarchar(30) NOT NULL,
    CONSTRAINT [PK_PlatformDomains] PRIMARY KEY ([Domain]),
    CONSTRAINT [FK_PlatformDomains_Platforms_PlatformCode] FOREIGN KEY ([PlatformCode]) REFERENCES [marketing].[Platforms] ([Code]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[Businesses] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Category] nvarchar(100) NOT NULL,
    [CountryCode] nchar(2) NULL,
    [City] nvarchar(100) NULL,
    [Address] nvarchar(300) NULL,
    [TimeZoneId] nvarchar(64) NOT NULL,
    [DefaultCurrency] nchar(3) NOT NULL,
    [ContactPhone] nvarchar(30) NULL,
    [Description] nvarchar(2000) NULL,
    [Status] nvarchar(30) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UpdatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_Businesses] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_Businesses_TenantId_Id] UNIQUE ([TenantId], [Id]),
    CONSTRAINT [CK_Businesses_DefaultCurrency] CHECK ([DefaultCurrency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'),
    CONSTRAINT [CK_Businesses_Name] CHECK (LEN(TRIM([Name])) > 0),
    CONSTRAINT [CK_Businesses_Status] CHECK ([Status] IN (N'Draft', N'Active', N'Paused', N'Closed')),
    CONSTRAINT [FK_Businesses_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [marketing].[Tenants] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[Subscriptions] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [PlanId] uniqueidentifier NOT NULL,
    [ProviderReference] nvarchar(200) NULL,
    [Status] nvarchar(30) NOT NULL,
    [PeriodStart] datetimeoffset NOT NULL,
    [PeriodEnd] datetimeoffset NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_Subscriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Subscriptions_Period] CHECK ([PeriodEnd] > [PeriodStart]),
    CONSTRAINT [CK_Subscriptions_Status] CHECK ([Status] IN (N'Trialing', N'Active', N'PastDue', N'Canceled', N'Expired')),
    CONSTRAINT [FK_Subscriptions_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [marketing].[Plans] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Subscriptions_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [marketing].[Tenants] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[TenantMemberships] (
    [TenantId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Role] nvarchar(30) NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [JoinedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_TenantMemberships] PRIMARY KEY ([TenantId], [UserId]),
    CONSTRAINT [CK_TenantMemberships_Role] CHECK ([Role] IN (N'Owner', N'Admin', N'Editor', N'Reviewer', N'Viewer')),
    CONSTRAINT [CK_TenantMemberships_Status] CHECK ([Status] IN (N'Invited', N'Active', N'Disabled')),
    CONSTRAINT [FK_TenantMemberships_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [marketing].[AppUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TenantMemberships_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [marketing].[Tenants] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[AiExecutions] (
    [Id] uniqueidentifier NOT NULL,
    [TaskType] nvarchar(30) NOT NULL,
    [ModelName] nvarchar(100) NOT NULL,
    [PromptVersion] nvarchar(50) NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [InputSnapshotJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [OutputSnapshotJson] nvarchar(max) NULL,
    [InputTokens] int NULL,
    [OutputTokens] int NULL,
    [EstimatedCostUsd] decimal(14,6) NULL,
    [ErrorCode] nvarchar(100) NULL,
    [StartedAt] datetimeoffset NULL,
    [CompletedAt] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AiExecutions] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_AiExecutions_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_AiExecutions_Cost] CHECK ([EstimatedCostUsd] IS NULL OR [EstimatedCostUsd] >= 0),
    CONSTRAINT [CK_AiExecutions_InputSnapshotJson] CHECK (ISJSON([InputSnapshotJson]) = 1),
    CONSTRAINT [CK_AiExecutions_OutputSnapshotJson] CHECK (ISJSON([OutputSnapshotJson]) = 1),
    CONSTRAINT [CK_AiExecutions_Status] CHECK ([Status] IN (N'Queued', N'Running', N'Succeeded', N'Failed')),
    CONSTRAINT [CK_AiExecutions_TaskType] CHECK ([TaskType] IN (N'Strategy', N'Copy', N'Creative', N'Review', N'Optimization', N'CompetitorAnalysis')),
    CONSTRAINT [CK_AiExecutions_Timing] CHECK ([CompletedAt] IS NULL OR [StartedAt] IS NULL OR [CompletedAt] >= [StartedAt]),
    CONSTRAINT [CK_AiExecutions_Tokens] CHECK (([InputTokens] IS NULL OR [InputTokens] >= 0) AND ([OutputTokens] IS NULL OR [OutputTokens] >= 0)),
    CONSTRAINT [FK_AiExecutions_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[AudienceSegments] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [TargetingRulesJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AudienceSegments] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_AudienceSegments_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_AudienceSegments_TargetingRulesJson] CHECK (ISJSON([TargetingRulesJson]) = 1),
    CONSTRAINT [FK_AudienceSegments_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[AuditEvents] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NULL,
    [ActorUserId] uniqueidentifier NULL,
    [EventType] nvarchar(100) NOT NULL,
    [ObjectType] nvarchar(100) NOT NULL,
    [ObjectId] uniqueidentifier NULL,
    [DetailsJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_AuditEvents] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_AuditEvents_DetailsJson] CHECK (ISJSON([DetailsJson]) = 1),
    CONSTRAINT [FK_AuditEvents_AppUsers_ActorUserId] FOREIGN KEY ([ActorUserId]) REFERENCES [marketing].[AppUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AuditEvents_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AuditEvents_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [marketing].[Tenants] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[BrandProfiles] (
    [BusinessId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [Dialect] nvarchar(50) NOT NULL,
    [Tone] nvarchar(50) NOT NULL,
    [EmojiLevel] tinyint NOT NULL DEFAULT CAST(1 AS tinyint),
    [Guidelines] nvarchar(4000) NULL,
    [ColorPaletteJson] nvarchar(max) NOT NULL DEFAULT (N'[]'),
    [UpdatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_BrandProfiles] PRIMARY KEY ([BusinessId]),
    CONSTRAINT [CK_BrandProfiles_ColorPaletteJson] CHECK (ISJSON([ColorPaletteJson]) = 1),
    CONSTRAINT [CK_BrandProfiles_EmojiLevel] CHECK ([EmojiLevel] BETWEEN 0 AND 3),
    CONSTRAINT [FK_BrandProfiles_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[BusinessHours] (
    [Id] uniqueidentifier NOT NULL,
    [Weekday] tinyint NOT NULL,
    [SlotNo] tinyint NOT NULL,
    [OpensAt] time NOT NULL,
    [ClosesAt] time NOT NULL,
    [CrossesMidnight] bit NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_BusinessHours] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_BusinessHours_Overnight] CHECK ([CrossesMidnight] = 0 OR [ClosesAt] <= [OpensAt]),
    CONSTRAINT [CK_BusinessHours_SameDay] CHECK ([CrossesMidnight] = 1 OR [ClosesAt] > [OpensAt]),
    CONSTRAINT [CK_BusinessHours_SlotNo] CHECK ([SlotNo] > 0),
    CONSTRAINT [CK_BusinessHours_Weekday] CHECK ([Weekday] BETWEEN 0 AND 6),
    CONSTRAINT [FK_BusinessHours_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[Offers] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(2000) NULL,
    [OfferPrice] decimal(14,2) NULL,
    [Currency] nchar(3) NULL,
    [DiscountPercent] decimal(5,2) NULL,
    [StartsAt] datetimeoffset NOT NULL,
    [EndsAt] datetimeoffset NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Offers] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_Offers_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_Offers_Currency] CHECK ([Currency] IS NULL OR [Currency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'),
    CONSTRAINT [CK_Offers_DiscountPercent] CHECK ([DiscountPercent] IS NULL OR ([DiscountPercent] > 0 AND [DiscountPercent] <= 100)),
    CONSTRAINT [CK_Offers_OfferPrice] CHECK ([OfferPrice] IS NULL OR [OfferPrice] >= 0),
    CONSTRAINT [CK_Offers_Period] CHECK ([EndsAt] > [StartsAt]),
    CONSTRAINT [CK_Offers_PriceCurrency] CHECK (([OfferPrice] IS NULL AND [Currency] IS NULL) OR ([OfferPrice] IS NOT NULL AND [Currency] IS NOT NULL)),
    CONSTRAINT [CK_Offers_Status] CHECK ([Status] IN (N'Draft', N'Active', N'Paused', N'Ended')),
    CONSTRAINT [FK_Offers_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[Products] (
    [Id] uniqueidentifier NOT NULL,
    [Sku] nvarchar(64) NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(2000) NULL,
    [Price] decimal(14,2) NOT NULL,
    [Currency] nchar(3) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UpdatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_Products_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_Products_Currency] CHECK ([Currency] COLLATE Latin1_General_BIN LIKE '[A-Z][A-Z][A-Z]'),
    CONSTRAINT [CK_Products_Name] CHECK (LEN(TRIM([Name])) > 0),
    CONSTRAINT [CK_Products_Price] CHECK ([Price] >= 0),
    CONSTRAINT [FK_Products_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[SocialAccounts] (
    [Id] uniqueidentifier NOT NULL,
    [PlatformCode] nvarchar(30) NOT NULL,
    [ExternalAccountId] nvarchar(200) NOT NULL,
    [Handle] nvarchar(100) NULL,
    [CredentialRef] nvarchar(300) NULL,
    [Status] nvarchar(30) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_SocialAccounts] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_SocialAccounts_BusinessId_Id_PlatformCode] UNIQUE ([BusinessId], [Id], [PlatformCode]),
    CONSTRAINT [CK_SocialAccounts_Status] CHECK ([Status] IN (N'Connected', N'Expired', N'Disconnected')),
    CONSTRAINT [FK_SocialAccounts_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SocialAccounts_Platforms_PlatformCode] FOREIGN KEY ([PlatformCode]) REFERENCES [marketing].[Platforms] ([Code]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[AutomationPolicies] (
    [BusinessId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [AutomationEnabled] bit NOT NULL DEFAULT CAST(1 AS bit),
    [MaxPostsPerDay] smallint NOT NULL DEFAULT CAST(3 AS smallint),
    [PublishWindowStart] time NULL,
    [PublishWindowEnd] time NULL,
    [MaxDiscountPercent] decimal(5,2) NULL,
    [MonthlyAdBudget] decimal(14,2) NULL,
    [BudgetCurrency] nchar(3) NULL,
    [RequireSeparateReviewer] bit NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_AutomationPolicies] PRIMARY KEY ([BusinessId]),
    CONSTRAINT [CK_AutomationPolicies_Budget] CHECK ([MonthlyAdBudget] IS NULL OR [MonthlyAdBudget] >= 0),
    CONSTRAINT [CK_AutomationPolicies_BudgetCurrency] CHECK (([MonthlyAdBudget] IS NULL AND [BudgetCurrency] IS NULL) OR ([MonthlyAdBudget] IS NOT NULL AND [BudgetCurrency] IS NOT NULL)),
    CONSTRAINT [CK_AutomationPolicies_MaxDiscount] CHECK ([MaxDiscountPercent] IS NULL OR [MaxDiscountPercent] BETWEEN 0 AND 100),
    CONSTRAINT [CK_AutomationPolicies_MaxPostsPerDay] CHECK ([MaxPostsPerDay] > 0),
    CONSTRAINT [CK_AutomationPolicies_Window] CHECK (([PublishWindowStart] IS NULL AND [PublishWindowEnd] IS NULL) OR ([PublishWindowStart] IS NOT NULL AND [PublishWindowEnd] IS NOT NULL)),
    CONSTRAINT [CK_AutomationPolicies_WindowOrder] CHECK ([PublishWindowStart] IS NULL OR [PublishWindowEnd] > [PublishWindowStart]),
    CONSTRAINT [FK_AutomationPolicies_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AutomationPolicies_TenantMemberships_TenantId_UpdatedBy] FOREIGN KEY ([TenantId], [UpdatedBy]) REFERENCES [marketing].[TenantMemberships] ([TenantId], [UserId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[Campaigns] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Objective] nvarchar(30) NOT NULL,
    [GoalStatement] nvarchar(1000) NOT NULL,
    [TargetMetric] nvarchar(50) NULL,
    [TargetValue] decimal(18,4) NULL,
    [Budget] decimal(14,2) NULL,
    [BudgetCurrency] nchar(3) NULL,
    [StartsAt] datetimeoffset NULL,
    [EndsAt] datetimeoffset NULL,
    [Status] nvarchar(30) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UpdatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Campaigns] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_Campaigns_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_Campaigns_Budget] CHECK ([Budget] IS NULL OR [Budget] >= 0),
    CONSTRAINT [CK_Campaigns_BudgetCurrency] CHECK (([Budget] IS NULL AND [BudgetCurrency] IS NULL) OR ([Budget] IS NOT NULL AND [BudgetCurrency] IS NOT NULL)),
    CONSTRAINT [CK_Campaigns_Objective] CHECK ([Objective] IN (N'Orders', N'Messages', N'Visits', N'Awareness', N'ProductPromotion', N'Followers')),
    CONSTRAINT [CK_Campaigns_Period] CHECK ([StartsAt] IS NULL OR [EndsAt] IS NULL OR [EndsAt] > [StartsAt]),
    CONSTRAINT [CK_Campaigns_Status] CHECK ([Status] IN (N'Draft', N'Planning', N'Ready', N'Active', N'Paused', N'Completed', N'Canceled')),
    CONSTRAINT [CK_Campaigns_TargetValue] CHECK ([TargetValue] IS NULL OR [TargetValue] >= 0),
    CONSTRAINT [FK_Campaigns_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Campaigns_TenantMemberships_TenantId_CreatedBy] FOREIGN KEY ([TenantId], [CreatedBy]) REFERENCES [marketing].[TenantMemberships] ([TenantId], [UserId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[TrackedSources] (
    [Id] uniqueidentifier NOT NULL,
    [Url] nvarchar(2048) NOT NULL,
    [NormalizedUrl] nvarchar(800) NOT NULL,
    [Host] nvarchar(255) NOT NULL,
    [PlatformCode] nvarchar(30) NOT NULL,
    [Kind] nvarchar(30) NOT NULL,
    [DisplayName] nvarchar(200) NULL,
    [Handle] nvarchar(100) NULL,
    [Notes] nvarchar(1000) NULL,
    [Status] nvarchar(30) NOT NULL,
    [LastScannedAt] datetimeoffset NULL,
    [AddedBy] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_TrackedSources] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_TrackedSources_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_TrackedSources_Kind] CHECK ([Kind] IN (N'Competitor', N'Professional', N'Inspiration')),
    CONSTRAINT [CK_TrackedSources_Status] CHECK ([Status] IN (N'Pending', N'Active', N'Paused', N'Failed')),
    CONSTRAINT [CK_TrackedSources_Url] CHECK ([Url] LIKE 'http%://%'),
    CONSTRAINT [FK_TrackedSources_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TrackedSources_Platforms_PlatformCode] FOREIGN KEY ([PlatformCode]) REFERENCES [marketing].[Platforms] ([Code]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TrackedSources_TenantMemberships_TenantId_AddedBy] FOREIGN KEY ([TenantId], [AddedBy]) REFERENCES [marketing].[TenantMemberships] ([TenantId], [UserId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[StrategyRuns] (
    [Id] uniqueidentifier NOT NULL,
    [WindowStart] datetimeoffset NOT NULL,
    [WindowEnd] datetimeoffset NOT NULL,
    [AlgorithmVersion] nvarchar(50) NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [InputSummaryJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [AiExecutionId] uniqueidentifier NULL,
    [StartedAt] datetimeoffset NULL,
    [CompletedAt] datetimeoffset NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_StrategyRuns] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_StrategyRuns_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_StrategyRuns_InputSummaryJson] CHECK (ISJSON([InputSummaryJson]) = 1),
    CONSTRAINT [CK_StrategyRuns_Status] CHECK ([Status] IN (N'Queued', N'Running', N'Completed', N'Failed')),
    CONSTRAINT [CK_StrategyRuns_Window] CHECK ([WindowEnd] > [WindowStart]),
    CONSTRAINT [FK_StrategyRuns_AiExecutions_BusinessId_AiExecutionId] FOREIGN KEY ([BusinessId], [AiExecutionId]) REFERENCES [marketing].[AiExecutions] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StrategyRuns_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[UsageEvents] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NULL,
    [AiExecutionId] uniqueidentifier NULL,
    [EventType] nvarchar(30) NOT NULL,
    [Units] decimal(14,4) NOT NULL,
    [CreditDelta] decimal(14,4) NOT NULL,
    [EstimatedCostUsd] decimal(14,6) NOT NULL,
    [IdempotencyKey] nvarchar(100) NOT NULL,
    [OccurredAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_UsageEvents] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_UsageEvents_Cost] CHECK ([EstimatedCostUsd] >= 0),
    CONSTRAINT [CK_UsageEvents_EventType] CHECK ([EventType] IN (N'CopyGeneration', N'ImageGeneration', N'VideoGeneration', N'Publish', N'CompetitorScan', N'CreditGrant', N'CreditAdjustment')),
    CONSTRAINT [CK_UsageEvents_ExecutionNeedsBusiness] CHECK ([AiExecutionId] IS NULL OR [BusinessId] IS NOT NULL),
    CONSTRAINT [CK_UsageEvents_Units] CHECK ([Units] >= 0),
    CONSTRAINT [FK_UsageEvents_AiExecutions_BusinessId_AiExecutionId] FOREIGN KEY ([BusinessId], [AiExecutionId]) REFERENCES [marketing].[AiExecutions] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UsageEvents_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UsageEvents_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [marketing].[Tenants] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[CustomerContacts] (
    [Id] uniqueidentifier NOT NULL,
    [AudienceSegmentId] uniqueidentifier NULL,
    [PhoneE164] nvarchar(16) NOT NULL,
    [DisplayName] nvarchar(150) NULL,
    [WhatsAppOptIn] bit NOT NULL,
    [OptInSource] nvarchar(30) NULL,
    [OptedInAt] datetimeoffset NULL,
    [OptedOutAt] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UpdatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CustomerContacts] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_CustomerContacts_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_CustomerContacts_OptIn] CHECK ([WhatsAppOptIn] = 0 OR ([OptInSource] IS NOT NULL AND [OptedInAt] IS NOT NULL)),
    CONSTRAINT [CK_CustomerContacts_OptInSource] CHECK ([OptInSource] IN (N'InStore', N'WebsiteForm', N'WhatsAppReply', N'ImportWithConsent')),
    CONSTRAINT [CK_CustomerContacts_PhoneE164] CHECK ([PhoneE164] LIKE '+[1-9]%' AND [PhoneE164] NOT LIKE '+%[^0-9]%' AND LEN([PhoneE164]) BETWEEN 8 AND 16),
    CONSTRAINT [FK_CustomerContacts_AudienceSegments_BusinessId_AudienceSegmentId] FOREIGN KEY ([BusinessId], [AudienceSegmentId]) REFERENCES [marketing].[AudienceSegments] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CustomerContacts_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[BrandTerms] (
    [Id] uniqueidentifier NOT NULL,
    [Term] nvarchar(100) NOT NULL,
    [Rule] nvarchar(30) NOT NULL,
    [Note] nvarchar(500) NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_BrandTerms] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_BrandTerms_Rule] CHECK ([Rule] IN (N'Preferred', N'Prohibited')),
    CONSTRAINT [CK_BrandTerms_Term] CHECK (LEN(TRIM([Term])) > 0),
    CONSTRAINT [FK_BrandTerms_BrandProfiles_BusinessId] FOREIGN KEY ([BusinessId]) REFERENCES [marketing].[BrandProfiles] ([BusinessId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BrandTerms_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[MediaAssets] (
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NULL,
    [Kind] nvarchar(30) NOT NULL,
    [ObjectKey] nvarchar(500) NOT NULL,
    [MimeType] nvarchar(100) NOT NULL,
    [ByteSize] bigint NOT NULL,
    [Sha256] nchar(64) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_MediaAssets] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_MediaAssets_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_MediaAssets_ByteSize] CHECK ([ByteSize] >= 0),
    CONSTRAINT [CK_MediaAssets_Kind] CHECK ([Kind] IN (N'Logo', N'ProductPhoto', N'BrandImage', N'GeneratedImage', N'Video', N'Document')),
    CONSTRAINT [FK_MediaAssets_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MediaAssets_Products_BusinessId_ProductId] FOREIGN KEY ([BusinessId], [ProductId]) REFERENCES [marketing].[Products] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[WhatsAppTemplates] (
    [Id] uniqueidentifier NOT NULL,
    [SocialAccountId] uniqueidentifier NOT NULL,
    [PlatformCode] nvarchar(30) NOT NULL,
    [TemplateName] nvarchar(200) NOT NULL,
    [LanguageCode] nvarchar(10) NOT NULL,
    [Category] nvarchar(30) NOT NULL,
    [ProviderStatus] nvarchar(30) NOT NULL,
    [BodyPreview] nvarchar(1000) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_WhatsAppTemplates] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_WhatsAppTemplates_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_WhatsAppTemplates_Category] CHECK ([Category] IN (N'Marketing', N'Utility')),
    CONSTRAINT [CK_WhatsAppTemplates_PlatformCode] CHECK ([PlatformCode] = N'whatsapp'),
    CONSTRAINT [CK_WhatsAppTemplates_ProviderStatus] CHECK ([ProviderStatus] IN (N'Pending', N'Approved', N'Rejected', N'Paused')),
    CONSTRAINT [FK_WhatsAppTemplates_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_WhatsAppTemplates_SocialAccounts_BusinessId_SocialAccountId_PlatformCode] FOREIGN KEY ([BusinessId], [SocialAccountId], [PlatformCode]) REFERENCES [marketing].[SocialAccounts] ([BusinessId], [Id], [PlatformCode]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[AutoApplyRules] (
    [BusinessId] uniqueidentifier NOT NULL,
    [ActionType] nvarchar(30) NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AutoApplyRules] PRIMARY KEY ([BusinessId], [ActionType]),
    CONSTRAINT [CK_AutoApplyRules_ActionType] CHECK ([ActionType] IN (N'NewContent', N'ReviseCopy', N'ChangeFormat', N'ChangeChannel', N'Reschedule', N'Pause')),
    CONSTRAINT [FK_AutoApplyRules_AutomationPolicies_BusinessId] FOREIGN KEY ([BusinessId]) REFERENCES [marketing].[AutomationPolicies] ([BusinessId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[AutomationPolicyPlatforms] (
    [BusinessId] uniqueidentifier NOT NULL,
    [PlatformCode] nvarchar(30) NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AutomationPolicyPlatforms] PRIMARY KEY ([BusinessId], [PlatformCode]),
    CONSTRAINT [FK_AutomationPolicyPlatforms_AutomationPolicies_BusinessId] FOREIGN KEY ([BusinessId]) REFERENCES [marketing].[AutomationPolicies] ([BusinessId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AutomationPolicyPlatforms_Platforms_PlatformCode] FOREIGN KEY ([PlatformCode]) REFERENCES [marketing].[Platforms] ([Code]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[CampaignAudiences] (
    [BusinessId] uniqueidentifier NOT NULL,
    [CampaignId] uniqueidentifier NOT NULL,
    [AudienceSegmentId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CampaignAudiences] PRIMARY KEY ([BusinessId], [CampaignId], [AudienceSegmentId]),
    CONSTRAINT [FK_CampaignAudiences_AudienceSegments_BusinessId_AudienceSegmentId] FOREIGN KEY ([BusinessId], [AudienceSegmentId]) REFERENCES [marketing].[AudienceSegments] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CampaignAudiences_Campaigns_BusinessId_CampaignId] FOREIGN KEY ([BusinessId], [CampaignId]) REFERENCES [marketing].[Campaigns] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[CampaignOffers] (
    [BusinessId] uniqueidentifier NOT NULL,
    [CampaignId] uniqueidentifier NOT NULL,
    [OfferId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CampaignOffers] PRIMARY KEY ([BusinessId], [CampaignId], [OfferId]),
    CONSTRAINT [FK_CampaignOffers_Campaigns_BusinessId_CampaignId] FOREIGN KEY ([BusinessId], [CampaignId]) REFERENCES [marketing].[Campaigns] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CampaignOffers_Offers_BusinessId_OfferId] FOREIGN KEY ([BusinessId], [OfferId]) REFERENCES [marketing].[Offers] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[CampaignPlans] (
    [Id] uniqueidentifier NOT NULL,
    [CampaignId] uniqueidentifier NOT NULL,
    [VersionNo] int NOT NULL,
    [PlanJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [GeneratedByExecutionId] uniqueidentifier NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CampaignPlans] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_CampaignPlans_PlanJson] CHECK (ISJSON([PlanJson]) = 1),
    CONSTRAINT [CK_CampaignPlans_VersionNo] CHECK ([VersionNo] > 0),
    CONSTRAINT [FK_CampaignPlans_AiExecutions_BusinessId_GeneratedByExecutionId] FOREIGN KEY ([BusinessId], [GeneratedByExecutionId]) REFERENCES [marketing].[AiExecutions] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CampaignPlans_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CampaignPlans_Campaigns_BusinessId_CampaignId] FOREIGN KEY ([BusinessId], [CampaignId]) REFERENCES [marketing].[Campaigns] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[CampaignProducts] (
    [BusinessId] uniqueidentifier NOT NULL,
    [CampaignId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CampaignProducts] PRIMARY KEY ([BusinessId], [CampaignId], [ProductId]),
    CONSTRAINT [FK_CampaignProducts_Campaigns_BusinessId_CampaignId] FOREIGN KEY ([BusinessId], [CampaignId]) REFERENCES [marketing].[Campaigns] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CampaignProducts_Products_BusinessId_ProductId] FOREIGN KEY ([BusinessId], [ProductId]) REFERENCES [marketing].[Products] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[SourcePosts] (
    [Id] uniqueidentifier NOT NULL,
    [TrackedSourceId] uniqueidentifier NOT NULL,
    [ExternalPostId] nvarchar(200) NULL,
    [PostUrl] nvarchar(2048) NULL,
    [PostedAt] datetimeoffset NULL,
    [Format] nvarchar(30) NOT NULL,
    [HookType] nvarchar(30) NULL,
    [CaptionText] nvarchar(max) NULL,
    [HashtagsJson] nvarchar(max) NOT NULL DEFAULT (N'[]'),
    [LikesCount] int NULL,
    [CommentsCount] int NULL,
    [SharesCount] int NULL,
    [ViewsCount] bigint NULL,
    [EngagementRate] decimal(9,6) NULL,
    [CapturedAt] datetimeoffset NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_SourcePosts] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_SourcePosts_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_SourcePosts_Counts] CHECK (([LikesCount] IS NULL OR [LikesCount] >= 0) AND ([CommentsCount] IS NULL OR [CommentsCount] >= 0) AND ([SharesCount] IS NULL OR [SharesCount] >= 0) AND ([ViewsCount] IS NULL OR [ViewsCount] >= 0)),
    CONSTRAINT [CK_SourcePosts_EngagementRate] CHECK ([EngagementRate] IS NULL OR [EngagementRate] >= 0),
    CONSTRAINT [CK_SourcePosts_Format] CHECK ([Format] IN (N'Text', N'SingleImage', N'Carousel', N'Video', N'Reel', N'Story', N'Live')),
    CONSTRAINT [CK_SourcePosts_HashtagsJson] CHECK (ISJSON([HashtagsJson]) = 1),
    CONSTRAINT [CK_SourcePosts_HookType] CHECK ([HookType] IN (N'Question', N'Offer', N'Story', N'SocialProof', N'Urgency', N'Informative', N'Humor', N'Tutorial')),
    CONSTRAINT [FK_SourcePosts_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SourcePosts_TrackedSources_BusinessId_TrackedSourceId] FOREIGN KEY ([BusinessId], [TrackedSourceId]) REFERENCES [marketing].[TrackedSources] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[CompetitorInsights] (
    [Id] uniqueidentifier NOT NULL,
    [TrackedSourceId] uniqueidentifier NULL,
    [StrategyRunId] uniqueidentifier NULL,
    [InsightType] nvarchar(30) NOT NULL,
    [Summary] nvarchar(1000) NOT NULL,
    [DetailsJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [Confidence] decimal(4,3) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CompetitorInsights] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_CompetitorInsights_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_CompetitorInsights_Confidence] CHECK ([Confidence] IS NULL OR [Confidence] BETWEEN 0 AND 1),
    CONSTRAINT [CK_CompetitorInsights_DetailsJson] CHECK (ISJSON([DetailsJson]) = 1),
    CONSTRAINT [CK_CompetitorInsights_InsightType] CHECK ([InsightType] IN (N'PostingTime', N'Format', N'Hook', N'Hashtags', N'Offer', N'Tone', N'Frequency', N'Engagement')),
    CONSTRAINT [FK_CompetitorInsights_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CompetitorInsights_StrategyRuns_BusinessId_StrategyRunId] FOREIGN KEY ([BusinessId], [StrategyRunId]) REFERENCES [marketing].[StrategyRuns] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CompetitorInsights_TrackedSources_BusinessId_TrackedSourceId] FOREIGN KEY ([BusinessId], [TrackedSourceId]) REFERENCES [marketing].[TrackedSources] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[StrategyRecommendations] (
    [Id] uniqueidentifier NOT NULL,
    [StrategyRunId] uniqueidentifier NOT NULL,
    [CampaignId] uniqueidentifier NULL,
    [ActionType] nvarchar(30) NOT NULL,
    [ProposedPayloadJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [ProposedPublishAt] datetimeoffset NULL,
    [Rationale] nvarchar(2000) NOT NULL,
    [Confidence] decimal(4,3) NULL,
    [DecisionStatus] nvarchar(30) NOT NULL,
    [AppliedAutomatically] bit NOT NULL,
    [DecidedBy] uniqueidentifier NULL,
    [DecidedAt] datetimeoffset NULL,
    [AppliedAt] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_StrategyRecommendations] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_StrategyRecommendations_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_StrategyRecommendations_ActionType] CHECK ([ActionType] IN (N'NewContent', N'ReviseCopy', N'ChangeFormat', N'ChangeChannel', N'Reschedule', N'Pause')),
    CONSTRAINT [CK_StrategyRecommendations_AppliedAt] CHECK (([DecisionStatus] = N'Applied' AND [AppliedAt] IS NOT NULL) OR ([DecisionStatus] <> N'Applied' AND [AppliedAt] IS NULL)),
    CONSTRAINT [CK_StrategyRecommendations_AutoApplied] CHECK ([AppliedAutomatically] = 0 OR ([DecisionStatus] = N'Applied' AND [DecidedBy] IS NULL)),
    CONSTRAINT [CK_StrategyRecommendations_Confidence] CHECK ([Confidence] IS NULL OR [Confidence] BETWEEN 0 AND 1),
    CONSTRAINT [CK_StrategyRecommendations_Decider] CHECK (([DecidedBy] IS NULL AND [DecidedAt] IS NULL) OR ([DecidedBy] IS NOT NULL AND [DecidedAt] IS NOT NULL)),
    CONSTRAINT [CK_StrategyRecommendations_DecisionStatus] CHECK ([DecisionStatus] IN (N'Pending', N'Accepted', N'Rejected', N'Applied')),
    CONSTRAINT [CK_StrategyRecommendations_HumanDecision] CHECK ([DecisionStatus] = N'Pending' OR [AppliedAutomatically] = 1 OR [DecidedBy] IS NOT NULL),
    CONSTRAINT [CK_StrategyRecommendations_ProposedPayloadJson] CHECK (ISJSON([ProposedPayloadJson]) = 1),
    CONSTRAINT [FK_StrategyRecommendations_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StrategyRecommendations_Campaigns_BusinessId_CampaignId] FOREIGN KEY ([BusinessId], [CampaignId]) REFERENCES [marketing].[Campaigns] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StrategyRecommendations_StrategyRuns_BusinessId_StrategyRunId] FOREIGN KEY ([BusinessId], [StrategyRunId]) REFERENCES [marketing].[StrategyRuns] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StrategyRecommendations_TenantMemberships_TenantId_DecidedBy] FOREIGN KEY ([TenantId], [DecidedBy]) REFERENCES [marketing].[TenantMemberships] ([TenantId], [UserId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[KnowledgeSources] (
    [Id] uniqueidentifier NOT NULL,
    [AssetId] uniqueidentifier NULL,
    [SourceType] nvarchar(30) NOT NULL,
    [SourceUrl] nvarchar(2048) NULL,
    [ExtractedText] nvarchar(max) NULL,
    [Status] nvarchar(30) NOT NULL,
    [ProcessedAt] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_KnowledgeSources] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_KnowledgeSources_AssetOrUrl] CHECK ([AssetId] IS NOT NULL OR [SourceUrl] IS NOT NULL),
    CONSTRAINT [CK_KnowledgeSources_SourceType] CHECK ([SourceType] IN (N'Menu', N'Guidelines', N'Website', N'HistoricalPost', N'Other')),
    CONSTRAINT [CK_KnowledgeSources_Status] CHECK ([Status] IN (N'Pending', N'Processed', N'Failed', N'Disabled')),
    CONSTRAINT [FK_KnowledgeSources_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_KnowledgeSources_MediaAssets_BusinessId_AssetId] FOREIGN KEY ([BusinessId], [AssetId]) REFERENCES [marketing].[MediaAssets] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[ContentItems] (
    [Id] uniqueidentifier NOT NULL,
    [CampaignId] uniqueidentifier NOT NULL,
    [PlatformCode] nvarchar(30) NOT NULL,
    [ContentType] nvarchar(30) NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [PlannedPublishAt] datetimeoffset NULL,
    [VariantGroupId] uniqueidentifier NULL,
    [VariantLabel] nvarchar(10) NULL,
    [SourceRecommendationId] uniqueidentifier NULL,
    [InspiredBySourcePostId] uniqueidentifier NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ContentItems] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_ContentItems_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [AK_ContentItems_BusinessId_Id_PlatformCode] UNIQUE ([BusinessId], [Id], [PlatformCode]),
    CONSTRAINT [CK_ContentItems_ContentType] CHECK ([ContentType] IN (N'Post', N'Story', N'Reel', N'Short', N'AdCopy', N'WhatsAppMessage')),
    CONSTRAINT [CK_ContentItems_Status] CHECK ([Status] IN (N'Draft', N'InReview', N'Approved', N'Scheduled', N'Published', N'Canceled')),
    CONSTRAINT [CK_ContentItems_Variant] CHECK (([VariantGroupId] IS NULL AND [VariantLabel] IS NULL) OR ([VariantGroupId] IS NOT NULL AND [VariantLabel] IS NOT NULL)),
    CONSTRAINT [CK_ContentItems_WhatsAppType] CHECK (([PlatformCode] = N'whatsapp' AND [ContentType] = N'WhatsAppMessage') OR ([PlatformCode] <> N'whatsapp' AND [ContentType] <> N'WhatsAppMessage')),
    CONSTRAINT [FK_ContentItems_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentItems_Campaigns_BusinessId_CampaignId] FOREIGN KEY ([BusinessId], [CampaignId]) REFERENCES [marketing].[Campaigns] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentItems_Platforms_PlatformCode] FOREIGN KEY ([PlatformCode]) REFERENCES [marketing].[Platforms] ([Code]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentItems_SourcePosts_BusinessId_InspiredBySourcePostId] FOREIGN KEY ([BusinessId], [InspiredBySourcePostId]) REFERENCES [marketing].[SourcePosts] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentItems_StrategyRecommendations_BusinessId_SourceRecommendationId] FOREIGN KEY ([BusinessId], [SourceRecommendationId]) REFERENCES [marketing].[StrategyRecommendations] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[ContentRevisions] (
    [Id] uniqueidentifier NOT NULL,
    [ContentItemId] uniqueidentifier NOT NULL,
    [RevisionNo] int NOT NULL,
    [Body] nvarchar(max) NOT NULL,
    [Format] nvarchar(30) NOT NULL,
    [HookType] nvarchar(30) NULL,
    [CallToAction] nvarchar(30) NULL,
    [IncludesOffer] bit NOT NULL,
    [WhatsAppTemplateId] uniqueidentifier NULL,
    [CreativeDetailsJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [FactsSnapshotJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [BrandSnapshotJson] nvarchar(max) NOT NULL DEFAULT (N'{}'),
    [GeneratedByExecutionId] uniqueidentifier NULL,
    [SourceRecommendationId] uniqueidentifier NULL,
    [CreatedBy] uniqueidentifier NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ContentRevisions] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_ContentRevisions_BusinessId_ContentItemId_Id] UNIQUE ([BusinessId], [ContentItemId], [Id]),
    CONSTRAINT [AK_ContentRevisions_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_ContentRevisions_Author] CHECK ([GeneratedByExecutionId] IS NOT NULL OR [CreatedBy] IS NOT NULL),
    CONSTRAINT [CK_ContentRevisions_BrandSnapshotJson] CHECK (ISJSON([BrandSnapshotJson]) = 1),
    CONSTRAINT [CK_ContentRevisions_CallToAction] CHECK ([CallToAction] IN (N'OrderNow', N'SendMessage', N'VisitStore', N'LearnMore', N'Follow', N'None')),
    CONSTRAINT [CK_ContentRevisions_CreativeDetailsJson] CHECK (ISJSON([CreativeDetailsJson]) = 1),
    CONSTRAINT [CK_ContentRevisions_FactsSnapshotJson] CHECK (ISJSON([FactsSnapshotJson]) = 1),
    CONSTRAINT [CK_ContentRevisions_Format] CHECK ([Format] IN (N'Text', N'SingleImage', N'Carousel', N'Video', N'Reel', N'Story', N'Live')),
    CONSTRAINT [CK_ContentRevisions_HookType] CHECK ([HookType] IN (N'Question', N'Offer', N'Story', N'SocialProof', N'Urgency', N'Informative', N'Humor', N'Tutorial')),
    CONSTRAINT [CK_ContentRevisions_RevisionNo] CHECK ([RevisionNo] > 0),
    CONSTRAINT [FK_ContentRevisions_AiExecutions_BusinessId_GeneratedByExecutionId] FOREIGN KEY ([BusinessId], [GeneratedByExecutionId]) REFERENCES [marketing].[AiExecutions] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentRevisions_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentRevisions_ContentItems_BusinessId_ContentItemId] FOREIGN KEY ([BusinessId], [ContentItemId]) REFERENCES [marketing].[ContentItems] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentRevisions_StrategyRecommendations_BusinessId_SourceRecommendationId] FOREIGN KEY ([BusinessId], [SourceRecommendationId]) REFERENCES [marketing].[StrategyRecommendations] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentRevisions_TenantMemberships_TenantId_CreatedBy] FOREIGN KEY ([TenantId], [CreatedBy]) REFERENCES [marketing].[TenantMemberships] ([TenantId], [UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentRevisions_WhatsAppTemplates_BusinessId_WhatsAppTemplateId] FOREIGN KEY ([BusinessId], [WhatsAppTemplateId]) REFERENCES [marketing].[WhatsAppTemplates] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[ContentReviews] (
    [Id] uniqueidentifier NOT NULL,
    [ContentItemId] uniqueidentifier NOT NULL,
    [RevisionId] uniqueidentifier NOT NULL,
    [ReviewerId] uniqueidentifier NOT NULL,
    [Decision] nvarchar(30) NOT NULL,
    [Feedback] nvarchar(2000) NULL,
    [DecidedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ContentReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_ContentReviews_BusinessId_ContentItemId_RevisionId_Id] UNIQUE ([BusinessId], [ContentItemId], [RevisionId], [Id]),
    CONSTRAINT [CK_ContentReviews_Decision] CHECK ([Decision] IN (N'Approved', N'Rejected', N'ChangesRequested')),
    CONSTRAINT [FK_ContentReviews_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentReviews_ContentRevisions_BusinessId_ContentItemId_RevisionId] FOREIGN KEY ([BusinessId], [ContentItemId], [RevisionId]) REFERENCES [marketing].[ContentRevisions] ([BusinessId], [ContentItemId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentReviews_TenantMemberships_TenantId_ReviewerId] FOREIGN KEY ([TenantId], [ReviewerId]) REFERENCES [marketing].[TenantMemberships] ([TenantId], [UserId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[ContentRevisionAssets] (
    [BusinessId] uniqueidentifier NOT NULL,
    [RevisionId] uniqueidentifier NOT NULL,
    [AssetId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [Position] tinyint NOT NULL,
    CONSTRAINT [PK_ContentRevisionAssets] PRIMARY KEY ([BusinessId], [RevisionId], [AssetId]),
    CONSTRAINT [FK_ContentRevisionAssets_ContentRevisions_BusinessId_RevisionId] FOREIGN KEY ([BusinessId], [RevisionId]) REFERENCES [marketing].[ContentRevisions] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentRevisionAssets_MediaAssets_BusinessId_AssetId] FOREIGN KEY ([BusinessId], [AssetId]) REFERENCES [marketing].[MediaAssets] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[ApprovedContentRevisions] (
    [BusinessId] uniqueidentifier NOT NULL,
    [ContentItemId] uniqueidentifier NOT NULL,
    [RevisionId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [ReviewId] uniqueidentifier NOT NULL,
    [ApprovedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_ApprovedContentRevisions] PRIMARY KEY ([BusinessId], [ContentItemId], [RevisionId]),
    CONSTRAINT [FK_ApprovedContentRevisions_ContentReviews_BusinessId_ContentItemId_RevisionId_ReviewId] FOREIGN KEY ([BusinessId], [ContentItemId], [RevisionId], [ReviewId]) REFERENCES [marketing].[ContentReviews] ([BusinessId], [ContentItemId], [RevisionId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[ApprovalRevocations] (
    [BusinessId] uniqueidentifier NOT NULL,
    [ContentItemId] uniqueidentifier NOT NULL,
    [RevisionId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [RevokedBy] uniqueidentifier NOT NULL,
    [Reason] nvarchar(1000) NULL,
    [RevokedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    CONSTRAINT [PK_ApprovalRevocations] PRIMARY KEY ([BusinessId], [ContentItemId], [RevisionId]),
    CONSTRAINT [FK_ApprovalRevocations_ApprovedContentRevisions_BusinessId_ContentItemId_RevisionId] FOREIGN KEY ([BusinessId], [ContentItemId], [RevisionId]) REFERENCES [marketing].[ApprovedContentRevisions] ([BusinessId], [ContentItemId], [RevisionId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalRevocations_TenantMemberships_TenantId_RevokedBy] FOREIGN KEY ([TenantId], [RevokedBy]) REFERENCES [marketing].[TenantMemberships] ([TenantId], [UserId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[PublicationJobs] (
    [Id] uniqueidentifier NOT NULL,
    [ContentItemId] uniqueidentifier NOT NULL,
    [RevisionId] uniqueidentifier NOT NULL,
    [SocialAccountId] uniqueidentifier NOT NULL,
    [PlatformCode] nvarchar(30) NOT NULL,
    [IdempotencyKey] nvarchar(100) NOT NULL,
    [ScheduledAt] datetimeoffset NOT NULL,
    [SourceRecommendationId] uniqueidentifier NULL,
    [Status] nvarchar(30) NOT NULL,
    [AttemptCount] int NOT NULL DEFAULT 0,
    [LastError] nvarchar(2000) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_PublicationJobs] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_PublicationJobs_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [AK_PublicationJobs_BusinessId_Id_SocialAccountId] UNIQUE ([BusinessId], [Id], [SocialAccountId]),
    CONSTRAINT [CK_PublicationJobs_AttemptCount] CHECK ([AttemptCount] >= 0),
    CONSTRAINT [CK_PublicationJobs_Status] CHECK ([Status] IN (N'Queued', N'Running', N'Succeeded', N'Retry', N'Failed', N'Canceled')),
    CONSTRAINT [FK_PublicationJobs_ApprovedContentRevisions_BusinessId_ContentItemId_RevisionId] FOREIGN KEY ([BusinessId], [ContentItemId], [RevisionId]) REFERENCES [marketing].[ApprovedContentRevisions] ([BusinessId], [ContentItemId], [RevisionId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PublicationJobs_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PublicationJobs_ContentItems_BusinessId_ContentItemId_PlatformCode] FOREIGN KEY ([BusinessId], [ContentItemId], [PlatformCode]) REFERENCES [marketing].[ContentItems] ([BusinessId], [Id], [PlatformCode]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PublicationJobs_SocialAccounts_BusinessId_SocialAccountId_PlatformCode] FOREIGN KEY ([BusinessId], [SocialAccountId], [PlatformCode]) REFERENCES [marketing].[SocialAccounts] ([BusinessId], [Id], [PlatformCode]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PublicationJobs_StrategyRecommendations_BusinessId_SourceRecommendationId] FOREIGN KEY ([BusinessId], [SourceRecommendationId]) REFERENCES [marketing].[StrategyRecommendations] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[MessageDeliveries] (
    [Id] uniqueidentifier NOT NULL,
    [PublicationJobId] uniqueidentifier NOT NULL,
    [ContactId] uniqueidentifier NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [ExternalMessageId] nvarchar(200) NULL,
    [LastError] nvarchar(2000) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UpdatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_MessageDeliveries] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_MessageDeliveries_Status] CHECK ([Status] IN (N'Queued', N'Sent', N'Delivered', N'Read', N'Failed')),
    CONSTRAINT [FK_MessageDeliveries_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MessageDeliveries_CustomerContacts_BusinessId_ContactId] FOREIGN KEY ([BusinessId], [ContactId]) REFERENCES [marketing].[CustomerContacts] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MessageDeliveries_PublicationJobs_BusinessId_PublicationJobId] FOREIGN KEY ([BusinessId], [PublicationJobId]) REFERENCES [marketing].[PublicationJobs] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[Publications] (
    [Id] uniqueidentifier NOT NULL,
    [PublicationJobId] uniqueidentifier NOT NULL,
    [SocialAccountId] uniqueidentifier NOT NULL,
    [ExternalPostId] nvarchar(200) NULL,
    [PublishedAt] datetimeoffset NOT NULL,
    [ExternalUrl] nvarchar(2048) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Publications] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_Publications_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [FK_Publications_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Publications_PublicationJobs_BusinessId_PublicationJobId_SocialAccountId] FOREIGN KEY ([BusinessId], [PublicationJobId], [SocialAccountId]) REFERENCES [marketing].[PublicationJobs] ([BusinessId], [Id], [SocialAccountId]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[ConversionEvents] (
    [Id] uniqueidentifier NOT NULL,
    [CampaignId] uniqueidentifier NOT NULL,
    [PublicationId] uniqueidentifier NULL,
    [EventType] nvarchar(30) NOT NULL,
    [Source] nvarchar(30) NOT NULL,
    [ExternalEventId] nvarchar(200) NULL,
    [OccurredAt] datetimeoffset NOT NULL,
    [RevenueAmount] decimal(14,2) NULL,
    [RevenueCurrency] nchar(3) NULL,
    [AttributionNote] nvarchar(1000) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ConversionEvents] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_ConversionEvents_EventType] CHECK ([EventType] IN (N'LinkClick', N'Message', N'Lead', N'Order', N'StoreVisit')),
    CONSTRAINT [CK_ConversionEvents_Revenue] CHECK ([RevenueAmount] IS NULL OR [RevenueAmount] >= 0),
    CONSTRAINT [CK_ConversionEvents_RevenueCurrency] CHECK (([RevenueAmount] IS NULL AND [RevenueCurrency] IS NULL) OR ([RevenueAmount] IS NOT NULL AND [RevenueCurrency] IS NOT NULL)),
    CONSTRAINT [CK_ConversionEvents_Source] CHECK ([Source] IN (N'TrackedLink', N'Integration', N'Manual', N'Coupon')),
    CONSTRAINT [FK_ConversionEvents_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ConversionEvents_Campaigns_BusinessId_CampaignId] FOREIGN KEY ([BusinessId], [CampaignId]) REFERENCES [marketing].[Campaigns] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ConversionEvents_Publications_BusinessId_PublicationId] FOREIGN KEY ([BusinessId], [PublicationId]) REFERENCES [marketing].[Publications] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[MetricObservations] (
    [Id] uniqueidentifier NOT NULL,
    [PublicationId] uniqueidentifier NOT NULL,
    [MetricCode] nvarchar(30) NOT NULL,
    [MetricValue] decimal(20,4) NOT NULL,
    [ObservedAt] datetimeoffset NOT NULL,
    [Source] nvarchar(30) NOT NULL,
    [RawPayloadJson] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_MetricObservations] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_MetricObservations_BusinessId_Id] UNIQUE ([BusinessId], [Id]),
    CONSTRAINT [CK_MetricObservations_MetricCode] CHECK ([MetricCode] IN (N'Impressions', N'Reach', N'Likes', N'Comments', N'Shares', N'Saves', N'LinkClicks', N'ProfileVisits', N'MessagesStarted', N'VideoViews', N'WatchTimeSeconds', N'Follows', N'Delivered', N'Read', N'Replies')),
    CONSTRAINT [CK_MetricObservations_MetricValue] CHECK ([MetricValue] >= 0),
    CONSTRAINT [CK_MetricObservations_RawPayloadJson] CHECK (ISJSON([RawPayloadJson]) = 1),
    CONSTRAINT [CK_MetricObservations_Source] CHECK ([Source] IN (N'PlatformApi', N'Webhook', N'Manual')),
    CONSTRAINT [FK_MetricObservations_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MetricObservations_Publications_BusinessId_PublicationId] FOREIGN KEY ([BusinessId], [PublicationId]) REFERENCES [marketing].[Publications] ([BusinessId], [Id]) ON DELETE NO ACTION
);

CREATE TABLE [marketing].[RecommendationEvidence] (
    [Id] uniqueidentifier NOT NULL,
    [RecommendationId] uniqueidentifier NOT NULL,
    [MetricObservationId] uniqueidentifier NULL,
    [SourcePostId] uniqueidentifier NULL,
    [CompetitorInsightId] uniqueidentifier NULL,
    [Interpretation] nvarchar(1000) NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_RecommendationEvidence] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_RecommendationEvidence_ExactlyOne] CHECK ((CASE WHEN [MetricObservationId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [SourcePostId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [CompetitorInsightId] IS NULL THEN 0 ELSE 1 END) = 1),
    CONSTRAINT [FK_RecommendationEvidence_Businesses_TenantId_BusinessId] FOREIGN KEY ([TenantId], [BusinessId]) REFERENCES [marketing].[Businesses] ([TenantId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RecommendationEvidence_CompetitorInsights_BusinessId_CompetitorInsightId] FOREIGN KEY ([BusinessId], [CompetitorInsightId]) REFERENCES [marketing].[CompetitorInsights] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RecommendationEvidence_MetricObservations_BusinessId_MetricObservationId] FOREIGN KEY ([BusinessId], [MetricObservationId]) REFERENCES [marketing].[MetricObservations] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RecommendationEvidence_SourcePosts_BusinessId_SourcePostId] FOREIGN KEY ([BusinessId], [SourcePostId]) REFERENCES [marketing].[SourcePosts] ([BusinessId], [Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RecommendationEvidence_StrategyRecommendations_BusinessId_RecommendationId] FOREIGN KEY ([BusinessId], [RecommendationId]) REFERENCES [marketing].[StrategyRecommendations] ([BusinessId], [Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'DisplayName', N'IsActive', N'SupportsCompetitorTracking', N'SupportsMessaging', N'SupportsPublishing') AND [object_id] = OBJECT_ID(N'[marketing].[Platforms]'))
    SET IDENTITY_INSERT [marketing].[Platforms] ON;
INSERT INTO [marketing].[Platforms] ([Code], [DisplayName], [IsActive], [SupportsCompetitorTracking], [SupportsMessaging], [SupportsPublishing])
VALUES (N'facebook', N'Facebook', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit)),
(N'instagram', N'Instagram', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit)),
(N'linkedin', N'LinkedIn', CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit)),
(N'snapchat', N'Snapchat', CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit)),
(N'tiktok', N'TikTok', CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit)),
(N'website', N'Website', CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit)),
(N'whatsapp', N'WhatsApp', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit)),
(N'x', N'X (Twitter)', CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit)),
(N'youtube', N'YouTube', CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'DisplayName', N'IsActive', N'SupportsCompetitorTracking', N'SupportsMessaging', N'SupportsPublishing') AND [object_id] = OBJECT_ID(N'[marketing].[Platforms]'))
    SET IDENTITY_INSERT [marketing].[Platforms] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Domain', N'PlatformCode') AND [object_id] = OBJECT_ID(N'[marketing].[PlatformDomains]'))
    SET IDENTITY_INSERT [marketing].[PlatformDomains] ON;
INSERT INTO [marketing].[PlatformDomains] ([Domain], [PlatformCode])
VALUES (N'facebook.com', N'facebook'),
(N'fb.com', N'facebook'),
(N'fb.me', N'facebook'),
(N'instagr.am', N'instagram'),
(N'instagram.com', N'instagram'),
(N'linkedin.com', N'linkedin'),
(N'snapchat.com', N'snapchat'),
(N'tiktok.com', N'tiktok'),
(N'twitter.com', N'x'),
(N'wa.me', N'whatsapp'),
(N'whatsapp.com', N'whatsapp'),
(N'x.com', N'x'),
(N'youtu.be', N'youtube'),
(N'youtube.com', N'youtube');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Domain', N'PlatformCode') AND [object_id] = OBJECT_ID(N'[marketing].[PlatformDomains]'))
    SET IDENTITY_INSERT [marketing].[PlatformDomains] OFF;

CREATE INDEX [IX_AiExecutions_BusinessId_TaskType_CreatedAt] ON [marketing].[AiExecutions] ([BusinessId], [TaskType], [CreatedAt]);

CREATE INDEX [IX_AiExecutions_TenantId_BusinessId] ON [marketing].[AiExecutions] ([TenantId], [BusinessId]);

CREATE INDEX [IX_ApprovalRevocations_TenantId_RevokedBy] ON [marketing].[ApprovalRevocations] ([TenantId], [RevokedBy]);

CREATE UNIQUE INDEX [IX_ApprovedContentRevisions_BusinessId_ContentItemId_RevisionId_ReviewId] ON [marketing].[ApprovedContentRevisions] ([BusinessId], [ContentItemId], [RevisionId], [ReviewId]);

CREATE UNIQUE INDEX [IX_AppUsers_AuthSubject] ON [marketing].[AppUsers] ([AuthSubject]);

CREATE UNIQUE INDEX [IX_AppUsers_Email] ON [marketing].[AppUsers] ([Email]);

CREATE UNIQUE INDEX [IX_AudienceSegments_BusinessId_Name] ON [marketing].[AudienceSegments] ([BusinessId], [Name]);

CREATE INDEX [IX_AudienceSegments_TenantId_BusinessId] ON [marketing].[AudienceSegments] ([TenantId], [BusinessId]);

CREATE INDEX [IX_AuditEvents_ActorUserId] ON [marketing].[AuditEvents] ([ActorUserId]);

CREATE INDEX [IX_AuditEvents_TenantId_BusinessId] ON [marketing].[AuditEvents] ([TenantId], [BusinessId]);

CREATE INDEX [IX_AuditEvents_TenantId_CreatedAt] ON [marketing].[AuditEvents] ([TenantId], [CreatedAt]);

CREATE UNIQUE INDEX [IX_AutomationPolicies_TenantId_BusinessId] ON [marketing].[AutomationPolicies] ([TenantId], [BusinessId]);

CREATE INDEX [IX_AutomationPolicies_TenantId_UpdatedBy] ON [marketing].[AutomationPolicies] ([TenantId], [UpdatedBy]);

CREATE INDEX [IX_AutomationPolicyPlatforms_PlatformCode] ON [marketing].[AutomationPolicyPlatforms] ([PlatformCode]);

CREATE UNIQUE INDEX [IX_BrandProfiles_TenantId_BusinessId] ON [marketing].[BrandProfiles] ([TenantId], [BusinessId]);

CREATE UNIQUE INDEX [IX_BrandTerms_BusinessId_Rule_Term] ON [marketing].[BrandTerms] ([BusinessId], [Rule], [Term]);

CREATE INDEX [IX_BrandTerms_TenantId_BusinessId] ON [marketing].[BrandTerms] ([TenantId], [BusinessId]);

CREATE INDEX [IX_Businesses_TenantId_Status] ON [marketing].[Businesses] ([TenantId], [Status]);

CREATE UNIQUE INDEX [IX_BusinessHours_BusinessId_Weekday_SlotNo] ON [marketing].[BusinessHours] ([BusinessId], [Weekday], [SlotNo]);

CREATE INDEX [IX_BusinessHours_TenantId_BusinessId] ON [marketing].[BusinessHours] ([TenantId], [BusinessId]);

CREATE INDEX [IX_CampaignAudiences_BusinessId_AudienceSegmentId] ON [marketing].[CampaignAudiences] ([BusinessId], [AudienceSegmentId]);

CREATE INDEX [IX_CampaignOffers_BusinessId_OfferId] ON [marketing].[CampaignOffers] ([BusinessId], [OfferId]);

CREATE UNIQUE INDEX [IX_CampaignPlans_BusinessId_CampaignId_VersionNo] ON [marketing].[CampaignPlans] ([BusinessId], [CampaignId], [VersionNo]);

CREATE INDEX [IX_CampaignPlans_BusinessId_GeneratedByExecutionId] ON [marketing].[CampaignPlans] ([BusinessId], [GeneratedByExecutionId]);

CREATE INDEX [IX_CampaignPlans_TenantId_BusinessId] ON [marketing].[CampaignPlans] ([TenantId], [BusinessId]);

CREATE INDEX [IX_CampaignProducts_BusinessId_ProductId] ON [marketing].[CampaignProducts] ([BusinessId], [ProductId]);

CREATE INDEX [IX_Campaigns_BusinessId_Status_CreatedAt] ON [marketing].[Campaigns] ([BusinessId], [Status], [CreatedAt]);

CREATE INDEX [IX_Campaigns_TenantId_BusinessId] ON [marketing].[Campaigns] ([TenantId], [BusinessId]);

CREATE INDEX [IX_Campaigns_TenantId_CreatedBy] ON [marketing].[Campaigns] ([TenantId], [CreatedBy]);

CREATE INDEX [IX_CompetitorInsights_BusinessId_InsightType_CreatedAt] ON [marketing].[CompetitorInsights] ([BusinessId], [InsightType], [CreatedAt]);

CREATE INDEX [IX_CompetitorInsights_BusinessId_StrategyRunId] ON [marketing].[CompetitorInsights] ([BusinessId], [StrategyRunId]);

CREATE INDEX [IX_CompetitorInsights_BusinessId_TrackedSourceId] ON [marketing].[CompetitorInsights] ([BusinessId], [TrackedSourceId]);

CREATE INDEX [IX_CompetitorInsights_TenantId_BusinessId] ON [marketing].[CompetitorInsights] ([TenantId], [BusinessId]);

CREATE INDEX [IX_ContentItems_BusinessId_CampaignId_Status] ON [marketing].[ContentItems] ([BusinessId], [CampaignId], [Status]);

CREATE INDEX [IX_ContentItems_BusinessId_InspiredBySourcePostId] ON [marketing].[ContentItems] ([BusinessId], [InspiredBySourcePostId]);

CREATE INDEX [IX_ContentItems_BusinessId_SourceRecommendationId] ON [marketing].[ContentItems] ([BusinessId], [SourceRecommendationId]);

CREATE UNIQUE INDEX [IX_ContentItems_BusinessId_VariantGroupId_VariantLabel] ON [marketing].[ContentItems] ([BusinessId], [VariantGroupId], [VariantLabel]) WHERE [VariantGroupId] IS NOT NULL;

CREATE INDEX [IX_ContentItems_PlatformCode] ON [marketing].[ContentItems] ([PlatformCode]);

CREATE INDEX [IX_ContentItems_TenantId_BusinessId] ON [marketing].[ContentItems] ([TenantId], [BusinessId]);

CREATE INDEX [IX_ContentReviews_TenantId_BusinessId] ON [marketing].[ContentReviews] ([TenantId], [BusinessId]);

CREATE INDEX [IX_ContentReviews_TenantId_ReviewerId] ON [marketing].[ContentReviews] ([TenantId], [ReviewerId]);

CREATE INDEX [IX_ContentRevisionAssets_BusinessId_AssetId] ON [marketing].[ContentRevisionAssets] ([BusinessId], [AssetId]);

CREATE UNIQUE INDEX [IX_ContentRevisions_BusinessId_ContentItemId_RevisionNo] ON [marketing].[ContentRevisions] ([BusinessId], [ContentItemId], [RevisionNo]);

CREATE INDEX [IX_ContentRevisions_BusinessId_GeneratedByExecutionId] ON [marketing].[ContentRevisions] ([BusinessId], [GeneratedByExecutionId]);

CREATE INDEX [IX_ContentRevisions_BusinessId_SourceRecommendationId] ON [marketing].[ContentRevisions] ([BusinessId], [SourceRecommendationId]);

CREATE INDEX [IX_ContentRevisions_BusinessId_WhatsAppTemplateId] ON [marketing].[ContentRevisions] ([BusinessId], [WhatsAppTemplateId]);

CREATE INDEX [IX_ContentRevisions_TenantId_BusinessId] ON [marketing].[ContentRevisions] ([TenantId], [BusinessId]);

CREATE INDEX [IX_ContentRevisions_TenantId_CreatedBy] ON [marketing].[ContentRevisions] ([TenantId], [CreatedBy]);

CREATE INDEX [IX_ConversionEvents_BusinessId_CampaignId] ON [marketing].[ConversionEvents] ([BusinessId], [CampaignId]);

CREATE INDEX [IX_ConversionEvents_BusinessId_PublicationId_EventType] ON [marketing].[ConversionEvents] ([BusinessId], [PublicationId], [EventType]);

CREATE UNIQUE INDEX [IX_ConversionEvents_BusinessId_Source_ExternalEventId] ON [marketing].[ConversionEvents] ([BusinessId], [Source], [ExternalEventId]) WHERE [ExternalEventId] IS NOT NULL;

CREATE INDEX [IX_ConversionEvents_TenantId_BusinessId] ON [marketing].[ConversionEvents] ([TenantId], [BusinessId]);

CREATE INDEX [IX_CustomerContacts_BusinessId_AudienceSegmentId] ON [marketing].[CustomerContacts] ([BusinessId], [AudienceSegmentId]);

CREATE UNIQUE INDEX [IX_CustomerContacts_BusinessId_PhoneE164] ON [marketing].[CustomerContacts] ([BusinessId], [PhoneE164]);

CREATE INDEX [IX_CustomerContacts_TenantId_BusinessId] ON [marketing].[CustomerContacts] ([TenantId], [BusinessId]);

CREATE INDEX [IX_KnowledgeSources_BusinessId_AssetId] ON [marketing].[KnowledgeSources] ([BusinessId], [AssetId]);

CREATE INDEX [IX_KnowledgeSources_TenantId_BusinessId] ON [marketing].[KnowledgeSources] ([TenantId], [BusinessId]);

CREATE UNIQUE INDEX [IX_MediaAssets_BusinessId_ObjectKey] ON [marketing].[MediaAssets] ([BusinessId], [ObjectKey]);

CREATE INDEX [IX_MediaAssets_BusinessId_ProductId] ON [marketing].[MediaAssets] ([BusinessId], [ProductId]);

CREATE INDEX [IX_MediaAssets_TenantId_BusinessId] ON [marketing].[MediaAssets] ([TenantId], [BusinessId]);

CREATE INDEX [IX_MessageDeliveries_BusinessId_ContactId] ON [marketing].[MessageDeliveries] ([BusinessId], [ContactId]);

CREATE UNIQUE INDEX [IX_MessageDeliveries_BusinessId_PublicationJobId_ContactId] ON [marketing].[MessageDeliveries] ([BusinessId], [PublicationJobId], [ContactId]);

CREATE INDEX [IX_MessageDeliveries_TenantId_BusinessId] ON [marketing].[MessageDeliveries] ([TenantId], [BusinessId]);

CREATE UNIQUE INDEX [IX_MetricObservations_BusinessId_PublicationId_MetricCode_ObservedAt] ON [marketing].[MetricObservations] ([BusinessId], [PublicationId], [MetricCode], [ObservedAt]);

CREATE INDEX [IX_MetricObservations_TenantId_BusinessId] ON [marketing].[MetricObservations] ([TenantId], [BusinessId]);

CREATE INDEX [IX_Offers_TenantId_BusinessId] ON [marketing].[Offers] ([TenantId], [BusinessId]);

CREATE UNIQUE INDEX [IX_Plans_Code] ON [marketing].[Plans] ([Code]);

CREATE INDEX [IX_PlatformDomains_PlatformCode] ON [marketing].[PlatformDomains] ([PlatformCode]);

CREATE INDEX [IX_Products_BusinessId_IsActive] ON [marketing].[Products] ([BusinessId], [IsActive]);

CREATE UNIQUE INDEX [IX_Products_BusinessId_Sku] ON [marketing].[Products] ([BusinessId], [Sku]) WHERE [Sku] IS NOT NULL;

CREATE INDEX [IX_Products_TenantId_BusinessId] ON [marketing].[Products] ([TenantId], [BusinessId]);

CREATE INDEX [IX_PublicationJobs_BusinessId_ContentItemId_PlatformCode] ON [marketing].[PublicationJobs] ([BusinessId], [ContentItemId], [PlatformCode]);

CREATE INDEX [IX_PublicationJobs_BusinessId_ContentItemId_RevisionId] ON [marketing].[PublicationJobs] ([BusinessId], [ContentItemId], [RevisionId]);

CREATE INDEX [IX_PublicationJobs_BusinessId_ScheduledAt] ON [marketing].[PublicationJobs] ([BusinessId], [ScheduledAt]);

CREATE UNIQUE INDEX [IX_PublicationJobs_BusinessId_SocialAccountId_IdempotencyKey] ON [marketing].[PublicationJobs] ([BusinessId], [SocialAccountId], [IdempotencyKey]);

CREATE INDEX [IX_PublicationJobs_BusinessId_SocialAccountId_PlatformCode] ON [marketing].[PublicationJobs] ([BusinessId], [SocialAccountId], [PlatformCode]);

CREATE INDEX [IX_PublicationJobs_BusinessId_SourceRecommendationId] ON [marketing].[PublicationJobs] ([BusinessId], [SourceRecommendationId]);

CREATE INDEX [IX_PublicationJobs_Due] ON [marketing].[PublicationJobs] ([Status], [ScheduledAt]) WHERE [Status] IN (N'Queued', N'Retry');

CREATE INDEX [IX_PublicationJobs_TenantId_BusinessId] ON [marketing].[PublicationJobs] ([TenantId], [BusinessId]);

CREATE UNIQUE INDEX [IX_Publications_BusinessId_PublicationJobId_SocialAccountId] ON [marketing].[Publications] ([BusinessId], [PublicationJobId], [SocialAccountId]);

CREATE UNIQUE INDEX [IX_Publications_BusinessId_SocialAccountId_ExternalPostId] ON [marketing].[Publications] ([BusinessId], [SocialAccountId], [ExternalPostId]) WHERE [ExternalPostId] IS NOT NULL;

CREATE INDEX [IX_Publications_TenantId_BusinessId] ON [marketing].[Publications] ([TenantId], [BusinessId]);

CREATE INDEX [IX_RecommendationEvidence_BusinessId_CompetitorInsightId] ON [marketing].[RecommendationEvidence] ([BusinessId], [CompetitorInsightId]);

CREATE INDEX [IX_RecommendationEvidence_BusinessId_MetricObservationId] ON [marketing].[RecommendationEvidence] ([BusinessId], [MetricObservationId]);

CREATE INDEX [IX_RecommendationEvidence_BusinessId_RecommendationId] ON [marketing].[RecommendationEvidence] ([BusinessId], [RecommendationId]);

CREATE INDEX [IX_RecommendationEvidence_BusinessId_SourcePostId] ON [marketing].[RecommendationEvidence] ([BusinessId], [SourcePostId]);

CREATE INDEX [IX_RecommendationEvidence_TenantId_BusinessId] ON [marketing].[RecommendationEvidence] ([TenantId], [BusinessId]);

CREATE INDEX [IX_SocialAccounts_PlatformCode] ON [marketing].[SocialAccounts] ([PlatformCode]);

CREATE INDEX [IX_SocialAccounts_TenantId_BusinessId] ON [marketing].[SocialAccounts] ([TenantId], [BusinessId]);

CREATE UNIQUE INDEX [IX_SocialAccounts_TenantId_PlatformCode_ExternalAccountId] ON [marketing].[SocialAccounts] ([TenantId], [PlatformCode], [ExternalAccountId]);

CREATE UNIQUE INDEX [IX_SourcePosts_BusinessId_TrackedSourceId_ExternalPostId] ON [marketing].[SourcePosts] ([BusinessId], [TrackedSourceId], [ExternalPostId]) WHERE [ExternalPostId] IS NOT NULL;

CREATE INDEX [IX_SourcePosts_BusinessId_TrackedSourceId_PostedAt] ON [marketing].[SourcePosts] ([BusinessId], [TrackedSourceId], [PostedAt]);

CREATE INDEX [IX_SourcePosts_TenantId_BusinessId] ON [marketing].[SourcePosts] ([TenantId], [BusinessId]);

CREATE INDEX [IX_StrategyRecommendations_BusinessId_CampaignId] ON [marketing].[StrategyRecommendations] ([BusinessId], [CampaignId]);

CREATE INDEX [IX_StrategyRecommendations_BusinessId_StrategyRunId] ON [marketing].[StrategyRecommendations] ([BusinessId], [StrategyRunId]);

CREATE INDEX [IX_StrategyRecommendations_Pending] ON [marketing].[StrategyRecommendations] ([BusinessId], [CreatedAt]) WHERE [DecisionStatus] = N'Pending';

CREATE INDEX [IX_StrategyRecommendations_TenantId_BusinessId] ON [marketing].[StrategyRecommendations] ([TenantId], [BusinessId]);

CREATE INDEX [IX_StrategyRecommendations_TenantId_DecidedBy] ON [marketing].[StrategyRecommendations] ([TenantId], [DecidedBy]);

CREATE INDEX [IX_StrategyRuns_BusinessId_AiExecutionId] ON [marketing].[StrategyRuns] ([BusinessId], [AiExecutionId]);

CREATE INDEX [IX_StrategyRuns_BusinessId_WindowEnd] ON [marketing].[StrategyRuns] ([BusinessId], [WindowEnd]);

CREATE INDEX [IX_StrategyRuns_TenantId_BusinessId] ON [marketing].[StrategyRuns] ([TenantId], [BusinessId]);

CREATE INDEX [IX_Subscriptions_PlanId] ON [marketing].[Subscriptions] ([PlanId]);

CREATE UNIQUE INDEX [IX_Subscriptions_ProviderReference] ON [marketing].[Subscriptions] ([ProviderReference]) WHERE [ProviderReference] IS NOT NULL;

CREATE UNIQUE INDEX [UX_Subscriptions_OneCurrentPerTenant] ON [marketing].[Subscriptions] ([TenantId]) WHERE [Status] IN (N'Trialing', N'Active');

CREATE INDEX [IX_TenantMemberships_UserId] ON [marketing].[TenantMemberships] ([UserId]);

CREATE INDEX [IX_TrackedSources_BusinessId_Kind_Status] ON [marketing].[TrackedSources] ([BusinessId], [Kind], [Status]);

CREATE UNIQUE INDEX [IX_TrackedSources_BusinessId_NormalizedUrl] ON [marketing].[TrackedSources] ([BusinessId], [NormalizedUrl]);

CREATE INDEX [IX_TrackedSources_PlatformCode] ON [marketing].[TrackedSources] ([PlatformCode]);

CREATE INDEX [IX_TrackedSources_TenantId_AddedBy] ON [marketing].[TrackedSources] ([TenantId], [AddedBy]);

CREATE INDEX [IX_TrackedSources_TenantId_BusinessId] ON [marketing].[TrackedSources] ([TenantId], [BusinessId]);

CREATE INDEX [IX_UsageEvents_BusinessId_AiExecutionId] ON [marketing].[UsageEvents] ([BusinessId], [AiExecutionId]);

CREATE INDEX [IX_UsageEvents_TenantId_BusinessId] ON [marketing].[UsageEvents] ([TenantId], [BusinessId]);

CREATE UNIQUE INDEX [IX_UsageEvents_TenantId_IdempotencyKey] ON [marketing].[UsageEvents] ([TenantId], [IdempotencyKey]);

CREATE INDEX [IX_UsageEvents_TenantId_OccurredAt] ON [marketing].[UsageEvents] ([TenantId], [OccurredAt]);

CREATE INDEX [IX_WhatsAppTemplates_BusinessId_SocialAccountId_PlatformCode] ON [marketing].[WhatsAppTemplates] ([BusinessId], [SocialAccountId], [PlatformCode]);

CREATE UNIQUE INDEX [IX_WhatsAppTemplates_BusinessId_SocialAccountId_TemplateName_LanguageCode] ON [marketing].[WhatsAppTemplates] ([BusinessId], [SocialAccountId], [TemplateName], [LanguageCode]);

CREATE INDEX [IX_WhatsAppTemplates_TenantId_BusinessId] ON [marketing].[WhatsAppTemplates] ([TenantId], [BusinessId]);

INSERT INTO [marketing].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261015124527_InitialCreate', N'10.0.12');

COMMIT;
GO

BEGIN TRANSACTION;
EXEC(N'
CREATE TRIGGER [marketing].[TR_ContentRevisions_Immutable]
ON [marketing].[ContentRevisions]
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted)
        THROW 50001, N''ContentRevisions is append-only: rows cannot be updated or deleted.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_ContentReviews_Immutable]
ON [marketing].[ContentReviews]
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted)
        THROW 50001, N''ContentReviews is append-only: rows cannot be updated or deleted.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_ApprovedContentRevisions_Immutable]
ON [marketing].[ApprovedContentRevisions]
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted)
        THROW 50001, N''ApprovedContentRevisions is append-only: rows cannot be updated or deleted.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_ApprovalRevocations_Immutable]
ON [marketing].[ApprovalRevocations]
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted)
        THROW 50001, N''ApprovalRevocations is append-only: rows cannot be updated or deleted.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_AuditEvents_Immutable]
ON [marketing].[AuditEvents]
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted)
        THROW 50001, N''AuditEvents is append-only: rows cannot be updated or deleted.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_ApprovedContentRevisions_Guard]
ON [marketing].[ApprovedContentRevisions]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN [marketing].[ContentReviews] r ON r.[BusinessId] = i.[BusinessId] AND r.[Id] = i.[ReviewId]
        WHERE r.[Decision] <> N''Approved'')
        THROW 50010, N''Only an approving review can authorize a revision.'', 1;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN [marketing].[ContentReviews] r ON r.[BusinessId] = i.[BusinessId] AND r.[Id] = i.[ReviewId]
        WHERE NOT EXISTS (
            SELECT 1 FROM [marketing].[TenantMemberships] m
            WHERE m.[TenantId] = r.[TenantId] AND m.[UserId] = r.[ReviewerId]
              AND m.[Status] = N''Active'' AND m.[Role] IN (N''Owner'', N''Admin'', N''Reviewer'')))
        THROW 50011, N''Approver must be an active owner, admin or reviewer.'', 1;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN [marketing].[ContentReviews] r ON r.[BusinessId] = i.[BusinessId] AND r.[Id] = i.[ReviewId]
        JOIN [marketing].[AutomationPolicies] p ON p.[BusinessId] = i.[BusinessId] AND p.[RequireSeparateReviewer] = 1
        JOIN [marketing].[ContentRevisions] cr ON cr.[BusinessId] = i.[BusinessId] AND cr.[Id] = i.[RevisionId]
        WHERE cr.[CreatedBy] = r.[ReviewerId])
        THROW 50012, N''The author of a revision cannot approve it for this business.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_ApprovalRevocations_Role]
ON [marketing].[ApprovalRevocations]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM [marketing].[TenantMemberships] m
            WHERE m.[TenantId] = i.[TenantId] AND m.[UserId] = i.[RevokedBy]
              AND m.[Status] = N''Active'' AND m.[Role] IN (N''Owner'', N''Admin'', N''Reviewer'')))
        THROW 50013, N''Only an active owner, admin or reviewer can revoke an approval.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_PublicationJobs_Rules]
ON [marketing].[PublicationJobs]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Status updates by the worker do not touch these columns: nothing to check.
    IF NOT (UPDATE([ScheduledAt]) OR UPDATE([PlatformCode]) OR UPDATE([SocialAccountId]) OR UPDATE([RevisionId]))
        RETURN;

    -- Serialize scheduling per business so two requests cannot both pass the daily cap.
    DECLARE @businessId uniqueidentifier, @resource nvarchar(255);
    DECLARE businesses CURSOR LOCAL FAST_FORWARD FOR SELECT DISTINCT [BusinessId] FROM inserted ORDER BY [BusinessId];
    OPEN businesses;
    FETCH NEXT FROM businesses INTO @businessId;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @resource = N''marketing.publication-jobs.'' + CONVERT(nvarchar(36), @businessId);
        EXEC sp_getapplock @Resource = @resource, @LockMode = ''Exclusive'', @LockOwner = ''Transaction'', @LockTimeout = 10000;
        FETCH NEXT FROM businesses INTO @businessId;
    END;
    CLOSE businesses;
    DEALLOCATE businesses;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (SELECT 1 FROM [marketing].[AutomationPolicies] p WHERE p.[BusinessId] = i.[BusinessId]))
        THROW 50020, N''The business owner must define an automation policy before anything is published.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[AutomationPolicies] p ON p.[BusinessId] = i.[BusinessId]
        WHERE p.[AutomationEnabled] = 0)
        THROW 50021, N''Automation is paused by the business owner.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM [marketing].[AutomationPolicyPlatforms] ap
            WHERE ap.[BusinessId] = i.[BusinessId] AND ap.[PlatformCode] = i.[PlatformCode]))
        THROW 50022, N''This platform is not allowed by the business owner.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[ApprovalRevocations] v
          ON v.[BusinessId] = i.[BusinessId] AND v.[ContentItemId] = i.[ContentItemId] AND v.[RevisionId] = i.[RevisionId])
        THROW 50023, N''The approval for this revision has been revoked.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[AutomationPolicies] p ON p.[BusinessId] = i.[BusinessId]
        JOIN [marketing].[Businesses] b ON b.[Id] = i.[BusinessId]
        WHERE p.[PublishWindowStart] IS NOT NULL
          AND (CAST(i.[ScheduledAt] AT TIME ZONE b.[TimeZoneId] AS time) < p.[PublishWindowStart]
               OR CAST(i.[ScheduledAt] AT TIME ZONE b.[TimeZoneId] AS time) >= p.[PublishWindowEnd]))
        THROW 50024, N''The scheduled time is outside the owner''''s publishing window.'', 1;

    IF EXISTS (
        SELECT 1
        FROM (
            SELECT DISTINCT i.[BusinessId], b.[TimeZoneId], p.[MaxPostsPerDay],
                   CAST(i.[ScheduledAt] AT TIME ZONE b.[TimeZoneId] AS date) AS [LocalDay]
            FROM inserted i
            JOIN [marketing].[Businesses] b ON b.[Id] = i.[BusinessId]
            JOIN [marketing].[AutomationPolicies] p ON p.[BusinessId] = i.[BusinessId]
        ) d
        WHERE (SELECT COUNT(*)
               FROM [marketing].[PublicationJobs] j
               WHERE j.[BusinessId] = d.[BusinessId]
                 AND j.[Status] NOT IN (N''Canceled'', N''Failed'')
                 AND CAST(j.[ScheduledAt] AT TIME ZONE d.[TimeZoneId] AS date) = d.[LocalDay]) > d.[MaxPostsPerDay])
        THROW 50025, N''The owner''''s daily post limit has been reached for this day.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.[PlatformCode] = N''whatsapp''
          AND NOT EXISTS (
              SELECT 1
              FROM [marketing].[ContentRevisions] cr
              JOIN [marketing].[WhatsAppTemplates] t ON t.[BusinessId] = cr.[BusinessId] AND t.[Id] = cr.[WhatsAppTemplateId]
              WHERE cr.[BusinessId] = i.[BusinessId] AND cr.[Id] = i.[RevisionId]
                AND t.[ProviderStatus] = N''Approved'' AND t.[SocialAccountId] = i.[SocialAccountId]))
        THROW 50026, N''WhatsApp messages need an approved template on the same WhatsApp account.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_MessageDeliveries_Consent]
ON [marketing].[MessageDeliveries]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[PublicationJobs] j ON j.[BusinessId] = i.[BusinessId] AND j.[Id] = i.[PublicationJobId]
        WHERE j.[PlatformCode] <> N''whatsapp'')
        THROW 50030, N''Message deliveries belong to WhatsApp jobs only.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[CustomerContacts] c ON c.[BusinessId] = i.[BusinessId] AND c.[Id] = i.[ContactId]
        WHERE c.[WhatsAppOptIn] = 0
           OR (c.[OptedOutAt] IS NOT NULL AND c.[OptedOutAt] >= c.[OptedInAt]))
        THROW 50031, N''The customer has not opted in to WhatsApp messages.'', 1;
END;');

EXEC(N'
CREATE TRIGGER [marketing].[TR_StrategyRecommendations_Guard]
ON [marketing].[StrategyRecommendations]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.[AppliedAutomatically] = 1
          AND NOT EXISTS (
              SELECT 1
              FROM [marketing].[AutomationPolicies] p
              JOIN [marketing].[AutoApplyRules] r ON r.[BusinessId] = p.[BusinessId]
              WHERE p.[BusinessId] = i.[BusinessId] AND p.[AutomationEnabled] = 1 AND r.[ActionType] = i.[ActionType]))
        THROW 50040, N''This recommendation type is not allowed to be applied automatically.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        LEFT JOIN deleted d ON d.[Id] = i.[Id]
        WHERE i.[DecidedBy] IS NOT NULL
          AND (d.[Id] IS NULL OR d.[DecidedBy] IS NULL OR d.[DecidedBy] <> i.[DecidedBy])
          AND NOT EXISTS (
              SELECT 1 FROM [marketing].[TenantMemberships] m
              WHERE m.[TenantId] = i.[TenantId] AND m.[UserId] = i.[DecidedBy]
                AND m.[Status] = N''Active'' AND m.[Role] IN (N''Owner'', N''Admin'')))
        THROW 50041, N''Only an active owner or admin can decide on recommendations.'', 1;
END;');

EXEC(N'
CREATE VIEW [marketing].[ContentPerformance]
AS
SELECT
    p.[TenantId],
    p.[BusinessId],
    ci.[CampaignId],
    ci.[Id]                 AS [ContentItemId],
    j.[RevisionId],
    p.[Id]                  AS [PublicationId],
    j.[PlatformCode],
    ci.[ContentType],
    cr.[Format],
    cr.[HookType],
    cr.[CallToAction],
    cr.[IncludesOffer],
    ci.[VariantGroupId],
    ci.[VariantLabel],
    COALESCE(cr.[SourceRecommendationId], j.[SourceRecommendationId], ci.[SourceRecommendationId]) AS [SourceRecommendationId],
    ci.[InspiredBySourcePostId],
    p.[PublishedAt],
    DATEDIFF(day, ''19000101'', CAST(p.[PublishedAt] AT TIME ZONE b.[TimeZoneId] AS date)) % 7 + 1 AS [LocalIsoWeekday],
    DATEPART(hour, p.[PublishedAt] AT TIME ZONE b.[TimeZoneId]) AS [LocalHour],
    m.[MetricCode],
    m.[MetricValue],
    m.[ObservedAt]
FROM [marketing].[Publications] p
JOIN [marketing].[PublicationJobs] j   ON j.[BusinessId] = p.[BusinessId] AND j.[Id] = p.[PublicationJobId]
JOIN [marketing].[ContentItems] ci     ON ci.[BusinessId] = j.[BusinessId] AND ci.[Id] = j.[ContentItemId]
JOIN [marketing].[ContentRevisions] cr ON cr.[BusinessId] = j.[BusinessId] AND cr.[Id] = j.[RevisionId]
JOIN [marketing].[Businesses] b        ON b.[Id] = p.[BusinessId]
CROSS APPLY (
    SELECT x.[MetricCode], x.[MetricValue], x.[ObservedAt]
    FROM (
        SELECT mo.[MetricCode], mo.[MetricValue], mo.[ObservedAt],
               ROW_NUMBER() OVER (PARTITION BY mo.[MetricCode] ORDER BY mo.[ObservedAt] DESC) AS [rn]
        FROM [marketing].[MetricObservations] mo
        WHERE mo.[BusinessId] = p.[BusinessId] AND mo.[PublicationId] = p.[Id]
    ) x
    WHERE x.[rn] = 1
) m;');

EXEC(N'
CREATE VIEW [marketing].[CompetitorBenchmark]
AS
SELECT
    s.[TenantId],
    s.[BusinessId],
    s.[Id]           AS [TrackedSourceId],
    s.[Kind],
    s.[PlatformCode],
    sp.[Format],
    sp.[HookType],
    DATEPART(hour, sp.[PostedAt] AT TIME ZONE b.[TimeZoneId]) AS [LocalHour],
    COUNT(*)                                          AS [Posts],
    CAST(AVG(sp.[EngagementRate]) AS decimal(18,6))   AS [AvgEngagementRate],
    CAST(AVG(CAST(sp.[LikesCount] AS decimal(18,2)))    AS decimal(18,2)) AS [AvgLikes],
    CAST(AVG(CAST(sp.[CommentsCount] AS decimal(18,2))) AS decimal(18,2)) AS [AvgComments],
    CAST(AVG(CAST(sp.[ViewsCount] AS decimal(20,2)))    AS decimal(20,2)) AS [AvgViews]
FROM [marketing].[TrackedSources] s
JOIN [marketing].[SourcePosts] sp ON sp.[BusinessId] = s.[BusinessId] AND sp.[TrackedSourceId] = s.[Id]
JOIN [marketing].[Businesses] b   ON b.[Id] = s.[BusinessId]
GROUP BY s.[TenantId], s.[BusinessId], s.[Id], s.[Kind], s.[PlatformCode], sp.[Format], sp.[HookType],
         DATEPART(hour, sp.[PostedAt] AT TIME ZONE b.[TimeZoneId]);');

INSERT INTO [marketing].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261015124537_AddBusinessRules', N'10.0.12');

COMMIT;
GO

