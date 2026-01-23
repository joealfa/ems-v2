namespace EmployeeManagementSystem.Domain.Enums;

/// <summary>
/// Represents the professional eligibility of an employee.
/// </summary>
public enum Eligibility
{
    /// <summary>
    /// Licensure Examination for Teachers.
    /// </summary>
    LET = 1,

    /// <summary>
    /// Professional Board Examination for Teachers.
    /// </summary>
    PBET = 2,

    /// <summary>
    /// Civil Service Professional.
    /// </summary>
    CivilServiceProfessional = 3,

    /// <summary>
    /// Civil Service Sub-Professional.
    /// </summary>
    CivilServiceSubProfessional = 4,

    /// <summary>
    /// Other eligibility not listed.
    /// </summary>
    Other = 99
}
