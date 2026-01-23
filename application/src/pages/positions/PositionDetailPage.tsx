import { useState, useEffect } from 'react';
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
import { positionsApi, type PositionResponseDto } from '../../api';

const PositionDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();

  const [position, setPosition] = useState<PositionResponseDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    loadPosition();
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadPosition = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await positionsApi.apiV1PositionsDisplayIdGet(
        Number(displayId)
      );
      setPosition(response.data);
    } catch (err) {
      console.error('Error loading position:', err);
      setError('Failed to load position data');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete this position?')
    )
      return;
    setDeleting(true);
    try {
      await positionsApi.apiV1PositionsDisplayIdDelete(Number(displayId));
      navigate('/positions');
    } catch (err) {
      console.error('Error deleting position:', err);
      setError('Failed to delete position');
    } finally {
      setDeleting(false);
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
        <Text color="red.500">{error || 'Position not found'}</Text>
        <Button mt={4} onClick={() => navigate('/positions')}>
          Back to Positions
        </Button>
      </Box>
    );
  }

  return (
    <Box maxW="800px">
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">{position.titleName}</Heading>
        <Flex gap={2}>
          <Button variant="outline" onClick={() => navigate('/positions')}>
            Back
          </Button>
          <Button
            colorPalette="blue"
            onClick={() => navigate(`/positions/${displayId}/edit`)}
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
                  <Text fontWeight="medium">{position.displayId}</Text>
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
    </Box>
  );
};

export default PositionDetailPage;
