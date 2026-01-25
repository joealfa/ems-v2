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
  itemsApi,
  type CreateItemDto,
  type UpdateItemDto,
  type ItemResponseDto,
} from '../../api';

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

  const [formData, setFormData] = useState<ItemFormData>(initialFormData);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    if (isEditMode) {
      loadItem();
    }
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadItem = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await itemsApi.apiV1ItemsDisplayIdGet(Number(displayId));
      const item: ItemResponseDto = response.data;
      setFormData({
        itemName: item.itemName || '',
        description: item.description || '',
        isActive: item.isActive ?? true,
      });
    } catch (err) {
      console.error('Error loading item:', err);
      setError('Failed to load item data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: keyof ItemFormData, value: string | boolean) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdateItemDto = {
          itemName: formData.itemName,
          description: formData.description || null,
          isActive: formData.isActive,
        };
        await itemsApi.apiV1ItemsDisplayIdPut(Number(displayId), updateDto);
      } else {
        const createDto: CreateItemDto = {
          itemName: formData.itemName,
          description: formData.description || null,
        };
        await itemsApi.apiV1ItemsPost(createDto);
      }
      navigate('/items');
    } catch (err) {
      console.error('Error saving item:', err);
      setError('Failed to save item');
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
                  onChange={e => handleChange('itemName', e.target.value)}
                  placeholder="Enter item name"
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
            {isEditMode ? 'Update Item' : 'Create Item'}
          </Button>
        </Flex>
      </form>
    </Box>
  );
};

export default ItemFormPage;
