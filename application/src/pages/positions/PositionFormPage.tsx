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
  Textarea,
  Checkbox,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  positionsApi,
  type CreatePositionDto,
  type UpdatePositionDto,
  type PositionResponseDto,
} from '../../api';

interface PositionFormData {
  titleName: string;
  description: string;
  isActive: boolean;
}

const initialFormData: PositionFormData = {
  titleName: '',
  description: '',
  isActive: true,
};

const PositionFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  const [formData, setFormData] = useState<PositionFormData>(initialFormData);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    if (isEditMode) {
      loadPosition();
    }
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadPosition = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await positionsApi.apiV1PositionsDisplayIdGet(
        Number(displayId)
      );
      const position: PositionResponseDto = response.data;
      setFormData({
        titleName: position.titleName || '',
        description: position.description || '',
        isActive: position.isActive ?? true,
      });
    } catch (err) {
      console.error('Error loading position:', err);
      setError('Failed to load position data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (
    field: keyof PositionFormData,
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
        const updateDto: UpdatePositionDto = {
          titleName: formData.titleName,
          description: formData.description || null,
          isActive: formData.isActive,
        };
        await positionsApi.apiV1PositionsDisplayIdPut(
          Number(displayId),
          updateDto
        );
      } else {
        const createDto: CreatePositionDto = {
          titleName: formData.titleName,
          description: formData.description || null,
        };
        await positionsApi.apiV1PositionsPost(createDto);
      }
      navigate('/positions');
    } catch (err) {
      console.error('Error saving position:', err);
      setError('Failed to save position');
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
          {isEditMode ? 'Edit Position' : 'Add New Position'}
        </Heading>
        <Button variant="outline" onClick={() => navigate(-1)}>
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
            <Heading size="md">Position Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Field.Root required>
                <Field.Label>Position Title</Field.Label>
                <Input
                  value={formData.titleName}
                  onChange={e => handleChange('titleName', e.target.value)}
                  placeholder="Enter position title"
                />
              </Field.Root>

              <Field.Root>
                <Field.Label>Description</Field.Label>
                <Textarea
                  value={formData.description}
                  onChange={e => handleChange('description', e.target.value)}
                  placeholder="Enter description (optional)"
                  rows={4}
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
          <Button variant="outline" onClick={() => navigate(-1)}>
            Cancel
          </Button>
          <Button type="submit" colorPalette="blue" loading={saving}>
            {isEditMode ? 'Update Position' : 'Create Position'}
          </Button>
        </Flex>
      </form>
    </Box>
  );
};

export default PositionFormPage;
