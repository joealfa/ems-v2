namespace EmployeeManagementSystem.Domain.Enums;

/// <summary>
/// Represents the civil status of a person.
/// </summary>
public enum CivilStatus
{
    /// <summary>
    /// Single or unmarried.
    /// </summary>
    Single = 1,

    /// <summary>
    /// Currently married.
    /// </summary>
    Married = 2,

    /// <summary>
    /// Solo parent.
    /// </summary>
    SoloParent = 3,

    /// <summary>
    /// Widow or widower.
    /// </summary>
    Widow = 4,

    /// <summary>
    /// Legally separated.
    /// </summary>
    Separated = 5,

    /// <summary>
    /// Other civil status not listed.
    /// </summary>
    Other = 99
}
