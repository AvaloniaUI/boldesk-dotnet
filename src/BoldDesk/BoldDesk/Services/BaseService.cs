using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using BoldDesk.Exceptions;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Base class for BoldDesk sub-services
/// </summary>
public abstract class BaseService
{
    protected readonly HttpClient HttpClient;
    protected readonly string BaseUrl;
    protected readonly JsonSerializerOptions JsonOptions;
    protected RateLimitInfo? LastRateLimitInfo;

    protected BaseService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions)
    {
        HttpClient = httpClient;
        BaseUrl = baseUrl;
        JsonOptions = jsonOptions;
    }

    /// <summary>
    /// Gets the last rate limit information from API responses
    /// </summary>
    public RateLimitInfo? GetLastRateLimitInfo() => LastRateLimitInfo;

    protected void ParseRateLimitHeaders(HttpResponseHeaders headers)
    {
        LastRateLimitInfo = new RateLimitInfo();

        if (headers.TryGetValues("x-rate-limit-limit", out var limitValues))
        {
            if (int.TryParse(limitValues.FirstOrDefault(), out var limit))
            {
                LastRateLimitInfo.Limit = limit;
            }
        }

        if (headers.TryGetValues("x-rate-limit-remaining", out var remainingValues))
        {
            if (int.TryParse(remainingValues.FirstOrDefault(), out var remaining))
            {
                LastRateLimitInfo.Remaining = remaining;
            }
        }

        if (headers.TryGetValues("x-rate-limit-reset", out var resetValues))
        {
            if (DateTime.TryParse(resetValues.FirstOrDefault(), out var reset))
            {
                LastRateLimitInfo.Reset = reset;
            }
        }
    }

    protected async Task EnsureRateLimitCompliance()
    {
        if (LastRateLimitInfo == null)
            return;

        // If we're getting close to the rate limit, wait
        if (LastRateLimitInfo.Remaining <= 5)
        {
            var waitTime = LastRateLimitInfo.Reset - DateTime.UtcNow;
            if (waitTime > TimeSpan.Zero && waitTime < TimeSpan.FromMinutes(2))
            {
                Console.WriteLine($"Rate limit nearly exceeded. Waiting {waitTime.TotalSeconds:F0} seconds...");
                await Task.Delay(waitTime);
            }
        }
    }

    protected async Task ThrowBoldDeskException(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        BoldDeskErrorResponse? errorResponse = null;

        // Try to parse error response
        if (!string.IsNullOrWhiteSpace(errorContent))
        {
            try
            {
                errorResponse = JsonSerializer.Deserialize<BoldDeskErrorResponse>(errorContent, JsonOptions);
            }
            catch
            {
                // If parsing fails, create a generic error response
                errorResponse = new BoldDeskErrorResponse
                {
                    Message = errorContent,
                    StatusCode = (int)response.StatusCode,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = errorContent,
                            ErrorType = "UnknownError"
                        }
                    }
                };
            }
        }

        // Throw specific exception types based on status code
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new BoldDeskAuthenticationException(errorResponse ?? new BoldDeskErrorResponse 
                { 
                    Message = "Authentication failed. Please verify your API key.",
                    StatusCode = 401,
                    Errors = new List<BoldDeskError> 
                    { 
                        new BoldDeskError 
                        { 
                            Field = "User", 
                            ErrorMessage = "Unauthorized", 
                            ErrorType = BoldDeskErrorType.Unauthorized 
                        } 
                    }
                });

            case HttpStatusCode.Forbidden:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Access denied. Your API key may not have permission to access this resource.",
                    StatusCode = 403,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "UserID",
                            ErrorMessage = "Access Denied",
                            ErrorType = BoldDeskErrorType.AccessDenied
                        }
                    }
                });

            case HttpStatusCode.BadRequest:
                throw new BoldDeskValidationException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = errorContent,
                            ErrorType = BoldDeskErrorType.InvalidValue
                        }
                    }
                });

            case HttpStatusCode.TooManyRequests:
                throw new BoldDeskRateLimitException(
                    errorResponse ?? new BoldDeskErrorResponse
                    {
                        Message = "Rate limit exceeded",
                        StatusCode = 429,
                        Errors = new List<BoldDeskError>
                        {
                            new BoldDeskError
                            {
                                Field = "",
                                ErrorMessage = "API calls quota exceeded",
                                ErrorType = BoldDeskErrorType.APICallQuotaExceeded
                            }
                        }
                    },
                    LastRateLimitInfo);

            case HttpStatusCode.NotFound:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Resource not found",
                    StatusCode = 404,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Not found",
                            ErrorType = BoldDeskErrorType.NotFound
                        }
                    }
                });

            case HttpStatusCode.MethodNotAllowed:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Method not allowed",
                    StatusCode = 405,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Method not allowed",
                            ErrorType = BoldDeskErrorType.MethodNotAllowed
                        }
                    }
                });

            case HttpStatusCode.UnsupportedMediaType:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Unsupported media type",
                    StatusCode = 415,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Unsupported Media Type",
                            ErrorType = BoldDeskErrorType.UnsupportedMediaType
                        }
                    }
                });

            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Unable to process your request. Please try again later",
                    StatusCode = (int)response.StatusCode,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Unable to process your request. Please try again later",
                            ErrorType = BoldDeskErrorType.UnknownError
                        }
                    }
                });

            default:
                throw new BoldDeskApiException(
                    errorResponse?.Message ?? $"API request failed with status {response.StatusCode}",
                    response.StatusCode,
                    errorResponse);
        }
    }

    protected async Task<T> ExecuteRequestAsync<T>(string url) where T : new()
    {
        try
        {
            var response = await HttpClient.GetAsync(url);
            
            // Parse rate limit headers
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return new T();
            }
            
            var result = JsonSerializer.Deserialize<T>(content, JsonOptions);
            
            return result ?? new T();
        }
        catch (BoldDeskApiException)
        {
            // Re-throw BoldDesk exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            // Wrap HTTP exceptions in BoldDeskApiException
            throw new BoldDeskApiException($"Network error while calling BoldDesk API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new BoldDeskApiException($"Failed to parse API response: {ex.Message}. The API may have returned an unexpected format.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new BoldDeskApiException($"API request timed out: {ex.Message}", ex);
        }
    }
}