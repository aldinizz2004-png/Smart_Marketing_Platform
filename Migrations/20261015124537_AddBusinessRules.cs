using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMarketing.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Business rules that a relational schema alone cannot express:
    /// approval guards, the owner's publishing limits, WhatsApp consent,
    /// append-only history, and the two reporting views used for learning.
    /// EF Core does not generate triggers or views, so they are written here as SQL.
    /// Every trigger is also declared in its entity configuration with HasTrigger(...)
    /// so EF Core saves changes in a trigger-compatible way.
    /// </summary>
    public partial class AddBusinessRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------------------------------------------------------------
            // 1. Append-only history
            // ---------------------------------------------------------------
            foreach (var table in AppendOnlyTables)
            {
                migrationBuilder.Sql(Exec($@"
CREATE TRIGGER [marketing].[TR_{table}_Immutable]
ON [marketing].[{table}]
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted)
        THROW 50001, N'{table} is append-only: rows cannot be updated or deleted.', 1;
END;"));
            }

            // ---------------------------------------------------------------
            // 2. Approval: the review must approve, and the approver must be
            //    an active owner / admin / reviewer (four-eyes rule optional).
            // ---------------------------------------------------------------
            migrationBuilder.Sql(Exec(@"
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
        WHERE r.[Decision] <> N'Approved')
        THROW 50010, N'Only an approving review can authorize a revision.', 1;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN [marketing].[ContentReviews] r ON r.[BusinessId] = i.[BusinessId] AND r.[Id] = i.[ReviewId]
        WHERE NOT EXISTS (
            SELECT 1 FROM [marketing].[TenantMemberships] m
            WHERE m.[TenantId] = r.[TenantId] AND m.[UserId] = r.[ReviewerId]
              AND m.[Status] = N'Active' AND m.[Role] IN (N'Owner', N'Admin', N'Reviewer')))
        THROW 50011, N'Approver must be an active owner, admin or reviewer.', 1;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN [marketing].[ContentReviews] r ON r.[BusinessId] = i.[BusinessId] AND r.[Id] = i.[ReviewId]
        JOIN [marketing].[AutomationPolicies] p ON p.[BusinessId] = i.[BusinessId] AND p.[RequireSeparateReviewer] = 1
        JOIN [marketing].[ContentRevisions] cr ON cr.[BusinessId] = i.[BusinessId] AND cr.[Id] = i.[RevisionId]
        WHERE cr.[CreatedBy] = r.[ReviewerId])
        THROW 50012, N'The author of a revision cannot approve it for this business.', 1;
END;"));

            migrationBuilder.Sql(Exec(@"
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
              AND m.[Status] = N'Active' AND m.[Role] IN (N'Owner', N'Admin', N'Reviewer')))
        THROW 50013, N'Only an active owner, admin or reviewer can revoke an approval.', 1;
END;"));

            // ---------------------------------------------------------------
            // 3. The owner's limits, checked whenever a job is scheduled or moved.
            // ---------------------------------------------------------------
            migrationBuilder.Sql(Exec(@"
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
        SET @resource = N'marketing.publication-jobs.' + CONVERT(nvarchar(36), @businessId);
        EXEC sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000;
        FETCH NEXT FROM businesses INTO @businessId;
    END;
    CLOSE businesses;
    DEALLOCATE businesses;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (SELECT 1 FROM [marketing].[AutomationPolicies] p WHERE p.[BusinessId] = i.[BusinessId]))
        THROW 50020, N'The business owner must define an automation policy before anything is published.', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[AutomationPolicies] p ON p.[BusinessId] = i.[BusinessId]
        WHERE p.[AutomationEnabled] = 0)
        THROW 50021, N'Automation is paused by the business owner.', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM [marketing].[AutomationPolicyPlatforms] ap
            WHERE ap.[BusinessId] = i.[BusinessId] AND ap.[PlatformCode] = i.[PlatformCode]))
        THROW 50022, N'This platform is not allowed by the business owner.', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[ApprovalRevocations] v
          ON v.[BusinessId] = i.[BusinessId] AND v.[ContentItemId] = i.[ContentItemId] AND v.[RevisionId] = i.[RevisionId])
        THROW 50023, N'The approval for this revision has been revoked.', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[AutomationPolicies] p ON p.[BusinessId] = i.[BusinessId]
        JOIN [marketing].[Businesses] b ON b.[Id] = i.[BusinessId]
        WHERE p.[PublishWindowStart] IS NOT NULL
          AND (CAST(i.[ScheduledAt] AT TIME ZONE b.[TimeZoneId] AS time) < p.[PublishWindowStart]
               OR CAST(i.[ScheduledAt] AT TIME ZONE b.[TimeZoneId] AS time) >= p.[PublishWindowEnd]))
        THROW 50024, N'The scheduled time is outside the owner''s publishing window.', 1;

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
                 AND j.[Status] NOT IN (N'Canceled', N'Failed')
                 AND CAST(j.[ScheduledAt] AT TIME ZONE d.[TimeZoneId] AS date) = d.[LocalDay]) > d.[MaxPostsPerDay])
        THROW 50025, N'The owner''s daily post limit has been reached for this day.', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.[PlatformCode] = N'whatsapp'
          AND NOT EXISTS (
              SELECT 1
              FROM [marketing].[ContentRevisions] cr
              JOIN [marketing].[WhatsAppTemplates] t ON t.[BusinessId] = cr.[BusinessId] AND t.[Id] = cr.[WhatsAppTemplateId]
              WHERE cr.[BusinessId] = i.[BusinessId] AND cr.[Id] = i.[RevisionId]
                AND t.[ProviderStatus] = N'Approved' AND t.[SocialAccountId] = i.[SocialAccountId]))
        THROW 50026, N'WhatsApp messages need an approved template on the same WhatsApp account.', 1;
