namespace EmployeeManagementSystem.Application.DTOs;

/// <summary>
/// Generic paginated result wrapper. Immutable record for response data.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public record PagedResult<T>
{
    /// <summary>
    /// Gets the list of items.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>
    /// Gets the total count of items.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// Query parameters for pagination. Kept as class for model binding with validation logic.
/// </summary>
public class PaginationQuery
{
    private const int MaxPageSize = 100;

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int PageNumber
    {
        get;
        set => field = value < 1 ? 1 : value;
    } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize
    {
        get;
        set => field = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
    } = 10;

    /// <summary>
    /// Gets or sets the search term.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the sort by field.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to sort descending.
    /// </summary>
    public bool SortDescending { get; set; }
}
