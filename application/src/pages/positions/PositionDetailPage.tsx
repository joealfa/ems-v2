import {
  Box,
  Heading,
  Button,
  Flex,
  Card,
  Stack,
  Text,
  Spinner,
  Badge,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import { usePosition, useDeletePosition } from '../../hooks/usePositions';
import { useConfirm, useToast } from '../../hooks';
import { ConfirmDialog } from '../../components/ui';

const PositionDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const { confirm, confirmDialog } = useConfirm();

  const { position, loading, error } = usePosition(Number(displayId));
  const { deletePosition, loading: deleting } = useDeletePosition();
  const { showSuccess, showError } = useToast();

  const handleDelete = async () => {
    if (!displayId) return;

    const confirmed = await confirm({
      title: 'Delete Position',
      message:
        'Are you sure you want to delete this position? This action cannot be undone.',
      confirmText: 'Delete',
      confirmColorScheme: 'red',
    });

    if (!confirmed) return;

    try {
      await deletePosition(Number(displayId));
      showSuccess(
        'Position Deleted',
        `${position?.titleName || 'Position'} has been deleted successfully.`
      );
      navigate('/positions');
    } catch (err) {
      console.error('Error deleting position:', err);
      showError(
        'Delete Failed',
        'Unable to delete position. Please try again.'
      );
    }
  };

  if (loading) {
    return (
      <Flex justify="center" align="center" h="100%">
        <Spinner size="lg" />
      </Flex>
    );
  }

  if (error || !position) {
    return (
      <Box>
        <Text color="red.500">{error?.message || 'Position not found'}</Text>
        <Button mt={4} onClick={() => navigate('/positions')}>
          Back to Positions
        </Button>
      </Box>
    );
  }

  return (
    <Box>
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">{position.titleName}</Heading>
        <Flex gap={2}>
          <Button variant="outline" onClick={() => navigate('/positions')}>
            Back
          </Button>
          <Button
            colorPalette="blue"
            onClick={() =>
              navigate(`/positions/${displayId}/edit`, {
                state: { fromView: true },
              })
            }
          >
            Edit
          </Button>
          <Button
            colorPalette="red"
            variant="outline"
            onClick={handleDelete}
            loading={deleting}
          >
            Delete
          </Button>
        </Flex>
      </Flex>

      <Stack gap={6}>
        <Card.Root>
          <Card.Header>
            <Heading size="md">Position Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Display ID
                  </Text>
                  <Text fontWeight="medium">
                    {position.displayId as unknown as number}
                  </Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Position Title
                  </Text>
                  <Text fontWeight="medium">{position.titleName}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Status
                  </Text>
                  <Badge colorPalette={position.isActive ? 'green' : 'red'}>
                    {position.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </Box>
              </Flex>

              {position.description && (
                <Box>
                  <Text color="fg.muted" fontSize="sm">
                    Description
                  </Text>
                  <Text fontWeight="medium">{position.description}</Text>
                </Box>
              )}
            </Stack>
          </Card.Body>
        </Card.Root>

        <Card.Root>
          <Card.Header>
            <Heading size="md">Audit Information</Heading>
          </Card.Header>
          <Card.Body>
            <Flex gap={8}>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created On
                </Text>
                <Text fontWeight="medium">
                  {position.createdOn
                    ? new Date(position.createdOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created By
                </Text>
                <Text fontWeight="medium">{position.createdBy || '-'}</Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified On
                </Text>
                <Text fontWeight="medium">
                  {position.modifiedOn
                    ? new Date(position.modifiedOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified By
                </Text>
                <Text fontWeight="medium">{position.modifiedBy || '-'}</Text>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>
      </Stack>
      <ConfirmDialog {...confirmDialog} />
    </Box>
  );
};

export default PositionDetailPage;
