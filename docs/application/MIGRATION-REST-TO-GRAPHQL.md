# Migration Guide: REST to GraphQL Document Operations

## Overview
Document operations (upload, download, delete) have been migrated from REST endpoints to GraphQL mutations and queries. This guide will help you update your frontend code.

## What Changed

### Before (REST)
```typescript
// Old REST endpoints
POST   /api/persons/{personId}/documents
GET    /api/persons/{personId}/documents/{documentId}/download
DELETE /api/persons/{personId}/documents/{documentId}
POST   /api/persons/{personId}/documents/profile-image
GET    /api/persons/{personId}/documents/profile-image
DELETE /api/persons/{personId}/documents/profile-image
```

### After (GraphQL)
```graphql
# Mutations
mutation UploadDocument($personId: Long!, $file: Upload!, $description: String)
mutation UpdateDocument($personId: Long!, $documentId: Long!, $input: UpdateDocumentInput!)
mutation DeleteDocument($personId: Long!, $documentId: Long!)
mutation UploadProfileImage($personId: Long!, $file: Upload!)
mutation DeleteProfileImage($personId: Long!)

# Queries
query GetPersonDocuments($personId: Long!, $pageNumber: Int, $pageSize: Int)
query GetDocument($personId: Long!, $documentId: Long!)
query GetProfileImageUrl($personId: Long!)
```

## Step-by-Step Migration

### 1. Create GraphQL Operations

Create new `.graphql` files in `application/src/graphql/operations/`:

**`documents.graphql`**:
```graphql
mutation UploadDocument($personDisplayId: Long!, $file: Upload!, $description: String) {
  uploadDocument(
    personDisplayId: $personDisplayId
    file: $file
    description: $description
  ) {
    displayId
    fileName
    fileUrl
    contentType
    fileSizeBytes
    description
    uploadedAt
    uploadedBy
  }
}

mutation UpdateDocument(
  $personDisplayId: Long!
  $documentDisplayId: Long!
  $input: UpdateDocumentInput!
) {
  updateDocument(
    personDisplayId: $personDisplayId
    documentDisplayId: $documentDisplayId
    input: $input
  ) {
    displayId
    fileName
    description
    modifiedAt
    modifiedBy
  }
}

mutation DeleteDocument($personDisplayId: Long!, $documentDisplayId: Long!) {
  deleteDocument(
    personDisplayId: $personDisplayId
    documentDisplayId: $documentDisplayId
  )
}

mutation UploadProfileImage($personDisplayId: Long!, $file: Upload!) {
  uploadProfileImage(personDisplayId: $personDisplayId, file: $file)
}

mutation DeleteProfileImage($personDisplayId: Long!) {
  deleteProfileImage(personDisplayId: $personDisplayId)
}

query GetPersonDocuments(
  $personDisplayId: Long!
  $pageNumber: Int
  $pageSize: Int
  $searchTerm: String
  $sortBy: String
  $sortDescending: Boolean
) {
  getPersonDocuments(
    personDisplayId: $personDisplayId
    pageNumber: $pageNumber
    pageSize: $pageSize
    searchTerm: $searchTerm
    sortBy: $sortBy
    sortDescending: $sortDescending
  ) {
    items {
      displayId
      fileName
      fileUrl
      contentType
      fileSizeBytes
      description
      uploadedAt
      uploadedBy
    }
    pageNumber
    pageSize
    totalCount
    totalPages
    hasNextPage
    hasPreviousPage
  }
}

query GetDocument($personDisplayId: Long!, $documentDisplayId: Long!) {
  getDocument(personDisplayId: $personDisplayId, documentDisplayId: $documentDisplayId) {
    displayId
    fileName
    fileUrl
    contentType
    fileSizeBytes
    description
    uploadedAt
    uploadedBy
    modifiedAt
    modifiedBy
  }
}

query GetProfileImageUrl($personDisplayId: Long!) {
  getProfileImageUrl(personDisplayId: $personDisplayId)
}
```

### 2. Regenerate Types

Run the code generator to create typed hooks:

```bash
cd application
npm run codegen
```

This will generate:
- `useUploadDocumentMutation()`
- `useUpdateDocumentMutation()`
- `useDeleteDocumentMutation()`
- `useUploadProfileImageMutation()`
- `useDeleteProfileImageMutation()`
- `useGetPersonDocumentsQuery()`
- `useGetDocumentQuery()`
- `useGetProfileImageUrlQuery()`

### 3. Update Component Code

**Before (REST with fetch/axios)**:
```typescript
// Old REST code
const uploadDocument = async (personId: number, file: File, description?: string) => {
  const formData = new FormData();
  formData.append('file', file);
  if (description) formData.append('description', description);

  const response = await fetch(`/api/persons/${personId}/documents`, {
    method: 'POST',
    headers: { 'Authorization': `Bearer ${token}` },
    body: formData
  });

  return response.json();
};
```

