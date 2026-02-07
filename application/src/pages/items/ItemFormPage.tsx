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
import { useItem, useCreateItem, useUpdateItem } from '../../hooks/useItems';
import type {
  CreateItemInput,
  UpdateItemInput,
} from '../../graphql/generated/graphql';
import { useToast } from '../../hooks';

interface ItemFormData {
  itemName: string;
  description: string;
  isActive: boolean;
}

const initialFormData: ItemFormData = {
  itemName: '',
  description: '',
  isActive: true,
};

const ItemFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  const { item, loading: loadingItem } = useItem(
    isEditMode ? Number(displayId) : 0
  );
  const { createItem, loading: creating } = useCreateItem();
  const { updateItem, loading: updating } = useUpdateItem();

  const [formData, setFormData] = useState<ItemFormData>(initialFormData);
  const [error, setError] = useState<string | null>(null);
  const { showSuccess, showError } = useToast();

  const loading = loadingItem;
  const saving = creating || updating;

  useEffect(() => {
    if (isEditMode && item) {
      // eslint-disable-next-line react-hooks/set-state-in-effect -- Populating form with loaded data is a valid pattern
      setFormData({
        itemName: item.itemName || '',
        description: item.description || '',
        isActive: item.isActive ?? true,
      });
    }
  }, [isEditMode, item]);

  const handleChange = useCallback(
    (field: keyof ItemFormData, value: string | boolean) => {
      setFormData((prev) => ({ ...prev, [field]: value }));
    },
    []
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdateItemInput = {
          itemName: formData.itemName,
          description: formData.description || null,
          isActive: formData.isActive,
        };
        await updateItem(Number(displayId), updateDto);
        showSuccess(
          'Item Updated',
          `${formData.itemName} has been updated successfully.`
        );
      } else {
        const createDto: CreateItemInput = {
          itemName: formData.itemName,
          description: formData.description || null,
        };
        await createItem(createDto);
        showSuccess(
          'Item Created',
          `${formData.itemName} has been added successfully.`
        );
      }
      navigate('/items');
    } catch (err) {
      console.error('Error saving item:', err);
      const errorMessage = 'Failed to save item';
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
        <Heading size="lg">{isEditMode ? 'Edit Item' : 'Add New Item'}</Heading>
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
            <Heading size="md">Item Information (Plantilla)</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Field.Root required>
                <Field.Label>Item Name</Field.Label>
                <Input
                  value={formData.itemName}
                  onChange={(e) => handleChange('itemName', e.target.value)}
                  placeholder="Enter item name"
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
            {isEditMode ? 'Update Item' : 'Create Item'}
          </Button>
        </Flex>
      </form>
    </Box>
  );
};

export default ItemFormPage;
