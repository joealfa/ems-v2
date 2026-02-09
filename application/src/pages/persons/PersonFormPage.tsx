import { useState, useEffect, useRef, useCallback } from 'react';
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
  Accordion,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  usePerson,
  useCreatePerson,
  useUpdatePerson,
} from '../../hooks/usePersons';
import {
  usePersonDocuments,
  useUploadDocument,
  getDocumentDownloadUrl,
  useDeleteDocument,
} from '../../hooks/useDocuments';
import type {
  CreatePersonInput,
  UpdatePersonInput,
  CreateAddressInput,
  CreateContactInput,
  UpsertAddressInput,
  UpsertContactInput,
  Gender,
  CivilStatus,
  AddressType,
  ContactType,
} from '../../graphql/generated/graphql';
import {
  DocumentsTable,
  formatFileSize,
  ProfileImageUpload,
} from '../../components/documents';
import { formatEnumLabel } from '../../utils';
import { useConfirm, useToast } from '../../hooks';
import { ConfirmDialog } from '../../components/ui';

// GraphQL enum values - used directly for both display and API calls
const GenderOptions: Gender[] = ['Male', 'Female'];
const CivilStatusOptions: CivilStatus[] = [
  'Single',
  'Married',
  'SoloParent',
  'Widow',
  'Separated',
  'Other',
];
const AddressTypeOptions: AddressType[] = ['Home', 'Business'];
const ContactTypeOptions: ContactType[] = ['Personal', 'Work'];

interface AddressFormData {
  displayId?: number;
  address1: string;
  address2: string;
  barangay: string;
  city: string;
  province: string;
  country: string;
  zipCode: string;
  isCurrent: boolean;
  isPermanent: boolean;
  addressType: AddressType;
}

interface ContactFormData {
  displayId?: number;
  mobile: string;
  landLine: string;
  fax: string;
  email: string;
  contactType: ContactType;
}

interface PersonFormData {
  firstName: string;
  lastName: string;
  middleName: string;
  dateOfBirth: string;
  gender: Gender;
  civilStatus: CivilStatus;
}

interface DocumentListItem {
  displayId?: number;
  fileName?: string | null;
  fileSize?: number | null;
  fileSizeBytes?: number | null;
  mimeType?: string | null;
  documentType?: string | null;
  description?: string | null;
  uploadedAt?: string | null;
  uploadedBy?: string | null;
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
  const { confirm, confirmDialog } = useConfirm();
  const { showSuccess, showError } = useToast();

  const {
    person,
    loading: loadingPerson,
    refetch: refetchPerson,
  } = usePerson(isEditMode ? Number(displayId) : 0);
  const { createPerson, loading: creating } = useCreatePerson();
  const { updatePerson, loading: updating } = useUpdatePerson();

  // Use GraphQL hook for documents
  const {
    documents: graphqlDocuments,
    loading: loadingDocuments,
    refetch: refetchDocuments,
  } = usePersonDocuments(isEditMode ? Number(displayId) : 0);

  // GraphQL mutations for document operations
  const { uploadDocument: uploadDocumentMutation } = useUploadDocument();
  const { deleteDocument: deleteDocumentMutation } = useDeleteDocument();

  const [formData, setFormData] = useState<PersonFormData>(initialFormData);
  const [addresses, setAddresses] = useState<AddressFormData[]>([]);
  const [contacts, setContacts] = useState<ContactFormData[]>([]);
  const [error, setError] = useState<string | null>(null);

  // Accordion state - track which items are expanded
  const [expandedAddresses, setExpandedAddresses] = useState<string[]>([]);
  const [expandedContacts, setExpandedContacts] = useState<string[]>([]);

  const loading = loadingPerson;
  const saving = creating || updating;

