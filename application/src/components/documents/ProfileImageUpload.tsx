import { useState, useRef } from 'react';
import { Box, Button, Flex, Text, Image, Spinner } from '@chakra-ui/react';
import { documentsApi, API_BASE_URL } from '../../api';

interface ProfileImageUploadProps {
  personDisplayId: number;
  currentImageUrl?: string | null;
  onImageUpdated: () => void;
}

const ProfileImageUpload = ({
  personDisplayId,
  currentImageUrl,
  onImageUpdated,
}: ProfileImageUploadProps) => {
  const [uploading, setUploading] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [imageVersion, setImageVersion] = useState(0);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validate file type
    if (!['image/jpeg', 'image/png'].includes(file.type)) {
      setError('Only JPEG and PNG images are allowed');
      return;
    }

    // Validate file size (5 MB max)
    if (file.size > 5 * 1024 * 1024) {
      setError('Image must be less than 5 MB');
      return;
    }

    setUploading(true);
    setError(null);

    try {
      await documentsApi.apiV1PersonsPersonDisplayIdDocumentsProfileImagePost(
        personDisplayId,
        file
      );
      setImageVersion(v => v + 1);
      onImageUpdated();
    } catch (err) {
      console.error('Error uploading profile image:', err);
      setError('Failed to upload profile image');
    } finally {
      setUploading(false);
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete the profile image?'))
      return;

    setDeleting(true);
    setError(null);

    try {
      await documentsApi.apiV1PersonsPersonDisplayIdDocumentsProfileImageDelete(
        personDisplayId
      );
      onImageUpdated();
    } catch (err) {
      console.error('Error deleting profile image:', err);
      setError('Failed to delete profile image');
    } finally {
      setDeleting(false);
    }
  };

  return (
    <Box>
      <Flex direction="column" align="center" gap={4}>
        <Box
          w="150px"
          h="150px"
          borderRadius="full"
          overflow="hidden"
          bg="bg.muted"
          display="flex"
          alignItems="center"
          justifyContent="center"
          borderWidth={2}
          borderColor="border.muted"
        >
          {uploading ? (
            <Spinner size="lg" />
          ) : currentImageUrl ? (
            <Image
              src={`${API_BASE_URL}/api/v1/persons/${personDisplayId}/documents/profile-image?v=${imageVersion}`}
              alt="Profile"
              w="100%"
              h="100%"
              objectFit="cover"
            />
          ) : (
            <Text fontSize="4xl" color="fg.muted">
              👤
            </Text>
          )}
        </Box>

        {error && (
          <Text fontSize="sm" color="red.500">
            {error}
          </Text>
        )}

        <Flex gap={2}>
          <Button
            size="sm"
            variant="outline"
            onClick={() => fileInputRef.current?.click()}
            disabled={uploading}
          >
            {currentImageUrl ? 'Change Photo' : 'Upload Photo'}
          </Button>
          {currentImageUrl && (
            <Button
              size="sm"
              variant="outline"
              colorPalette="red"
              onClick={handleDelete}
              loading={deleting}
            >
              Remove
            </Button>
          )}
        </Flex>

        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png"
          onChange={handleFileSelect}
          style={{ display: 'none' }}
        />

        <Text fontSize="xs" color="fg.muted">
          JPEG or PNG, max 5 MB
        </Text>
      </Flex>
    </Box>
  );
};

export default ProfileImageUpload;
