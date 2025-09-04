using System.Net;
using BoldDesk.Models;

namespace BoldDesk.Exceptions;

public class BoldDeskApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public BoldDeskErrorResponse? ErrorResponse { get; }
    public List<BoldDeskError> Errors => ErrorResponse?.Errors ?? new List<BoldDeskError>();

    public BoldDeskApiException(string message) : base(message)
    {
        StatusCode = HttpStatusCode.InternalServerError;
    }

    public BoldDeskApiException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public BoldDeskApiException(BoldDeskErrorResponse errorResponse) 
        : base(errorResponse?.Message ?? "An error occurred while calling the BoldDesk API")
    {
        ErrorResponse = errorResponse;
        StatusCode = (HttpStatusCode)(errorResponse?.StatusCode ?? 500);
    }

    public BoldDeskApiException(string message, HttpStatusCode statusCode, BoldDeskErrorResponse? errorResponse) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorResponse = errorResponse;
    }

    public BoldDeskApiException(string message, Exception innerException) 
        : base(message, innerException)
    {
        StatusCode = HttpStatusCode.InternalServerError;
    }

    public bool HasFieldError(string fieldName)
    {
        return Errors.Any(e => e.Field.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
    }

    public BoldDeskError? GetFieldError(string fieldName)
    {
        return Errors.FirstOrDefault(e => e.Field.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasErrorType(string errorType)
    {
        return Errors.Any(e => e.ErrorType.Equals(errorType, StringComparison.OrdinalIgnoreCase));
    }

    public override string ToString()
    {
        var baseString = base.ToString();
        
        if (ErrorResponse != null && Errors.Any())
        {
            var errorDetails = string.Join(Environment.NewLine, 
                Errors.Select(e => $"  - Field: {e.Field}, Type: {e.ErrorType}, Message: {e.ErrorMessage}"));
            
            return $"{baseString}{Environment.NewLine}API Errors:{Environment.NewLine}{errorDetails}";
        }
        
        return baseString;
    }
}