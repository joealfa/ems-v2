import { useQuery, useMutation, useLazyQuery } from '@apollo/client';
import {
  GetEmploymentsDocument,
  GetEmploymentDocument,
  CreateEmploymentDocument,
  UpdateEmploymentDocument,
  DeleteEmploymentDocument,
  type CreateEmploymentInput,
  type UpdateEmploymentInput,
} from '../graphql/generated/graphql';

/**
 * Hook for fetching paginated employments list
 */
export function useEmployments(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  displayIdFilter?: string;
  employeeNameFilter?: string;
  positionFilter?: string;
  depEdIdFilter?: string;
  employmentStatus?: number;
  isActive?: boolean;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const { data, loading, error, refetch } = useQuery(GetEmploymentsDocument, {
    variables,
    fetchPolicy: 'cache-and-network',
  });

  return {
    employments: data?.employments?.items ?? [],
    totalCount: data?.employments?.totalCount ?? 0,
    pageNumber: data?.employments?.pageNumber ?? 1,
    pageSize: data?.employments?.pageSize ?? 10,
    totalPages: data?.employments?.totalPages ?? 0,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for lazy fetching employments (useful for AG Grid infinite scrolling)
 */
export function useEmploymentsLazy() {
  const [fetchEmployments, { data, loading, error }] = useLazyQuery(
    GetEmploymentsDocument,
    {
      fetchPolicy: 'network-only',
    }
  );

  return {
    fetchEmployments,
    employments: data?.employments?.items ?? [],
    totalCount: data?.employments?.totalCount ?? 0,
    loading,
    error,
  };
}

/**
 * Hook for fetching a single employment by displayId
 */
export function useEmployment(displayId: number) {
  const { data, loading, error, refetch } = useQuery(GetEmploymentDocument, {
    variables: { displayId },
    skip: !displayId,
  });

  return {
    employment: data?.employment,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for creating an employment
 */
export function useCreateEmployment() {
  const [createEmployment, { data, loading, error }] = useMutation(
    CreateEmploymentDocument,
    {
      refetchQueries: [GetEmploymentsDocument],
    }
  );

  const handleCreate = async (input: CreateEmploymentInput) => {
    const result = await createEmployment({ variables: { input } });
    return result.data?.createEmployment;
  };

  return {
    createEmployment: handleCreate,
    employment: data?.createEmployment,
    loading,
    error,
  };
}

/**
 * Hook for updating an employment
 */
export function useUpdateEmployment() {
  const [updateEmployment, { data, loading, error }] = useMutation(
    UpdateEmploymentDocument
  );

  const handleUpdate = async (
    displayId: number,
    input: UpdateEmploymentInput
  ) => {
    const result = await updateEmployment({ variables: { displayId, input } });
    return result.data?.updateEmployment;
  };

  return {
    updateEmployment: handleUpdate,
    employment: data?.updateEmployment,
    loading,
    error,
  };
}

/**
 * Hook for deleting an employment
 */
export function useDeleteEmployment() {
  const [deleteEmployment, { loading, error }] = useMutation(
    DeleteEmploymentDocument,
    {
      refetchQueries: [GetEmploymentsDocument],
    }
  );

  const handleDelete = async (displayId: number) => {
    const result = await deleteEmployment({ variables: { displayId } });
    return result.data?.deleteEmployment ?? false;
  };

  return {
    deleteEmployment: handleDelete,
    loading,
    error,
  };
}
