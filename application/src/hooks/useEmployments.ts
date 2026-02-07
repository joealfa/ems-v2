import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import { graphqlRequest } from '../graphql/graphql-client';
import { employmentKeys, dashboardKeys } from '../graphql/query-keys';
import {
  GetEmploymentsDocument,
  GetEmploymentDocument,
  CreateEmploymentDocument,
  UpdateEmploymentDocument,
  DeleteEmploymentDocument,
  AddSchoolToEmploymentDocument,
  RemoveSchoolFromEmploymentDocument,
  type GetEmploymentsQuery,
  type GetEmploymentsQueryVariables,
  type GetEmploymentQuery,
  type CreateEmploymentInput,
  type UpdateEmploymentInput,
  type CreateEmploymentSchoolInput,
  type CreateEmploymentMutation,
  type CreateEmploymentMutationVariables,
  type UpdateEmploymentMutation,
  type UpdateEmploymentMutationVariables,
  type DeleteEmploymentMutation,
  type DeleteEmploymentMutationVariables,
  type AddSchoolToEmploymentMutation,
  type AddSchoolToEmploymentMutationVariables,
  type RemoveSchoolFromEmploymentMutation,
  type RemoveSchoolFromEmploymentMutationVariables,
} from '../graphql/generated/graphql';

export function useEmployments(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  displayIdFilter?: string;
  employeeNameFilter?: string;
  positionFilter?: string;
  depEdIdFilter?: string;
  employmentStatus?: string;
  isActive?: boolean;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const query = useQuery({
    queryKey: employmentKeys.list(variables ?? {}),
    queryFn: () =>
      graphqlRequest<GetEmploymentsQuery, GetEmploymentsQueryVariables>(
        GetEmploymentsDocument,
        variables ?? {}
      ),
  });

  return {
    employments: query.data?.employments?.items ?? [],
    totalCount: query.data?.employments?.totalCount ?? 0,
    pageNumber: query.data?.employments?.pageNumber ?? 1,
    pageSize: query.data?.employments?.pageSize ?? 10,
    totalPages: query.data?.employments?.totalPages ?? 0,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useEmploymentsLazy() {
  const fetchEmployments = useCallback(
    async (args: { variables: GetEmploymentsQueryVariables }) => {
      const data = await graphqlRequest<
        GetEmploymentsQuery,
        GetEmploymentsQueryVariables
      >(GetEmploymentsDocument, args.variables);
      return { data };
    },
    []
  );

  return {
    fetchEmployments,
    loading: false,
  };
}

export function useEmployment(displayId: number) {
  const query = useQuery({
    queryKey: employmentKeys.detail(displayId),
    queryFn: () =>
      graphqlRequest<GetEmploymentQuery, { displayId: number }>(
        GetEmploymentDocument,
        { displayId }
      ),
    enabled: !!displayId,
  });

  return {
    employment: query.data?.employment,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}

export function useCreateEmployment() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: CreateEmploymentMutationVariables) =>
      graphqlRequest<
        CreateEmploymentMutation,
        CreateEmploymentMutationVariables
      >(CreateEmploymentDocument, variables),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: employmentKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleCreate = async (input: CreateEmploymentInput) => {
    const result = await mutation.mutateAsync({ input });
    return result.createEmployment;
  };

  return {
    createEmployment: handleCreate,
    employment: mutation.data?.createEmployment,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useUpdateEmployment() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: UpdateEmploymentMutationVariables) =>
      graphqlRequest<
        UpdateEmploymentMutation,
        UpdateEmploymentMutationVariables
      >(UpdateEmploymentDocument, variables),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: employmentKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: employmentKeys.lists() });
    },
  });

  const handleUpdate = async (
    displayId: number,
    input: UpdateEmploymentInput
  ) => {
    const result = await mutation.mutateAsync({ displayId, input });
    return result.updateEmployment;
  };

  return {
    updateEmployment: handleUpdate,
    employment: mutation.data?.updateEmployment,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useDeleteEmployment() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeleteEmploymentMutationVariables) =>
      graphqlRequest<
        DeleteEmploymentMutation,
        DeleteEmploymentMutationVariables
      >(DeleteEmploymentDocument, variables),
    onSuccess: (_data, variables) => {
      queryClient.removeQueries({
        queryKey: employmentKeys.detail(variables.displayId),
      });
      queryClient.invalidateQueries({ queryKey: employmentKeys.lists() });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
    },
  });

  const handleDelete = async (displayId: number) => {
    const result = await mutation.mutateAsync({ displayId });
    return result.deleteEmployment ?? false;
  };

  return {
    deleteEmployment: handleDelete,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useAddSchoolToEmployment() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: AddSchoolToEmploymentMutationVariables) =>
      graphqlRequest<
        AddSchoolToEmploymentMutation,
        AddSchoolToEmploymentMutationVariables
      >(AddSchoolToEmploymentDocument, variables),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: employmentKeys.detail(variables.employmentDisplayId),
      });
    },
  });

  const handleAdd = async (
    employmentDisplayId: number,
    input: CreateEmploymentSchoolInput
  ) => {
    const result = await mutation.mutateAsync({
      employmentDisplayId,
      input,
    });
    return result.addSchoolToEmployment;
  };

  return {
    addSchoolToEmployment: handleAdd,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useRemoveSchoolFromEmployment() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: RemoveSchoolFromEmploymentMutationVariables) =>
      graphqlRequest<
        RemoveSchoolFromEmploymentMutation,
        RemoveSchoolFromEmploymentMutationVariables
      >(RemoveSchoolFromEmploymentDocument, variables),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: employmentKeys.detail(variables.employmentDisplayId),
      });
    },
  });

  const handleRemove = async (
    employmentDisplayId: number,
    schoolAssignmentDisplayId: number
  ) => {
    const result = await mutation.mutateAsync({
      employmentDisplayId,
      schoolAssignmentDisplayId,
    });
    return result.removeSchoolFromEmployment ?? false;
  };

  return {
    removeSchoolFromEmployment: handleRemove,
    loading: mutation.isPending,
    error: mutation.error,
  };
}
