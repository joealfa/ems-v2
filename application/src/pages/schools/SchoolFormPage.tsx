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
  Spinner,
  Text,
  Checkbox,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  schoolsApi,
  type CreateSchoolDto,
  type UpdateSchoolDto,
  type SchoolResponseDto,
} from '../../api';

interface SchoolFormData {
  schoolName: string;
  isActive: boolean;
}

const initialFormData: SchoolFormData = {
  schoolName: '',
  isActive: true,
};

const SchoolFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  const [formData, setFormData] = useState<SchoolFormData>(initialFormData);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    if (isEditMode) {
      loadSchool();
    }
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadSchool = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await schoolsApi.apiV1SchoolsDisplayIdGet(
        Number(displayId)
      );
      const school: SchoolResponseDto = response.data;
      setFormData({
        schoolName: school.schoolName || '',
        isActive: school.isActive ?? true,
      });
    } catch (err) {
      console.error('Error loading school:', err);
      setError('Failed to load school data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (
    field: keyof SchoolFormData,
    value: string | boolean
  ) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdateSchoolDto = {
          schoolName: formData.schoolName,
          isActive: formData.isActive,
        };
        await schoolsApi.apiV1SchoolsDisplayIdPut(Number(displayId), updateDto);
      } else {
        const createDto: CreateSchoolDto = {
          schoolName: formData.schoolName,
        };
        await schoolsApi.apiV1SchoolsPost(createDto);
      }
      navigate('/schools');
    } catch (err) {
      console.error('Error saving school:', err);
      setError('Failed to save school');
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
          {isEditMode ? 'Edit School' : 'Add New School'}
        </Heading>
        <Button variant="outline" onClick={() => navigate('/schools')}>
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
            <Heading size="md">School Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Field.Root required>
                <Field.Label>School Name</Field.Label>
                <Input
                  value={formData.schoolName}
                  onChange={e => handleChange('schoolName', e.target.value)}
                  placeholder="Enter school name"
                />
              </Field.Root>

              {isEditMode && (
                <Checkbox.Root
                  checked={formData.isActive}
                  onCheckedChange={e => handleChange('isActive', !!e.checked)}
                >
                  <Checkbox.HiddenInput />
                  <Checkbox.Control />
                  <Checkbox.Label>Active</Checkbox.Label>
                </Checkbox.Root>
              )}
            </Stack>
          </Card.Body>
        </Card.Root>

        <Flex justify="flex-end" mt={6} gap={4}>
          <Button variant="outline" onClick={() => navigate('/schools')}>
            Cancel
          </Button>
          <Button type="submit" colorPalette="blue" loading={saving}>
            {isEditMode ? 'Update School' : 'Create School'}
          </Button>
        </Flex>
      </form>
    </Box>
  );
};

export default SchoolFormPage;
