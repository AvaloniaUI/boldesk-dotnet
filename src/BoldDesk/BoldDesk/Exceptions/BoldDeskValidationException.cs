using System.Net;
using BoldDesk.Models;

namespace BoldDesk.Exceptions;

public class BoldDeskValidationException : BoldDeskApiException
{
    public Dictionary<string, List<string>> FieldErrors { get; }

    public BoldDeskValidationException(string message) 
        : base(message, HttpStatusCode.BadRequest)
    {
        FieldErrors = new Dictionary<string, List<string>>();
    }

    public BoldDeskValidationException(BoldDeskErrorResponse errorResponse) 
        : base(errorResponse)
    {
        FieldErrors = new Dictionary<string, List<string>>();
        
        foreach (var error in errorResponse.Errors)
        {
            if (!string.IsNullOrEmpty(error.Field))
            {
                if (!FieldErrors.ContainsKey(error.Field))
                {
                    FieldErrors[error.Field] = new List<string>();
                }
                FieldErrors[error.Field].Add(error.ErrorMessage);
            }
        }
    }

    public bool HasFieldErrors()
    {
        return FieldErrors.Any();
    }

    public List<string> GetFieldErrors(string fieldName)
    {
        return FieldErrors.TryGetValue(fieldName, out var errors) 
            ? errors 
            : new List<string>();
    }

    public override string ToString()
    {
        var baseString = base.ToString();
        
        if (HasFieldErrors())
        {
            var fieldErrorsString = string.Join(Environment.NewLine,
                FieldErrors.Select(kv => $"  {kv.Key}: {string.Join(", ", kv.Value)}"));
            
            return $"{baseString}{Environment.NewLine}Validation Errors:{Environment.NewLine}{fieldErrorsString}";
        }
        
        return baseString;
    }
}