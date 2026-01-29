import { useQuery, useMutation, useLazyQuery } from '@apollo/client';
import {
  GetSalaryGradesDocument,
  GetSalaryGradeDocument,
  CreateSalaryGradeDocument,
  UpdateSalaryGradeDocument,
  DeleteSalaryGradeDocument,
  type CreateSalaryGradeInput,
  type UpdateSalaryGradeInput,
} from '../graphql/generated/graphql';

/**
 * Hook for fetching paginated salary grades list
 */
export function useSalaryGrades(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const { data, loading, error, refetch } = useQuery(GetSalaryGradesDocument, {
    variables,
    fetchPolicy: 'cache-and-network',
  });

  return {
    salaryGrades: data?.salaryGrades?.items ?? [],
    totalCount: data?.salaryGrades?.totalCount ?? 0,
    pageNumber: data?.salaryGrades?.pageNumber ?? 1,
    pageSize: data?.salaryGrades?.pageSize ?? 10,
    totalPages: data?.salaryGrades?.totalPages ?? 0,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for lazy fetching salary grades (useful for AG Grid infinite scrolling)
 */
export function useSalaryGradesLazy() {
  const [fetchSalaryGrades, { data, loading, error }] = useLazyQuery(
    GetSalaryGradesDocument,
    {
      fetchPolicy: 'network-only',
    }
  );

  return {
    fetchSalaryGrades,
    salaryGrades: data?.salaryGrades?.items ?? [],
    totalCount: data?.salaryGrades?.totalCount ?? 0,
    loading,
    error,
  };
}

/**
 * Hook for fetching a single salary grade by displayId
 */
export function useSalaryGrade(displayId: number) {
  const { data, loading, error, refetch } = useQuery(GetSalaryGradeDocument, {
    variables: { displayId },
    skip: !displayId,
  });

  return {
    salaryGrade: data?.salaryGrade,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for creating a salary grade
 */
export function useCreateSalaryGrade() {
  const [createSalaryGrade, { data, loading, error }] = useMutation(
    CreateSalaryGradeDocument,
    {
      refetchQueries: [GetSalaryGradesDocument],
    }
  );

  const handleCreate = async (input: CreateSalaryGradeInput) => {
    const result = await createSalaryGrade({ variables: { input } });
    return result.data?.createSalaryGrade;
  };

  return {
    createSalaryGrade: handleCreate,
    salaryGrade: data?.createSalaryGrade,
    loading,
    error,
  };
}

/**
 * Hook for updating a salary grade
 */
export function useUpdateSalaryGrade() {
  const [updateSalaryGrade, { data, loading, error }] = useMutation(
    UpdateSalaryGradeDocument
  );

  const handleUpdate = async (
    displayId: number,
    input: UpdateSalaryGradeInput
  ) => {
    const result = await updateSalaryGrade({ variables: { displayId, input } });
    return result.data?.updateSalaryGrade;
  };

  return {
    updateSalaryGrade: handleUpdate,
    salaryGrade: data?.updateSalaryGrade,
    loading,
    error,
  };
}

/**
 * Hook for deleting a salary grade
 */
export function useDeleteSalaryGrade() {
  const [deleteSalaryGrade, { loading, error }] = useMutation(
    DeleteSalaryGradeDocument,
    {
      refetchQueries: [GetSalaryGradesDocument],
    }
  );

  const handleDelete = async (displayId: number) => {
    const result = await deleteSalaryGrade({ variables: { displayId } });
    return result.data?.deleteSalaryGrade ?? false;
  };

  return {
    deleteSalaryGrade: handleDelete,
    loading,
    error,
  };
}
