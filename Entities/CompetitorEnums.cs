namespace SmartMarketing.Domain.Enums;

// Why the owner added this URL: a direct competitor, a professional
// account in the same field, or general inspiration.
public enum TrackedSourceKind { Competitor, Professional, Inspiration }

public enum TrackingStatus { Pending, Active, Paused, Failed }

public enum InsightType { PostingTime, Format, Hook, Hashtags, Offer, Tone, Frequency, Engagement }
