import { useQuery, useMutation, useLazyQuery } from '@apollo/client';
import {
  GetPersonsDocument,
  GetPersonDocument,
  CreatePersonDocument,
  UpdatePersonDocument,
  DeletePersonDocument,
  type CreatePersonInput,
  type UpdatePersonInput,
} from '../graphql/generated/graphql';

/**
 * Hook for fetching paginated persons list
 */
export function usePersons(variables?: {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  fullNameFilter?: string;
  displayIdFilter?: string;
  gender?: number;
  civilStatus?: number;
  sortBy?: string;
  sortDescending?: boolean;
}) {
  const { data, loading, error, refetch } = useQuery(GetPersonsDocument, {
    variables,
    fetchPolicy: 'cache-and-network',
  });

  return {
    persons: data?.persons?.items ?? [],
    totalCount: data?.persons?.totalCount ?? 0,
    pageNumber: data?.persons?.pageNumber ?? 1,
    pageSize: data?.persons?.pageSize ?? 10,
    totalPages: data?.persons?.totalPages ?? 0,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for lazy fetching persons (useful for AG Grid infinite scrolling)
 */
export function usePersonsLazy() {
  const [fetchPersons, { data, loading, error }] = useLazyQuery(
    GetPersonsDocument,
    {
      fetchPolicy: 'network-only',
    }
  );

  return {
    fetchPersons,
    persons: data?.persons?.items ?? [],
    totalCount: data?.persons?.totalCount ?? 0,
    loading,
    error,
  };
}

/**
 * Hook for fetching a single person by displayId
 */
export function usePerson(displayId: number) {
  const { data, loading, error, refetch } = useQuery(GetPersonDocument, {
    variables: { displayId },
    skip: !displayId,
  });

  return {
    person: data?.person,
    loading,
    error,
    refetch,
  };
}

/**
 * Hook for creating a person
 */
export function useCreatePerson() {
  const [createPerson, { data, loading, error }] = useMutation(
    CreatePersonDocument,
    {
      refetchQueries: [GetPersonsDocument],
    }
  );

  const handleCreate = async (input: CreatePersonInput) => {
    const result = await createPerson({ variables: { input } });
    return result.data?.createPerson;
  };

  return {
    createPerson: handleCreate,
    person: data?.createPerson,
    loading,
    error,
  };
}

/**
 * Hook for updating a person
 */
export function useUpdatePerson() {
  const [updatePerson, { data, loading, error }] =
    useMutation(UpdatePersonDocument);

  const handleUpdate = async (displayId: number, input: UpdatePersonInput) => {
    const result = await updatePerson({ variables: { displayId, input } });
    return result.data?.updatePerson;
  };

  return {
    updatePerson: handleUpdate,
    person: data?.updatePerson,
    loading,
    error,
  };
}

/**
 * Hook for deleting a person
 */
export function useDeletePerson() {
  const [deletePerson, { loading, error }] = useMutation(DeletePersonDocument, {
    refetchQueries: [GetPersonsDocument],
  });

  const handleDelete = async (displayId: number) => {
    const result = await deletePerson({ variables: { displayId } });
    return result.data?.deletePerson ?? false;
  };

  return {
    deletePerson: handleDelete,
    loading,
    error,
  };
}
