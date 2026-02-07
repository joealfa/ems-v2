import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import { graphqlRequest } from '../graphql/graphql-client';
import { personKeys, dashboardKeys } from '../graphql/query-keys';
import {
  GetPersonsDocument,
  GetPersonDocument,
  CreatePersonDocument,
  UpdatePersonDocument,
  DeletePersonDocument,
  type GetPersonsQuery,
  type GetPersonsQueryVariables,
  type GetPersonQuery,
  type CreatePersonInput,
  type UpdatePersonInput,
  type CreatePersonMutation,
  type CreatePersonMutationVariables,
  type UpdatePersonMutation,
  type UpdatePersonMutationVariables,
  type DeletePersonMutation,
  type DeletePersonMutationVariables,
} from '../graphql/generated/graphql';

export function usePersons(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  fullNameFilter?: string;
  displayIdFilter?: string;
  gender?: string;
  civilStatus?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const query = useQuery({
    queryKey: personKeys.list(variables ?? {}),
    queryFn: () =>
      graphqlRequest<GetPersonsQuery, GetPersonsQueryVariables>(
        GetPersonsDocument,
        variables ?? {}
      ),
  });

  return {
    persons: query.data?.persons?.items ?? [],
    totalCount: query.data?.persons?.totalCount ?? 0,
    pageNumber: query.data?.persons?.pageNumber ?? 1,
    pageSize: query.data?.persons?.pageSize ?? 10,
    totalPages: query.data?.persons?.totalPages ?? 0,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function usePersonsLazy() {
  const fetchPersons = useCallback(
    async (args: { variables: GetPersonsQueryVariables }) => {
      const data = await graphqlRequest<
        GetPersonsQuery,
        GetPersonsQueryVariables
      >(GetPersonsDocument, args.variables);
      return { data };
    },
    []
  );

  return {
    fetchPersons,
    loading: false,
  };
}

export function usePerson(displayId: number) {
  const query = useQuery({
    queryKey: personKeys.detail(displayId),
    queryFn: () =>
      graphqlRequest<GetPersonQuery, { displayId: number }>(GetPersonDocument, {
        displayId,
      }),
    enabled: !!displayId,
  });

  return {
    person: query.data?.person,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useCreatePerson() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: CreatePersonMutationVariables) =>
      graphqlRequest<CreatePersonMutation, CreatePersonMutationVariables>(
        CreatePersonDocument,
        variables
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: personKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleCreate = async (input: CreatePersonInput) => {
    const result = await mutation.mutateAsync({ input });
    return result.createPerson;
  };

  return {
    createPerson: handleCreate,
    person: mutation.data?.createPerson,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useUpdatePerson() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: UpdatePersonMutationVariables) =>
      graphqlRequest<UpdatePersonMutation, UpdatePersonMutationVariables>(
        UpdatePersonDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: personKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: personKeys.lists() });
    },
  });

  const handleUpdate = async (displayId: number, input: UpdatePersonInput) => {
    const result = await mutation.mutateAsync({ displayId, input });
    return result.updatePerson;
  };

  return {
    updatePerson: handleUpdate,
    person: mutation.data?.updatePerson,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useDeletePerson() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeletePersonMutationVariables) =>
      graphqlRequest<DeletePersonMutation, DeletePersonMutationVariables>(
        DeletePersonDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.removeQueries({
        queryKey: personKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: personKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleDelete = async (displayId: number) => {
    const result = await mutation.mutateAsync({ displayId });
    return result.deletePerson ?? false;
  };

  return {
    deletePerson: handleDelete,
    loading: mutation.isPending,
    error: mutation.error,
  };
}
