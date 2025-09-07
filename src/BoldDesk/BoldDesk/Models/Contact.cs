using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Represents a contact in BoldDesk
/// </summary>
public class Contact
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }
    
    [JsonPropertyName("contactDisplayName")]
    public string ContactDisplayName { get; set; } = string.Empty;
    
    [JsonPropertyName("contactName")]
    public string? ContactName { get; set; }
    
    [JsonPropertyName("emailId")]
    public string EmailId { get; set; } = string.Empty;
    
    [JsonPropertyName("secondaryEmailId")]
    public string? SecondaryEmailId { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("lastActivityOn")]
    public DateTime? LastActivityOn { get; set; }
    
    [JsonPropertyName("isVerified")]
    public bool IsVerified { get; set; }
    
    [JsonPropertyName("isBlocked")]
    public bool IsBlocked { get; set; }
    
    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
    
    [JsonPropertyName("contactPhoneNo")]
    public string? ContactPhoneNo { get; set; }
    
    [JsonPropertyName("contactMobileNo")]
    public string? ContactMobileNo { get; set; }
    
    [JsonPropertyName("contactAddress")]
    public string? ContactAddress { get; set; }
    
    [JsonPropertyName("timeZoneId")]
    public IdNamePair? TimeZoneId { get; set; }
    
    [JsonPropertyName("languageId")]
    public IdNamePair? LanguageId { get; set; }
    
    [JsonPropertyName("contactTag")]
    public List<IdNamePair>? ContactTag { get; set; }
    
    [JsonPropertyName("contactGroup")]
    public List<ContactGroupInfo>? ContactGroup { get; set; }
    
    [JsonPropertyName("contactExternalReferenceId")]
    public string? ContactExternalReferenceId { get; set; }
    
    [JsonPropertyName("contactJobTitle")]
    public string? ContactJobTitle { get; set; }
    
    [JsonPropertyName("contactNotes")]
    public string? ContactNotes { get; set; }
    
    [JsonPropertyName("colorCode")]
    public string? ColorCode { get; set; }
    
    [JsonPropertyName("shortCode")]
    public string? ShortCode { get; set; }
    
    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
    
    [JsonPropertyName("lastModifiedOn")]
    public DateTime LastModifiedOn { get; set; }
    
    [JsonPropertyName("primaryContactGroup")]
    public ContactGroupInfo? PrimaryContactGroup { get; set; }
    
    [JsonPropertyName("savedItemId")]
    public string? SavedItemId { get; set; }
    
    [JsonPropertyName("userCreationSource")]
    public object? UserCreationSource { get; set; }
    
    [JsonPropertyName("dataToken")]
    public string? DataToken { get; set; }
    
    [JsonPropertyName("isTfaEnabled")]
    public bool IsTfaEnabled { get; set; }
    
    [JsonPropertyName("profileImageUrl")]
    public string? ProfileImageUrl { get; set; }
    
    [JsonPropertyName("userCountryId")]
    [JsonConverter(typeof(NullableIntConverter))]
    public int? UserCountryId { get; set; }
    
    [JsonPropertyName("contactCustomFields")]
    public Dictionary<string, object>? ContactCustomFields { get; set; }
    
    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }
}

/// <summary>
/// Contact group information within a contact
/// </summary>
public class ContactGroupInfo
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("accessScopeId")]
    public int AccessScopeId { get; set; }
    
    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Generic ID-Name pair used in various models
/// </summary>
public class IdNamePair
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
