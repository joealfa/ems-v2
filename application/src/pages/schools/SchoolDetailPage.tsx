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
import { useSchool, useDeleteSchool } from '../../hooks/useSchools';
import { formatAddress } from '../../utils/formatters';
import { useConfirm } from '../../hooks';
import { ConfirmDialog } from '../../components/ui';

const SchoolDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const { confirm, confirmDialog } = useConfirm();

  const { school, loading, error } = useSchool(Number(displayId));
  const { deleteSchool, loading: deleting } = useDeleteSchool();

  const handleDelete = async () => {
    if (!displayId) return;

    const confirmed = await confirm({
      title: 'Delete School',
      message:
        'Are you sure you want to delete this school? This action cannot be undone.',
      confirmText: 'Delete',
      confirmColorScheme: 'red',
    });

    if (!confirmed) return;

    try {
      await deleteSchool(Number(displayId));
      navigate('/schools');
    } catch (err) {
      console.error('Error deleting school:', err);
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
        <Text color="red.500">{error?.message || 'School not found'}</Text>
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
                  <Text fontWeight="medium">
                    {school.displayId as unknown as number}
                  </Text>
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
                {school.addresses.map((address, index) =>
                  address ? (
                    <Box key={index} p={4} borderWidth={1} borderRadius="md">
                      <Flex gap={2} mb={2}>
                        <Badge colorPalette="blue">{address.addressType}</Badge>
                        {address.isCurrent && (
                          <Badge colorPalette="green">Current</Badge>
                        )}
                      </Flex>
                      <Text>{formatAddress(address)}</Text>
                    </Box>
                  ) : null
                )}
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
                {school.contacts.map((contact, index) =>
                  contact ? (
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
                  ) : null
                )}
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
      <ConfirmDialog {...confirmDialog} />
    </Box>
  );
};

export default SchoolDetailPage;
