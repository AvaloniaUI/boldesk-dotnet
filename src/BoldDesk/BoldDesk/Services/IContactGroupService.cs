using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Service for managing contact groups in BoldDesk
/// </summary>
public interface IContactGroupService
{
    /// <summary>
    /// Get a list of contact groups
    /// </summary>
    Task<BoldDeskResponse<ContactGroup>> ListContactGroupsAsync(
        ContactGroupQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a list of all contact groups
    /// </summary>
    IAsyncEnumerable<ContactGroup> ListAllContactGroupsAsync(
        ContactGroupQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a contact group by ID
    /// </summary>
    Task<ContactGroupDetail> GetContactGroupAsync(
        long contactGroupId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a contact group by name
    /// </summary>
    Task<ContactGroupDetail> GetContactGroupByNameAsync(
        string contactGroupName,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a new contact group
    /// </summary>
    Task<ContactGroupOperationResponse> AddContactGroupAsync(
        AddContactGroupRequest request,
        bool skipDependencyValidation = false,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update a contact group
    /// </summary>
    Task<ContactGroupOperationResponse> UpdateContactGroupAsync(
        long contactGroupId,
        UpdateContactGroupFieldsRequest request,
        bool skipDependencyValidation = false,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete contact groups
    /// </summary>
    Task<ContactGroupDeleteResponse> DeleteContactGroupsAsync(
        DeleteContactGroupsRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get contacts in a contact group
    /// </summary>
    Task<BoldDeskResponse<Contact>> GetContactsByGroupAsync(
        long contactGroupId,
        ContactGroupContactsQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all contacts in a contact group
    /// </summary>
    IAsyncEnumerable<Contact> GetAllContactsByGroupAsync(
        long contactGroupId,
        ContactGroupContactsQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add multiple contacts to a contact group
    /// </summary>
    Task<AddContactToGroupResponse> AddContactsToGroupAsync(
        long contactGroupId,
        List<AddContactToGroupRequest> contacts,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove a contact from a contact group
    /// </summary>
    Task<ContactGroupOperationResponse> RemoveContactFromGroupAsync(
        long contactGroupId,
        long userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Change ticket access scope for a contact in a contact group
    /// </summary>
    Task<ContactGroupOperationResponse> ChangeTicketAccessScopeAsync(
        long contactGroupId,
        long contactId,
        bool viewAllTickets,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get contact group domains
    /// </summary>
    Task<BoldDeskResponse<DomainInfo>> GetContactGroupDomainsAsync(
        ContactGroupDomainsQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all contact group domains
    /// </summary>
    IAsyncEnumerable<DomainInfo> GetAllContactGroupDomainsAsync(
        ContactGroupDomainsQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get contact group fields
    /// </summary>
    Task<List<ContactGroupField>> GetContactGroupFieldsAsync(
        ContactGroupFieldsQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a specific contact group field
    /// </summary>
    Task<object> GetContactGroupFieldAsync(
        int fieldId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get notes for a contact group
    /// </summary>
    Task<ContactGroupNotesResponse> GetContactGroupNotesAsync(
        long contactGroupId,
        ContactGroupNotesQueryParameters? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a note to a contact group
    /// </summary>
    Task<ContactGroupOperationResponse> AddContactGroupNoteAsync(
        long contactGroupId,
        ContactGroupNoteRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update a contact group note
    /// </summary>
    Task<ContactGroupOperationResponse> UpdateContactGroupNoteAsync(
        long noteId,
        ContactGroupNoteRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a contact group note
    /// </summary>
    Task<ContactGroupOperationResponse> DeleteContactGroupNoteAsync(
        long noteId,
        CancellationToken cancellationToken = default);
}