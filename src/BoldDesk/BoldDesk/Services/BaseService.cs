using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BoldDesk.Exceptions;
using BoldDesk.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BoldDesk.Services;

/// <summary>
/// Base class for BoldDesk sub-services
/// </summary>
public abstract class BaseService
{
    protected readonly HttpClient HttpClient;
    protected readonly string BaseUrl;
    protected readonly JsonSerializerOptions JsonOptions;
    protected readonly ILogger Logger;
    protected RateLimitInfo? LastRateLimitInfo;

    protected BaseService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions, ILogger? logger = null)
    {
        HttpClient = httpClient;
        BaseUrl = baseUrl;
        JsonOptions = jsonOptions;
        Logger = logger ?? NullLogger.Instance;
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
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Logger.LogDebug("[BoldDesk GET] Starting request to: {Url}", url);

        try
        {
            var response = await HttpClient.GetAsync(url);
            stopwatch.Stop();

            Logger.LogDebug("[BoldDesk GET] Response {StatusCode} from {Url} in {ElapsedMs}ms",
                (int)response.StatusCode, url, stopwatch.ElapsedMilliseconds);

            // Parse rate limit headers
            ParseRateLimitHeaders(response.Headers);

            if (LastRateLimitInfo != null)
            {
                Logger.LogDebug("[BoldDesk] Rate limit: {Remaining}/{Limit}, resets at {Reset}",
                    LastRateLimitInfo.Remaining, LastRateLimitInfo.Limit, LastRateLimitInfo.Reset);
            }

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("[BoldDesk GET] FAILED {StatusCode} for {Url}. Response body: {Response}",
                    (int)response.StatusCode, url, content);
                await ThrowBoldDeskException(response);
            }

            Logger.LogDebug("[BoldDesk GET] Success for {Url}. Response length: {ContentLength} chars", url, content?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(content))
            {
                Logger.LogWarning("[BoldDesk GET] Empty response body for {Url}", url);
                return new T();
            }

            Logger.LogTrace("[BoldDesk GET] Response body: {Content}", content);

            var result = JsonSerializer.Deserialize<T>(content, JsonOptions);

            if (result == null)
            {
                Logger.LogWarning("[BoldDesk GET] Deserialization returned null for {Url}", url);
            }

            return result ?? new T();
        }
        catch (BoldDeskApiException)
        {
            // Already logged above, just rethrow
            throw;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            Logger.LogError(ex, "[BoldDesk GET] Network error after {ElapsedMs}ms for {Url}: {Message}",
                stopwatch.ElapsedMilliseconds, url, ex.Message);
            throw new BoldDeskApiException($"Network error while calling BoldDesk API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, "[BoldDesk GET] JSON deserialization failed for {Url}: {Message}", url, ex.Message);
            throw new BoldDeskApiException($"Failed to parse API response: {ex.Message}. The API may have returned an unexpected format.", ex);
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();
            Logger.LogError(ex, "[BoldDesk GET] Request timeout after {ElapsedMs}ms for {Url}: {Message}",
                stopwatch.ElapsedMilliseconds, url, ex.Message);
            throw new BoldDeskTimeoutException(
                "A timeout occurred while communicating with our support system. Please try again later.",
                url,
                stopwatch.ElapsedMilliseconds,
                ex);
        }
    }

    protected async Task<TResponse> ExecutePostRequestAsync<TRequest, TResponse>(string url, TRequest request) where TResponse : new()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var requestJson = JsonSerializer.Serialize(request, JsonOptions);

        Logger.LogDebug("[BoldDesk POST] Starting request to: {Url}", url);
        Logger.LogTrace("[BoldDesk POST] Request body: {RequestBody}", requestJson);

        try
        {
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(url, content);
            stopwatch.Stop();

            Logger.LogDebug("[BoldDesk POST] Response {StatusCode} from {Url} in {ElapsedMs}ms",
                (int)response.StatusCode, url, stopwatch.ElapsedMilliseconds);

            ParseRateLimitHeaders(response.Headers);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("[BoldDesk POST] FAILED {StatusCode} for {Url}. Request: {Request}. Response: {Response}",
                    (int)response.StatusCode, url, requestJson, responseContent);
                await ThrowBoldDeskException(response);
            }

            Logger.LogDebug("[BoldDesk POST] Success for {Url}. Response length: {ContentLength} chars", url, responseContent?.Length ?? 0);
            Logger.LogTrace("[BoldDesk POST] Response body: {Content}", responseContent);

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return new TResponse();
            }

            return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions) ?? new TResponse();
        }
        catch (BoldDeskApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            Logger.LogError(ex, "[BoldDesk POST] Network error after {ElapsedMs}ms for {Url}: {Message}",
                stopwatch.ElapsedMilliseconds, url, ex.Message);
            throw new BoldDeskApiException($"Network error while calling BoldDesk API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, "[BoldDesk POST] JSON error for {Url}: {Message}", url, ex.Message);
            throw new BoldDeskApiException($"Failed to parse API response: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();
            Logger.LogError(ex, "[BoldDesk POST] Timeout after {ElapsedMs}ms for {Url}", stopwatch.ElapsedMilliseconds, url);
            throw new BoldDeskTimeoutException(
                "A timeout occurred while communicating with our support system. Please try again later.",
                url,
                stopwatch.ElapsedMilliseconds,
                ex);
        }
    }
}