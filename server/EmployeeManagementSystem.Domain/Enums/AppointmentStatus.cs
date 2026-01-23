namespace EmployeeManagementSystem.Domain.Enums;

/// <summary>
/// Represents the appointment status of an employee.
/// </summary>
public enum AppointmentStatus
{
    /// <summary>
    /// Original appointment.
    /// </summary>
    Original = 1,

    /// <summary>
    /// Promotional appointment.
    /// </summary>
    Promotion = 2,

    /// <summary>
    /// Transfer appointment.
    /// </summary>
    Transfer = 3,

    /// <summary>
    /// Reappointment.
    /// </summary>
    Reappointment = 4
}
