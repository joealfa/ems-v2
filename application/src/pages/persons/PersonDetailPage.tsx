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
import { personsApi, type PersonResponseDto } from '../../api';
import {
  PersonDocuments,
  ProfileImageUpload,
} from '../../components/documents';

const PersonDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();

  const [person, setPerson] = useState<PersonResponseDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    loadPerson();
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadPerson = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await personsApi.apiV1PersonsDisplayIdGet(
        Number(displayId)
      );
      setPerson(response.data);
    } catch (err) {
      console.error('Error loading person:', err);
      setError('Failed to load person data');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete this person?')
    )
      return;
    setDeleting(true);
    try {
      await personsApi.apiV1PersonsDisplayIdDelete(Number(displayId));
      navigate('/persons');
    } catch (err) {
      console.error('Error deleting person:', err);
      setError('Failed to delete person');
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

  if (error || !person) {
    return (
      <Box>
        <Text color="red.500">{error || 'Person not found'}</Text>
        <Button mt={4} onClick={() => navigate('/persons')}>
          Back to Persons
        </Button>
      </Box>
    );
  }

  return (
    <Box maxW="900px">
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">{person.fullName}</Heading>
        <Flex gap={2}>
          <Button variant="outline" onClick={() => navigate('/persons')}>
            Back
          </Button>
          <Button
            colorPalette="blue"
            onClick={() =>
              navigate(`/persons/${displayId}/edit`, {
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
        {/* Profile Image and Basic Info */}
        <Card.Root>
          <Card.Header>
            <Heading size="md">Personal Information</Heading>
          </Card.Header>
          <Card.Body>
            <Flex gap={8}>
              {/* Profile Image Section */}
              <Box>
                <ProfileImageUpload
                  personDisplayId={Number(displayId)}
                  currentImageUrl={person.profileImageUrl}
                  onImageUpdated={loadPerson}
                />
              </Box>

              {/* Personal Details */}
              <Box flex={1}>
                <Stack gap={4}>
                  <Flex gap={8}>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        Display ID
                      </Text>
                      <Text fontWeight="medium">{person.displayId}</Text>
                    </Box>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        Full Name
                      </Text>
                      <Text fontWeight="medium">{person.fullName}</Text>
                    </Box>
                  </Flex>

                  <Flex gap={8}>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        First Name
                      </Text>
                      <Text fontWeight="medium">{person.firstName}</Text>
                    </Box>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        Middle Name
                      </Text>
                      <Text fontWeight="medium">
                        {person.middleName || '-'}
                      </Text>
                    </Box>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        Last Name
                      </Text>
                      <Text fontWeight="medium">{person.lastName}</Text>
                    </Box>
                  </Flex>

                  <Flex gap={8}>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        Date of Birth
                      </Text>
                      <Text fontWeight="medium">
                        {person.dateOfBirth
                          ? new Date(person.dateOfBirth).toLocaleDateString()
                          : '-'}
                      </Text>
                    </Box>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        Gender
                      </Text>
                      <Badge
                        colorPalette={
                          person.gender === 'Male' ? 'blue' : 'pink'
                        }
                      >
                        {person.gender}
                      </Badge>
                    </Box>
                    <Box flex={1}>
                      <Text color="fg.muted" fontSize="sm">
                        Civil Status
                      </Text>
                      <Badge colorPalette="gray">
                        {person.civilStatus === 'SoloParent'
                          ? 'Solo Parent'
                          : person.civilStatus}
                      </Badge>
                    </Box>
                  </Flex>
                </Stack>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>

        {/* Addresses Section */}
        {person.addresses && person.addresses.length > 0 && (
          <Card.Root>
            <Card.Header>
              <Heading size="md">Addresses</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                {person.addresses.map((address, index) => (
                  <Box key={index} p={4} borderWidth={1} borderRadius="md">
                    <Flex gap={2} mb={2}>
                      <Badge colorPalette="blue">{address.addressType}</Badge>
                      {address.isCurrent && (
                        <Badge colorPalette="green">Current</Badge>
                      )}
                      {address.isPermanent && (
                        <Badge colorPalette="purple">Permanent</Badge>
                      )}
                    </Flex>
                    <Text>{address.fullAddress}</Text>
                  </Box>
                ))}
              </Stack>
            </Card.Body>
          </Card.Root>
        )}

        {/* Contacts Section */}
        {person.contacts && person.contacts.length > 0 && (
          <Card.Root>
            <Card.Header>
              <Heading size="md">Contacts</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                {person.contacts.map((contact, index) => (
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

        {/* Documents Section */}
        <PersonDocuments personDisplayId={Number(displayId)} />

        {/* Audit Information */}
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
                  {person.createdOn
                    ? new Date(person.createdOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created By
                </Text>
                <Text fontWeight="medium">{person.createdBy || '-'}</Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified On
                </Text>
                <Text fontWeight="medium">
                  {person.modifiedOn
                    ? new Date(person.modifiedOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified By
                </Text>
                <Text fontWeight="medium">{person.modifiedBy || '-'}</Text>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>
      </Stack>
    </Box>
  );
};

export default PersonDetailPage;
