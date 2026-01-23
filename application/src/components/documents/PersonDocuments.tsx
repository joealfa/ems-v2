import { useState, useEffect, useRef } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Card,
  Text,
  Badge,
  Input,
  Spinner,
  Table,
} from '@chakra-ui/react';
import { documentsApi, type DocumentListDto, API_BASE_URL } from '../../api';

interface PersonDocumentsProps {
  personDisplayId: number;
}

const formatFileSize = (bytes: number | undefined): string => {
  if (!bytes) return '-';
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
};

const getDocumentTypeColor = (type: string | undefined): string => {
  switch (type) {
    case 'Pdf':
      return 'red';
    case 'Word':
      return 'blue';
    case 'Excel':
      return 'green';
    case 'PowerPoint':
      return 'orange';
    case 'ImageJpeg':
    case 'ImagePng':
      return 'purple';
    default:
      return 'gray';
  }
};

const PersonDocuments = ({ personDisplayId }: PersonDocumentsProps) => {
  const [documents, setDocuments] = useState<DocumentListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showUploadForm, setShowUploadForm] = useState(false);
  const [description, setDescription] = useState('');
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    loadDocuments();
  }, [personDisplayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadDocuments = async () => {
    setLoading(true);
    try {
      const response =
        await documentsApi.apiV1PersonsPersonDisplayIdDocumentsGet(
          personDisplayId,
          1,
          100
        );
      setDocuments(response.data.items || []);
    } catch (err) {
      console.error('Error loading documents:', err);
      setError('Failed to load documents');
    } finally {
      setLoading(false);
    }
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      setSelectedFiles(Array.from(files));
    }
  };

  const handleUpload = async () => {
    if (selectedFiles.length === 0) return;

    setUploading(true);
    setError(null);

    const errors: string[] = [];
    let successCount = 0;

    for (const file of selectedFiles) {
      try {
        await documentsApi.apiV1PersonsPersonDisplayIdDocumentsPost(
          personDisplayId,
          file,
          description || undefined
        );
        successCount++;
      } catch (err: unknown) {
        console.error('Error uploading document:', err);
        const errorMessage = err instanceof Error ? err.message : 'Unknown error';
        errors.push(`${file.name}: ${errorMessage}`);
      }
    }

    if (errors.length > 0) {
      setError(`Failed to upload ${errors.length} file(s). Check file types - only PDF, Word, Excel, PowerPoint, JPEG, PNG are allowed.`);
    }

    if (successCount > 0) {
      setSelectedFiles([]);
      setDescription('');
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
      await loadDocuments();
    }

    if (errors.length === 0) {
      setShowUploadForm(false);
    }

    setUploading(false);
  };

  const handleDownload = (
    documentDisplayId: number,
    fileName: string | null | undefined
  ) => {
    const url = `${API_BASE_URL}/api/v1/persons/${personDisplayId}/documents/${documentDisplayId}/download`;
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName || 'document';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleDelete = async (documentDisplayId: number) => {
    if (!window.confirm('Are you sure you want to delete this document?'))
      return;

    try {
      await documentsApi.apiV1PersonsPersonDisplayIdDocumentsDocumentDisplayIdDelete(
        personDisplayId,
        documentDisplayId
      );
      await loadDocuments();
    } catch (err) {
      console.error('Error deleting document:', err);
      setError('Failed to delete document');
    }
  };

  if (loading) {
    return (
      <Card.Root>
        <Card.Body>
          <Flex justify="center" py={8}>
            <Spinner size="lg" />
          </Flex>
        </Card.Body>
      </Card.Root>
    );
  }

  return (
    <Card.Root>
      <Card.Header>
        <Flex justify="space-between" align="center">
          <Heading size="md">Documents</Heading>
          <Button
            size="sm"
            colorPalette="blue"
            onClick={() => setShowUploadForm(!showUploadForm)}
          >
            {showUploadForm ? 'Cancel' : 'Upload Document'}
          </Button>
        </Flex>
      </Card.Header>
      <Card.Body>
        {error && (
          <Box mb={4} p={3} bg="red.100" color="red.800" borderRadius="md">
            <Text fontSize="sm">{error}</Text>
          </Box>
        )}

        {showUploadForm && (
          <Box mb={6} p={4} borderWidth={1} borderRadius="md" borderColor="border.muted">
            <Text fontWeight="semibold" mb={4}>
              Upload New Document
            </Text>
            <Flex gap={4} align="flex-end" wrap="wrap">
              <Box flex="1" minW="200px">
                <Text fontWeight="medium" mb={2} fontSize="sm">
                  Select Files
                </Text>
                <input
                  ref={fileInputRef}
                  type="file"
                  multiple
                  onChange={handleFileSelect}
                  accept=".pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.jpg,.jpeg,.png"
                  style={{ position: 'absolute', width: '1px', height: '1px', padding: '0', margin: '-1px', overflow: 'hidden', clip: 'rect(0,0,0,0)', border: '0' }}
                />
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => fileInputRef.current?.click()}
                  width="full"
                >
                  {selectedFiles.length > 0 
                    ? `${selectedFiles.length} file(s) selected` 
                    : 'Choose Files...'}
                </Button>
                {selectedFiles.length > 0 && (
                  <Box mt={1}>
                    {selectedFiles.map((file, index) => (
                      <Text key={index} fontSize="xs" color="fg.muted">
                        {file.name} ({formatFileSize(file.size)})
                      </Text>
                    ))}
                  </Box>
                )}
              </Box>

              <Box flex="1" minW="200px">
                <Text fontWeight="medium" mb={2} fontSize="sm">
                  Description (optional)
                </Text>
                <Input
                  value={description}
                  onChange={e => setDescription(e.target.value)}
                  placeholder="Enter description"
                  size="sm"
                />
              </Box>

              <Box>
                <Button
                  colorPalette="blue"
                  size="sm"
                  onClick={handleUpload}
                  disabled={selectedFiles.length === 0}
                  loading={uploading}
                >
                  Upload
                </Button>
              </Box>
            </Flex>
            <Text fontSize="xs" color="fg.muted" mt={2}>
              Supported formats: PDF, Word, Excel, PowerPoint, JPEG, PNG (Max 50 MB)
            </Text>
          </Box>
        )}

        {documents.length === 0 ? (
          <Text color="fg.muted" textAlign="center" py={8}>
            No documents uploaded yet.
          </Text>
        ) : (
          <Table.Root size="sm">
            <Table.Header>
              <Table.Row>
                <Table.ColumnHeader>File Name</Table.ColumnHeader>
                <Table.ColumnHeader>Type</Table.ColumnHeader>
                <Table.ColumnHeader>Size</Table.ColumnHeader>
                <Table.ColumnHeader>Description</Table.ColumnHeader>
                <Table.ColumnHeader>Uploaded</Table.ColumnHeader>
                <Table.ColumnHeader textAlign="right">
                  Actions
                </Table.ColumnHeader>
              </Table.Row>
            </Table.Header>
            <Table.Body>
              {documents.map(doc => (
                <Table.Row key={doc.displayId}>
                  <Table.Cell>
                    <Text fontWeight="medium" fontSize="sm">
                      {doc.fileName}
                    </Text>
                  </Table.Cell>
                  <Table.Cell>
                    <Badge
                      colorPalette={getDocumentTypeColor(doc.documentType)}
                      size="sm"
                    >
                      {doc.documentType}
                    </Badge>
                  </Table.Cell>
                  <Table.Cell>
                    <Text fontSize="sm">
                      {formatFileSize(doc.fileSizeBytes)}
                    </Text>
                  </Table.Cell>
                  <Table.Cell>
                    <Text fontSize="sm" color="fg.muted" maxW="200px" truncate>
                      {doc.description || '-'}
                    </Text>
                  </Table.Cell>
                  <Table.Cell>
                    <Text fontSize="sm">
                      {doc.createdOn
                        ? new Date(doc.createdOn).toLocaleDateString()
                        : '-'}
                    </Text>
                  </Table.Cell>
                  <Table.Cell textAlign="right">
                    <Flex gap={1} justify="flex-end">
                      <Button
                        size="xs"
                        variant="ghost"
                        onClick={() =>
                          handleDownload(doc.displayId!, doc.fileName)
                        }
                      >
                        Download
                      </Button>
                      <Button
                        size="xs"
                        variant="ghost"
                        colorPalette="red"
                        onClick={() => handleDelete(doc.displayId!)}
                      >
                        Delete
                      </Button>
                    </Flex>
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table.Root>
        )}
      </Card.Body>
    </Card.Root>
  );
};

export default PersonDocuments;
