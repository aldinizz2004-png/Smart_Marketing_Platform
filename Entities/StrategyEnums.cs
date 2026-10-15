namespace SmartMarketing.Domain.Enums;

public enum AiTaskType { Strategy, Copy, Creative, Review, Optimization, CompetitorAnalysis }

public enum ExecutionStatus { Queued, Running, Succeeded, Failed }

public enum CampaignObjective { Orders, Messages, Visits, Awareness, ProductPromotion, Followers }

public enum CampaignStatus { Draft, Planning, Ready, Active, Paused, Completed, Canceled }

public enum RunStatus { Queued, Running, Completed, Failed }

public enum RecommendationAction { NewContent, ReviseCopy, ChangeFormat, ChangeChannel, Reschedule, Pause }

public enum DecisionStatus { Pending, Accepted, Rejected, Applied }
