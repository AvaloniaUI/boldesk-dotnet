using BoldDesk.Models;

namespace BoldDesk.Services;

public interface IFieldService
{
    Task<FieldOptionsResponse> ListFieldOptionsAsync(string apiName, FieldOptionQueryParameters? parameters = null);
    Task<FieldApiResponse> AddFieldOptionsAsync(string apiName, List<string> fieldOptions);
    Task<FieldApiResponse> RemoveFieldOptionAsync(long fieldOptionId);
    Task<FieldApiResponse> SetFieldOptionReadOnlyAsync(long fieldOptionId, bool isReadOnly);
    Task<FieldApiResponse> ChangeFieldOptionPositionAsync(int fieldId, long fieldOptionId, FieldPositionChangeParameters parameters);
    Task<FieldApiResponse> SetDefaultFieldOptionAsync(int fieldId, long fieldOptionId);
    Task<FieldApiResponse> RemoveDefaultFieldOptionAsync(int fieldId, long fieldOptionId);
}