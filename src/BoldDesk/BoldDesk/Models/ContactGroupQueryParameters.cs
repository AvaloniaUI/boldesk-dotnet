namespace BoldDesk.Models;

/// <summary>
/// Query parameters for listing contact groups
/// </summary>
public class ContactGroupQueryParameters
{
    /// <summary>
    /// Specifies the number of the page to be fetched
    /// </summary>
    public int? Page { get; set; }
    
    /// <summary>
    /// Number of records to be fetched in a specified page
    /// </summary>
    public int? PerPage { get; set; }
    
    /// <summary>
    /// Determines whether the total number of records to be returned or not
    /// </summary>
    public bool? RequiresCounts { get; set; }
    
    /// <summary>
    /// Provides Q parameter for filtering the contact group module by specified fields
    /// Values allowed are: createdon, lastmodifiedon, ids
    /// </summary>
    public string[]? Q { get; set; }
    
    /// <summary>
    /// Filters the results by using any string associated with the contact group name
    /// </summary>
    public string? Filter { get; set; }
    
    /// <summary>
    /// Sorting order of the records to be displayed
    /// Values allowed are: ContactGroupId, contactGroupName, createdon, lastmodifiedon
    /// </summary>
    public string? OrderBy { get; set; }
}

/// <summary>
/// Query parameters for listing contacts in a contact group
/// </summary>
public class ContactGroupContactsQueryParameters
{
    /// <summary>
    /// Specifies the number of the page to be fetched
    /// </summary>
    public int? Page { get; set; }
    
    /// <summary>
    /// Number of records to be fetched in a specified page
    /// </summary>
    public int? PerPage { get; set; }
    
    /// <summary>
    /// Determines whether the total number of records to be returned or not
    /// </summary>
    public bool? RequiresCounts { get; set; }
    
    /// <summary>
    /// Filters the results by using any string associated with the contact group
    /// </summary>
    public string? Filter { get; set; }
    
    /// <summary>
    /// Sorting order of the records to be displayed
    /// Values allowed are: userId, contactDisplayName, contactName, emailId, secondaryEmailId, lastActivityOn, contactJobTitle, createdOn
    /// </summary>
    public string? OrderBy { get; set; }
}

/// <summary>
/// Query parameters for listing contact group domains
/// </summary>
public class ContactGroupDomainsQueryParameters
{
    /// <summary>
    /// Specifies the number of the page to be fetched
    /// </summary>
    public int? Page { get; set; }
    
    /// <summary>
    /// Number of records to be fetched in a specified page
    /// </summary>
    public int? PerPage { get; set; }
    
    /// <summary>
    /// Determines whether the total number of records to be returned or not
    /// </summary>
    public bool? RequiresCounts { get; set; }
    
    /// <summary>
    /// Filters the results by using any string associated with domain name of the contact group
    /// </summary>
    public string? Filter { get; set; }
    
    /// <summary>
    /// Sorting order of the records to be displayed
    /// Values allowed are: id, name
    /// </summary>
    public string? OrderBy { get; set; }
    
    /// <summary>
    /// Filter by specific domain IDs
    /// </summary>
    public long[]? DomainIds { get; set; }
}

/// <summary>
/// Query parameters for listing contact group notes
/// </summary>
public class ContactGroupNotesQueryParameters
{
    /// <summary>
    /// Specifies the number of the page to be fetched
    /// </summary>
    public int? Page { get; set; }
    
    /// <summary>
    /// Number of records to be fetched in a specified page
    /// </summary>
    public int? PerPage { get; set; }
    
    /// <summary>
    /// Determines whether the total number of records to be returned or not
    /// </summary>
    public bool? RequiresCounts { get; set; }
    
    /// <summary>
    /// Sorting order of the records to be displayed
    /// Values allowed are: id, subject, description, createdOn, updatedOn
    /// </summary>
    public string? OrderBy { get; set; }
}

/// <summary>
/// Query parameters for listing contact group fields
/// </summary>
public class ContactGroupFieldsQueryParameters
{
    /// <summary>
    /// Filters the results by using any string associated with the api_name
    /// </summary>
    public string? Filter { get; set; }
    
    /// <summary>
    /// Specifies the number of the page to be fetched
    /// </summary>
    public int? Page { get; set; }
    
    /// <summary>
    /// Number of records to be fetched in a specified page
    /// </summary>
    public int? PerPage { get; set; }
    
    /// <summary>
    /// Determines whether the total number of records to be returned or not
    /// </summary>
    public bool? RequiresCounts { get; set; }
    
    /// <summary>
    /// Sorting order of the records to be displayed
    /// </summary>
    public string? OrderBy { get; set; }
}