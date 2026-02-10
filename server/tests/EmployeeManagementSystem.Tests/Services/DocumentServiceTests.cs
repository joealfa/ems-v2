using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Document;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace EmployeeManagementSystem.Tests.Services;

public class DocumentServiceTests
{
    private readonly Mock<IRepository<Document>> _documentRepositoryMock;
    private readonly Mock<IRepository<Person>> _personRepositoryMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly Mock<ILogger<DocumentService>> _loggerMock;
    private readonly DocumentService _documentService;

    public DocumentServiceTests()
    {
        _documentRepositoryMock = new Mock<IRepository<Document>>();
        _personRepositoryMock = new Mock<IRepository<Person>>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _loggerMock = new Mock<ILogger<DocumentService>>();

        _documentService = new DocumentService(
            _documentRepositoryMock.Object,
            _personRepositoryMock.Object,
            _blobStorageServiceMock.Object,
            _loggerMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenPersonAndDocumentExist_ReturnsDocumentResponseDto()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 200000000001L;
        Person person = CreateTestPerson(personDisplayId);
        Document document = CreateTestDocument(documentDisplayId, person.Id);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document> { document }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        // Act
        Result<DocumentResponseDto> result = await _documentService.GetByDisplayIdAsync(personDisplayId, documentDisplayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(documentDisplayId, result.Value.DisplayId);
        Assert.Equal(document.FileName, result.Value.FileName);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenPersonDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 999999999999L;
        long documentDisplayId = 200000000001L;

        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();
        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result<DocumentResponseDto> result = await _documentService.GetByDisplayIdAsync(personDisplayId, documentDisplayId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenDocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 999999999999L;
        Person person = CreateTestPerson(personDisplayId);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        // Act
        Result<DocumentResponseDto> result = await _documentService.GetByDisplayIdAsync(personDisplayId, documentDisplayId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_WhenPersonExists_ReturnsPagedResult()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10 };

        IQueryable<Document> documents = new List<Document>
        {
            CreateTestDocument(200000000001L, person.Id, "Document1.pdf"),
            CreateTestDocument(200000000002L, person.Id, "Document2.pdf"),
            CreateTestDocument(200000000003L, person.Id, "Document3.pdf")
        }.BuildMockQueryable();

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        // Act
        PagedResult<DocumentListDto> result = await _documentService.GetPagedAsync(personDisplayId, query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WhenPersonDoesNotExist_ReturnsEmptyResult()
    {
        // Arrange
        long personDisplayId = 999999999999L;
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10 };

        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();
        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        PagedResult<DocumentListDto> result = await _documentService.GetPagedAsync(personDisplayId, query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10, SearchTerm = "Report" };

        IQueryable<Document> documents = new List<Document>
        {
            CreateTestDocument(200000000001L, person.Id, "Report.pdf"),
            CreateTestDocument(200000000002L, person.Id, "Contract.pdf"),
            CreateTestDocument(200000000003L, person.Id, "Annual_Report.pdf")
        }.BuildMockQueryable();

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        // Act
        PagedResult<DocumentListDto> result = await _documentService.GetPagedAsync(personDisplayId, query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount); // Report.pdf and Annual_Report.pdf
    }

    #endregion

    #region UploadAsync Tests

    [Fact]
    public async Task UploadAsync_WithValidData_ReturnsDocumentResponseDto()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        UploadDocumentDto uploadDto = new()
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            FileSizeBytes = 1024,
            FileStream = new MemoryStream(),
            Description = "Test document"
        };

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _blobStorageServiceMock
            .Setup(b => b.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://storage.blob.core.windows.net/documents/test.pdf");

        _ = _documentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Document d, CancellationToken _) => d);

        // Act
        Result<DocumentResponseDto> result = await _documentService.UploadAsync(personDisplayId, uploadDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("test.pdf", result.Value.FileName);
        Assert.Equal(".pdf", result.Value.FileExtension);
        Assert.Equal(DocumentType.Pdf, result.Value.DocumentType);

        _blobStorageServiceMock.Verify(b => b.UploadAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _documentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadAsync_WhenPersonDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 999999999999L;
        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();

        UploadDocumentDto uploadDto = new()
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            FileSizeBytes = 1024,
            FileStream = new MemoryStream()
        };

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result<DocumentResponseDto> result = await _documentService.UploadAsync(personDisplayId, uploadDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    [Theory]
    [InlineData(".exe")]
    [InlineData(".bat")]
    [InlineData(".sh")]
    [InlineData(".dll")]
    public async Task UploadAsync_WithDisallowedExtension_ReturnsBadRequest(string extension)
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        UploadDocumentDto uploadDto = new()
        {
            FileName = $"test{extension}",
            ContentType = "application/octet-stream",
            FileSizeBytes = 1024,
            FileStream = new MemoryStream()
        };

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result<DocumentResponseDto> result = await _documentService.UploadAsync(personDisplayId, uploadDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.BadRequest, result.FailureType);
    }

    [Theory]
    [InlineData(".pdf", DocumentType.Pdf)]
    [InlineData(".doc", DocumentType.Word)]
    [InlineData(".docx", DocumentType.Word)]
    [InlineData(".xls", DocumentType.Excel)]
    [InlineData(".xlsx", DocumentType.Excel)]
    [InlineData(".jpg", DocumentType.ImageJpeg)]
    [InlineData(".jpeg", DocumentType.ImageJpeg)]
    [InlineData(".png", DocumentType.ImagePng)]
    public async Task UploadAsync_WithAllowedExtension_SetsCorrectDocumentType(string extension, DocumentType expectedType)
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        UploadDocumentDto uploadDto = new()
        {
            FileName = $"test{extension}",
            ContentType = "application/octet-stream",
            FileSizeBytes = 1024,
            FileStream = new MemoryStream()
        };

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _blobStorageServiceMock
            .Setup(b => b.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://storage.blob.core.windows.net/documents/test.pdf");

        _ = _documentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Document d, CancellationToken _) => d);

        // Act
        Result<DocumentResponseDto> result = await _documentService.UploadAsync(personDisplayId, uploadDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedType, result.Value!.DocumentType);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenDocumentExists_UpdatesDescription()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 200000000001L;
        Person person = CreateTestPerson(personDisplayId);
        Document document = CreateTestDocument(documentDisplayId, person.Id);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document> { document }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);
        _ = _documentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        UpdateDocumentDto updateDto = new() { Description = "Updated description" };

        // Act
        Result<DocumentResponseDto> result = await _documentService.UpdateAsync(personDisplayId, documentDisplayId, updateDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _documentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenDocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 999999999999L;
        Person person = CreateTestPerson(personDisplayId);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        UpdateDocumentDto updateDto = new() { Description = "Updated description" };

        // Act
        Result<DocumentResponseDto> result = await _documentService.UpdateAsync(personDisplayId, documentDisplayId, updateDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenDocumentExists_DeletesSuccessfully()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 200000000001L;
        Person person = CreateTestPerson(personDisplayId);
        Document document = CreateTestDocument(documentDisplayId, person.Id);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document> { document }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);
        _ = _documentRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _documentService.DeleteAsync(personDisplayId, documentDisplayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _documentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenDocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 999999999999L;
        Person person = CreateTestPerson(personDisplayId);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        // Act
        Result result = await _documentService.DeleteAsync(personDisplayId, documentDisplayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region DownloadAsync Tests

    [Fact]
    public async Task DownloadAsync_WhenDocumentExists_ReturnsDownloadResult()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 200000000001L;
        Person person = CreateTestPerson(personDisplayId);
        Document document = CreateTestDocument(documentDisplayId, person.Id);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document> { document }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        MemoryStream expectedStream = new(new byte[] { 1, 2, 3 });
        _ = _blobStorageServiceMock
            .Setup(b => b.DownloadAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedStream);

        // Act
        Result<BlobDownloadResultDto> result = await _documentService.DownloadAsync(personDisplayId, documentDisplayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(document.FileName, result.Value.FileName);
        Assert.Equal(document.ContentType, result.Value.ContentType);
    }

    [Fact]
    public async Task DownloadAsync_WhenDocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        long documentDisplayId = 999999999999L;
        Person person = CreateTestPerson(personDisplayId);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        IQueryable<Document> documents = new List<Document>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _documentRepositoryMock.Setup(r => r.Query()).Returns(documents);

        // Act
        Result<BlobDownloadResultDto> result = await _documentService.DownloadAsync(personDisplayId, documentDisplayId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region UploadProfileImageAsync Tests

    [Fact]
    public async Task UploadProfileImageAsync_WithValidImage_UploadsSuccessfully()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        UploadDocumentDto uploadDto = new()
        {
            FileName = "profile.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024,
            FileStream = new MemoryStream()
        };

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _blobStorageServiceMock
            .Setup(b => b.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://storage.blob.core.windows.net/profile-images/profile.jpg");

        _ = _personRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result<string> result = await _documentService.UploadProfileImageAsync(personDisplayId, uploadDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _personRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(".pdf")]
    [InlineData(".doc")]
    [InlineData(".exe")]
    public async Task UploadProfileImageAsync_WithNonImageFile_ReturnsBadRequest(string extension)
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        UploadDocumentDto uploadDto = new()
        {
            FileName = $"profile{extension}",
            ContentType = "application/octet-stream",
            FileSizeBytes = 1024,
            FileStream = new MemoryStream()
        };

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result<string> result = await _documentService.UploadProfileImageAsync(personDisplayId, uploadDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.BadRequest, result.FailureType);
    }

    #endregion

    #region DeleteProfileImageAsync Tests

    [Fact]
    public async Task DeleteProfileImageAsync_WhenProfileImageExists_DeletesSuccessfully()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        person.ProfileImageUrl = "https://storage.blob.core.windows.net/profile-images/profile.jpg";

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _blobStorageServiceMock
            .Setup(b => b.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ = _personRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _documentService.DeleteProfileImageAsync(personDisplayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _blobStorageServiceMock.Verify(b => b.DeleteAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProfileImageAsync_WhenNoProfileImage_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        person.ProfileImageUrl = null;

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result result = await _documentService.DeleteProfileImageAsync(personDisplayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region GetProfileImageAsync Tests

    [Fact]
    public async Task GetProfileImageAsync_WhenProfileImageExists_ReturnsDownloadResult()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        person.ProfileImageUrl = "https://storage.blob.core.windows.net/profile-images/profile.jpg";

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        MemoryStream expectedStream = new(new byte[] { 1, 2, 3 });
        _ = _blobStorageServiceMock
            .Setup(b => b.DownloadAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedStream);

        // Act
        Result<BlobDownloadResultDto> result = await _documentService.GetProfileImageAsync(personDisplayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("image/jpeg", result.Value.ContentType);
    }

    [Fact]
    public async Task GetProfileImageAsync_WhenNoProfileImage_ReturnsNotFound()
    {
        // Arrange
        long personDisplayId = 100000000001L;
        Person person = CreateTestPerson(personDisplayId);
        person.ProfileImageUrl = null;

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result<BlobDownloadResultDto> result = await _documentService.GetProfileImageAsync(personDisplayId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region Helper Methods

    private static Person CreateTestPerson(long displayId, string firstName = "Test", string lastName = "Person")
    {
        Person person = new()
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            CivilStatus = CivilStatus.Single,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(person, displayId);

        return person;
    }

    private static Document CreateTestDocument(long displayId, Guid personId, string fileName = "test.pdf")
    {
        Document document = new()
        {
            FileName = fileName,
            FileExtension = Path.GetExtension(fileName),
            ContentType = "application/pdf",
            FileSizeBytes = 1024,
            DocumentType = DocumentType.Pdf,
            BlobUrl = $"https://storage.blob.core.windows.net/documents/{fileName}",
            BlobName = $"{personId}/{Guid.NewGuid()}.pdf",
            ContainerName = "documents",
            PersonId = personId,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(document, displayId);

        return document;
    }

    #endregion
}
