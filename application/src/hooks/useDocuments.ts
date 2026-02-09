import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { graphqlRequest } from '../graphql/graphql-client';
import { documentKeys } from '../graphql/query-keys';
import {
  GetPersonDocumentsDocument,
  GetDocumentDocument,
  GetProfileImageUrlDocument,
  UpdateDocumentDocument,
  DeleteDocumentDocument,
  DeleteProfileImageDocument,
  type GetPersonDocumentsQuery,
  type GetPersonDocumentsQueryVariables,
  type GetDocumentQuery,
  type GetDocumentQueryVariables,
  type GetProfileImageUrlQuery,
  type GetProfileImageUrlQueryVariables,
  type UpdateDocumentMutation,
  type UpdateDocumentMutationVariables,
  type DeleteDocumentMutation,
  type DeleteDocumentMutationVariables,
  type DeleteProfileImageMutation,
  type DeleteProfileImageMutationVariables,
} from '../graphql/generated/graphql';

const GRAPHQL_URL =
  import.meta.env.VITE_GRAPHQL_URL || 'https://localhost:5003/graphql';

const GATEWAY_BASE_URL =
  import.meta.env.VITE_GRAPHQL_URL?.replace('/graphql', '') ||
  'https://localhost:5003';

const performUpload = async (
  query: string,
  variables: Record<string, unknown>,
  fileFieldPath: string,
  file: File
) => {
  const variablesForOp = { ...variables, [fileFieldPath]: null };

  const formData = new FormData();
  formData.append(
    'operations',
    JSON.stringify({ query, variables: variablesForOp })
  );
  formData.append(
    'map',
    JSON.stringify({ '0': [`variables.${fileFieldPath}`] })
  );
  formData.append('0', file);

  const response = await fetch(GRAPHQL_URL, {
    method: 'POST',
    headers: {
      'GraphQL-Preflight': '1',
    },
    credentials: 'include',
    body: formData,
  });

  const result = await response.json();
  if (result.errors) {
    throw new Error(result.errors[0].message);
  }
  return result.data;
};

export const usePersonDocuments = (
  personDisplayId: number,
  options?: {
    pageNumber?: number;
    pageSize?: number;
    searchTerm?: string;
    sortBy?: string;
    sortDescending?: boolean;
  }
) => {
  const variables: GetPersonDocumentsQueryVariables = {
    personDisplayId,
    pageNumber: options?.pageNumber ?? 1,
    pageSize: options?.pageSize ?? 100,
    searchTerm: options?.searchTerm,
    sortBy: options?.sortBy,
    sortDescending: options?.sortDescending,
  };

  const query = useQuery({
    queryKey: documentKeys.list(personDisplayId, variables),
    queryFn: () =>
      graphqlRequest<GetPersonDocumentsQuery, GetPersonDocumentsQueryVariables>(
        GetPersonDocumentsDocument,
        variables
      ),
    enabled: !!personDisplayId,
  });

  return {
    documents: query.data?.personDocuments?.items ?? [],
    totalCount: query.data?.personDocuments?.totalCount ?? 0,
    loading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
};

export const useDocument = (
  personDisplayId: number,
  documentDisplayId: number
) => {
  const query = useQuery({
    queryKey: documentKeys.detail(personDisplayId, documentDisplayId),
    queryFn: () =>
      graphqlRequest<GetDocumentQuery, GetDocumentQueryVariables>(
        GetDocumentDocument,
        { personDisplayId, documentDisplayId }
      ),
    enabled: !!documentDisplayId,
  });

  return {
    document: query.data?.document,
    loading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
};

export const useUpdateDocument = () => {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: UpdateDocumentMutationVariables) =>
      graphqlRequest<UpdateDocumentMutation, UpdateDocumentMutationVariables>(
        UpdateDocumentDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: documentKeys.list(variables.personDisplayId),
      });
    },
  });

  return {
    updateDocument: async (
      personDisplayId: number,
      documentDisplayId: number,
      description?: string
    ) => {
      const result = await mutation.mutateAsync({
        personDisplayId,
        documentDisplayId,
        description,
      });
      return result.updateDocument;
    },
    updating: mutation.isPending,
    error: mutation.error,
  };
};

