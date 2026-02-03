namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for uploading a document")]
public class UploadDocumentInput
{
    [GraphQLDescription("The file to upload")]
    public IFile? File { get; set; }

    [GraphQLDescription("Optional description for the document")]
    public string? Description { get; set; }
}

[GraphQLDescription("Input for updating document metadata")]
public class UpdateDocumentInput
{
    [GraphQLDescription("Optional description for the document")]
    public string? Description { get; set; }
}
