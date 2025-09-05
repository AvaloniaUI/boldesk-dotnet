using BoldDesk.QueryBuilder;

namespace BoldDesk.Models;

/// <summary>
/// Query parameters for listing contacts
/// </summary>
public class ContactQueryParameters
{
    /// <summary>
    /// Q parameter for filtering contacts by specified fields
    /// Values allowed are 'createdon', 'lastmodifiedon', 'ids'
    /// </summary>
    public string[]? Q { get; set; }

    /// <summary>
    /// Filter the results by using any string associated with the contact's name or email
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Specifies the number of the page to be fetched
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Defines the number of records to be fetched in a specified page
    /// </summary>
    public int PerPage { get; set; } = 50;

    /// <summary>
    /// Determines whether the total number of records to be returned or not
    /// </summary>
    public bool RequiresCounts { get; set; } = true;

    /// <summary>
    /// Sorting order of the records to be displayed
    /// Values allowed are: 'userId', 'contactName', 'emailId', 'createdon', 'lastmodifiedon'
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Name of the view to apply while fetching the contacts
    /// </summary>
    public string? View { get; set; }

    /// <summary>
    /// Contact group ID filter
    /// </summary>
    public long? ContactGroupId { get; set; }

    /// <summary>
    /// Creates a new instance from a ContactQueryBuilder
    /// </summary>
    public static ContactQueryParameters From(ContactQueryBuilder builder)
    {
        return builder.ToParameters();
    }

    /// <summary>
    /// Creates a new instance from a ContactQueryBuilder with additional parameters
    /// </summary>
    public static ContactQueryParameters From(ContactQueryBuilder builder, int page = 1, int perPage = 50, 
        string? filter = null, string? view = null, long? contactGroupId = null, string? orderBy = null, bool requiresCounts = true)
    {
        return builder.ToParameters(page, perPage, filter, view, contactGroupId, orderBy, requiresCounts);
    }
}