export const useDeleteDocument = () => {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeleteDocumentMutationVariables) =>
      graphqlRequest<DeleteDocumentMutation, DeleteDocumentMutationVariables>(
        DeleteDocumentDocument,
        variables
      ),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: documentKeys.list(variables.personDisplayId),
      });
    },
  });

  return {
    deleteDocument: async (
      personDisplayId: number,
      documentDisplayId: number
    ) => {
      const result = await mutation.mutateAsync({
        personDisplayId,
        documentDisplayId,
      });
      return result.deleteDocument;
    },
    deleting: mutation.isPending,
    error: mutation.error,
  };
};

export const useDeleteProfileImage = () => {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: DeleteProfileImageMutationVariables) =>
      graphqlRequest<
        DeleteProfileImageMutation,
        DeleteProfileImageMutationVariables
      >(DeleteProfileImageDocument, variables),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: documentKeys.profileImage(variables.personDisplayId),
      });
    },
  });

  return {
    deleteProfileImage: async (personDisplayId: number) => {
      const result = await mutation.mutateAsync({ personDisplayId });
      return result.deleteProfileImage;
    },
    deleting: mutation.isPending,
    error: mutation.error,
  };
};

export const useProfileImageUrl = (personDisplayId: number) => {
  const query = useQuery({
    queryKey: documentKeys.profileImage(personDisplayId),
    queryFn: () =>
      graphqlRequest<GetProfileImageUrlQuery, GetProfileImageUrlQueryVariables>(
        GetProfileImageUrlDocument,
        { personDisplayId }
      ),
    enabled: !!personDisplayId,
    staleTime: 5 * 60 * 1000,
  });

  return {
    profileImageUrl: query.data?.profileImageUrl,
    loading: query.isLoading,
    error: query.error,
  };
};

export const useUploadDocument = () => {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: async ({
      personDisplayId,
      file,
      description,
    }: {
      personDisplayId: number;
      file: File;
      description?: string;
    }) => {
      return performUpload(
        `mutation UploadDocument($personDisplayId: Long!, $file: Upload!, $description: String) {
          uploadDocument(personDisplayId: $personDisplayId, file: $file, description: $description) {
            displayId
            fileName
            fileSizeBytes
            contentType
            description
            createdOn
            createdBy
          }
        }`,
        { personDisplayId, description },
        'file',
        file
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: documentKeys.list(variables.personDisplayId),
      });
    },
  });

  return {
    uploadDocument: async (
      personDisplayId: number,
      file: File,
      description?: string
    ) => {
      const result = await mutation.mutateAsync({
        personDisplayId,
        file,
        description,
      });
      return result.uploadDocument;
    },
    uploading: mutation.isPending,
    error: mutation.error,
  };
};

export const useUploadProfileImage = () => {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: async ({
      personDisplayId,
      file,
    }: {
      personDisplayId: number;
      file: File;
    }) => {
      return performUpload(
        `mutation UploadProfileImage($personDisplayId: Long!, $file: Upload!) {
          uploadProfileImage(personDisplayId: $personDisplayId, file: $file)
        }`,
        { personDisplayId },
        'file',
        file
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: documentKeys.profileImage(variables.personDisplayId),
      });
    },
  });

  return {
    uploadProfileImage: async (personDisplayId: number, file: File) => {
      const result = await mutation.mutateAsync({ personDisplayId, file });
      return result.uploadProfileImage;
    },
    uploading: mutation.isPending,
    error: mutation.error,
  };
};

export const getDocumentDownloadUrl = (
  personDisplayId: number,
  documentDisplayId: number
) => {
  return `${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents/${documentDisplayId}/download`;
};
