import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import { graphqlRequest } from '../graphql/graphql-client';
import { salaryGradeKeys, dashboardKeys } from '../graphql/query-keys';
import {
  GetSalaryGradesDocument,
  GetSalaryGradeDocument,
  CreateSalaryGradeDocument,
  UpdateSalaryGradeDocument,
  DeleteSalaryGradeDocument,
  type GetSalaryGradesQuery,
  type GetSalaryGradesQueryVariables,
  type GetSalaryGradeQuery,
  type CreateSalaryGradeInput,
  type UpdateSalaryGradeInput,
  type CreateSalaryGradeMutation,
  type CreateSalaryGradeMutationVariables,
  type UpdateSalaryGradeMutation,
  type UpdateSalaryGradeMutationVariables,
  type DeleteSalaryGradeMutation,
  type DeleteSalaryGradeMutationVariables,
} from '../graphql/generated/graphql';

export function useSalaryGrades(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const query = useQuery({
    queryKey: salaryGradeKeys.list(variables ?? {}),
    queryFn: () =>
      graphqlRequest<GetSalaryGradesQuery, GetSalaryGradesQueryVariables>(
        GetSalaryGradesDocument,
        variables ?? {}
      ),
  });

  return {
    salaryGrades: query.data?.salaryGrades?.items ?? [],
    totalCount: query.data?.salaryGrades?.totalCount ?? 0,
    pageNumber: query.data?.salaryGrades?.pageNumber ?? 1,
    pageSize: query.data?.salaryGrades?.pageSize ?? 10,
    totalPages: query.data?.salaryGrades?.totalPages ?? 0,
    loading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useSalaryGradesLazy() {
  const fetchSalaryGrades = useCallback(
    async (args: { variables: GetSalaryGradesQueryVariables }) => {
      const data = await graphqlRequest<
        GetSalaryGradesQuery,
        GetSalaryGradesQueryVariables
      >(GetSalaryGradesDocument, args.variables);
      return { data };
    },
    []
  );

  return {
    fetchSalaryGrades,
    loading: false,
  };
}

export function useSalaryGrade(displayId: number) {
  const query = useQuery({
    queryKey: salaryGradeKeys.detail(displayId),
    queryFn: () =>
      graphqlRequest<GetSalaryGradeQuery, { displayId: number }>(
        GetSalaryGradeDocument,
        { displayId }
      ),
    enabled: !!displayId,
  });

  return {
    salaryGrade: query.data?.salaryGrade,
    loading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useCreateSalaryGrade() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: CreateSalaryGradeMutationVariables) =>
      graphqlRequest<
        CreateSalaryGradeMutation,
        CreateSalaryGradeMutationVariables
      >(CreateSalaryGradeDocument, variables),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: salaryGradeKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleCreate = async (input: CreateSalaryGradeInput) => {
    const result = await mutation.mutateAsync({ input });
    return result.createSalaryGrade;
  };

  return {
    createSalaryGrade: handleCreate,
    salaryGrade: mutation.data?.createSalaryGrade,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useUpdateSalaryGrade() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: UpdateSalaryGradeMutationVariables) =>
      graphqlRequest<
        UpdateSalaryGradeMutation,
        UpdateSalaryGradeMutationVariables
      >(UpdateSalaryGradeDocument, variables),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: salaryGradeKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: salaryGradeKeys.lists() });
    },
  });

  const handleUpdate = async (
    displayId: number,
    input: UpdateSalaryGradeInput
  ) => {
    const result = await mutation.mutateAsync({ displayId, input });
    return result.updateSalaryGrade;
  };

  return {
    updateSalaryGrade: handleUpdate,
    salaryGrade: mutation.data?.updateSalaryGrade,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useDeleteSalaryGrade() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeleteSalaryGradeMutationVariables) =>
      graphqlRequest<
        DeleteSalaryGradeMutation,
        DeleteSalaryGradeMutationVariables
      >(DeleteSalaryGradeDocument, variables),
    onSuccess: (_data, variables) => {
      queryClient.removeQueries({
        queryKey: salaryGradeKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: salaryGradeKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleDelete = async (displayId: number) => {
    const result = await mutation.mutateAsync({ displayId });
    return result.deleteSalaryGrade ?? false;
  };

  return {
    deleteSalaryGrade: handleDelete,
    loading: mutation.isPending,
    error: mutation.error,
  };
}
