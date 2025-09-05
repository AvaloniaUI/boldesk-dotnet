using System;
using System.Threading.Tasks;
using BoldDesk;
using BoldDesk.Models;

namespace BoldDesk.Examples;

/// <summary>
/// Example demonstrating how to use the Fields API
/// </summary>
public class FieldsExample
{
    public static async Task RunExample(IBoldDeskClient client)
    {
        Console.WriteLine("=== Fields API Example ===\n");

        // Example 1: List field options
        Console.WriteLine("1. Listing field options for a custom dropdown:");
        var fieldOptions = await client.Fields.ListFieldOptionsAsync("cf_single_dropdown", 
            new FieldOptionQueryParameters
            {
                Filter = "sample",
                Page = 1,
                PerPage = 10,
                RequiresCounts = true,
                OrderBy = "name"
            });

        foreach (var option in fieldOptions.Result)
        {
            Console.WriteLine($"  - {option.Name} (ID: {option.Id}, ReadOnly: {option.IsReadOnly}, Default: {option.IsDefault})");
        }

        // Example 2: Add new field options
        Console.WriteLine("\n2. Adding new field options:");
        var addResult = await client.Fields.AddFieldOptionsAsync("cf_canned_dropdown", 
            new List<string> { "Option 1", "Option 2", "Option 3" });
        Console.WriteLine($"  Result: {addResult.Message}");

        // Example 3: Set field option as read-only
        Console.WriteLine("\n3. Setting field option as read-only:");
        var readOnlyResult = await client.Fields.SetFieldOptionReadOnlyAsync(2335, true);
        Console.WriteLine($"  Result: {readOnlyResult.Message}");

        // Example 4: Change field option position
        Console.WriteLine("\n4. Changing field option position:");
        var positionResult = await client.Fields.ChangeFieldOptionPositionAsync(2394, 2477, 
            new FieldPositionChangeParameters
            {
                ToPosition = 1,
                IsSortByAlphabeticalOrder = false,
                IsMoveToTopPosition = false,
                IsMoveToBottomPosition = false
            });
        Console.WriteLine($"  Result: {positionResult.Message}");

        // Example 5: Set default field option
        Console.WriteLine("\n5. Setting default field option:");
        var setDefaultResult = await client.Fields.SetDefaultFieldOptionAsync(2393, 2471);
        Console.WriteLine($"  Result: {setDefaultResult.Message}");

        // Example 6: Remove default field option
        Console.WriteLine("\n6. Removing default field option:");
        var removeDefaultResult = await client.Fields.RemoveDefaultFieldOptionAsync(2393, 2471);
        Console.WriteLine($"  Result: {removeDefaultResult.Message}");

        // Example 7: Remove field option
        Console.WriteLine("\n7. Removing field option:");
        var removeResult = await client.Fields.RemoveFieldOptionAsync(2335);
        Console.WriteLine($"  Result: {removeResult.Message}");
    }

    /// <summary>
    /// Example showing how to manage dropdown field options
    /// </summary>
    public static async Task ManageDropdownOptions(IBoldDeskClient client, string fieldApiName)
    {
        Console.WriteLine($"\n=== Managing Dropdown Options for {fieldApiName} ===\n");

        // List current options
        var currentOptions = await client.Fields.ListFieldOptionsAsync(fieldApiName);
        Console.WriteLine($"Current options count: {currentOptions.Result.Count}");

        // Add new options
        var newOptions = new List<string> { "New Option A", "New Option B" };
        await client.Fields.AddFieldOptionsAsync(fieldApiName, newOptions);
        Console.WriteLine($"Added {newOptions.Count} new options");

        // List options again to see the changes
        var updatedOptions = await client.Fields.ListFieldOptionsAsync(fieldApiName);
        Console.WriteLine($"Updated options count: {updatedOptions.Result.Count}");

        // Find a specific option and make it default
        var optionToMakeDefault = updatedOptions.Result.FirstOrDefault(o => o.Name == "New Option A");
        if (optionToMakeDefault != null)
        {
            // Assuming we have the field ID (this would need to be obtained from another API)
            int fieldId = 123; // Replace with actual field ID
            await client.Fields.SetDefaultFieldOptionAsync(fieldId, optionToMakeDefault.Id);
            Console.WriteLine($"Made '{optionToMakeDefault.Name}' the default option");
        }
    }

    /// <summary>
    /// Example showing how to work with hierarchical field options
    /// </summary>
    public static async Task WorkWithHierarchicalOptions(IBoldDeskClient client, string fieldApiName, int parentOptionId)
    {
        Console.WriteLine($"\n=== Working with Hierarchical Options ===\n");

        // List child options for a parent
        var childOptions = await client.Fields.ListFieldOptionsAsync(fieldApiName,
            new FieldOptionQueryParameters
            {
                ParentOptionId = parentOptionId,
                Page = 1,
                PerPage = 50
            });

        Console.WriteLine($"Found {childOptions.Result.Count} child options for parent ID {parentOptionId}:");
        foreach (var option in childOptions.Result)
        {
            Console.WriteLine($"  - {option.Name} (Sort Order: {option.SortOrder})");
        }

        // Reorder the options alphabetically
        if (childOptions.Result.Count > 0)
        {
            int fieldId = 456; // Replace with actual field ID
            await client.Fields.ChangeFieldOptionPositionAsync(fieldId, childOptions.Result[0].Id,
                new FieldPositionChangeParameters
                {
                    ToPosition = 1,
                    IsSortByAlphabeticalOrder = true,
                    IsMoveToTopPosition = false,
                    IsMoveToBottomPosition = false
                });
            Console.WriteLine("Reordered options alphabetically");
        }
    }
}