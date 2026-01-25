import { useState, useEffect, useRef } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Input,
  Card,
  Stack,
  Field,
  NativeSelect,
  Spinner,
  Text,
  Checkbox,
  Image,
  Center,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  personsApi,
  documentsApi,
  type CreatePersonDto,
  type UpdatePersonDto,
  type PersonResponseDto,
  type CreateAddressDto,
  type CreateContactDto,
  type DocumentListDto,
  CreatePersonDtoGenderEnum,
  CreatePersonDtoCivilStatusEnum,
  CreateAddressDtoAddressTypeEnum,
  CreateContactDtoContactTypeEnum,
  API_BASE_URL,
} from '../../api';
import { DocumentsTable, formatFileSize } from '../../components/documents';

interface AddressFormData {
  address1: string;
  address2: string;
  barangay: string;
  city: string;
  province: string;
  country: string;
  zipCode: string;
  isCurrent: boolean;
  isPermanent: boolean;
  addressType: string;
}

interface ContactFormData {
  mobile: string;
  landLine: string;
  fax: string;
  email: string;
  contactType: string;
}

interface PersonFormData {
  firstName: string;
  lastName: string;
  middleName: string;
  dateOfBirth: string;
  gender: string;
  civilStatus: string;
}

const initialAddressData: AddressFormData = {
  address1: '',
  address2: '',
  barangay: '',
  city: '',
  province: '',
  country: 'Philippines',
  zipCode: '',
  isCurrent: false,
  isPermanent: false,
  addressType: 'Home',
};

const initialContactData: ContactFormData = {
  mobile: '',
  landLine: '',
  fax: '',
  email: '',
  contactType: 'Personal',
};

const initialFormData: PersonFormData = {
  firstName: '',
  lastName: '',
  middleName: '',
  dateOfBirth: '',
  gender: 'Male',
  civilStatus: 'Single',
};

const PersonFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  const [formData, setFormData] = useState<PersonFormData>(initialFormData);
  const [addresses, setAddresses] = useState<AddressFormData[]>([]);
  const [contacts, setContacts] = useState<ContactFormData[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Profile image state
  const [profileImageUrl, setProfileImageUrl] = useState<string | null>(null);
  const [profileImageVersion, setProfileImageVersion] = useState(0);
  const [uploadingImage, setUploadingImage] = useState(false);
  const profileInputRef = useRef<HTMLInputElement>(null);

  // Documents state
  const [documents, setDocuments] = useState<DocumentListDto[]>([]);
  const [loadingDocuments, setLoadingDocuments] = useState(false);
  const [uploadingDocument, setUploadingDocument] = useState(false);
  const [documentDescription, setDocumentDescription] = useState('');
  const [selectedDocumentFiles, setSelectedDocumentFiles] = useState<File[]>(
    []
  );
  const documentInputRef = useRef<HTMLInputElement>(null);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    if (isEditMode) {
      loadPerson();
      loadDocuments();
    }
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadPerson = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await personsApi.apiV1PersonsDisplayIdGet(
        Number(displayId)
      );
      const person: PersonResponseDto = response.data;
      setFormData({
        firstName: person.firstName || '',
        lastName: person.lastName || '',
        middleName: person.middleName || '',
        dateOfBirth: person.dateOfBirth?.split('T')[0] || '',
        gender: person.gender || 'Male',
        civilStatus: person.civilStatus || 'Single',
      });
      setProfileImageUrl(person.profileImageUrl || null);

      // Load existing addresses
      if (person.addresses && person.addresses.length > 0) {
        setAddresses(
          person.addresses.map(addr => ({
            address1: addr.address1 || '',
            address2: addr.address2 || '',
            barangay: addr.barangay || '',
            city: addr.city || '',
            province: addr.province || '',
            country: addr.country || 'Philippines',
            zipCode: addr.zipCode || '',
            isCurrent: addr.isCurrent || false,
            isPermanent: addr.isPermanent || false,
            addressType: addr.addressType || 'Home',
          }))
        );
      }

      // Load existing contacts
      if (person.contacts && person.contacts.length > 0) {
        setContacts(
          person.contacts.map(contact => ({
            mobile: contact.mobile || '',
            landLine: contact.landLine || '',
            fax: contact.fax || '',
            email: contact.email || '',
            contactType: contact.contactType || 'Personal',
          }))
        );
      }
    } catch (err) {
      console.error('Error loading person:', err);
      setError('Failed to load person data');
    } finally {
      setLoading(false);
    }
  };

  const loadDocuments = async () => {
    if (!displayId) return;
    setLoadingDocuments(true);
    try {
      const response =
        await documentsApi.apiV1PersonsPersonDisplayIdDocumentsGet(
          Number(displayId),
          1,
          100
        );
      setDocuments(response.data.items || []);
    } catch (err) {
      console.error('Error loading documents:', err);
    } finally {
      setLoadingDocuments(false);
    }
  };

  const handleChange = (field: keyof PersonFormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  // Address handlers
  const addAddress = () => {
    setAddresses(prev => [...prev, { ...initialAddressData }]);
  };

  const removeAddress = (index: number) => {
    setAddresses(prev => prev.filter((_, i) => i !== index));
  };

  const updateAddress = (
    index: number,
    field: keyof AddressFormData,
    value: string | boolean
  ) => {
    setAddresses(prev =>
      prev.map((addr, i) => (i === index ? { ...addr, [field]: value } : addr))
    );
  };

  // Contact handlers
  const addContact = () => {
    setContacts(prev => [...prev, { ...initialContactData }]);
  };

  const removeContact = (index: number) => {
    setContacts(prev => prev.filter((_, i) => i !== index));
  };

  const updateContact = (
    index: number,
    field: keyof ContactFormData,
    value: string
  ) => {
    setContacts(prev =>
      prev.map((contact, i) =>
        i === index ? { ...contact, [field]: value } : contact
      )
    );
  };

  // Profile image handlers
  const handleProfileImageSelect = async (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = e.target.files?.[0];
    if (!file || !displayId) return;

    if (!['image/jpeg', 'image/png'].includes(file.type)) {
      setError('Only JPEG and PNG images are allowed');
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      setError('Image must be less than 5 MB');
      return;
    }

    setUploadingImage(true);
    setError(null);

    try {
      await documentsApi.apiV1PersonsPersonDisplayIdDocumentsProfileImagePost(
        Number(displayId),
        file
      );
      setProfileImageVersion(v => v + 1);
      await loadPerson();
    } catch (err) {
      console.error('Error uploading profile image:', err);
      setError('Failed to upload profile image');
    } finally {
      setUploadingImage(false);
      if (profileInputRef.current) {
        profileInputRef.current.value = '';
      }
    }
  };

  const handleDeleteProfileImage = async () => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete the profile image?')
    )
      return;

    setUploadingImage(true);
    try {
      await documentsApi.apiV1PersonsPersonDisplayIdDocumentsProfileImageDelete(
        Number(displayId)
      );
      setProfileImageUrl(null);
    } catch (err) {
      console.error('Error deleting profile image:', err);
      setError('Failed to delete profile image');
    } finally {
      setUploadingImage(false);
    }
  };

  // Document handlers
  const handleDocumentFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      setSelectedDocumentFiles(Array.from(files));
    }
  };

  const handleDocumentUpload = async () => {
    if (selectedDocumentFiles.length === 0 || !displayId) return;

    setUploadingDocument(true);
    setError(null);

    const errors: string[] = [];
    let successCount = 0;

    for (const file of selectedDocumentFiles) {
      try {
        await documentsApi.apiV1PersonsPersonDisplayIdDocumentsPost(
          Number(displayId),
          file,
          documentDescription || undefined
        );
        successCount++;
      } catch (err) {
        console.error('Error uploading document:', err);
        errors.push(file.name);
      }
    }

    if (errors.length > 0) {
      setError(
        `Failed to upload ${errors.length} file(s). Check file types - only PDF, Word, Excel, PowerPoint, JPEG, PNG are allowed.`
      );
    }

    if (successCount > 0) {
      setSelectedDocumentFiles([]);
      setDocumentDescription('');
      if (documentInputRef.current) {
        documentInputRef.current.value = '';
      }
      await loadDocuments();
    }

    setUploadingDocument(false);
  };

  const handleDocumentDownload = (
    documentDisplayId: number,
    fileName: string | null | undefined
  ) => {
    const url = `${API_BASE_URL}/api/v1/persons/${displayId}/documents/${documentDisplayId}/download`;
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName || 'document';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleDocumentDelete = async (documentDisplayId: number) => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete this document?')
    )
      return;

    try {
      await documentsApi.apiV1PersonsPersonDisplayIdDocumentsDocumentDisplayIdDelete(
        Number(displayId),
        documentDisplayId
      );
      await loadDocuments();
    } catch (err) {
      console.error('Error deleting document:', err);
      setError('Failed to delete document');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);

    try {
      const addressDtos: CreateAddressDto[] = addresses
        .filter(addr => addr.address1 && addr.city && addr.province)
        .map(addr => ({
          address1: addr.address1,
          address2: addr.address2 || null,
          barangay: addr.barangay || null,
          city: addr.city,
          province: addr.province,
          country: addr.country || null,
          zipCode: addr.zipCode || null,
          isCurrent: addr.isCurrent,
          isPermanent: addr.isPermanent,
          addressType: addr.addressType as CreateAddressDto['addressType'],
        }));

      const contactDtos: CreateContactDto[] = contacts
        .filter(contact => contact.mobile || contact.email || contact.landLine)
        .map(contact => ({
          mobile: contact.mobile || null,
          landLine: contact.landLine || null,
          fax: contact.fax || null,
          email: contact.email || null,
          contactType: contact.contactType as CreateContactDto['contactType'],
        }));

      if (isEditMode) {
        const updateDto: UpdatePersonDto = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          middleName: formData.middleName || null,
          dateOfBirth: formData.dateOfBirth,
          gender: formData.gender as UpdatePersonDto['gender'],
          civilStatus: formData.civilStatus as UpdatePersonDto['civilStatus'],
        };
        await personsApi.apiV1PersonsDisplayIdPut(Number(displayId), updateDto);
      } else {
        const createDto: CreatePersonDto = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          middleName: formData.middleName || null,
          dateOfBirth: formData.dateOfBirth,
          gender: formData.gender as CreatePersonDto['gender'],
          civilStatus: formData.civilStatus as CreatePersonDto['civilStatus'],
          addresses: addressDtos.length > 0 ? addressDtos : null,
          contacts: contactDtos.length > 0 ? contactDtos : null,
        };
        await personsApi.apiV1PersonsPost(createDto);
      }
      navigate('/persons');
    } catch (err) {
      console.error('Error saving person:', err);
      setError('Failed to save person');
    } finally {
      setSaving(false);
    }
  };

  // Generate initials for avatar
  const getInitials = () => {
    const first = formData.firstName?.[0] || '';
    const last = formData.lastName?.[0] || '';
    return (first + last).toUpperCase() || '?';
  };

  if (loading) {
    return (
      <Flex justify="center" align="center" h="100%">
        <Spinner size="lg" />
      </Flex>
    );
  }

  return (
    <Box maxW="900px">
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">
          {isEditMode ? 'Edit Person' : 'Add New Person'}
        </Heading>
        <Button variant="outline" onClick={() => navigate('/persons')}>
          Cancel
        </Button>
      </Flex>

      {error && (
        <Box mb={4} p={4} bg="red.100" color="red.800" borderRadius="md">
          <Text>{error}</Text>
        </Box>
      )}

      <form onSubmit={handleSubmit}>
        <Stack gap={6}>
          {/* Personal Information with Profile Image */}
          <Card.Root>
            <Card.Header>
              <Heading size="md">Personal Information</Heading>
            </Card.Header>
            <Card.Body>
              <Flex gap={8}>
                {/* Profile Image Section - only show in edit mode */}
                {isEditMode && (
                  <Box>
                    <Flex direction="column" align="center" gap={4}>
                      <Box
                        w="120px"
                        h="120px"
                        borderRadius="full"
                        overflow="hidden"
                        bg="bg.muted"
                        display="flex"
                        alignItems="center"
                        justifyContent="center"
                        borderWidth={2}
                        borderColor="border.muted"
                      >
                        {uploadingImage ? (
                          <Spinner size="lg" />
                        ) : profileImageUrl ? (
                          <Image
                            src={`${API_BASE_URL}/api/v1/persons/${displayId}/documents/profile-image?v=${profileImageVersion}`}
                            alt="Profile"
                            w="100%"
                            h="100%"
                            objectFit="cover"
                          />
                        ) : (
                          <Center
                            w="100%"
                            h="100%"
                            bg="bg.muted"
                            color="fg.muted"
                            fontSize="2xl"
                            fontWeight="bold"
                          >
                            {getInitials()}
                          </Center>
                        )}
                      </Box>

                      <Flex gap={2}>
                        <Button
                          size="xs"
                          variant="outline"
                          onClick={() => profileInputRef.current?.click()}
                          disabled={uploadingImage}
                        >
                          {profileImageUrl ? 'Change' : 'Upload'}
                        </Button>
                        {profileImageUrl && (
                          <Button
                            size="xs"
                            variant="outline"
                            colorPalette="red"
                            onClick={handleDeleteProfileImage}
                            disabled={uploadingImage}
                          >
                            Remove
                          </Button>
                        )}
                      </Flex>

                      <input
                        ref={profileInputRef}
                        type="file"
                        accept="image/jpeg,image/png"
                        onChange={handleProfileImageSelect}
                        style={{ display: 'none' }}
                      />

                      <Text fontSize="xs" color="fg.muted">
                        JPEG or PNG, max 5 MB
                      </Text>
                    </Flex>
                  </Box>
                )}

                {/* Form Fields */}
                <Box flex={1}>
                  <Stack gap={4}>
                    <Flex gap={4}>
                      <Field.Root flex={1} required>
                        <Field.Label>First Name</Field.Label>
                        <Input
                          value={formData.firstName}
                          onChange={e =>
                            handleChange('firstName', e.target.value)
                          }
                          placeholder="Enter first name"
                        />
                      </Field.Root>

                      <Field.Root flex={1} required>
                        <Field.Label>Last Name</Field.Label>
                        <Input
                          value={formData.lastName}
                          onChange={e =>
                            handleChange('lastName', e.target.value)
                          }
                          placeholder="Enter last name"
                        />
                      </Field.Root>
                    </Flex>

                    <Field.Root>
                      <Field.Label>Middle Name</Field.Label>
                      <Input
                        value={formData.middleName}
                        onChange={e =>
                          handleChange('middleName', e.target.value)
                        }
                        placeholder="Enter middle name (optional)"
                      />
                    </Field.Root>

                    <Flex gap={4}>
                      <Field.Root flex={1} required>
                        <Field.Label>Date of Birth</Field.Label>
                        <Input
                          type="date"
                          value={formData.dateOfBirth}
                          onChange={e =>
                            handleChange('dateOfBirth', e.target.value)
                          }
                        />
                      </Field.Root>

                      <Field.Root flex={1} required>
                        <Field.Label>Gender</Field.Label>
                        <NativeSelect.Root>
                          <NativeSelect.Field
                            value={formData.gender}
                            onChange={e =>
                              handleChange('gender', e.target.value)
                            }
                          >
                            {Object.values(CreatePersonDtoGenderEnum).map(
                              gender => (
                                <option key={gender} value={gender}>
                                  {gender}
                                </option>
                              )
                            )}
                          </NativeSelect.Field>
                          <NativeSelect.Indicator />
                        </NativeSelect.Root>
                      </Field.Root>

                      <Field.Root flex={1} required>
                        <Field.Label>Civil Status</Field.Label>
                        <NativeSelect.Root>
                          <NativeSelect.Field
                            value={formData.civilStatus}
                            onChange={e =>
                              handleChange('civilStatus', e.target.value)
                            }
                          >
                            {Object.values(CreatePersonDtoCivilStatusEnum).map(
                              status => (
                                <option key={status} value={status}>
                                  {status === 'SoloParent'
                                    ? 'Solo Parent'
                                    : status}
                                </option>
                              )
                            )}
                          </NativeSelect.Field>
                          <NativeSelect.Indicator />
                        </NativeSelect.Root>
                      </Field.Root>
                    </Flex>
                  </Stack>
                </Box>
              </Flex>
            </Card.Body>
          </Card.Root>

          {/* Addresses Section */}
          <Card.Root>
            <Card.Header>
              <Flex justify="space-between" align="center">
                <Heading size="md">Addresses</Heading>
                <Button size="sm" variant="outline" onClick={addAddress}>
                  + Add Address
                </Button>
              </Flex>
            </Card.Header>
            <Card.Body>
              {addresses.length === 0 ? (
                <Text color="fg.muted" fontSize="sm">
                  No addresses added. Click &quot;Add Address&quot; to add one.
                </Text>
              ) : (
                <Stack gap={6}>
                  {addresses.map((address, index) => (
                    <Box
                      key={index}
                      p={4}
                      borderWidth={1}
                      borderRadius="md"
                      position="relative"
                    >
                      <Button
                        size="xs"
                        variant="outline"
                        colorPalette="red"
                        position="absolute"
                        top={2}
                        right={2}
                        onClick={() => removeAddress(index)}
                      >
                        Remove
                      </Button>

                      <Stack gap={4}>
                        <Flex gap={4}>
                          <Field.Root flex={1}>
                            <Field.Label>Address Type</Field.Label>
                            <NativeSelect.Root>
                              <NativeSelect.Field
                                value={address.addressType}
                                onChange={e =>
                                  updateAddress(
                                    index,
                                    'addressType',
                                    e.target.value
                                  )
                                }
                              >
                                {Object.values(
                                  CreateAddressDtoAddressTypeEnum
                                ).map(type => (
                                  <option key={type} value={type}>
                                    {type}
                                  </option>
                                ))}
                              </NativeSelect.Field>
                              <NativeSelect.Indicator />
                            </NativeSelect.Root>
                          </Field.Root>

                          <Flex gap={4} align="flex-end">
                            <Checkbox.Root
                              checked={address.isCurrent}
                              onCheckedChange={e =>
                                updateAddress(index, 'isCurrent', !!e.checked)
                              }
                            >
                              <Checkbox.HiddenInput />
                              <Checkbox.Control />
                              <Checkbox.Label>Current</Checkbox.Label>
                            </Checkbox.Root>

                            <Checkbox.Root
                              checked={address.isPermanent}
                              onCheckedChange={e =>
                                updateAddress(index, 'isPermanent', !!e.checked)
                              }
                            >
                              <Checkbox.HiddenInput />
                              <Checkbox.Control />
                              <Checkbox.Label>Permanent</Checkbox.Label>
                            </Checkbox.Root>
                          </Flex>
                        </Flex>

                        <Field.Root required>
                          <Field.Label>Street Address</Field.Label>
                          <Input
                            value={address.address1}
                            onChange={e =>
                              updateAddress(index, 'address1', e.target.value)
                            }
                            placeholder="House/Unit/Building number, Street name"
                          />
                        </Field.Root>

                        <Field.Root>
                          <Field.Label>Address Line 2</Field.Label>
                          <Input
                            value={address.address2}
                            onChange={e =>
                              updateAddress(index, 'address2', e.target.value)
                            }
                            placeholder="Subdivision, Village, etc. (optional)"
                          />
                        </Field.Root>

                        <Flex gap={4}>
                          <Field.Root flex={1}>
                            <Field.Label>Barangay</Field.Label>
                            <Input
                              value={address.barangay}
                              onChange={e =>
                                updateAddress(index, 'barangay', e.target.value)
                              }
                              placeholder="Barangay"
                            />
                          </Field.Root>

                          <Field.Root flex={1} required>
                            <Field.Label>City/Municipality</Field.Label>
                            <Input
                              value={address.city}
                              onChange={e =>
                                updateAddress(index, 'city', e.target.value)
                              }
                              placeholder="City or Municipality"
                            />
                          </Field.Root>
                        </Flex>

                        <Flex gap={4}>
                          <Field.Root flex={1} required>
                            <Field.Label>Province</Field.Label>
                            <Input
                              value={address.province}
                              onChange={e =>
                                updateAddress(index, 'province', e.target.value)
                              }
                              placeholder="Province"
                            />
                          </Field.Root>

                          <Field.Root flex={1}>
                            <Field.Label>Zip Code</Field.Label>
                            <Input
                              value={address.zipCode}
                              onChange={e =>
                                updateAddress(index, 'zipCode', e.target.value)
                              }
                              placeholder="Zip Code"
                            />
                          </Field.Root>

                          <Field.Root flex={1}>
                            <Field.Label>Country</Field.Label>
                            <Input
                              value={address.country}
                              onChange={e =>
                                updateAddress(index, 'country', e.target.value)
                              }
                              placeholder="Country"
                            />
                          </Field.Root>
                        </Flex>
                      </Stack>
                    </Box>
                  ))}
                </Stack>
              )}
            </Card.Body>
          </Card.Root>

          {/* Contacts Section */}
          <Card.Root>
            <Card.Header>
              <Flex justify="space-between" align="center">
                <Heading size="md">Contacts</Heading>
                <Button size="sm" variant="outline" onClick={addContact}>
                  + Add Contact
                </Button>
              </Flex>
            </Card.Header>
            <Card.Body>
              {contacts.length === 0 ? (
                <Text color="fg.muted" fontSize="sm">
                  No contacts added. Click &quot;Add Contact&quot; to add one.
                </Text>
              ) : (
                <Stack gap={6}>
                  {contacts.map((contact, index) => (
                    <Box
                      key={index}
                      p={4}
                      borderWidth={1}
                      borderRadius="md"
                      position="relative"
                    >
                      <Button
                        size="xs"
                        variant="outline"
                        colorPalette="red"
                        position="absolute"
                        top={2}
                        right={2}
                        onClick={() => removeContact(index)}
                      >
                        Remove
                      </Button>

                      <Stack gap={4}>
                        <Field.Root maxW="200px">
                          <Field.Label>Contact Type</Field.Label>
                          <NativeSelect.Root>
                            <NativeSelect.Field
                              value={contact.contactType}
                              onChange={e =>
                                updateContact(
                                  index,
                                  'contactType',
                                  e.target.value
                                )
                              }
                            >
                              {Object.values(
                                CreateContactDtoContactTypeEnum
                              ).map(type => (
                                <option key={type} value={type}>
                                  {type}
                                </option>
                              ))}
                            </NativeSelect.Field>
                            <NativeSelect.Indicator />
                          </NativeSelect.Root>
                        </Field.Root>

                        <Flex gap={4}>
                          <Field.Root flex={1}>
                            <Field.Label>Mobile</Field.Label>
                            <Input
                              value={contact.mobile}
                              onChange={e =>
                                updateContact(index, 'mobile', e.target.value)
                              }
                              placeholder="e.g., +63 912 345 6789"
                            />
                          </Field.Root>

                          <Field.Root flex={1}>
                            <Field.Label>Email</Field.Label>
                            <Input
                              type="email"
                              value={contact.email}
                              onChange={e =>
                                updateContact(index, 'email', e.target.value)
                              }
                              placeholder="e.g., email@example.com"
                            />
                          </Field.Root>
                        </Flex>

                        <Flex gap={4}>
                          <Field.Root flex={1}>
                            <Field.Label>Landline</Field.Label>
                            <Input
                              value={contact.landLine}
                              onChange={e =>
                                updateContact(index, 'landLine', e.target.value)
                              }
                              placeholder="e.g., (02) 1234 5678"
                            />
                          </Field.Root>

                          <Field.Root flex={1}>
                            <Field.Label>Fax</Field.Label>
                            <Input
                              value={contact.fax}
                              onChange={e =>
                                updateContact(index, 'fax', e.target.value)
                              }
                              placeholder="Fax number (optional)"
                            />
                          </Field.Root>
                        </Flex>
                      </Stack>
                    </Box>
                  ))}
                </Stack>
              )}
            </Card.Body>
          </Card.Root>

          {/* Documents Section - only show in edit mode */}
          {isEditMode && (
            <Card.Root>
              <Card.Header>
                <Heading size="md">Documents</Heading>
              </Card.Header>
              <Card.Body>
                <Stack gap={4}>
                  {/* Upload Form */}
                  <Box
                    p={4}
                    borderWidth={1}
                    borderRadius="md"
                    borderColor="border.muted"
                  >
                    <Heading size="sm" mb={4}>
                      Upload New Document
                    </Heading>
                    <Flex gap={4} align="flex-end" wrap="wrap">
                      <Box flex={1} minW="200px">
                        <Text fontWeight="medium" mb={2} fontSize="sm">
                          Select Files
                        </Text>
                        <input
                          ref={documentInputRef}
                          type="file"
                          multiple
                          onChange={handleDocumentFileSelect}
                          accept=".pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.jpg,.jpeg,.png"
                          style={{
                            position: 'absolute',
                            width: '1px',
                            height: '1px',
                            padding: '0',
                            margin: '-1px',
                            overflow: 'hidden',
                            clip: 'rect(0,0,0,0)',
                            border: '0',
                          }}
                        />
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => documentInputRef.current?.click()}
                          width="full"
                        >
                          {selectedDocumentFiles.length > 0
                            ? `${selectedDocumentFiles.length} file(s) selected`
                            : 'Choose Files...'}
                        </Button>
                        {selectedDocumentFiles.length > 0 && (
                          <Box mt={1}>
                            {selectedDocumentFiles.map((file, index) => (
                              <Text key={index} fontSize="xs" color="fg.muted">
                                {file.name} ({formatFileSize(file.size)})
                              </Text>
                            ))}
                          </Box>
                        )}
                      </Box>
                      <Box flex={1} minW="200px">
                        <Text fontWeight="medium" mb={2} fontSize="sm">
                          Description (optional)
                        </Text>
                        <Input
                          value={documentDescription}
                          onChange={e => setDocumentDescription(e.target.value)}
                          placeholder="Enter description"
                          size="sm"
                        />
                      </Box>
                      <Button
                        colorPalette="blue"
                        size="sm"
                        onClick={handleDocumentUpload}
                        loading={uploadingDocument}
                        disabled={selectedDocumentFiles.length === 0}
                      >
                        Upload
                      </Button>
                    </Flex>
                    <Text fontSize="xs" color="fg.muted" mt={2}>
                      Supported formats: PDF, Word, Excel, PowerPoint, JPEG, PNG
                      (Max 50 MB)
                    </Text>
                  </Box>

                  {/* Documents List */}
                  {loadingDocuments ? (
                    <Flex justify="center" py={4}>
                      <Spinner size="md" />
                    </Flex>
                  ) : (
                    <DocumentsTable
                      documents={documents}
                      onDownload={handleDocumentDownload}
                      onDelete={handleDocumentDelete}
                      showActions={true}
                    />
                  )}
                </Stack>
              </Card.Body>
            </Card.Root>
          )}

          {/* Submit Buttons */}
          <Flex justify="flex-end" gap={4}>
            <Button variant="outline" onClick={() => navigate('/persons')}>
              Cancel
            </Button>
            <Button type="submit" colorPalette="blue" loading={saving}>
              {isEditMode ? 'Update Person' : 'Create Person'}
            </Button>
          </Flex>
        </Stack>
      </form>
    </Box>
  );
};

export default PersonFormPage;