**After (GraphQL with Apollo Client)**:
```typescript
import { useUploadDocumentMutation } from '@/graphql/generated/graphql';

const DocumentUpload = ({ personId }: { personId: number }) => {
  const [uploadDocument, { loading, error }] = useUploadDocumentMutation();

  const handleUpload = async (file: File, description?: string) => {
    try {
      const { data } = await uploadDocument({
        variables: {
          personDisplayId: personId,
          file,
          description
        },
        // Optional: refetch queries after upload
        refetchQueries: ['GetPersonDocuments', 'GetPerson']
      });

      console.log('Document uploaded:', data?.uploadDocument);
    } catch (err) {
      console.error('Upload failed:', err);
    }
  };

  return (
    <input
      type="file"
      onChange={(e) => {
        const file = e.target.files?.[0];
        if (file) handleUpload(file);
      }}
      disabled={loading}
    />
  );
};
```

### 4. Profile Image Example

**Before (REST)**:
```typescript
const uploadProfileImage = async (personId: number, file: File) => {
  const formData = new FormData();
  formData.append('file', file);

  await fetch(`/api/persons/${personId}/documents/profile-image`, {
    method: 'POST',
    body: formData
  });
};

const profileImageUrl = `/api/persons/${personId}/documents/profile-image`;
```

**After (GraphQL)**:
```typescript
import { useUploadProfileImageMutation, useGetProfileImageUrlQuery } from '@/graphql/generated/graphql';

const ProfileImage = ({ personId }: { personId: number }) => {
  const [uploadProfileImage] = useUploadProfileImageMutation();
  const { data: imageUrlData } = useGetProfileImageUrlQuery({
    variables: { personDisplayId: personId }
  });

  const handleUpload = async (file: File) => {
    await uploadProfileImage({
      variables: { personDisplayId: personId, file },
      refetchQueries: ['GetProfileImageUrl', 'GetPerson']
    });
  };

  return (
    <div>
      {imageUrlData?.getProfileImageUrl && (
        <img src={imageUrlData.getProfileImageUrl} alt="Profile" />
      )}
      <input type="file" accept="image/*" onChange={(e) => {
        const file = e.target.files?.[0];
        if (file) handleUpload(file);
      }} />
    </div>
  );
};
```

### 5. Document List Example

**Before (REST)**:
```typescript
const fetchDocuments = async (personId: number, page = 1) => {
  const response = await fetch(
    `/api/persons/${personId}/documents?pageNumber=${page}&pageSize=10`
  );
  return response.json();
};
```

**After (GraphQL)**:
```typescript
import { useGetPersonDocumentsQuery } from '@/graphql/generated/graphql';

const DocumentList = ({ personId }: { personId: number }) => {
  const { data, loading, error } = useGetPersonDocumentsQuery({
    variables: {
      personDisplayId: personId,
      pageNumber: 1,
      pageSize: 10
    }
  });

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <ul>
      {data?.getPersonDocuments?.items?.map(doc => (
        <li key={doc.displayId}>
          {doc.fileName} - {doc.description}
        </li>
      ))}
    </ul>
  );
};
```

## Important Notes

### File Upload Configuration

Make sure your Apollo Client is configured to support file uploads:

```typescript
// In your Apollo Client setup
import { createUploadLink } from 'apollo-upload-client';

const uploadLink = createUploadLink({
  uri: 'http://localhost:5000/graphql',
  headers: {
    'apollo-require-preflight': 'true'
  }
});

const client = new ApolloClient({
  link: uploadLink,
  cache: new InMemoryCache()
});
```

### Authentication

Authentication tokens are automatically forwarded from the HTTP context in GraphQL mutations. Ensure your requests include the `Authorization` header:

```typescript
const authLink = setContext((_, { headers }) => {
  const token = localStorage.getItem('token');
  return {
    headers: {
      ...headers,
      authorization: token ? `Bearer ${token}` : '',
    }
  };
});
```

### File Size Limits

- Documents: 50 MB max
- Profile images: 5 MB max
- Allowed image types: JPEG, PNG
- Allowed document types: PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX, JPEG, PNG

## Testing Checklist

- [ ] Upload document works
- [ ] Update document description works
- [ ] Delete document works
- [ ] Upload profile image works
- [ ] Delete profile image works
- [ ] Get documents list with pagination works
- [ ] Get single document details works
- [ ] Profile image displays correctly
- [ ] Authentication token is properly forwarded
- [ ] Error handling works for all operations
- [ ] Loading states are displayed
- [ ] Cache updates after mutations

## Troubleshooting

### File upload fails with 413 (Payload Too Large)
- Check file size limits
- Ensure proper request size configuration in backend

### File upload returns 400 (Bad Request)
- Verify file type is allowed
- Check that file variable is properly passed

### Profile image not displaying
- Ensure `getProfileImageUrl` query returns valid URL
- Check CORS configuration
- Verify image exists on server

### Authentication errors
- Ensure `Authorization` header is included
- Check token validity
- Verify token format: `Bearer <token>`

## Support

For questions or issues, refer to:
- [Gateway Structure Documentation](../server/GATEWAY-STRUCTURE.md)
- [GraphQL Usage Guide](../application/GRAPHQL_USAGE.md)
- [Apollo Client Documentation](https://www.apollographql.com/docs/react/)
