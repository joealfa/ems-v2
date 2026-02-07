import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import { graphqlRequest } from '../graphql/graphql-client';
import { positionKeys, dashboardKeys } from '../graphql/query-keys';
import {
  GetPositionsDocument,
  GetPositionDocument,
  CreatePositionDocument,
  UpdatePositionDocument,
  DeletePositionDocument,
  type GetPositionsQuery,
  type GetPositionsQueryVariables,
  type GetPositionQuery,
  type CreatePositionInput,
  type UpdatePositionInput,
  type CreatePositionMutation,
  type CreatePositionMutationVariables,
  type UpdatePositionMutation,
  type UpdatePositionMutationVariables,
  type DeletePositionMutation,
  type DeletePositionMutationVariables,
} from '../graphql/generated/graphql';

export function usePositions(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const query = useQuery({
    queryKey: positionKeys.list(variables ?? {}),
    queryFn: () =>
      graphqlRequest<GetPositionsQuery, GetPositionsQueryVariables>(
        GetPositionsDocument,
        variables ?? {}
      ),
  });

  return {
    positions: query.data?.positions?.items ?? [],
    totalCount: query.data?.positions?.totalCount ?? 0,
    pageNumber: query.data?.positions?.pageNumber ?? 1,
    pageSize: query.data?.positions?.pageSize ?? 10,
    totalPages: query.data?.positions?.totalPages ?? 0,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function usePositionsLazy() {
  const fetchPositions = useCallback(async (args: {
    variables: GetPositionsQueryVariables;
  }) => {
    const data = await graphqlRequest<
      GetPositionsQuery,
      GetPositionsQueryVariables
    >(GetPositionsDocument, args.variables);
    return { data };
  }, []);

  return {
    fetchPositions,
    loading: false,
  };
}

export function usePosition(displayId: number) {
  const query = useQuery({
    queryKey: positionKeys.detail(displayId),
    queryFn: () =>
      graphqlRequest<GetPositionQuery, { displayId: number }>(
        GetPositionDocument,
        { displayId }
      ),
    enabled: !!displayId,
  });

  return {
    position: query.data?.position,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useCreatePosition() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: CreatePositionMutationVariables) =>
      graphqlRequest<CreatePositionMutation, CreatePositionMutationVariables>(
        CreatePositionDocument,
        variables
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: positionKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleCreate = async (input: CreatePositionInput) => {
    const result = await mutation.mutateAsync({ input });
    return result.createPosition;
  };

  return {
    createPosition: handleCreate,
    position: mutation.data?.createPosition,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useUpdatePosition() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: UpdatePositionMutationVariables) =>
      graphqlRequest<UpdatePositionMutation, UpdatePositionMutationVariables>(
        UpdatePositionDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: positionKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: positionKeys.lists() });
    },
  });

  const handleUpdate = async (
    displayId: number,
    input: UpdatePositionInput
  ) => {
    const result = await mutation.mutateAsync({ displayId, input });
    return result.updatePosition;
  };

  return {
    updatePosition: handleUpdate,
    position: mutation.data?.updatePosition,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useDeletePosition() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeletePositionMutationVariables) =>
      graphqlRequest<DeletePositionMutation, DeletePositionMutationVariables>(
        DeletePositionDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.removeQueries({
        queryKey: positionKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: positionKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleDelete = async (displayId: number) => {
    const result = await mutation.mutateAsync({ displayId });
    return result.deletePosition ?? false;
  };

  return {
    deletePosition: handleDelete,
    loading: mutation.isPending,
    error: mutation.error,
  };
}
