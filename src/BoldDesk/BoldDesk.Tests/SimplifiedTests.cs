using System.Net;
using System.Text.Json;
using BoldDesk.Exceptions;
using BoldDesk.Models;
using BoldDesk.Services;
using BoldDesk.Tests.TestHelpers;

namespace BoldDesk.Tests;

[TestFixture]
public class SimplifiedTests
{
    private JsonSerializerOptions _jsonOptions;

    [SetUp]
    public void Setup()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Test]
    public void BoldDeskClient_CanBeCreated()
    {
        var client = new BoldDeskClient("test.bolddesk.com", "test-key");
        Assert.That(client, Is.Not.Null);
        client.Dispose();
    }

    [Test]
    public void AgentDetail_HasCorrectProperties()
    {
        var agent = new AgentDetail
        {
            UserId = 123,
            Name = "Test Agent",
            DisplayName = "Test Display",
            EmailId = "test@example.com"
        };

        Assert.That(agent.UserId, Is.EqualTo(123));
        Assert.That(agent.Name, Is.EqualTo("Test Agent"));
        Assert.That(agent.EmailId, Is.EqualTo("test@example.com"));
    }

    [Test]
    public void BoldDeskApiException_CanBeThrown()
    {
        var ex = new BoldDeskApiException("Test error", HttpStatusCode.BadRequest);
        Assert.That(ex.Message, Is.EqualTo("Test error"));
        Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void MockHttpMessageHandler_Works()
    {
        var mockHandler = new MockHttpMessageHandler();
        mockHandler.AddResponse(HttpStatusCode.OK, "test content");
        
        var httpClient = new HttpClient(mockHandler);
        Assert.That(httpClient, Is.Not.Null);
    }

    [Test]
    public void RateLimitInfo_CanBeCreated()
    {
        var info = new RateLimitInfo
        {
            Limit = 100,
            Remaining = 50,
            Reset = DateTime.UtcNow
        };

        Assert.That(info.Limit, Is.EqualTo(100));
        Assert.That(info.Remaining, Is.EqualTo(50));
    }

    [Test]
    public void BoldDeskResponse_GenericWorks()
    {
        var response = new BoldDeskResponse<string>
        {
            Result = new List<string> { "item1", "item2" },
            Count = 2
        };

        Assert.That(response.Result.Count, Is.EqualTo(2));
        Assert.That(response.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task AgentService_CanBeCreated()
    {
        var mockHandler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(mockHandler);
        var service = new AgentService(httpClient, "https://api.test.com", _jsonOptions);
        
        Assert.That(service, Is.Not.Null);
        Assert.That(service, Is.InstanceOf<IAgentService>());
    }

    [Test] 
    public void TicketQueryParameters_HasDefaultValues()
    {
        var parameters = new TicketQueryParameters();
        
        Assert.That(parameters.Page, Is.EqualTo(1));
        Assert.That(parameters.PerPage, Is.EqualTo(30));
    }

    [Test]
    public void AgentQueryParameters_HasDefaultValues()
    {
        var parameters = new AgentQueryParameters();
        
        Assert.That(parameters.Page, Is.EqualTo(1));
        Assert.That(parameters.PerPage, Is.EqualTo(30));
        Assert.That(parameters.RequiresCounts, Is.False);
    }

    [Test]
    public void BoldDeskErrorResponse_CanContainErrors()
    {
        var errorResponse = new BoldDeskErrorResponse
        {
            Message = "Error occurred",
            StatusCode = 400,
            Errors = new List<BoldDeskError>
            {
                new BoldDeskError
                {
                    Field = "test",
                    ErrorMessage = "Test error",
                    ErrorType = BoldDeskErrorType.InvalidValue
                }
            }
        };

        Assert.That(errorResponse.Message, Is.EqualTo("Error occurred"));
        Assert.That(errorResponse.Errors.Count, Is.EqualTo(1));
    }

    [Test]
    public void BoldDeskValidationException_CanBeCreated()
    {
        var errorResponse = new BoldDeskErrorResponse
        {
            Message = "Validation failed",
            StatusCode = 400,
            Errors = new List<BoldDeskError>
            {
                new BoldDeskError { Field = "field1", ErrorMessage = "Error 1" }
            }
        };

        var ex = new BoldDeskValidationException(errorResponse);
        
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.ErrorResponse, Is.Not.Null);
    }

    [Test]
    public void BoldDeskClient_HasAllServices()
    {
        var mockHandler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(mockHandler);
        var client = new BoldDeskClient(httpClient, "test.bolddesk.com", "key");

        Assert.That(client.Tickets, Is.Not.Null);
        Assert.That(client.Agents, Is.Not.Null);
        Assert.That(client.Brands, Is.Not.Null);
        Assert.That(client.Worklogs, Is.Not.Null);
        Assert.That(client.Contacts, Is.Not.Null);
        Assert.That(client.ContactGroups, Is.Not.Null);

        client.Dispose();
    }

    [Test]
    public async Task BaseService_RateLimitHandling()
    {
        var mockHandler = new MockHttpMessageHandler();
        var headers = new Dictionary<string, string>
        {
            { "x-rate-limit-limit", "100" },
            { "x-rate-limit-remaining", "50" }
        };
        
        mockHandler.AddResponse(HttpStatusCode.OK, null, headers);
        
        var httpClient = new HttpClient(mockHandler);
        var service = new TestableService(httpClient, "https://api.test.com", _jsonOptions);
        
        await service.MakeTestRequest();
        
        var rateLimitInfo = service.GetLastRateLimitInfo();
        Assert.That(rateLimitInfo, Is.Not.Null);
        Assert.That(rateLimitInfo!.Limit, Is.EqualTo(100));
    }

    private class TestableService : BaseService
    {
        public TestableService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions)
            : base(httpClient, baseUrl, jsonOptions)
        {
        }

        public async Task<object> MakeTestRequest()
        {
            return await ExecuteRequestAsync<object>("https://api.test.com/test");
        }
    }
}