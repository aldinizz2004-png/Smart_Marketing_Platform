namespace SmartMarketing.Domain.Enums;

public enum BusinessStatus { Draft, Active, Paused, Closed }

public enum BrandTermRule { Preferred, Prohibited }

public enum OfferStatus { Draft, Active, Paused, Ended }

public enum MediaKind { Logo, ProductPhoto, BrandImage, GeneratedImage, Video, Document }

public enum KnowledgeSourceType { Menu, Guidelines, Website, HistoricalPost, Other }

public enum ProcessingStatus { Pending, Processed, Failed, Disabled }

public enum SocialAccountStatus { Connected, Expired, Disconnected }

public enum OptInSource { InStore, WebsiteForm, WhatsAppReply, ImportWithConsent }

public enum TemplateCategory { Marketing, Utility }

public enum TemplateStatus { Pending, Approved, Rejected, Paused }
