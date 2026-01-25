import { Box, Button, Flex, Text, Badge, Table } from '@chakra-ui/react';
import type { DocumentListDto } from '../../api';
import { formatFileSize, getDocumentTypeColor } from './utils';

interface DocumentsTableProps {
  documents: DocumentListDto[];
  onDownload: (
    documentDisplayId: number,
    fileName: string | null | undefined
  ) => void;
  onDelete?: (documentDisplayId: number) => void;
  showActions?: boolean;
}

const DocumentsTable = ({
  documents,
  onDownload,
  onDelete,
  showActions = true,
}: DocumentsTableProps) => {
  if (documents.length === 0) {
    return (
      <Text color="fg.muted" textAlign="center" py={8}>
        No documents uploaded yet.
      </Text>
    );
  }

  return (
    <Box overflowX="auto">
      <Table.Root size="sm" variant="outline">
        <Table.Header>
          <Table.Row>
            <Table.ColumnHeader px={4} py={3} textAlign="center">
              File Name
            </Table.ColumnHeader>
            <Table.ColumnHeader px={4} py={3} textAlign="center">
              Type
            </Table.ColumnHeader>
            <Table.ColumnHeader px={4} py={3} textAlign="center">
              Size
            </Table.ColumnHeader>
            <Table.ColumnHeader px={4} py={3} textAlign="center">
              Description
            </Table.ColumnHeader>
            {showActions && (
              <Table.ColumnHeader px={4} py={3} textAlign="center">
                Actions
              </Table.ColumnHeader>
            )}
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {documents.map(doc => (
            <Table.Row key={doc.displayId}>
              <Table.Cell px={4} py={2}>
                <Text fontWeight="medium" fontSize="sm">
                  {doc.fileName}
                </Text>
              </Table.Cell>
              <Table.Cell px={4} py={2} textAlign="center">
                <Badge
                  colorPalette={getDocumentTypeColor(doc.documentType)}
                  size="sm"
                >
                  {doc.documentType}
                </Badge>
              </Table.Cell>
              <Table.Cell px={4} py={2} textAlign="center">
                <Text fontSize="sm">{formatFileSize(doc.fileSizeBytes)}</Text>
              </Table.Cell>
              <Table.Cell px={4} py={2}>
                <Text fontSize="sm" color="fg.muted" maxW="200px" truncate>
                  {doc.description || '-'}
                </Text>
              </Table.Cell>
              {showActions && (
                <Table.Cell px={4} py={2} textAlign="center">
                  <Flex gap={2} justify="center">
                    <Button
                      size="xs"
                      variant="ghost"
                      onClick={() => onDownload(doc.displayId!, doc.fileName)}
                    >
                      Download
                    </Button>
                    {onDelete && (
                      <Button
                        size="xs"
                        variant="outline"
                        colorPalette="red"
                        onClick={() => onDelete(doc.displayId!)}
                      >
                        Delete
                      </Button>
                    )}
                  </Flex>
                </Table.Cell>
              )}
            </Table.Row>
          ))}
        </Table.Body>
      </Table.Root>
    </Box>
  );
};

export default DocumentsTable;
