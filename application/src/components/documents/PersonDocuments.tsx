import { useState, useRef } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Card,
  Text,
  Input,
  Spinner,
} from '@chakra-ui/react';
import DocumentsTable from './DocumentsTable';
import { formatFileSize } from '../../utils';
import {
  usePersonDocuments,
  useUploadDocument,
  useDeleteDocument,
  getDocumentDownloadUrl,
} from '../../hooks/useDocuments';
import { useConfirm } from '../../hooks';
import { ConfirmDialog } from '../ui';

interface PersonDocumentsProps {
  personDisplayId: number;
}

const PersonDocuments = ({ personDisplayId }: PersonDocumentsProps) => {
  const { documents, loading, refetch } = usePersonDocuments(personDisplayId);
  const { uploadDocument, uploading } = useUploadDocument();
  const { deleteDocument } = useDeleteDocument();
  const { confirm, confirmDialog } = useConfirm();
  const [error, setError] = useState<string | null>(null);
  const [showUploadForm, setShowUploadForm] = useState(false);
  const [description, setDescription] = useState('');
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      setSelectedFiles(Array.from(files));
    }
  };

  const handleUpload = async () => {
    if (selectedFiles.length === 0) return;

    setError(null);

    const errors: string[] = [];
    let successCount = 0;

    for (const file of selectedFiles) {
      try {
        await uploadDocument(personDisplayId, file, description || undefined);
        successCount++;
      } catch (err: unknown) {
        console.error('Error uploading document:', err);
        const errorMessage =
          err instanceof Error ? err.message : 'Unknown error';
        errors.push(`${file.name}: ${errorMessage}`);
      }
    }

    if (errors.length > 0) {
      setError(
        `Failed to upload ${errors.length} file(s). Check file types - only PDF, Word, Excel, PowerPoint, JPEG, PNG are allowed.`
      );
    }

    if (successCount > 0) {
      setSelectedFiles([]);
      setDescription('');
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
      await refetch();
    }

    if (errors.length === 0) {
      setShowUploadForm(false);
    }
  };

  const handleDownload = async (
    documentDisplayId: number,
    fileName: string | null | undefined
  ) => {
    try {
      const url = getDocumentDownloadUrl(personDisplayId, documentDisplayId);
      const response = await fetch(url);

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const blob = await response.blob();
      const blobUrl = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = blobUrl;
      link.download = fileName || 'document';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(blobUrl);
    } catch (err) {
      console.error('Error downloading document:', err);
      setError('Failed to download document');
    }
  };

  const handleDelete = async (documentDisplayId: number) => {
    const confirmed = await confirm({
      title: 'Delete Document',
      message:
        'Are you sure you want to delete this document? This action cannot be undone.',
      confirmText: 'Delete',
      confirmColorScheme: 'red',
    });

    if (!confirmed) return;

    try {
      await deleteDocument(personDisplayId, documentDisplayId);
      await refetch();
    } catch (err) {
      console.error('Error deleting document:', err);
      setError('Failed to delete document');
    }
  };

  // Transform GraphQL documents to the format expected by DocumentsTable
  const transformedDocuments = documents
    .filter((doc): doc is NonNullable<typeof doc> => doc !== null)
    .map((doc) => ({
      displayId: doc.displayId,
      fileName: doc.fileName,
      fileSize: doc.fileSizeBytes,
      fileSizeBytes: doc.fileSizeBytes,
      mimeType: doc.contentType,
      documentType: doc.documentType,
      description: doc.description,
      uploadedAt: doc.createdOn,
      uploadedBy: doc.createdBy,
    }));

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
          <Box
            mb={6}
            p={4}
            borderWidth={1}
            borderRadius="md"
            borderColor="border.muted"
          >
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
                  style={{
                    position: 'absolute',
                    width: '1px',
                    height: '1px',
                    padding: '0',
                    margin: '-1px',
                    overflow: 'hidden',
                    clip: 'rect(0,0,0,0)',
                    border: '0',
                  }}
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
                  onChange={(e) => setDescription(e.target.value)}
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
              Supported formats: PDF, Word, Excel, PowerPoint, JPEG, PNG (Max 50
              MB)
            </Text>
          </Box>
        )}

        <DocumentsTable
          documents={transformedDocuments}
          onDownload={handleDownload}
          onDelete={handleDelete}
          showActions={true}
        />
      </Card.Body>
      <ConfirmDialog {...confirmDialog} />
    </Card.Root>
  );
};

export default PersonDocuments;
