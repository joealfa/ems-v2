import { useState, useEffect, useCallback } from 'react';
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
  usePosition,
  useCreatePosition,
  useUpdatePosition,
} from '../../hooks/usePositions';
import type {
  CreatePositionInput,
  UpdatePositionInput,
} from '../../graphql/generated/graphql';
import { useToast } from '../../hooks';

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

  const { position, loading: loadingPosition } = usePosition(
    isEditMode ? Number(displayId) : 0
  );
  const { createPosition, loading: creating } = useCreatePosition();
  const { updatePosition, loading: updating } = useUpdatePosition();

  const [formData, setFormData] = useState<PositionFormData>(initialFormData);
  const [error, setError] = useState<string | null>(null);
  const { showSuccess, showError } = useToast();

  const loading = loadingPosition;
  const saving = creating || updating;

  useEffect(() => {
    if (isEditMode && position) {
      // eslint-disable-next-line react-hooks/set-state-in-effect -- Populating form with loaded data is a valid pattern
      setFormData({
        titleName: position.titleName || '',
        description: position.description || '',
        isActive: position.isActive ?? true,
      });
    }
  }, [isEditMode, position]);

  const handleChange = useCallback(
    (field: keyof PositionFormData, value: string | boolean) => {
      setFormData((prev) => ({ ...prev, [field]: value }));
    },
    []
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdatePositionInput = {
          titleName: formData.titleName,
          description: formData.description || null,
          isActive: formData.isActive,
        };
        await updatePosition(Number(displayId), updateDto);
        showSuccess(
          'Position Updated',
          `${formData.titleName} has been updated successfully.`
        );
      } else {
        const createDto: CreatePositionInput = {
          titleName: formData.titleName,
          description: formData.description || null,
        };
        await createPosition(createDto);
        showSuccess(
          'Position Created',
          `${formData.titleName} has been added successfully.`
        );
      }
      navigate('/positions');
    } catch (err) {
      console.error('Error saving position:', err);
      const errorMessage = 'Failed to save position';
      setError(errorMessage);
      showError('Operation Failed', errorMessage);
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
    <Box>
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
                  onChange={(e) => handleChange('titleName', e.target.value)}
                  placeholder="Enter position title"
                />
              </Field.Root>

              <Field.Root>
                <Field.Label>Description</Field.Label>
                <Textarea
                  value={formData.description}
                  onChange={(e) => handleChange('description', e.target.value)}
                  placeholder="Enter description (optional)"
                  rows={4}
                />
              </Field.Root>

              {isEditMode && (
                <Checkbox.Root
                  checked={formData.isActive}
                  onCheckedChange={(e) => handleChange('isActive', !!e.checked)}
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