END;"));

            // ---------------------------------------------------------------
            // 4. WhatsApp consent: only opted-in customers receive messages.
            // ---------------------------------------------------------------
            migrationBuilder.Sql(Exec(@"
CREATE TRIGGER [marketing].[TR_MessageDeliveries_Consent]
ON [marketing].[MessageDeliveries]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[PublicationJobs] j ON j.[BusinessId] = i.[BusinessId] AND j.[Id] = i.[PublicationJobId]
        WHERE j.[PlatformCode] <> N'whatsapp')
        THROW 50030, N'Message deliveries belong to WhatsApp jobs only.', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN [marketing].[CustomerContacts] c ON c.[BusinessId] = i.[BusinessId] AND c.[Id] = i.[ContactId]
        WHERE c.[WhatsAppOptIn] = 0
           OR (c.[OptedOutAt] IS NOT NULL AND c.[OptedOutAt] >= c.[OptedInAt]))
        THROW 50031, N'The customer has not opted in to WhatsApp messages.', 1;
END;"));

            // ---------------------------------------------------------------
            // 5. The AI may only auto-apply what the owner allowed; human
            //    decisions on recommendations are for owners and admins.
            // ---------------------------------------------------------------
            migrationBuilder.Sql(Exec(@"
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
        THROW 50040, N'This recommendation type is not allowed to be applied automatically.', 1;

    IF EXISTS (
        SELECT 1 FROM inserted i
        LEFT JOIN deleted d ON d.[Id] = i.[Id]
        WHERE i.[DecidedBy] IS NOT NULL
          AND (d.[Id] IS NULL OR d.[DecidedBy] IS NULL OR d.[DecidedBy] <> i.[DecidedBy])
          AND NOT EXISTS (
              SELECT 1 FROM [marketing].[TenantMemberships] m
              WHERE m.[TenantId] = i.[TenantId] AND m.[UserId] = i.[DecidedBy]
                AND m.[Status] = N'Active' AND m.[Role] IN (N'Owner', N'Admin')))
        THROW 50041, N'Only an active owner or admin can decide on recommendations.', 1;
END;"));

            // ---------------------------------------------------------------
            // 6. Learning views
            // ---------------------------------------------------------------
            migrationBuilder.Sql(Exec(@"
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
    DATEDIFF(day, '19000101', CAST(p.[PublishedAt] AT TIME ZONE b.[TimeZoneId] AS date)) % 7 + 1 AS [LocalIsoWeekday],
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
) m;"));

            migrationBuilder.Sql(Exec(@"
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
         DATEPART(hour, sp.[PostedAt] AT TIME ZONE b.[TimeZoneId]);"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [marketing].[CompetitorBenchmark];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [marketing].[ContentPerformance];");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [marketing].[TR_StrategyRecommendations_Guard];");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [marketing].[TR_MessageDeliveries_Consent];");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [marketing].[TR_PublicationJobs_Rules];");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [marketing].[TR_ApprovalRevocations_Role];");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [marketing].[TR_ApprovedContentRevisions_Guard];");

            foreach (var table in AppendOnlyTables)
            {
                migrationBuilder.Sql($"DROP TRIGGER IF EXISTS [marketing].[TR_{table}_Immutable];");
            }
        }

        // CREATE TRIGGER / CREATE VIEW must be the first statement of a batch.
        // Running them through EXEC keeps that true both for 'database update'
        // and for the generated SQL script (where a whole migration is one batch).
        private static string Exec(string sql) => "EXEC(N'" + sql.Replace("'", "''") + "');";

        private static readonly string[] AppendOnlyTables =
        {
            "ContentRevisions",
            "ContentReviews",
            "ApprovedContentRevisions",
            "ApprovalRevocations",
            "AuditEvents",
        };
    }
}
