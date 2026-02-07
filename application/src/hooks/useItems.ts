import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import { graphqlRequest } from '../graphql/graphql-client';
import { itemKeys, dashboardKeys } from '../graphql/query-keys';
import {
  GetItemsDocument,
  GetItemDocument,
  CreateItemDocument,
  UpdateItemDocument,
  DeleteItemDocument,
  type GetItemsQuery,
  type GetItemsQueryVariables,
  type GetItemQuery,
  type CreateItemInput,
  type UpdateItemInput,
  type CreateItemMutation,
  type CreateItemMutationVariables,
  type UpdateItemMutation,
  type UpdateItemMutationVariables,
  type DeleteItemMutation,
  type DeleteItemMutationVariables,
} from '../graphql/generated/graphql';

export function useItems(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const query = useQuery({
    queryKey: itemKeys.list(variables ?? {}),
    queryFn: () =>
      graphqlRequest<GetItemsQuery, GetItemsQueryVariables>(
        GetItemsDocument,
        variables ?? {}
      ),
  });

  return {
    items: query.data?.items?.items ?? [],
    totalCount: query.data?.items?.totalCount ?? 0,
    pageNumber: query.data?.items?.pageNumber ?? 1,
    pageSize: query.data?.items?.pageSize ?? 10,
    totalPages: query.data?.items?.totalPages ?? 0,
    loading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useItemsLazy() {
  const fetchItems = useCallback(
    async (args: { variables: GetItemsQueryVariables }) => {
      const data = await graphqlRequest<GetItemsQuery, GetItemsQueryVariables>(
        GetItemsDocument,
        args.variables
      );
      return { data };
    },
    []
  );

  return {
    fetchItems,
    loading: false,
  };
}

export function useItem(displayId: number) {
  const query = useQuery({
    queryKey: itemKeys.detail(displayId),
    queryFn: () =>
      graphqlRequest<GetItemQuery, { displayId: number }>(GetItemDocument, {
        displayId,
      }),
    enabled: !!displayId,
  });

  return {
    item: query.data?.item,
    loading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useCreateItem() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: CreateItemMutationVariables) =>
      graphqlRequest<CreateItemMutation, CreateItemMutationVariables>(
        CreateItemDocument,
        variables
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: itemKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleCreate = async (input: CreateItemInput) => {
    const result = await mutation.mutateAsync({ input });
    return result.createItem;
  };

  return {
    createItem: handleCreate,
    item: mutation.data?.createItem,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useUpdateItem() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: UpdateItemMutationVariables) =>
      graphqlRequest<UpdateItemMutation, UpdateItemMutationVariables>(
        UpdateItemDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: itemKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: itemKeys.lists() });
    },
  });

  const handleUpdate = async (displayId: number, input: UpdateItemInput) => {
    const result = await mutation.mutateAsync({ displayId, input });
    return result.updateItem;
  };

  return {
    updateItem: handleUpdate,
    item: mutation.data?.updateItem,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useDeleteItem() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeleteItemMutationVariables) =>
      graphqlRequest<DeleteItemMutation, DeleteItemMutationVariables>(
        DeleteItemDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.removeQueries({
        queryKey: itemKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: itemKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleDelete = async (displayId: number) => {
    const result = await mutation.mutateAsync({ displayId });
    return result.deleteItem ?? false;
  };

  return {
    deleteItem: handleDelete,
    loading: mutation.isPending,
    error: mutation.error,
  };
}
