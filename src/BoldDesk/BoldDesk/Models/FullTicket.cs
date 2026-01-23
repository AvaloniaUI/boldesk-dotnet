using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BoldDesk.Models
{
    public class FullTicket
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("isVisibleInCustomerPortal")]
        public bool IsVisibleInCustomerPortal { get; set; }

        [JsonPropertyName("brand")]
        public BrandInfo? Brand { get; set; }

        [JsonPropertyName("requester")]
        public UserInfo? Requester { get; set; }

        [JsonPropertyName("agent")]
        public AgentInfo? Agent { get; set; }

        [JsonPropertyName("group")]
        public IdNameBool? Group { get; set; }

        [JsonPropertyName("category")]
        public IdName? Category { get; set; }

        [JsonPropertyName("priority")]
        public IdName? Priority { get; set; }

        [JsonPropertyName("resolutionDue")]
        public DateTime? ResolutionDue { get; set; }

        [JsonPropertyName("additionalNotifications")]
        public List<UserInfo>? AdditionalNotifications { get; set; }

        [JsonPropertyName("source")]
        public IdName? Source { get; set; }

        [JsonPropertyName("status")]
        public StatusInfo? Status { get; set; }

        [JsonPropertyName("type")]
        public IdName? Type { get; set; }

        [JsonPropertyName("tag")]
        public List<Tag>? Tags { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("createdBy")]
        public UserInfo? CreatedBy { get; set; }

        [JsonPropertyName("lastRepliedOn")]
        public DateTime? LastRepliedOn { get; set; }

        [JsonPropertyName("responseDue")]
        public DateTime? ResponseDue { get; set; }

        [JsonPropertyName("lastStatusChangedOn")]
        public DateTime? LastStatusChangedOn { get; set; }

        [JsonPropertyName("updatesCount")]
        public int UpdatesCount { get; set; }

        [JsonPropertyName("notesCount")]
        public int NotesCount { get; set; }

        [JsonPropertyName("contactGroup")]
        public ContactGroupInfo? ContactGroup { get; set; }

        [JsonPropertyName("closedOn")]
        public DateTime? ClosedOn { get; set; }

        [JsonPropertyName("closedBy")]
        public UserInfo? ClosedBy { get; set; }

        [JsonPropertyName("customFields")]
        public object? CustomFields { get; set; }

        [JsonPropertyName("isSpam")]
        public bool IsSpam { get; set; }

        [JsonPropertyName("totalWorkLogged")]
        public int? TotalWorkLogged { get; set; }

        [JsonPropertyName("totalBillableWorkLogged")]
        public int? TotalBillableWorkLogged { get; set; }

        [JsonPropertyName("feedbackCategory")]
        public string? FeedbackCategory { get; set; }

        [JsonPropertyName("watchers")]
        public List<UserInfo>? Watchers { get; set; }

        [JsonPropertyName("externalReferenceId")]
        public string? ExternalReferenceId { get; set; }

        [JsonPropertyName("ticketFormDetails")]
        public object? TicketFormDetails { get; set; }

        [JsonPropertyName("state")]
        public IdName? State { get; set; }
    }

    public class BrandInfo
    {
        [JsonPropertyName("brandId")]
        public int BrandId { get; set; }

        [JsonPropertyName("brandName")]
        public string? BrandName { get; set; }

        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }

        [JsonPropertyName("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }

    public class UserInfo
    {
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("mobileNumber")]
        public string? MobileNumber { get; set; }

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

    public class AgentInfo
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("shortCode")]
        public string? ShortCode { get; set; }

        [JsonPropertyName("colorCode")]
        public string? ColorCode { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
    }
    

    public class IdName
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class IdNameBool
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class StatusInfo
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("textColor")]
        public string? TextColor { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
