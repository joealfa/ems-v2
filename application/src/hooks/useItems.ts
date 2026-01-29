import { useQuery, useMutation, useLazyQuery } from '@apollo/client';
import {
  GetItemsDocument,
  GetItemDocument,
  CreateItemDocument,
  UpdateItemDocument,
  DeleteItemDocument,
  type CreateItemInput,
  type UpdateItemInput,
} from '../graphql/generated/graphql';

/**
 * Hook for fetching paginated items list
 */
export function useItems(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const { data, loading, error, refetch } = useQuery(GetItemsDocument, {
    variables,
    fetchPolicy: 'cache-and-network',
  });

  return {
    items: data?.items?.items ?? [],
    totalCount: data?.items?.totalCount ?? 0,
    pageNumber: data?.items?.pageNumber ?? 1,
    pageSize: data?.items?.pageSize ?? 10,
    totalPages: data?.items?.totalPages ?? 0,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for lazy fetching items (useful for AG Grid infinite scrolling)
 */
export function useItemsLazy() {
  const [fetchItems, { data, loading, error }] = useLazyQuery(
    GetItemsDocument,
    {
      fetchPolicy: 'network-only',
    }
  );

  return {
    fetchItems,
    items: data?.items?.items ?? [],
    totalCount: data?.items?.totalCount ?? 0,
    loading,
    error,
  };
}

/**
 * Hook for fetching a single item by displayId
 */
export function useItem(displayId: number) {
  const { data, loading, error, refetch } = useQuery(GetItemDocument, {
    variables: { displayId },
    skip: !displayId,
  });

  return {
    item: data?.item,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for creating an item
 */
export function useCreateItem() {
  const [createItem, { data, loading, error }] = useMutation(
    CreateItemDocument,
    {
      refetchQueries: [GetItemsDocument],
    }
  );

  const handleCreate = async (input: CreateItemInput) => {
    const result = await createItem({ variables: { input } });
    return result.data?.createItem;
  };

  return {
    createItem: handleCreate,
    item: data?.createItem,
    loading,
    error,
  };
}

/**
 * Hook for updating an item
 */
export function useUpdateItem() {
  const [updateItem, { data, loading, error }] =
    useMutation(UpdateItemDocument);

  const handleUpdate = async (displayId: number, input: UpdateItemInput) => {
    const result = await updateItem({ variables: { displayId, input } });
    return result.data?.updateItem;
  };

  return {
    updateItem: handleUpdate,
    item: data?.updateItem,
    loading,
    error,
  };
}

/**
 * Hook for deleting an item
 */
export function useDeleteItem() {
  const [deleteItem, { loading, error }] = useMutation(DeleteItemDocument, {
    refetchQueries: [GetItemsDocument],
  });

  const handleDelete = async (displayId: number) => {
    const result = await deleteItem({ variables: { displayId } });
    return result.data?.deleteItem ?? false;
  };

  return {
    deleteItem: handleDelete,
    loading,
    error,
  };
}
