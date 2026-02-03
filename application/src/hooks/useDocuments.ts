import { useQuery, useMutation } from '@apollo/client';
import {
  GetPersonDocumentsDocument,
  GetDocumentDocument,
  GetProfileImageUrlDocument,
  UploadDocumentDocument,
  UploadProfileImageDocument,
  UpdateDocumentDocument,
  DeleteDocumentDocument,
  DeleteProfileImageDocument,
  type GetPersonDocumentsQuery,
  type GetPersonDocumentsQueryVariables,
  type GetDocumentQuery,
  type GetDocumentQueryVariables,
  type GetProfileImageUrlQuery,
  type GetProfileImageUrlQueryVariables,
  type UploadDocumentMutation,
  type UploadDocumentMutationVariables,
  type UploadProfileImageMutation,
  type UploadProfileImageMutationVariables,
  type UpdateDocumentMutation,
  type UpdateDocumentMutationVariables,
  type DeleteDocumentMutation,
  type DeleteDocumentMutationVariables,
  type DeleteProfileImageMutation,
  type DeleteProfileImageMutationVariables,
} from '../graphql/generated/graphql';

// Gateway base URL for REST file download endpoints (GraphQL doesn't handle file streaming)
const GATEWAY_BASE_URL =
  import.meta.env.VITE_GRAPHQL_URL?.replace('/graphql', '') ||
  'https://localhost:5003';

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
  const { data, loading, error, refetch } = useQuery<
    GetPersonDocumentsQuery,
    GetPersonDocumentsQueryVariables
  >(GetPersonDocumentsDocument, {
    variables: {
      personDisplayId,
      pageNumber: options?.pageNumber ?? 1,
      pageSize: options?.pageSize ?? 100,
      searchTerm: options?.searchTerm,
      sortBy: options?.sortBy,
      sortDescending: options?.sortDescending,
    },
    fetchPolicy: 'cache-and-network',
  });

  return {
    documents: data?.personDocuments?.items ?? [],
    totalCount: data?.personDocuments?.totalCount ?? 0,
    loading,
    error,
    refetch,
  };
};

export const useDocument = (
  personDisplayId: number,
  documentDisplayId: number
) => {
  const { data, loading, error, refetch } = useQuery<
    GetDocumentQuery,
    GetDocumentQueryVariables
  >(GetDocumentDocument, {
    variables: { personDisplayId, documentDisplayId },
    skip: !documentDisplayId,
  });

  return {
    document: data?.document,
    loading,
    error,
    refetch,
  };
};

export const useUpdateDocument = () => {
  const [updateDocument, { loading, error }] = useMutation<
    UpdateDocumentMutation,
    UpdateDocumentMutationVariables
  >(UpdateDocumentDocument);

  return {
    updateDocument: async (
      personDisplayId: number,
      documentDisplayId: number,
      description?: string
    ) => {
      const result = await updateDocument({
        variables: { personDisplayId, documentDisplayId, description },
      });
      return result.data?.updateDocument;
    },
    updating: loading,
    error,
  };
};

export const useDeleteDocument = () => {
  const [deleteDocument, { loading, error }] = useMutation<
    DeleteDocumentMutation,
    DeleteDocumentMutationVariables
  >(DeleteDocumentDocument);

  return {
    deleteDocument: async (
      personDisplayId: number,
      documentDisplayId: number
    ) => {
      const result = await deleteDocument({
        variables: { personDisplayId, documentDisplayId },
        refetchQueries: [
          {
            query: GetPersonDocumentsDocument,
            variables: { personDisplayId, pageNumber: 1, pageSize: 100 },
          },
        ],
      });
      return result.data?.deleteDocument;
    },
    deleting: loading,
    error,
  };
};

export const useDeleteProfileImage = () => {
  const [deleteProfileImage, { loading, error }] = useMutation<
    DeleteProfileImageMutation,
    DeleteProfileImageMutationVariables
  >(DeleteProfileImageDocument);

  return {
    deleteProfileImage: async (personDisplayId: number) => {
      const result = await deleteProfileImage({
        variables: { personDisplayId },
      });
      return result.data?.deleteProfileImage;
    },
    deleting: loading,
    error,
  };
};

// Hook to get profile image URL
export const useProfileImageUrl = (personDisplayId: number) => {
  const { data, loading, error } = useQuery<
    GetProfileImageUrlQuery,
    GetProfileImageUrlQueryVariables
  >(GetProfileImageUrlDocument, {
    variables: { personDisplayId },
    skip: !personDisplayId,
  });

  return {
    profileImageUrl: data?.profileImageUrl,
    loading,
    error,
  };
};

// Hook to upload a document
export const useUploadDocument = () => {
  const [uploadDocument, { loading, error }] = useMutation<
    UploadDocumentMutation,
    UploadDocumentMutationVariables
  >(UploadDocumentDocument);

  return {
    uploadDocument: async (
      personDisplayId: number,
      file: File,
      description?: string
    ) => {
      const result = await uploadDocument({
        variables: { personDisplayId, file, description },
        refetchQueries: [
          {
            query: GetPersonDocumentsDocument,
            variables: { personDisplayId, pageNumber: 1, pageSize: 100 },
          },
        ],
      });
      return result.data?.uploadDocument;
    },
    uploading: loading,
    error,
  };
};

// Hook to upload a profile image
export const useUploadProfileImage = () => {
  const [uploadProfileImage, { loading, error }] = useMutation<
    UploadProfileImageMutation,
    UploadProfileImageMutationVariables
  >(UploadProfileImageDocument);

  return {
    uploadProfileImage: async (personDisplayId: number, file: File) => {
      const result = await uploadProfileImage({
        variables: { personDisplayId, file },
        refetchQueries: [
          {
            query: GetProfileImageUrlDocument,
            variables: { personDisplayId },
          },
        ],
      });
      return result.data?.uploadProfileImage;
    },
    uploading: loading,
    error,
  };
};

// Helper function to get the document download URL (still uses REST endpoint for file streaming)
export const getDocumentDownloadUrl = (
  personDisplayId: number,
  documentDisplayId: number
) => {
  return `${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents/${documentDisplayId}/download`;
};
