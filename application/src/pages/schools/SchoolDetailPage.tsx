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
import { schoolsApi, type SchoolResponseDto } from '../../api';

const SchoolDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();

  const [school, setSchool] = useState<SchoolResponseDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    loadSchool();
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadSchool = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await schoolsApi.apiV1SchoolsDisplayIdGet(
        Number(displayId)
      );
      setSchool(response.data);
    } catch (err) {
      console.error('Error loading school:', err);
      setError('Failed to load school data');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete this school?')
    )
      return;
    setDeleting(true);
    try {
      await schoolsApi.apiV1SchoolsDisplayIdDelete(Number(displayId));
      navigate('/schools');
    } catch (err) {
      console.error('Error deleting school:', err);
      setError('Failed to delete school');
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

  if (error || !school) {
    return (
      <Box>
        <Text color="red.500">{error || 'School not found'}</Text>
        <Button mt={4} onClick={() => navigate('/schools')}>
          Back to Schools
        </Button>
      </Box>
    );
  }

  return (
    <Box maxW="800px">
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">{school.schoolName}</Heading>
        <Flex gap={2}>
          <Button variant="outline" onClick={() => navigate('/schools')}>
            Back
          </Button>
          <Button
            colorPalette="blue"
            onClick={() =>
              navigate(`/schools/${displayId}/edit`, {
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
            <Heading size="md">School Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Display ID
                  </Text>
                  <Text fontWeight="medium">{school.displayId}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    School Name
                  </Text>
                  <Text fontWeight="medium">{school.schoolName}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Status
                  </Text>
                  <Badge colorPalette={school.isActive ? 'green' : 'red'}>
                    {school.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </Box>
              </Flex>
            </Stack>
          </Card.Body>
        </Card.Root>

        {school.addresses && school.addresses.length > 0 && (
          <Card.Root>
            <Card.Header>
              <Heading size="md">Addresses</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                {school.addresses.map((address, index) => (
                  <Box key={index} p={4} borderWidth={1} borderRadius="md">
                    <Flex gap={2} mb={2}>
                      <Badge colorPalette="blue">{address.addressType}</Badge>
                      {address.isCurrent && (
                        <Badge colorPalette="green">Current</Badge>
                      )}
                    </Flex>
                    <Text>{address.fullAddress}</Text>
                  </Box>
                ))}
              </Stack>
            </Card.Body>
          </Card.Root>
        )}

        {school.contacts && school.contacts.length > 0 && (
          <Card.Root>
            <Card.Header>
              <Heading size="md">Contacts</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                {school.contacts.map((contact, index) => (
                  <Box key={index} p={4} borderWidth={1} borderRadius="md">
                    <Badge colorPalette="blue" mb={2}>
                      {contact.contactType}
                    </Badge>
                    {contact.email && (
                      <Text>
                        <Text as="span" color="fg.muted">
                          Email:{' '}
                        </Text>
                        {contact.email}
                      </Text>
                    )}
                    {contact.mobile && (
                      <Text>
                        <Text as="span" color="fg.muted">
                          Mobile:{' '}
                        </Text>
                        {contact.mobile}
                      </Text>
                    )}
                    {contact.landLine && (
                      <Text>
                        <Text as="span" color="fg.muted">
                          Landline:{' '}
                        </Text>
                        {contact.landLine}
                      </Text>
                    )}
                  </Box>
                ))}
              </Stack>
            </Card.Body>
          </Card.Root>
        )}

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
                  {school.createdOn
                    ? new Date(school.createdOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created By
                </Text>
                <Text fontWeight="medium">{school.createdBy || '-'}</Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified On
                </Text>
                <Text fontWeight="medium">
                  {school.modifiedOn
                    ? new Date(school.modifiedOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified By
                </Text>
                <Text fontWeight="medium">{school.modifiedBy || '-'}</Text>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>
      </Stack>
    </Box>
  );
};

export default SchoolDetailPage;
