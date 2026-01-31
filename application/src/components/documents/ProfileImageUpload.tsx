import { useState, useRef, useContext, useEffect } from 'react';
import { Box, Button, Flex, Text, Image, Spinner } from '@chakra-ui/react';
import { AuthContext } from '../../contexts/AuthContext';
import {
  getProfileImageUrl,
  uploadProfileImage,
  deleteProfileImageRest,
} from '../../hooks/useDocuments';

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
  const authContext = useContext(AuthContext);
  const accessToken = authContext?.accessToken ?? null;
  const [uploading, setUploading] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [imageVersion, setImageVersion] = useState(0);
  const [imageBlobUrl, setImageBlobUrl] = useState<string | null>(null);
  const [loadingImage, setLoadingImage] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Fetch profile image with authentication through Gateway
  useEffect(() => {
    const fetchImage = async () => {
      if (!currentImageUrl || !accessToken) {
        setImageBlobUrl(null);
        return;
      }

      setLoadingImage(true);
      try {
        const url = getProfileImageUrl(personDisplayId, imageVersion);
        const response = await fetch(url, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const blob = await response.blob();
        const blobUrl = URL.createObjectURL(blob);
        setImageBlobUrl(blobUrl);
      } catch (err) {
        console.error('Error loading profile image:', err);
        setImageBlobUrl(null);
      } finally {
        setLoadingImage(false);
      }
    };

    fetchImage();

    // Cleanup blob URL on unmount or when dependencies change
    return () => {
      if (imageBlobUrl) {
        URL.revokeObjectURL(imageBlobUrl);
      }
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [personDisplayId, currentImageUrl, accessToken, imageVersion]);

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file || !accessToken) return;

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
      const response = await uploadProfileImage(
        personDisplayId,
        file,
        accessToken
      );

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      setImageVersion((v) => v + 1);
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
    if (!accessToken) return;

    setDeleting(true);
    setError(null);

    try {
      const response = await deleteProfileImageRest(
        personDisplayId,
        accessToken
      );

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      setImageBlobUrl(null);
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
          {uploading || loadingImage ? (
            <Spinner size="lg" />
          ) : imageBlobUrl ? (
            <Image
              src={imageBlobUrl}
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
