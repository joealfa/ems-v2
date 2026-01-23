namespace EmployeeManagementSystem.Domain.Enums;

/// <summary>
/// Represents the type of document.
/// </summary>
public enum DocumentType
{
    /// <summary>
    /// PDF document.
    /// </summary>
    Pdf,

    /// <summary>
    /// Microsoft Word document (.doc, .docx).
    /// </summary>
    Word,

    /// <summary>
    /// Microsoft Excel spreadsheet (.xls, .xlsx).
    /// </summary>
    Excel,

    /// <summary>
    /// Microsoft PowerPoint presentation (.ppt, .pptx).
    /// </summary>
    PowerPoint,

    /// <summary>
    /// JPEG image (.jpg, .jpeg).
    /// </summary>
    ImageJpeg,

    /// <summary>
    /// PNG image (.png).
    /// </summary>
    ImagePng,

    /// <summary>
    /// Other document type.
    /// </summary>
    Other
}
