import { useQuery, useMutation } from '@apollo/client';
import {
  GetPersonDocumentsDocument,
  GetDocumentDocument,
  UpdateDocumentDocument,
  DeleteDocumentDocument,
  DeleteProfileImageDocument,
  type GetPersonDocumentsQuery,
  type GetPersonDocumentsQueryVariables,
  type GetDocumentQuery,
  type GetDocumentQueryVariables,
  type UpdateDocumentMutation,
  type UpdateDocumentMutationVariables,
  type DeleteDocumentMutation,
  type DeleteDocumentMutationVariables,
  type DeleteProfileImageMutation,
  type DeleteProfileImageMutationVariables,
} from '../graphql/generated/graphql';

// Gateway base URL for document proxy endpoints
const GATEWAY_BASE_URL =
  import.meta.env.VITE_GRAPHQL_URL?.replace('/graphql', '') ||
  'http://localhost:5100';

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

// Helper function to get the document download URL through the Gateway
export const getDocumentDownloadUrl = (
  personDisplayId: number,
  documentDisplayId: number
) => {
  return `${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents/${documentDisplayId}/download`;
};

// Helper function to get the profile image URL through the Gateway
export const getProfileImageUrl = (
  personDisplayId: number,
  version?: number
) => {
  const baseUrl = `${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents/profile-image`;
  return version ? `${baseUrl}?v=${version}` : baseUrl;
};

// Helper function to upload a document through the Gateway
export const uploadDocument = async (
  personDisplayId: number,
  file: File,
  description: string | undefined,
  accessToken: string
): Promise<Response> => {
  const formData = new FormData();
  formData.append('file', file);
  if (description) {
    formData.append('description', description);
  }

  return fetch(`${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
    body: formData,
  });
};

// Helper function to upload a profile image through the Gateway
export const uploadProfileImage = async (
  personDisplayId: number,
  file: File,
  accessToken: string
): Promise<Response> => {
  const formData = new FormData();
  formData.append('file', file);

  return fetch(
    `${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents/profile-image`,
    {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
      body: formData,
    }
  );
};

// Helper function to delete a profile image through the Gateway (REST endpoint)
export const deleteProfileImageRest = async (
  personDisplayId: number,
  accessToken: string
): Promise<Response> => {
  return fetch(
    `${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents/profile-image`,
    {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );
};

// Helper function to delete a document through the Gateway (REST endpoint)
export const deleteDocumentRest = async (
  personDisplayId: number,
  documentDisplayId: number,
  accessToken: string
): Promise<Response> => {
  return fetch(
    `${GATEWAY_BASE_URL}/api/persons/${personDisplayId}/documents/${documentDisplayId}`,
    {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );
};
