import { useQuery, useMutation, useLazyQuery } from '@apollo/client';
import {
  GetPositionsDocument,
  GetPositionDocument,
  CreatePositionDocument,
  UpdatePositionDocument,
  DeletePositionDocument,
  type CreatePositionInput,
  type UpdatePositionInput,
} from '../graphql/generated/graphql';

/**
 * Hook for fetching paginated positions list
 */
export function usePositions(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const { data, loading, error, refetch } = useQuery(GetPositionsDocument, {
    variables,
    fetchPolicy: 'cache-and-network',
  });

  return {
    positions: data?.positions?.items ?? [],
    totalCount: data?.positions?.totalCount ?? 0,
    pageNumber: data?.positions?.pageNumber ?? 1,
    pageSize: data?.positions?.pageSize ?? 10,
    totalPages: data?.positions?.totalPages ?? 0,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for lazy fetching positions (useful for AG Grid infinite scrolling)
 */
export function usePositionsLazy() {
  const [fetchPositions, { data, loading, error }] = useLazyQuery(
    GetPositionsDocument,
    {
      fetchPolicy: 'network-only',
    }
  );

  return {
    fetchPositions,
    positions: data?.positions?.items ?? [],
    totalCount: data?.positions?.totalCount ?? 0,
    loading,
    error,
  };
}

/**
 * Hook for fetching a single position by displayId
 */
export function usePosition(displayId: number) {
  const { data, loading, error, refetch } = useQuery(GetPositionDocument, {
    variables: { displayId },
    skip: !displayId,
  });

  return {
    position: data?.position,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for creating a position
 */
export function useCreatePosition() {
  const [createPosition, { data, loading, error }] = useMutation(
    CreatePositionDocument,
    {
      refetchQueries: [GetPositionsDocument],
    }
  );

  const handleCreate = async (input: CreatePositionInput) => {
    const result = await createPosition({ variables: { input } });
    return result.data?.createPosition;
  };

  return {
    createPosition: handleCreate,
    position: data?.createPosition,
    loading,
    error,
  };
}

/**
 * Hook for updating a position
 */
export function useUpdatePosition() {
  const [updatePosition, { data, loading, error }] = useMutation(
    UpdatePositionDocument
  );

  const handleUpdate = async (
    displayId: number,
    input: UpdatePositionInput
  ) => {
    const result = await updatePosition({ variables: { displayId, input } });
    return result.data?.updatePosition;
  };

  return {
    updatePosition: handleUpdate,
    position: data?.updatePosition,
    loading,
    error,
  };
}

/**
 * Hook for deleting a position
 */
export function useDeletePosition() {
  const [deletePosition, { loading, error }] = useMutation(
    DeletePositionDocument,
    {
      refetchQueries: [GetPositionsDocument],
    }
  );

  const handleDelete = async (displayId: number) => {
    const result = await deletePosition({ variables: { displayId } });
    return result.data?.deletePosition ?? false;
  };

  return {
    deletePosition: handleDelete,
    loading,
    error,
  };
}
