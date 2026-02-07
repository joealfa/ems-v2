import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import { graphqlRequest } from '../graphql/graphql-client';
import { schoolKeys, dashboardKeys } from '../graphql/query-keys';
import {
  GetSchoolsDocument,
  GetSchoolDocument,
  CreateSchoolDocument,
  UpdateSchoolDocument,
  DeleteSchoolDocument,
  type GetSchoolsQuery,
  type GetSchoolsQueryVariables,
  type GetSchoolQuery,
  type CreateSchoolInput,
  type UpdateSchoolInput,
  type CreateSchoolMutation,
  type CreateSchoolMutationVariables,
  type UpdateSchoolMutation,
  type UpdateSchoolMutationVariables,
  type DeleteSchoolMutation,
  type DeleteSchoolMutationVariables,
} from '../graphql/generated/graphql';

export function useSchools(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const query = useQuery({
    queryKey: schoolKeys.list(variables ?? {}),
    queryFn: () =>
      graphqlRequest<GetSchoolsQuery, GetSchoolsQueryVariables>(
        GetSchoolsDocument,
        variables ?? {}
      ),
  });

  return {
    schools: query.data?.schools?.items ?? [],
    totalCount: query.data?.schools?.totalCount ?? 0,
    pageNumber: query.data?.schools?.pageNumber ?? 1,
    pageSize: query.data?.schools?.pageSize ?? 10,
    totalPages: query.data?.schools?.totalPages ?? 0,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useSchoolsLazy() {
  const fetchSchools = useCallback(
    async (args: { variables: GetSchoolsQueryVariables }) => {
      const data = await graphqlRequest<
        GetSchoolsQuery,
        GetSchoolsQueryVariables
      >(GetSchoolsDocument, args.variables);
      return { data };
    },
    []
  );

  return {
    fetchSchools,
    loading: false,
  };
}

export function useSchool(displayId: number) {
  const query = useQuery({
    queryKey: schoolKeys.detail(displayId),
    queryFn: () =>
      graphqlRequest<GetSchoolQuery, { displayId: number }>(GetSchoolDocument, {
        displayId,
      }),
    enabled: !!displayId,
  });

  return {
    school: query.data?.school,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useCreateSchool() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: CreateSchoolMutationVariables) =>
      graphqlRequest<CreateSchoolMutation, CreateSchoolMutationVariables>(
        CreateSchoolDocument,
        variables
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: schoolKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleCreate = async (input: CreateSchoolInput) => {
    const result = await mutation.mutateAsync({ input });
    return result.createSchool;
  };

  return {
    createSchool: handleCreate,
    school: mutation.data?.createSchool,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useUpdateSchool() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: UpdateSchoolMutationVariables) =>
      graphqlRequest<UpdateSchoolMutation, UpdateSchoolMutationVariables>(
        UpdateSchoolDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: schoolKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: schoolKeys.lists() });
    },
  });

  const handleUpdate = async (displayId: number, input: UpdateSchoolInput) => {
    const result = await mutation.mutateAsync({ displayId, input });
    return result.updateSchool;
  };

  return {
    updateSchool: handleUpdate,
    school: mutation.data?.updateSchool,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useDeleteSchool() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeleteSchoolMutationVariables) =>
      graphqlRequest<DeleteSchoolMutation, DeleteSchoolMutationVariables>(
        DeleteSchoolDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.removeQueries({
        queryKey: schoolKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: schoolKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleDelete = async (displayId: number) => {
    const result = await mutation.mutateAsync({ displayId });
    return result.deleteSchool ?? false;
  };

  return {
    deleteSchool: handleDelete,
    loading: mutation.isPending,
    error: mutation.error,
  };
}
