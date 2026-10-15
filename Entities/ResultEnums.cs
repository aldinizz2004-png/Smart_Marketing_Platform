namespace SmartMarketing.Domain.Enums;

public enum MetricCode
{
    Impressions, Reach, Likes, Comments, Shares, Saves, LinkClicks, ProfileVisits,
    MessagesStarted, VideoViews, WatchTimeSeconds, Follows, Delivered, Read, Replies
}

public enum MetricSource { PlatformApi, Webhook, Manual }

public enum ConversionType { LinkClick, Message, Lead, Order, StoreVisit }

public enum ConversionSource { TrackedLink, Integration, Manual, Coupon }

public enum UsageEventType
{
    CopyGeneration, ImageGeneration, VideoGeneration, Publish, CompetitorScan,
    CreditGrant, CreditAdjustment
}
