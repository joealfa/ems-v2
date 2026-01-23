import { useState, useEffect } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Input,
  Card,
  Stack,
  Field,
  NativeSelect,
  Spinner,
  Text,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  personsApi,
  type CreatePersonDto,
  type UpdatePersonDto,
  type PersonResponseDto,
  CreatePersonDtoGenderEnum,
  CreatePersonDtoCivilStatusEnum,
} from '../../api';

interface PersonFormData {
  firstName: string;
  lastName: string;
  middleName: string;
  dateOfBirth: string;
  gender: string;
  civilStatus: string;
}

const initialFormData: PersonFormData = {
  firstName: '',
  lastName: '',
  middleName: '',
  dateOfBirth: '',
  gender: 'Male',
  civilStatus: 'Single',
};

const PersonFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  const [formData, setFormData] = useState<PersonFormData>(initialFormData);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    if (isEditMode) {
      loadPerson();
    }
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadPerson = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await personsApi.apiV1PersonsDisplayIdGet(
        Number(displayId)
      );
      const person: PersonResponseDto = response.data;
      setFormData({
        firstName: person.firstName || '',
        lastName: person.lastName || '',
        middleName: person.middleName || '',
        dateOfBirth: person.dateOfBirth?.split('T')[0] || '',
        gender: person.gender || 'Male',
        civilStatus: person.civilStatus || 'Single',
      });
    } catch (err) {
      console.error('Error loading person:', err);
      setError('Failed to load person data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: keyof PersonFormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdatePersonDto = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          middleName: formData.middleName || null,
          dateOfBirth: formData.dateOfBirth,
          gender: formData.gender as UpdatePersonDto['gender'],
          civilStatus: formData.civilStatus as UpdatePersonDto['civilStatus'],
        };
        await personsApi.apiV1PersonsDisplayIdPut(Number(displayId), updateDto);
      } else {
        const createDto: CreatePersonDto = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          middleName: formData.middleName || null,
          dateOfBirth: formData.dateOfBirth,
          gender: formData.gender as CreatePersonDto['gender'],
          civilStatus: formData.civilStatus as CreatePersonDto['civilStatus'],
        };
        await personsApi.apiV1PersonsPost(createDto);
      }
      navigate('/persons');
    } catch (err) {
      console.error('Error saving person:', err);
      setError('Failed to save person');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <Flex justify="center" align="center" h="100%">
        <Spinner size="lg" />
      </Flex>
    );
  }

  return (
    <Box maxW="800px">
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">
          {isEditMode ? 'Edit Person' : 'Add New Person'}
        </Heading>
        <Button variant="outline" onClick={() => navigate('/persons')}>
          Cancel
        </Button>
      </Flex>

      {error && (
        <Box mb={4} p={4} bg="red.100" color="red.800" borderRadius="md">
          <Text>{error}</Text>
        </Box>
      )}

      <form onSubmit={handleSubmit}>
        <Card.Root>
          <Card.Header>
            <Heading size="md">Personal Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Flex gap={4}>
                <Field.Root flex={1} required>
                  <Field.Label>First Name</Field.Label>
                  <Input
                    value={formData.firstName}
                    onChange={e => handleChange('firstName', e.target.value)}
                    placeholder="Enter first name"
                  />
                </Field.Root>

                <Field.Root flex={1} required>
                  <Field.Label>Last Name</Field.Label>
                  <Input
                    value={formData.lastName}
                    onChange={e => handleChange('lastName', e.target.value)}
                    placeholder="Enter last name"
                  />
                </Field.Root>
              </Flex>

              <Field.Root>
                <Field.Label>Middle Name</Field.Label>
                <Input
                  value={formData.middleName}
                  onChange={e => handleChange('middleName', e.target.value)}
                  placeholder="Enter middle name (optional)"
                />
              </Field.Root>

              <Flex gap={4}>
                <Field.Root flex={1} required>
                  <Field.Label>Date of Birth</Field.Label>
                  <Input
                    type="date"
                    value={formData.dateOfBirth}
                    onChange={e => handleChange('dateOfBirth', e.target.value)}
                  />
                </Field.Root>

                <Field.Root flex={1} required>
                  <Field.Label>Gender</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={formData.gender}
                      onChange={e => handleChange('gender', e.target.value)}
                    >
                      {Object.values(CreatePersonDtoGenderEnum).map(gender => (
                        <option key={gender} value={gender}>
                          {gender}
                        </option>
                      ))}
                    </NativeSelect.Field>
                    <NativeSelect.Indicator />
                  </NativeSelect.Root>
                </Field.Root>

                <Field.Root flex={1} required>
                  <Field.Label>Civil Status</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={formData.civilStatus}
                      onChange={e =>
                        handleChange('civilStatus', e.target.value)
                      }
                    >
                      {Object.values(CreatePersonDtoCivilStatusEnum).map(
                        status => (
                          <option key={status} value={status}>
                            {status === 'SoloParent' ? 'Solo Parent' : status}
                          </option>
                        )
                      )}
                    </NativeSelect.Field>
                    <NativeSelect.Indicator />
                  </NativeSelect.Root>
                </Field.Root>
              </Flex>
            </Stack>
          </Card.Body>
        </Card.Root>

        <Flex justify="flex-end" mt={6} gap={4}>
          <Button variant="outline" onClick={() => navigate('/persons')}>
            Cancel
          </Button>
          <Button type="submit" colorPalette="blue" loading={saving}>
            {isEditMode ? 'Update Person' : 'Create Person'}
          </Button>
        </Flex>
      </form>
    </Box>
  );
};

export default PersonFormPage;
