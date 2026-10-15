namespace SmartMarketing.Domain.Enums;

public enum TenantStatus { Active, Suspended, Closed }

public enum MembershipRole { Owner, Admin, Editor, Reviewer, Viewer }

public enum MembershipStatus { Invited, Active, Disabled }

public enum SubscriptionStatus { Trialing, Active, PastDue, Canceled, Expired }
