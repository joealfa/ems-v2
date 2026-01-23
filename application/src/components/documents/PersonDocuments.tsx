import { useState, useEffect, useRef } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Card,
  Stack,
  Text,
  Badge,
  Input,
  Spinner,
  Textarea,
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
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
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
    const file = e.target.files?.[0];
    if (file) {
      setSelectedFile(file);
    }
  };

  const handleUpload = async () => {
    if (!selectedFile) return;

    setUploading(true);
    setError(null);

    try {
      await documentsApi.apiV1PersonsPersonDisplayIdDocumentsPost(
        personDisplayId,
        selectedFile,
        description || undefined
      );
      setSelectedFile(null);
      setDescription('');
      setShowUploadForm(false);
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
      await loadDocuments();
    } catch (err) {
      console.error('Error uploading document:', err);
      setError('Failed to upload document');
    } finally {
      setUploading(false);
    }
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
          <Box mb={6} p={4} borderWidth={1} borderRadius="md" bg="bg.muted">
            <Stack gap={4}>
              <Box>
                <Text fontWeight="medium" mb={2}>
                  Select File
                </Text>
                <Input
                  ref={fileInputRef}
                  type="file"
                  onChange={handleFileSelect}
                  accept=".pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.jpg,.jpeg,.png"
                  p={1}
                />
                <Text fontSize="xs" color="fg.muted" mt={1}>
                  Supported formats: PDF, Word, Excel, PowerPoint, JPEG, PNG
                  (Max 50 MB)
                </Text>
              </Box>

              {selectedFile && (
                <Box>
                  <Text fontSize="sm" color="fg.muted">
                    Selected: {selectedFile.name} (
                    {formatFileSize(selectedFile.size)})
                  </Text>
                </Box>
              )}

              <Box>
                <Text fontWeight="medium" mb={2}>
                  Description (Optional)
                </Text>
                <Textarea
                  value={description}
                  onChange={e => setDescription(e.target.value)}
                  placeholder="Enter a description for this document"
                  rows={2}
                />
              </Box>

              <Flex justify="flex-end" gap={2}>
                <Button
                  variant="outline"
                  onClick={() => {
                    setShowUploadForm(false);
                    setSelectedFile(null);
                    setDescription('');
                  }}
                >
                  Cancel
                </Button>
                <Button
                  colorPalette="blue"
                  onClick={handleUpload}
                  disabled={!selectedFile}
                  loading={uploading}
                >
                  Upload
                </Button>
              </Flex>
            </Stack>
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
