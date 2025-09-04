namespace BoldDesk.Models;

public static class BoldDeskErrorType
{
    public const string NotExist = "NotExist";
    public const string AlreadyExists = "AlreadyExists";
    public const string FieldRequired = "FieldRequired";
    public const string InvalidValue = "InvalidValue";
    public const string LengthExceeds = "LengthExceeds";
    public const string DetailsNotFound = "DetailsNotFound";
    public const string AlreadyActivated = "AlreadyActivated";
    public const string AlreadyDeactivated = "AlreadyDeactivated";
    public const string AlreadyBlocked = "AlreadyBlocked";
    public const string AlreadyUnblocked = "AlreadyUnblocked";
    public const string AlreadyAvailable = "AlreadyAvailable";
    public const string AlreadyNotAvailable = "AlreadyNotAvailable";
    public const string AlreadyDeleted = "AlreadyDeleted";
    public const string AlreadyRestored = "AlreadyRestored";
    public const string NoChangeFound = "NoChangeFound";
    public const string QuotaExceedsLimit = "QuotaExceedsLimit";
    public const string NotAllowed = "NotAllowed";
    public const string FileSizeLarge = "FileSizeLarge";
    public const string FileExtensionInvalid = "FileExtensionInvalid";
    public const string Unauthorized = "Unauthorized";
    public const string AccessDenied = "AccessDenied";
    public const string NotFound = "NotFound";
    public const string MethodNotAllowed = "MethodNotAllowed";
    public const string UnsupportedMediaType = "UnsupportedMediaType";
    public const string APICallQuotaExceeded = "APICallQuotaExceeded";
    public const string UnknownError = "UnknownError";
}