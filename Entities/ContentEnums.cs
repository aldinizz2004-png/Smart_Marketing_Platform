namespace SmartMarketing.Domain.Enums;

public enum ContentType { Post, Story, Reel, Short, AdCopy, WhatsAppMessage }

public enum ContentFormat { Text, SingleImage, Carousel, Video, Reel, Story, Live }

public enum HookType { Question, Offer, Story, SocialProof, Urgency, Informative, Humor, Tutorial }

public enum CallToAction { OrderNow, SendMessage, VisitStore, LearnMore, Follow, None }

public enum ContentStatus { Draft, InReview, Approved, Scheduled, Published, Canceled }

public enum ReviewDecision { Approved, Rejected, ChangesRequested }

public enum JobStatus { Queued, Running, Succeeded, Retry, Failed, Canceled }

public enum DeliveryStatus { Queued, Sent, Delivered, Read, Failed }
