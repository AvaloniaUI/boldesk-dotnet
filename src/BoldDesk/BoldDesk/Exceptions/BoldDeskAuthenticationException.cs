using System.Net;
using BoldDesk.Models;

namespace BoldDesk.Exceptions;

public class BoldDeskAuthenticationException : BoldDeskApiException
{
    public BoldDeskAuthenticationException(string message) 
        : base(message, HttpStatusCode.Unauthorized)
    {
    }

    public BoldDeskAuthenticationException(BoldDeskErrorResponse errorResponse) 
        : base(errorResponse)
    {
    }

    public BoldDeskAuthenticationException() 
        : base("Authentication failed. Please verify your API key is correct and has not expired.", HttpStatusCode.Unauthorized)
    {
    }
}