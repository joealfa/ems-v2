import { useState, useEffect } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Input,
  Card,
  Stack,
  Field,
  Spinner,
  Text,
  Checkbox,
  NativeSelect,
  Accordion,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  useSchool,
  useCreateSchool,
  useUpdateSchool,
} from '../../hooks/useSchools';
import type {
  CreateSchoolInput,
  UpdateSchoolInput,
  CreateAddressInput,
  CreateContactInput,
  UpsertAddressInput,
  UpsertContactInput,
} from '../../graphql/generated/graphql';

// Enum definitions matching backend values
const AddressTypeEnum = {
  Home: 0,
  Business: 1,
  Other: 2,
} as const;

const AddressTypeOptions = ['Home', 'Business', 'Other'];

const ContactTypeEnum = {
  Personal: 0,
  Work: 1,
  Emergency: 2,
  Other: 3,
} as const;

const ContactTypeOptions = ['Personal', 'Work', 'Emergency', 'Other'];

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
  addressType: string;
}

interface ContactFormData {
  displayId?: number;
  mobile: string;
  landLine: string;
  fax: string;
  email: string;
  contactType: string;
}

interface SchoolFormData {
  schoolName: string;
  isActive: boolean;
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
  addressType: 'Business',
};

const initialContactData: ContactFormData = {
  mobile: '',
  landLine: '',
  fax: '',
  email: '',
  contactType: 'Work',
};

const initialFormData: SchoolFormData = {
  schoolName: '',
  isActive: true,
};

const SchoolFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  // GraphQL hooks
  const { school, loading: loadingSchool } = useSchool(
    isEditMode ? Number(displayId) : 0
  );
  const { createSchool, loading: creating } = useCreateSchool();
  const { updateSchool, loading: updating } = useUpdateSchool();

  const [formData, setFormData] = useState<SchoolFormData>(initialFormData);
  const [addresses, setAddresses] = useState<AddressFormData[]>([]);
  const [contacts, setContacts] = useState<ContactFormData[]>([]);
  const [error, setError] = useState<string | null>(null);

  const loading = loadingSchool;
  const saving = creating || updating;

  useEffect(() => {
    if (isEditMode && school) {
      // Map GraphQL uppercase enum strings to form display values
      const addressTypeMap: Record<string, string> = {
        HOME: 'Home',
        BUSINESS: 'Business',
        OTHER: 'Other',
      };
      const contactTypeMap: Record<string, string> = {
        PERSONAL: 'Personal',
        WORK: 'Work',
        EMERGENCY: 'Emergency',
        OTHER: 'Other',
      };

      // eslint-disable-next-line react-hooks/set-state-in-effect -- Populating form with loaded data is a valid pattern
      setFormData({
        schoolName: school.schoolName || '',
        isActive: school.isActive ?? true,
      });

      // Load existing addresses
      if (school.addresses && school.addresses.length > 0) {
        setAddresses(
          school.addresses
            .filter(addr => addr !== null)
            .map(addr => ({
              displayId: addr.displayId as number,
              address1: addr.address1 || '',
              address2: addr.address2 || '',
              barangay: addr.barangay || '',
              city: addr.city || '',
              province: addr.province || '',
              country: addr.country || 'Philippines',
              zipCode: addr.zipCode || '',
              isCurrent: addr.isCurrent || false,
              isPermanent: addr.isPermanent || false,
              addressType: addressTypeMap[addr.addressType] || 'Business',
            }))
        );
      }

      // Load existing contacts
      if (school.contacts && school.contacts.length > 0) {
        setContacts(
          school.contacts
            .filter(contact => contact !== null)
            .map(contact => ({
              displayId: contact.displayId as number,
              mobile: contact.mobile || '',
              landLine: contact.landLine || '',
              fax: contact.fax || '',
              email: contact.email || '',
              contactType: contactTypeMap[contact.contactType] || 'Work',
            }))
        );
      }
    }
  }, [isEditMode, school]);

  const handleChange = (
    field: keyof SchoolFormData,
    value: string | boolean
  ) => {
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
    return `${address.addressType}: ${summary}${tagText}`;
  };

  // Generate contact summary for accordion header
  const getContactSummary = (contact: ContactFormData, index: number) => {
    const parts = [];
    if (contact.mobile) parts.push(contact.mobile);
    if (contact.email) parts.push(contact.email);
    if (contact.landLine) parts.push(contact.landLine);

    const summary =
      parts.length > 0 ? parts.join(' | ') : `Contact ${index + 1}`;
    return `${contact.contactType}: ${summary}`;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isEditMode) {
        const addressDtos: UpsertAddressInput[] = addresses
          .filter(addr => addr.address1 && addr.city && addr.province)
          .map(addr => ({
            displayId: addr.displayId || null,
            address1: addr.address1,
            address2: addr.address2 || null,
            barangay: addr.barangay || null,
            city: addr.city,
            province: addr.province,
            country: addr.country || null,
            zipCode: addr.zipCode || null,
            isCurrent: addr.isCurrent,
            isPermanent: addr.isPermanent,
            addressType:
              AddressTypeEnum[addr.addressType as keyof typeof AddressTypeEnum],
          }));

        const contactDtos: UpsertContactInput[] = contacts
          .filter(
            contact => contact.mobile || contact.email || contact.landLine
          )
          .map(contact => ({
            displayId: contact.displayId || null,
            mobile: contact.mobile || null,
            landLine: contact.landLine || null,
            fax: contact.fax || null,
            email: contact.email || null,
            contactType:
              ContactTypeEnum[
                contact.contactType as keyof typeof ContactTypeEnum
              ],
          }));

        const updateDto: UpdateSchoolInput = {
          schoolName: formData.schoolName,
          isActive: formData.isActive,
          addresses: addressDtos.length > 0 ? addressDtos : [],
          contacts: contactDtos.length > 0 ? contactDtos : [],
        };
        await updateSchool(Number(displayId), updateDto);
      } else {
        const addressDtos: CreateAddressInput[] = addresses
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
            addressType:
              AddressTypeEnum[addr.addressType as keyof typeof AddressTypeEnum],
          }));

        const contactDtos: CreateContactInput[] = contacts
          .filter(
            contact => contact.mobile || contact.email || contact.landLine
          )
          .map(contact => ({
            mobile: contact.mobile || null,
            landLine: contact.landLine || null,
            fax: contact.fax || null,
            email: contact.email || null,
            contactType:
              ContactTypeEnum[
                contact.contactType as keyof typeof ContactTypeEnum
              ],
          }));

        const createDto: CreateSchoolInput = {
          schoolName: formData.schoolName,
          addresses: addressDtos.length > 0 ? addressDtos : null,
          contacts: contactDtos.length > 0 ? contactDtos : null,
        };
        await createSchool(createDto);
      }
      navigate('/schools');
    } catch (err) {
      console.error('Error saving school:', err);
      setError('Failed to save school');
    }
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
          {isEditMode ? 'Edit School' : 'Add New School'}
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

      <form onSubmit={handleSubmit}>
        <Stack gap={6}>
          {/* School Information */}
          <Card.Root>
            <Card.Header>
              <Heading size="md">School Information</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                <Field.Root required>
                  <Field.Label>School Name</Field.Label>
                  <Input
                    value={formData.schoolName}
                    onChange={e => handleChange('schoolName', e.target.value)}
                    placeholder="Enter school name"
                  />
                </Field.Root>

                {isEditMode && (
                  <Checkbox.Root
                    checked={formData.isActive}
                    onCheckedChange={e => handleChange('isActive', !!e.checked)}
                  >
                    <Checkbox.HiddenInput />
                    <Checkbox.Control />
                    <Checkbox.Label>Active</Checkbox.Label>
                  </Checkbox.Root>
                )}
              </Stack>
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
                  defaultValue={addresses.map((_, i) => `address-${i}`)}
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
                                      onChange={e =>
                                        updateAddress(
                                          index,
                                          'addressType',
                                          e.target.value
                                        )
                                      }
                                    >
                                      {AddressTypeOptions.map((type, idx) => (
                                        <option key={idx} value={type}>
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
                                    onCheckedChange={e =>
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

                              <Field.Root required>
                                <Field.Label>Street Address</Field.Label>
                                <Input
                                  value={address.address1}
                                  onChange={e =>
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
                                  onChange={e =>
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
                                    onChange={e =>
                                      updateAddress(
                                        index,
                                        'barangay',
                                        e.target.value
                                      )
                                    }
                                    placeholder="Barangay"
                                  />
                                </Field.Root>

                                <Field.Root flex={1} required>
                                  <Field.Label>City/Municipality</Field.Label>
                                  <Input
                                    value={address.city}
                                    onChange={e =>
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
                                <Field.Root flex={1} required>
                                  <Field.Label>Province</Field.Label>
                                  <Input
                                    value={address.province}
                                    onChange={e =>
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
                                    onChange={e =>
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
                                    onChange={e =>
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
                  defaultValue={contacts.map((_, i) => `contact-${i}`)}
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
                                    onChange={e =>
                                      updateContact(
                                        index,
                                        'contactType',
                                        e.target.value
                                      )
                                    }
                                  >
                                    {ContactTypeOptions.map(type => (
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
                                    onChange={e =>
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
                                    onChange={e =>
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
                                    onChange={e =>
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

          {/* Submit Buttons */}
          <Flex justify="flex-end" gap={4}>
            <Button variant="outline" onClick={() => navigate(-1)}>
              Cancel
            </Button>
            <Button type="submit" colorPalette="blue" loading={saving}>
              {isEditMode ? 'Update School' : 'Create School'}
            </Button>
          </Flex>
        </Stack>
      </form>
    </Box>
  );
};

export default SchoolFormPage;