  // Documents state - transform GraphQL documents for DocumentsTable
  const documents: DocumentListItem[] = graphqlDocuments
    .filter((doc): doc is NonNullable<typeof doc> => doc !== null)
    .map((doc) => ({
      displayId: doc.displayId,
      fileName: doc.fileName,
      fileSize: doc.fileSizeBytes,
      fileSizeBytes: doc.fileSizeBytes,
      mimeType: doc.contentType,
      documentType: doc.documentType,
      description: doc.description,
      uploadedAt: doc.createdOn,
      uploadedBy: doc.createdBy,
    }));
  const [uploadingDocument, setUploadingDocument] = useState(false);
  const [documentDescription, setDocumentDescription] = useState('');
  const [selectedDocumentFiles, setSelectedDocumentFiles] = useState<File[]>(
    []
  );
  const documentInputRef = useRef<HTMLInputElement>(null);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    if (isEditMode && person) {
      setFormData({
        firstName: person.firstName || '',
        lastName: person.lastName || '',
        middleName: person.middleName || '',
        dateOfBirth: person.dateOfBirth?.split('T')[0] || '',
        gender: person.gender || 'MALE',
        civilStatus: person.civilStatus || 'SINGLE',
      });

      // Load existing addresses
      if (person.addresses && person.addresses.length > 0) {
        setAddresses(
          person.addresses
            .filter((addr) => addr !== null)
            .map((addr) => ({
              displayId: addr.displayId,
              address1: addr.address1 || '',
              address2: addr.address2 || '',
              barangay: addr.barangay || '',
              city: addr.city || '',
              province: addr.province || '',
              country: addr.country || 'Philippines',
              zipCode: addr.zipCode || '',
              isCurrent: addr.isCurrent || false,
              isPermanent: addr.isPermanent || false,
              addressType: addr.addressType || 'HOME',
            }))
        );
      }

      // Load existing contacts
      if (person.contacts && person.contacts.length > 0) {
        setContacts(
          person.contacts
            .filter((contact) => contact !== null)
            .map((contact) => ({
              displayId: contact.displayId,
              mobile: contact.mobile || '',
              landLine: contact.landLine || '',
              fax: contact.fax || '',
              email: contact.email || '',
              contactType: contact.contactType || 'PERSONAL',
            }))
        );
      }
    }
  }, [person, displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const handleChange = useCallback(
    (field: keyof PersonFormData, value: string) => {
      setFormData((prev) => ({ ...prev, [field]: value }));
    },
    []
  );

  // Address handlers
  const addAddress = () => {
    setAddresses((prev) => {
      const newIndex = prev.length;
      setExpandedAddresses([`address-${newIndex}`]); // Expand only the new item
      return [...prev, { ...initialAddressData }];
    });
  };

  const removeAddress = (index: number) => {
    setAddresses((prev) => prev.filter((_, i) => i !== index));
    setExpandedAddresses((prev) =>
      prev.filter((key) => key !== `address-${index}`)
    );
  };

  const updateAddress = useCallback(
    (index: number, field: keyof AddressFormData, value: string | boolean) => {
      setAddresses((prev) =>
        prev.map((addr, i) =>
          i === index ? { ...addr, [field]: value } : addr
        )
      );
    },
    []
  );

  // Contact handlers
  const addContact = () => {
    setContacts((prev) => {
      const newIndex = prev.length;
      setExpandedContacts([`contact-${newIndex}`]); // Expand only the new item
      return [...prev, { ...initialContactData }];
    });
  };

  const removeContact = (index: number) => {
    setContacts((prev) => prev.filter((_, i) => i !== index));
    setExpandedContacts((prev) =>
      prev.filter((key) => key !== `contact-${index}`)
    );
  };

  const updateContact = useCallback(
    (index: number, field: keyof ContactFormData, value: string) => {
      setContacts((prev) =>
        prev.map((contact, i) =>
          i === index ? { ...contact, [field]: value } : contact
        )
      );
    },
    []
  );

  // Document handlers - using Gateway proxy
  const handleDocumentFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      setSelectedDocumentFiles(Array.from(files));
    }
  };

  const handleDocumentDescriptionChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      setDocumentDescription(e.target.value);
    },
    []
  );

  const handleDocumentUpload = async () => {
    if (selectedDocumentFiles.length === 0 || !displayId) return;

    setUploadingDocument(true);
    setError(null);

    const errors: string[] = [];
    let successCount = 0;

    for (const file of selectedDocumentFiles) {
      try {
        await uploadDocumentMutation(
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
      const errorMsg = `Failed to upload ${errors.length} file(s). Check file types - only PDF, Word, Excel, PowerPoint, JPEG, PNG are allowed.`;
      setError(errorMsg);
      showError('Upload Failed', errorMsg);
    }

    if (successCount > 0) {
      showSuccess(
        'Documents Uploaded',
        `Successfully uploaded ${successCount} document(s).`
      );
      setSelectedDocumentFiles([]);
      setDocumentDescription('');
      if (documentInputRef.current) {
        documentInputRef.current.value = '';
      }
      await refetchDocuments();
    }

    setUploadingDocument(false);
  };

  const handleDocumentDownload = async (
    documentDisplayId: number,
    fileName: string | null | undefined
  ) => {
    if (!displayId) return;

    try {
      const url = getDocumentDownloadUrl(Number(displayId), documentDisplayId);
      const response = await fetch(url, {
        credentials: 'include',
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const blob = await response.blob();
      const blobUrl = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = blobUrl;
      link.download = fileName || 'document';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(blobUrl);
      showSuccess(
        'Download Complete',
        `${fileName || 'Document'} downloaded successfully.`
      );
    } catch (err) {
      console.error('Error downloading document:', err);
      const errorMsg = 'Failed to download document';
      setError(errorMsg);
      showError('Download Failed', errorMsg);
    }
  };

  const handleDocumentDelete = async (documentDisplayId: number) => {
    if (!displayId) return;

    const confirmed = await confirm({
      title: 'Delete Document',
      message:
        'Are you sure you want to delete this document? This action cannot be undone.',
      confirmText: 'Delete',
      confirmColorScheme: 'red',
    });

    if (!confirmed) return;

    try {
      await deleteDocumentMutation(Number(displayId), documentDisplayId);
      showSuccess(
        'Document Deleted',
        'Document has been deleted successfully.'
      );
      await refetchDocuments();
    } catch (err) {
      console.error('Error deleting document:', err);
      const errorMsg = 'Failed to delete document';
      setError(errorMsg);
      showError('Delete Failed', errorMsg);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isEditMode) {
        // For edit mode, use UpsertAddressInput/UpsertContactInput with displayId
        const upsertAddressDtos: UpsertAddressInput[] = addresses
          .filter((addr) => addr.address1 && addr.city && addr.province)
          .map((addr) => ({
            displayId: addr.displayId,
            address1: addr.address1,
            address2: addr.address2 || undefined,
            barangay: addr.barangay || undefined,
            city: addr.city,
            province: addr.province,
            country: addr.country || undefined,
            zipCode: addr.zipCode || undefined,
            isCurrent: addr.isCurrent,
            isPermanent: addr.isPermanent,
            addressType: addr.addressType,
          }));

        const upsertContactDtos: UpsertContactInput[] = contacts
          .filter(
            (contact) => contact.mobile || contact.email || contact.landLine
          )
          .map((contact) => ({
            displayId: contact.displayId,
            mobile: contact.mobile || undefined,
            landLine: contact.landLine || undefined,
            fax: contact.fax || undefined,
            email: contact.email || undefined,
            contactType: contact.contactType,
          }));

        const updateDto: UpdatePersonInput = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          middleName: formData.middleName || undefined,
          dateOfBirth: formData.dateOfBirth,
          gender: formData.gender,
          civilStatus: formData.civilStatus,
          addresses: upsertAddressDtos,
          contacts: upsertContactDtos,
        };
        await updatePerson(Number(displayId), updateDto);
        showSuccess(
          'Person Updated',
          `${formData.firstName} ${formData.lastName} has been updated successfully.`
        );
      } else {
        // For create mode, use CreateAddressInput/CreateContactInput
        const addressDtos: CreateAddressInput[] = addresses
          .filter((addr) => addr.address1 && addr.city && addr.province)
          .map((addr) => ({
            address1: addr.address1,
            address2: addr.address2 || undefined,
            barangay: addr.barangay || undefined,
            city: addr.city,
            province: addr.province,
            country: addr.country || undefined,
            zipCode: addr.zipCode || undefined,
            isCurrent: addr.isCurrent,
            isPermanent: addr.isPermanent,
            addressType: addr.addressType,
          }));

        const contactDtos: CreateContactInput[] = contacts
          .filter(
            (contact) => contact.mobile || contact.email || contact.landLine
          )
          .map((contact) => ({
            mobile: contact.mobile || undefined,
            landLine: contact.landLine || undefined,
            fax: contact.fax || undefined,
            email: contact.email || undefined,
            contactType: contact.contactType,
          }));

        const createDto: CreatePersonInput = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          middleName: formData.middleName || undefined,
          dateOfBirth: formData.dateOfBirth,
          gender: formData.gender,
          civilStatus: formData.civilStatus,
          addresses: addressDtos.length > 0 ? addressDtos : undefined,
          contacts: contactDtos.length > 0 ? contactDtos : undefined,
        };
        await createPerson(createDto);
        showSuccess(
          'Person Created',
          `${formData.firstName} ${formData.lastName} has been added successfully.`
        );
      }
      navigate('/persons');
    } catch (err) {
      console.error('Error saving person:', err);
      const errorMessage = 'Failed to save person';
      setError(errorMessage);
      showError('Operation Failed', errorMessage);
    }
  };

  // Generate address summary for accordion header
  const getAddressSummary = (address: AddressFormData, index: number) => {
    const parts = [];
    if (address.address1) parts.push(address.address1);
    if (address.city) parts.push(address.city);
    if (address.province) parts.push(address.province);

    const tags = [];
    if (address.isCurrent) tags.push('Current');
    if (address.isPermanent) tags.push('Permanent');

    const summary =
      parts.length > 0 ? parts.join(', ') : `Address ${index + 1}`;
    const tagText = tags.length > 0 ? ` (${tags.join(', ')})` : '';
    return `${formatEnumLabel(address.addressType)}: ${summary}${tagText}`;
  };

  // Generate contact summary for accordion header
  const getContactSummary = (contact: ContactFormData, index: number) => {
    const parts = [];
    if (contact.mobile) parts.push(contact.mobile);
    if (contact.email) parts.push(contact.email);
    if (contact.landLine) parts.push(contact.landLine);

    const summary =
      parts.length > 0 ? parts.join(' | ') : `Contact ${index + 1}`;
    return `${formatEnumLabel(contact.contactType)}: ${summary}`;
  };

  if (loading) {
    return (
      <Flex justify="center" align="center" h="100%">
        <Spinner size="lg" />
      </Flex>
    );
  }

  return (
    <Box>
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">
          {isEditMode ? 'Edit Person' : 'Add New Person'}
        </Heading>
        <Button variant="outline" onClick={() => navigate(-1)}>
          Cancel
        </Button>
      </Flex>

      {error && (
        <Box mb={4} p={4} bg="red.100" color="red.800" borderRadius="md">
          <Text>{error}</Text>
        </Box>
      )}

      <form onSubmit={handleSubmit} noValidate>
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
                    <ProfileImageUpload
                      personDisplayId={Number(displayId)}
                      currentImageUrl={person?.profileImageUrl}
                      hasProfileImage={person?.hasProfileImage}
                      firstName={formData.firstName || person?.firstName}
                      lastName={formData.lastName || person?.lastName}
                      onImageUpdated={refetchPerson}
                    />
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
                          onChange={(e) =>
                            handleChange('firstName', e.target.value)
                          }
                          placeholder="Enter first name"
                        />
                      </Field.Root>

                      <Field.Root flex={1} required>
                        <Field.Label>Last Name</Field.Label>
                        <Input
                          value={formData.lastName}
                          onChange={(e) =>
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
                        onChange={(e) =>
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
                          onChange={(e) =>
                            handleChange('dateOfBirth', e.target.value)
                          }
                        />
                      </Field.Root>

                      <Field.Root flex={1} required>
                        <Field.Label>Gender</Field.Label>
                        <NativeSelect.Root>
                          <NativeSelect.Field
                            value={formData.gender}
                            onChange={(e) =>
                              handleChange('gender', e.target.value)
                            }
                          >
                            {GenderOptions.map((gender) => (
                              <option key={gender} value={gender}>
                                {formatEnumLabel(gender)}
                              </option>
                            ))}
                          </NativeSelect.Field>
                          <NativeSelect.Indicator />
                        </NativeSelect.Root>
                      </Field.Root>

                      <Field.Root flex={1} required>
                        <Field.Label>Civil Status</Field.Label>
                        <NativeSelect.Root>
                          <NativeSelect.Field
                            value={formData.civilStatus}
                            onChange={(e) =>
                              handleChange('civilStatus', e.target.value)
                            }
                          >
                            {CivilStatusOptions.map((status) => (
                              <option key={status} value={status}>
                                {formatEnumLabel(status)}
                              </option>
                            ))}
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
                <Accordion.Root
                  multiple
                  value={expandedAddresses}
                  onValueChange={(details) =>
                    setExpandedAddresses(details.value)
                  }
                >
                  <Stack gap={3}>
                    {addresses.map((address, index) => (
                      <Accordion.Item
                        key={index}
                        value={`address-${index}`}
                        borderWidth="1px"
                        borderRadius="md"
                        borderColor="border.muted"
                        overflow="hidden"
                      >
                        <Accordion.ItemTrigger
                          px={4}
                          py={3}
                          cursor="pointer"
                          _hover={{ bg: 'bg.muted' }}
                        >
                          <Text
                            fontWeight="medium"
                            fontSize="sm"
                            flex={1}
                            textAlign="left"
                          >
                            {getAddressSummary(address, index)}
                          </Text>
                          <Accordion.ItemIndicator />
                        </Accordion.ItemTrigger>
                        <Accordion.ItemContent>
                          <Box px={4} pb={4} pt={2}>
                            <Flex justify="flex-end" mb={3}>
                              <Button
                                size="xs"
                                variant="outline"
                                colorPalette="red"
                                onClick={() => removeAddress(index)}
                              >
                                Remove
                              </Button>
                            </Flex>

                            <Stack gap={4}>
                              <Flex gap={4}>
                                <Field.Root flex={1}>
                                  <Field.Label>Address Type</Field.Label>
                                  <NativeSelect.Root>
                                    <NativeSelect.Field
                                      value={address.addressType}
                                      onChange={(e) =>
                                        updateAddress(
                                          index,
                                          'addressType',
                                          e.target.value
                                        )
                                      }
                                    >
                                      {AddressTypeOptions.map((type) => (
                                        <option key={type} value={type}>
                                          {formatEnumLabel(type)}
                                        </option>
                                      ))}
                                    </NativeSelect.Field>
                                    <NativeSelect.Indicator />
                                  </NativeSelect.Root>
                                </Field.Root>

                                <Flex gap={4} align="flex-end">
                                  <Checkbox.Root
                                    checked={address.isCurrent}
                                    onCheckedChange={(e) =>
                                      updateAddress(
                                        index,
                                        'isCurrent',
                                        !!e.checked
                                      )
                                    }
                                  >
                                    <Checkbox.HiddenInput />
                                    <Checkbox.Control />
                                    <Checkbox.Label>Current</Checkbox.Label>
                                  </Checkbox.Root>

                                  <Checkbox.Root
                                    checked={address.isPermanent}
                                    onCheckedChange={(e) =>
                                      updateAddress(
                                        index,
                                        'isPermanent',
                                        !!e.checked
                                      )
                                    }
                                  >
                                    <Checkbox.HiddenInput />
                                    <Checkbox.Control />
                                    <Checkbox.Label>Permanent</Checkbox.Label>
                                  </Checkbox.Root>
                                </Flex>
                              </Flex>

                              <Field.Root>
                                <Field.Label>Street Address</Field.Label>
                                <Input
                                  value={address.address1}
                                  onChange={(e) =>
                                    updateAddress(
                                      index,
                                      'address1',
                                      e.target.value
                                    )
                                  }
                                  placeholder="House/Unit/Building number, Street name"
                                />
                              </Field.Root>

                              <Field.Root>
                                <Field.Label>Address Line 2</Field.Label>
                                <Input
                                  value={address.address2}
                                  onChange={(e) =>
                                    updateAddress(
                                      index,
                                      'address2',
                                      e.target.value
                                    )
                                  }
                                  placeholder="Subdivision, Village, etc. (optional)"
                                />
                              </Field.Root>

                              <Flex gap={4}>
                                <Field.Root flex={1}>
                                  <Field.Label>Barangay</Field.Label>
                                  <Input
                                    value={address.barangay}
                                    onChange={(e) =>
                                      updateAddress(
                                        index,
                                        'barangay',
                                        e.target.value
                                      )
                                    }
                                    placeholder="Barangay"
                                  />
                                </Field.Root>

                                <Field.Root flex={1}>
                                  <Field.Label>City/Municipality</Field.Label>
                                  <Input
                                    value={address.city}
                                    onChange={(e) =>
                                      updateAddress(
                                        index,
                                        'city',
                                        e.target.value
                                      )
                                    }
                                    placeholder="City or Municipality"
                                  />
                                </Field.Root>
                              </Flex>

                              <Flex gap={4}>
                                <Field.Root flex={1}>
                                  <Field.Label>Province</Field.Label>
                                  <Input
                                    value={address.province}
                                    onChange={(e) =>
                                      updateAddress(
                                        index,
                                        'province',
                                        e.target.value
                                      )
                                    }
                                    placeholder="Province"
                                  />
                                </Field.Root>

                                <Field.Root flex={1}>
                                  <Field.Label>Zip Code</Field.Label>
                                  <Input
                                    value={address.zipCode}
                                    onChange={(e) =>
                                      updateAddress(
                                        index,
                                        'zipCode',
                                        e.target.value
                                      )
                                    }
                                    placeholder="Zip Code"
                                  />
                                </Field.Root>

                                <Field.Root flex={1}>
                                  <Field.Label>Country</Field.Label>
                                  <Input
                                    value={address.country}
                                    onChange={(e) =>
                                      updateAddress(
                                        index,
                                        'country',
                                        e.target.value
                                      )
                                    }
                                    placeholder="Country"
                                  />
                                </Field.Root>
                              </Flex>
                            </Stack>
                          </Box>
                        </Accordion.ItemContent>
                      </Accordion.Item>
                    ))}
                  </Stack>
                </Accordion.Root>
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
                <Accordion.Root
                  multiple
                  value={expandedContacts}
                  onValueChange={(details) =>
                    setExpandedContacts(details.value)
                  }
                >
                  <Stack gap={3}>
                    {contacts.map((contact, index) => (
                      <Accordion.Item
                        key={index}
                        value={`contact-${index}`}
                        borderWidth="1px"
                        borderRadius="md"
                        borderColor="border.muted"
                        overflow="hidden"
                      >
                        <Accordion.ItemTrigger
                          px={4}
                          py={3}
                          cursor="pointer"
                          _hover={{ bg: 'bg.muted' }}
                        >
                          <Text
                            fontWeight="medium"
                            fontSize="sm"
                            flex={1}
                            textAlign="left"
                          >
                            {getContactSummary(contact, index)}
                          </Text>
                          <Accordion.ItemIndicator />
                        </Accordion.ItemTrigger>
                        <Accordion.ItemContent>
                          <Box px={4} pb={4} pt={2}>
                            <Flex justify="flex-end" mb={3}>
                              <Button
                                size="xs"
                                variant="outline"
                                colorPalette="red"
                                onClick={() => removeContact(index)}
                              >
                                Remove
                              </Button>
                            </Flex>

                            <Stack gap={4}>
                              <Field.Root maxW="200px">
                                <Field.Label>Contact Type</Field.Label>
                                <NativeSelect.Root>
                                  <NativeSelect.Field
                                    value={contact.contactType}
                                    onChange={(e) =>
                                      updateContact(
                                        index,
                                        'contactType',
                                        e.target.value
                                      )
                                    }
                                  >
                                    {ContactTypeOptions.map((type) => (
                                      <option key={type} value={type}>
                                        {formatEnumLabel(type)}
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
                                    onChange={(e) =>
                                      updateContact(
                                        index,
                                        'mobile',
                                        e.target.value
                                      )
                                    }
                                    placeholder="e.g., +63 912 345 6789"
                                  />
                                </Field.Root>

                                <Field.Root flex={1}>
                                  <Field.Label>Email</Field.Label>
                                  <Input
                                    type="email"
                                    value={contact.email}
                                    onChange={(e) =>
                                      updateContact(
                                        index,
                                        'email',
                                        e.target.value
                                      )
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
                                    onChange={(e) =>
                                      updateContact(
                                        index,
                                        'landLine',
                                        e.target.value
                                      )
                                    }
                                    placeholder="e.g., (02) 1234 5678"
                                  />
                                </Field.Root>

                                <Field.Root flex={1}>
                                  <Field.Label>Fax</Field.Label>
                                  <Input
                                    value={contact.fax}
                                    onChange={(e) =>
                                      updateContact(
                                        index,
                                        'fax',
                                        e.target.value
                                      )
                                    }
                                    placeholder="Fax number (optional)"
                                  />
                                </Field.Root>
                              </Flex>
                            </Stack>
                          </Box>
                        </Accordion.ItemContent>
                      </Accordion.Item>
                    ))}
                  </Stack>
                </Accordion.Root>
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
                          onChange={handleDocumentDescriptionChange}
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
            <Button variant="outline" onClick={() => navigate(-1)}>
              Cancel
            </Button>
            <Button type="submit" colorPalette="blue" loading={saving}>
              {isEditMode ? 'Update Person' : 'Create Person'}
            </Button>
          </Flex>
        </Stack>
      </form>
      <ConfirmDialog {...confirmDialog} />
    </Box>
  );
};

export default PersonFormPage;
