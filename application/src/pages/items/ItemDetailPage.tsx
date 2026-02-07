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
import { useItem, useDeleteItem } from '../../hooks/useItems';
import { useConfirm, useToast } from '../../hooks';
import { ConfirmDialog } from '../../components/ui';

const ItemDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const { confirm, confirmDialog } = useConfirm();

  const { item, loading, error } = useItem(Number(displayId));
  const { deleteItem, loading: deleting } = useDeleteItem();
  const { showSuccess, showError } = useToast();

  const handleDelete = async () => {
    if (!displayId) return;

    const confirmed = await confirm({
      title: 'Delete Item',
      message:
        'Are you sure you want to delete this item? This action cannot be undone.',
      confirmText: 'Delete',
      confirmColorScheme: 'red',
    });

    if (!confirmed) return;

    try {
      await deleteItem(Number(displayId));
      showSuccess(
        'Item Deleted',
        `${item?.itemName || 'Item'} has been deleted successfully.`
      );
      navigate('/items');
    } catch (err) {
      console.error('Error deleting item:', err);
      showError('Delete Failed', 'Unable to delete item. Please try again.');
    }
  };

  if (loading) {
    return (
      <Flex justify="center" align="center" h="100%">
        <Spinner size="lg" />
      </Flex>
    );
  }

  if (error || !item) {
    return (
      <Box>
        <Text color="red.500">{error?.message || 'Item not found'}</Text>
        <Button mt={4} onClick={() => navigate('/items')}>
          Back to Items
        </Button>
      </Box>
    );
  }

  return (
    <Box>
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">{item.itemName}</Heading>
        <Flex gap={2}>
          <Button variant="outline" onClick={() => navigate('/items')}>
            Back
          </Button>
          <Button
            colorPalette="blue"
            onClick={() =>
              navigate(`/items/${displayId}/edit`, {
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
            <Heading size="md">Item Information (Plantilla)</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Display ID
                  </Text>
                  <Text fontWeight="medium">
                    {item.displayId as unknown as number}
                  </Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Item Name
                  </Text>
                  <Text fontWeight="medium">{item.itemName}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Status
                  </Text>
                  <Badge colorPalette={item.isActive ? 'green' : 'red'}>
                    {item.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </Box>
              </Flex>

              {item.description && (
                <Box>
                  <Text color="fg.muted" fontSize="sm">
                    Description
                  </Text>
                  <Text fontWeight="medium">{item.description}</Text>
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
                  {item.createdOn
                    ? new Date(item.createdOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created By
                </Text>
                <Text fontWeight="medium">{item.createdBy || '-'}</Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified On
                </Text>
                <Text fontWeight="medium">
                  {item.modifiedOn
                    ? new Date(item.modifiedOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified By
                </Text>
                <Text fontWeight="medium">{item.modifiedBy || '-'}</Text>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>
      </Stack>
      <ConfirmDialog {...confirmDialog} />
    </Box>
  );
};

export default ItemDetailPage;
