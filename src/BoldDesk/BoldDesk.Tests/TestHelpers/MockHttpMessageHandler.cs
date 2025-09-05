using System.Net;
using System.Text;
using System.Text.Json;

namespace BoldDesk.Tests.TestHelpers;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<MockResponse> _responses = new();
    private readonly List<HttpRequestMessage> _capturedRequests = new();

    public IReadOnlyList<HttpRequestMessage> CapturedRequests => _capturedRequests.AsReadOnly();

    public void AddResponse(HttpStatusCode statusCode, object? content = null, Dictionary<string, string>? headers = null)
    {
        _responses.Enqueue(new MockResponse
        {
            StatusCode = statusCode,
            Content = content,
            Headers = headers
        });
    }

    public void AddJsonResponse<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK, Dictionary<string, string>? headers = null)
    {
        _responses.Enqueue(new MockResponse
        {
            StatusCode = statusCode,
            Content = content,
            Headers = headers
        });
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _capturedRequests.Add(request);

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException("No mock response configured for this request");
        }

        var mockResponse = _responses.Dequeue();
        var response = new HttpResponseMessage(mockResponse.StatusCode);

        if (mockResponse.Content != null)
        {
            var json = JsonSerializer.Serialize(mockResponse.Content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        if (mockResponse.Headers != null)
        {
            foreach (var header in mockResponse.Headers)
            {
                response.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return await Task.FromResult(response);
    }

    private class MockResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object? Content { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
    }
}