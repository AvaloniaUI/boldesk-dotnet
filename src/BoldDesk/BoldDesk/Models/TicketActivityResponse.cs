using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BoldDesk.Models
{
    public class TicketActivityResponse
    {
        [JsonPropertyName("result")]
        public List<TicketActivity> Result { get; set; } = new();

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class TicketActivity
    {
        [JsonPropertyName("isFirstUpdate")]
        public bool IsFirstUpdate { get; set; }

        [JsonPropertyName("isUpdatedByCustomer")]
        public bool IsUpdatedByCustomer { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("hasAttachment")]
        public bool HasAttachment { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedOn { get; set; }

        [JsonPropertyName("updatedBy")]
        public TicketActivityUser UpdatedBy { get; set; } = new();

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("ticketUpdatesFlag")]
        public string? TicketUpdatesFlag { get; set; }

        [JsonPropertyName("ticketUpdatesFlagId")]
        public int? TicketUpdatesFlagId { get; set; }

        [JsonPropertyName("messageTag")]
        public List<string> MessageTag { get; set; } = new();

        [JsonPropertyName("isAnyEmailDeliveryFailed")]
        public bool IsAnyEmailDeliveryFailed { get; set; }
    }
    

    public class TicketActivityUser
    {
        [JsonPropertyName("agentShiftId")]
        public int AgentShiftId { get; set; }

        [JsonPropertyName("agentShiftName")]
        public string? AgentShiftName { get; set; }

        [JsonPropertyName("ticketLimit")]
        public int TicketLimit { get; set; }

        [JsonPropertyName("chatLimit")]
        public int ChatLimit { get; set; }

        [JsonPropertyName("emailId")]
        public string? EmailId { get; set; }

        [JsonPropertyName("shortCode")]
        public string? ShortCode { get; set; }

        [JsonPropertyName("colorCode")]
        public string? ColorCode { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("profileImageUrl")]
        public string? ProfileImageUrl { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("isAgent")]
        public bool IsAgent { get; set; }
    }
}
