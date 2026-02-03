# Gateway GraphQL Quick Reference

## Document Operations

### Mutations

#### Upload Document
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
    description
  }
}
```

#### Update Document
```graphql
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
    description
  }
}
```

#### Delete Document
```graphql
mutation DeleteDocument($personDisplayId: Long!, $documentDisplayId: Long!) {
  deleteDocument(
    personDisplayId: $personDisplayId
    documentDisplayId: $documentDisplayId
  )
}
```

#### Upload Profile Image
```graphql
mutation UploadProfileImage($personDisplayId: Long!, $file: Upload!) {
  uploadProfileImage(personDisplayId: $personDisplayId, file: $file)
}
```

#### Delete Profile Image
```graphql
mutation DeleteProfileImage($personDisplayId: Long!) {
  deleteProfileImage(personDisplayId: $personDisplayId)
}
```

### Queries

#### Get Documents
```graphql
query GetPersonDocuments($personDisplayId: Long!, $pageNumber: Int, $pageSize: Int) {
  getPersonDocuments(
    personDisplayId: $personDisplayId
    pageNumber: $pageNumber
    pageSize: $pageSize
  ) {
    items {
      displayId
      fileName
      fileUrl
    }
    totalCount
  }
}
```

#### Get Profile Image URL
```graphql
query GetProfileImageUrl($personDisplayId: Long!) {
  getProfileImageUrl(personDisplayId: $personDisplayId)
}
```

## Authentication Operations

### Mutations

#### Google Login
```graphql
mutation GoogleLogin($idToken: String!) {
  googleLogin(idToken: $idToken) {
    accessToken
    refreshToken
    expiresIn
  }
}
```

#### Refresh Token
```graphql
mutation RefreshToken($refreshToken: String!) {
  refreshToken(refreshToken: $refreshToken) {
    accessToken
    refreshToken
    expiresIn
  }
}
```

#### Logout
```graphql
mutation Logout($refreshToken: String!) {
  logout(refreshToken: $refreshToken)
}
```

## TypeScript Usage Examples

### Upload Document Hook
```typescript
import { useUploadDocumentMutation } from '@/graphql/generated/graphql';

const [uploadDocument, { loading }] = useUploadDocumentMutation();

await uploadDocument({
  variables: {
    personDisplayId: 123,
    file: fileObject,
    description: 'My document'
  }
});
```

### Get Documents Hook
```typescript
import { useGetPersonDocumentsQuery } from '@/graphql/generated/graphql';

const { data, loading } = useGetPersonDocumentsQuery({
  variables: {
    personDisplayId: 123,
    pageNumber: 1,
    pageSize: 10
  }
});
```

### Profile Image Hook
```typescript
import { useGetProfileImageUrlQuery } from '@/graphql/generated/graphql';

const { data } = useGetProfileImageUrlQuery({
  variables: { personDisplayId: 123 }
});

const imageUrl = data?.getProfileImageUrl;
```

## File Constraints

- **Documents**: 50 MB max
- **Profile Images**: 5 MB max
- **Allowed Image Types**: JPEG, PNG
- **Allowed Document Types**: PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX, JPEG, PNG

## Cache Invalidation

Document mutations automatically invalidate:
- Person cache entry
- Documents list cache (where applicable)

Profile image mutations invalidate:
- Person cache entry
- Profile image URL cache
