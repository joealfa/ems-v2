namespace EmployeeManagementSystem.Application.DTOs;

/// <summary>
/// Generic paginated result wrapper.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Gets or sets the list of items.
    /// </summary>
    public List<T> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the total count of items.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

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
/// Query parameters for pagination.
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
