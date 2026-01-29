import { useQuery, useMutation, useLazyQuery } from '@apollo/client';
import {
  GetSchoolsDocument,
  GetSchoolDocument,
  CreateSchoolDocument,
  UpdateSchoolDocument,
  DeleteSchoolDocument,
  type CreateSchoolInput,
  type UpdateSchoolInput,
} from '../graphql/generated/graphql';

/**
 * Hook for fetching paginated schools list
 */
export function useSchools(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const { data, loading, error, refetch } = useQuery(GetSchoolsDocument, {
    variables,
    fetchPolicy: 'cache-and-network',
  });

  return {
    schools: data?.schools?.items ?? [],
    totalCount: data?.schools?.totalCount ?? 0,
    pageNumber: data?.schools?.pageNumber ?? 1,
    pageSize: data?.schools?.pageSize ?? 10,
    totalPages: data?.schools?.totalPages ?? 0,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for lazy fetching schools (useful for AG Grid infinite scrolling)
 */
export function useSchoolsLazy() {
  const [fetchSchools, { data, loading, error }] = useLazyQuery(
    GetSchoolsDocument,
    {
      fetchPolicy: 'network-only',
    }
  );

  return {
    fetchSchools,
    schools: data?.schools?.items ?? [],
    totalCount: data?.schools?.totalCount ?? 0,
    loading,
    error,
  };
}

/**
 * Hook for fetching a single school by displayId
 */
export function useSchool(displayId: number) {
  const { data, loading, error, refetch } = useQuery(GetSchoolDocument, {
    variables: { displayId },
    skip: !displayId,
  });

  return {
    school: data?.school,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for creating a school
 */
export function useCreateSchool() {
  const [createSchool, { data, loading, error }] = useMutation(
    CreateSchoolDocument,
    {
      refetchQueries: [GetSchoolsDocument],
    }
  );

  const handleCreate = async (input: CreateSchoolInput) => {
    const result = await createSchool({ variables: { input } });
    return result.data?.createSchool;
  };

  return {
    createSchool: handleCreate,
    school: data?.createSchool,
    loading,
    error,
  };
}

/**
 * Hook for updating a school
 */
export function useUpdateSchool() {
  const [updateSchool, { data, loading, error }] =
    useMutation(UpdateSchoolDocument);

  const handleUpdate = async (displayId: number, input: UpdateSchoolInput) => {
    const result = await updateSchool({ variables: { displayId, input } });
    return result.data?.updateSchool;
  };

  return {
    updateSchool: handleUpdate,
    school: data?.updateSchool,
    loading,
    error,
  };
}

/**
 * Hook for deleting a school
 */
export function useDeleteSchool() {
  const [deleteSchool, { loading, error }] = useMutation(DeleteSchoolDocument, {
    refetchQueries: [GetSchoolsDocument],
  });

  const handleDelete = async (displayId: number) => {
    const result = await deleteSchool({ variables: { displayId } });
    return result.data?.deleteSchool ?? false;
  };

  return {
    deleteSchool: handleDelete,
    loading,
    error,
  };
}
