using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Interface for Contact service operations
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Lists contacts with pagination
    /// </summary>
    Task<BoldDeskResponse<Contact>> ListContactsAsync(ContactQueryParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all contacts using pagination
    /// </summary>
    IAsyncEnumerable<Contact> ListAllContactsAsync(ContactQueryParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contact by user ID
    /// </summary>
    Task<Contact> GetContactAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contact by email address
    /// </summary>
    Task<Contact> GetContactByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new contact
    /// </summary>
    Task<ContactOperationResponse> CreateContactAsync(CreateContactRequest request, bool skipDependencyValidation = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing contact
    /// </summary>
    Task<ContactMessageResponse> UpdateContactAsync(long contactId, UpdateContactRequest request, bool skipDependencyValidation = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes contacts
    /// </summary>
    Task<ContactDeleteResponse> DeleteContactsAsync(DeleteContactRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes contacts
    /// </summary>
    Task<ContactDeleteResponse> PermanentDeleteContactsAsync(PermanentDeleteContactRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Blocks a contact
    /// </summary>
    Task<ContactMessageResponse> BlockContactAsync(long contactId, bool markTicketAsSpam = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unblocks a contact
    /// </summary>
    Task<ContactMessageResponse> UnblockContactAsync(long contactId, bool removeTicketFromSpam = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple contact groups to a contact
    /// </summary>
    Task<AddContactGroupResponse> AddContactGroupsAsync(long contactId, List<AddContactGroupsRequest> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple contact groups from a contact
    /// </summary>
    Task<RemoveContactGroupResponse> RemoveContactGroupsAsync(long contactId, RemoveContactGroupsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets contact groups associated with a contact
    /// </summary>
    Task<BoldDeskResponse<ContactGroupInfo>> GetContactGroupsByContactIdAsync(long contactId, ContactGroupQueryParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the primary contact group of a contact
    /// </summary>
    Task<ContactMessageResponse> ChangePrimaryContactGroupAsync(long contactId, long contactGroupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts a contact to an agent
    /// </summary>
    Task<ContactMessageResponse> ConvertContactToAgentAsync(long userId, ConvertContactToAgentRequest request, bool skipDependencyValidation = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists contact notes
    /// </summary>
    Task<ContactNotesResponse> ListContactNotesAsync(long contactId, ContactQueryParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a note to a contact
    /// </summary>
    Task<ContactOperationResponse> AddContactNoteAsync(long contactId, ContactNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a contact note
    /// </summary>
    Task<ContactMessageResponse> UpdateContactNoteAsync(long noteId, ContactNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a contact note
    /// </summary>
    Task<ContactMessageResponse> DeleteContactNoteAsync(long noteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Merges contacts
    /// </summary>
    Task<ContactDeleteResponse> MergeContactsAsync(MergeContactRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists contact fields
    /// </summary>
    Task<BoldDeskResponse<ContactField>> ListContactFieldsAsync(ContactQueryParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contact field by ID
    /// </summary>
    Task<ContactField> GetContactFieldAsync(int contactFieldId, CancellationToken cancellationToken = default);
}