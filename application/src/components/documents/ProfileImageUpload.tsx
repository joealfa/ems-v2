import { useState, useRef, useContext, useEffect } from 'react';
import { Box, Button, Flex, Text, Image, Spinner } from '@chakra-ui/react';
import { AuthContext } from '../../contexts/AuthContext';
import {
  useProfileImageUrl,
  useUploadProfileImage,
  useDeleteProfileImage,
} from '../../hooks/useDocuments';
import { useConfirm } from '../../hooks';
import { ConfirmDialog } from '../ui';
import { getInitials } from '../../utils/helper';

interface ProfileImageUploadProps {
  personDisplayId: number;
  currentImageUrl?: string | null;
  hasProfileImage?: boolean;
  firstName?: string | null | undefined;
  lastName?: string | null | undefined;
  onImageUpdated: () => void;
}

const ProfileImageUpload = ({
  personDisplayId,
  currentImageUrl,
  hasProfileImage,
  firstName,
  lastName,
  onImageUpdated,
}: ProfileImageUploadProps) => {
  const authContext = useContext(AuthContext);
  const accessToken = authContext?.accessToken ?? null;
  const { confirm, confirmDialog } = useConfirm();
  const [error, setError] = useState<string | null>(null);
  const [imageVersion, setImageVersion] = useState(0);
  const [imageBlobUrl, setImageBlobUrl] = useState<string | null>(null);
  const [loadingImage, setLoadingImage] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const { uploadProfileImage: uploadProfileImageMutation, uploading } =
    useUploadProfileImage();
  const { deleteProfileImage: deleteProfileImageMutation, deleting } =
    useDeleteProfileImage();

  // Use GraphQL query to get the profile image URL
  const { profileImageUrl: graphqlImageUrl, loading: loadingUrl } =
    useProfileImageUrl(personDisplayId);

  // Fetch profile image with authentication
  useEffect(() => {
    const fetchImage = async () => {
      // Only fetch if hasProfileImage flag is true or currentImageUrl exists
      if (
        !currentImageUrl ||
        !accessToken ||
        !graphqlImageUrl ||
        !hasProfileImage
      ) {
        setImageBlobUrl(null);
        return;
      }

      setLoadingImage(true);
      try {
        // Use the URL from GraphQL query with cache busting
        const url = `${graphqlImageUrl}?v=${imageVersion}`;
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
  }, [
    personDisplayId,
    currentImageUrl,
    hasProfileImage,
    accessToken,
    imageVersion,
    graphqlImageUrl,
  ]);

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

    setError(null);

    try {
      await uploadProfileImageMutation(personDisplayId, file);

      setImageVersion((v) => v + 1);
      onImageUpdated();
    } catch (err) {
      console.error('Error uploading profile image:', err);
      setError('Failed to upload profile image');
    } finally {
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    }
  };

  const handleDelete = async () => {
    const confirmed = await confirm({
      title: 'Delete Profile Image',
      message:
        'Are you sure you want to delete the profile image? This action cannot be undone.',
      confirmText: 'Delete',
      confirmColorScheme: 'red',
    });

    if (!confirmed) return;

    setError(null);

    try {
      await deleteProfileImageMutation(personDisplayId);

      setImageBlobUrl(null);
      onImageUpdated();
    } catch (err) {
      console.error('Error deleting profile image:', err);
      setError('Failed to delete profile image');
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
          {uploading || loadingImage || loadingUrl ? (
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
            <Flex
              w="100%"
              h="100%"
              alignItems="center"
              justifyContent="center"
              bg="bg.muted"
              color="fg"
            >
              <Text fontSize="5xl" fontWeight="bold">
                {getInitials(firstName, lastName)}
              </Text>
            </Flex>
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
      <ConfirmDialog {...confirmDialog} />
    </Box>
  );
};

export default ProfileImageUpload;
