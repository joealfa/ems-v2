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
  NativeSelect,
  Checkbox,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  useEmployment,
  useCreateEmployment,
  useUpdateEmployment,
} from '../../hooks/useEmployments';
import { usePositions } from '../../hooks/usePositions';
import { useSalaryGrades } from '../../hooks/useSalaryGrades';
import { useItems } from '../../hooks/useItems';
import type {
  CreateEmploymentInput,
  UpdateEmploymentInput,
  PersonListFieldsFragment,
  AppointmentStatus,
  EmploymentStatus,
  Eligibility,
} from '../../graphql/generated/graphql';
import PersonSearchSelect from '../../components/PersonSearchSelect';
import { formatEnumLabel } from '../../utils/formatters';
import {
  AppointmentStatusOptions,
  EmploymentStatusOptions,
  EligibilityOptions,
} from '../../utils/mapper';

interface EmploymentFormData {
  depEdId: string;
  psipopItemNumber: string;
  tinId: string;
  gsisId: string;
  philHealthId: string;
  dateOfOriginalAppointment: string;
  appointmentStatus: AppointmentStatus;
  employmentStatus: EmploymentStatus;
  eligibility: Eligibility;
  personDisplayId: number | '';
  positionDisplayId: number | '';
  salaryGradeDisplayId: number | '';
  itemDisplayId: number | '';
  isActive: boolean;
}

const initialFormData: EmploymentFormData = {
  depEdId: '',
  psipopItemNumber: '',
  tinId: '',
  gsisId: '',
  philHealthId: '',
  dateOfOriginalAppointment: '',
  appointmentStatus: 'Original',
  employmentStatus: 'Regular',
  eligibility: 'LET',
  personDisplayId: '',
  positionDisplayId: '',
  salaryGradeDisplayId: '',
  itemDisplayId: '',
  isActive: true,
};

const EmploymentFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  // GraphQL hooks
  const { employment, loading: loadingEmployment } = useEmployment(
    isEditMode ? Number(displayId) : 0
  );
  const { createEmployment, loading: creating } = useCreateEmployment();
  const { updateEmployment, loading: updating } = useUpdateEmployment();

  // Lookup data hooks
  const { positions, loading: loadingPositions } = usePositions({
    pageSize: 1000,
  });
  const { salaryGrades, loading: loadingSalaryGrades } = useSalaryGrades({
    pageSize: 1000,
  });
  const { items, loading: loadingItems } = useItems({ pageSize: 1000 });

  const [formData, setFormData] = useState<EmploymentFormData>(initialFormData);
  const [error, setError] = useState<string | null>(null);
  const [selectedPerson, setSelectedPerson] =
    useState<PersonListFieldsFragment | null>(null);

  const loading =
    loadingEmployment ||
    loadingPositions ||
    loadingSalaryGrades ||
    loadingItems;
  const saving = creating || updating;

  useEffect(() => {
    if (isEditMode && employment) {
      // eslint-disable-next-line react-hooks/set-state-in-effect -- Populating form with loaded data is a valid pattern
      setFormData({
        depEdId: employment.depEdId || '',
        psipopItemNumber: employment.psipopItemNumber || '',
        tinId: employment.tinId || '',
        gsisId: employment.gsisId || '',
        philHealthId: employment.philHealthId || '',
        dateOfOriginalAppointment:
          employment.dateOfOriginalAppointment?.split('T')[0] || '',
        appointmentStatus: employment.appointmentStatus || 'ORIGINAL',
        employmentStatus: employment.employmentStatus || 'REGULAR',
        eligibility: employment.eligibility || 'LET',
        personDisplayId: (employment.person?.displayId as number) || '',
        positionDisplayId: (employment.position?.displayId as number) || '',
        salaryGradeDisplayId:
          (employment.salaryGrade?.displayId as number) || '',
        itemDisplayId: (employment.item?.displayId as number) || '',
        isActive: employment.isActive ?? true,
      });
    }
  }, [isEditMode, employment]);

  const handleChange = (
    field: keyof EmploymentFormData,
    value: string | number | boolean
  ) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdateEmploymentInput = {
          depEdId: formData.depEdId || null,
          psipopItemNumber: formData.psipopItemNumber || null,
          tinId: formData.tinId || null,
          gsisId: formData.gsisId || null,
          philHealthId: formData.philHealthId || null,
          dateOfOriginalAppointment: formData.dateOfOriginalAppointment || null,
          appointmentStatus: formData.appointmentStatus,
          employmentStatus: formData.employmentStatus,
          eligibility: formData.eligibility,
          positionDisplayId: Number(formData.positionDisplayId),
          salaryGradeDisplayId: Number(formData.salaryGradeDisplayId),
          itemDisplayId: Number(formData.itemDisplayId),
          isActive: formData.isActive,
        };
        await updateEmployment(Number(displayId), updateDto);
      } else {
        const createDto: CreateEmploymentInput = {
          depEdId: formData.depEdId || null,
          psipopItemNumber: formData.psipopItemNumber || null,
          tinId: formData.tinId || null,
          gsisId: formData.gsisId || null,
          philHealthId: formData.philHealthId || null,
          dateOfOriginalAppointment: formData.dateOfOriginalAppointment || null,
          appointmentStatus: formData.appointmentStatus,
          employmentStatus: formData.employmentStatus,
          eligibility: formData.eligibility,
          personDisplayId: Number(formData.personDisplayId),
          positionDisplayId: Number(formData.positionDisplayId),
          salaryGradeDisplayId: Number(formData.salaryGradeDisplayId),
          itemDisplayId: Number(formData.itemDisplayId),
        };
        await createEmployment(createDto);
      }
      navigate('/employments');
    } catch (err) {
      console.error('Error saving employment:', err);
      setError('Failed to save employment');
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
          {isEditMode ? 'Edit Employment' : 'Add New Employment'}
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
          {!isEditMode && (
            <Card.Root>
              <Card.Header>
                <Heading size="md">Employee Selection</Heading>
              </Card.Header>
              <Card.Body>
                <Flex gap={6}>
                  <Box flex={1}>
                    <Field.Root required>
                      <Field.Label>Select Person</Field.Label>
                      <PersonSearchSelect
                        value={formData.personDisplayId}
                        onChange={(value, person) => {
                          handleChange('personDisplayId', value);
                          setSelectedPerson(person);
                        }}
                      />
                    </Field.Root>
                  </Box>
                  {selectedPerson && (
                    <Box
                      flex={1}
                      p={4}
                      borderWidth="1px"
                      borderRadius="md"
                      borderColor="border.muted"
                      bg="bg"
                    >
                      <Stack gap={2}>
                        <Box>
                          <Text fontSize="sm" color="fg.muted">
                            Full Name
                          </Text>
                          <Text fontWeight="medium">
                            {selectedPerson.fullName}
                          </Text>
                        </Box>
                        <Flex gap={6}>
                          <Box>
                            <Text fontSize="sm" color="fg.muted">
                              Date of Birth
                            </Text>
                            <Text>
                              {selectedPerson.dateOfBirth
                                ? new Date(
                                    selectedPerson.dateOfBirth
                                  ).toLocaleDateString()
                                : 'N/A'}
                            </Text>
                          </Box>
                          <Box>
                            <Text fontSize="sm" color="fg.muted">
                              Gender
                            </Text>
                            <Text>{selectedPerson.gender || 'N/A'}</Text>
                          </Box>
                          <Box>
                            <Text fontSize="sm" color="fg.muted">
                              Civil Status
                            </Text>
                            <Text>{selectedPerson.civilStatus || 'N/A'}</Text>
                          </Box>
                        </Flex>
                      </Stack>
                    </Box>
                  )}
                </Flex>
              </Card.Body>
            </Card.Root>
          )}

          <Card.Root>
            <Card.Header>
              <Heading size="md">Government IDs</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                <Flex gap={4}>
                  <Field.Root flex={1}>
                    <Field.Label>DepEd ID</Field.Label>
                    <Input
                      value={formData.depEdId}
                      onChange={(e) => handleChange('depEdId', e.target.value)}
                      placeholder="Enter DepEd ID"
                    />
                  </Field.Root>
                  <Field.Root flex={1}>
                    <Field.Label>PSIPOP Item Number</Field.Label>
                    <Input
                      value={formData.psipopItemNumber}
                      onChange={(e) =>
                        handleChange('psipopItemNumber', e.target.value)
                      }
                      placeholder="Enter PSIPOP Item Number"
                    />
                  </Field.Root>
                </Flex>

                <Flex gap={4}>
                  <Field.Root flex={1}>
                    <Field.Label>TIN</Field.Label>
                    <Input
                      value={formData.tinId}
                      onChange={(e) => handleChange('tinId', e.target.value)}
                      placeholder="Enter TIN"
                    />
                  </Field.Root>
                  <Field.Root flex={1}>
                    <Field.Label>GSIS ID</Field.Label>
                    <Input
                      value={formData.gsisId}
                      onChange={(e) => handleChange('gsisId', e.target.value)}
                      placeholder="Enter GSIS ID"
                    />
                  </Field.Root>
                  <Field.Root flex={1}>
                    <Field.Label>PhilHealth ID</Field.Label>
                    <Input
                      value={formData.philHealthId}
                      onChange={(e) =>
                        handleChange('philHealthId', e.target.value)
                      }
                      placeholder="Enter PhilHealth ID"
                    />
                  </Field.Root>
                </Flex>
              </Stack>
            </Card.Body>
          </Card.Root>

          <Card.Root>
            <Card.Header>
              <Heading size="md">Employment Details</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                <Flex gap={4}>
                  <Field.Root flex={1}>
                    <Field.Label>Date of Original Appointment</Field.Label>
                    <Input
                      type="date"
                      value={formData.dateOfOriginalAppointment}
                      onChange={(e) =>
                        handleChange(
                          'dateOfOriginalAppointment',
                          e.target.value
                        )
                      }
                    />
                  </Field.Root>
                  <Field.Root flex={1} required>
                    <Field.Label>Appointment Status</Field.Label>
                    <NativeSelect.Root>
                      <NativeSelect.Field
                        value={formData.appointmentStatus}
                        onChange={(e) =>
                          handleChange('appointmentStatus', e.target.value)
                        }
                      >
                        {AppointmentStatusOptions.map((status) => (
                          <option key={status} value={status}>
                            {formatEnumLabel(status)}
                          </option>
                        ))}
                      </NativeSelect.Field>
                      <NativeSelect.Indicator />
                    </NativeSelect.Root>
                  </Field.Root>
                </Flex>

                <Flex gap={4}>
                  <Field.Root flex={1} required>
                    <Field.Label>Employment Status</Field.Label>
                    <NativeSelect.Root>
                      <NativeSelect.Field
                        value={formData.employmentStatus}
                        onChange={(e) =>
                          handleChange('employmentStatus', e.target.value)
                        }
                      >
                        {EmploymentStatusOptions.map((status) => (
                          <option key={status} value={status}>
                            {formatEnumLabel(status)}
                          </option>
                        ))}
                      </NativeSelect.Field>
                      <NativeSelect.Indicator />
                    </NativeSelect.Root>
                  </Field.Root>
                  <Field.Root flex={1} required>
                    <Field.Label>Eligibility</Field.Label>
                    <NativeSelect.Root>
                      <NativeSelect.Field
                        value={formData.eligibility}
                        onChange={(e) =>
                          handleChange('eligibility', e.target.value)
                        }
                      >
                        {EligibilityOptions.map((elig) => (
                          <option key={elig} value={elig}>
                            {formatEnumLabel(elig)}
                          </option>
                        ))}
                      </NativeSelect.Field>
                      <NativeSelect.Indicator />
                    </NativeSelect.Root>
                  </Field.Root>
                </Flex>
              </Stack>
            </Card.Body>
          </Card.Root>

          <Card.Root>
            <Card.Header>
              <Heading size="md">Position & Compensation</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                <Flex gap={4}>
                  <Field.Root flex={1} required>
                    <Field.Label>Position</Field.Label>
                    <NativeSelect.Root>
                      <NativeSelect.Field
                        value={formData.positionDisplayId}
                        onChange={(e) =>
                          handleChange('positionDisplayId', e.target.value)
                        }
                      >
                        <option value="">-- Select Position --</option>
                        {positions
                          .filter((pos) => pos !== null)
                          .map((pos) => (
                            <option
                              key={pos.displayId as number}
                              value={pos.displayId as number}
                            >
                              {pos.titleName}
                            </option>
                          ))}
                      </NativeSelect.Field>
                      <NativeSelect.Indicator />
                    </NativeSelect.Root>
                  </Field.Root>
                  <Field.Root flex={1} required>
                    <Field.Label>Salary Grade</Field.Label>
                    <NativeSelect.Root>
                      <NativeSelect.Field
                        value={formData.salaryGradeDisplayId}
                        onChange={(e) =>
                          handleChange('salaryGradeDisplayId', e.target.value)
                        }
                      >
                        <option value="">-- Select Salary Grade --</option>
                        {salaryGrades
                          .filter((sg) => sg !== null)
                          .map((sg) => (
                            <option
                              key={sg.displayId as number}
                              value={sg.displayId as number}
                            >
                              {sg.salaryGradeName} - Step {sg.step as number}
                            </option>
                          ))}
                      </NativeSelect.Field>
                      <NativeSelect.Indicator />
                    </NativeSelect.Root>
                  </Field.Root>
                </Flex>

                <Field.Root required>
                  <Field.Label>Plantilla Item</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={formData.itemDisplayId}
                      onChange={(e) =>
                        handleChange('itemDisplayId', e.target.value)
                      }
                    >
                      <option value="">-- Select Item --</option>
                      {items
                        .filter((item) => item !== null)
                        .map((item) => (
                          <option
                            key={item.displayId as number}
                            value={item.displayId as number}
                          >
                            {item.itemName}
                          </option>
                        ))}
                    </NativeSelect.Field>
                    <NativeSelect.Indicator />
                  </NativeSelect.Root>
                </Field.Root>

                {isEditMode && (
                  <Checkbox.Root
                    checked={formData.isActive}
                    onCheckedChange={(e) =>
                      handleChange('isActive', !!e.checked)
                    }
                  >
                    <Checkbox.HiddenInput />
                    <Checkbox.Control />
                    <Checkbox.Label>Active</Checkbox.Label>
                  </Checkbox.Root>
                )}
              </Stack>
            </Card.Body>
          </Card.Root>
        </Stack>

        <Flex justify="flex-end" mt={6} gap={4}>
          <Button variant="outline" onClick={() => navigate(-1)}>
            Cancel
          </Button>
          <Button type="submit" colorPalette="blue" loading={saving}>
            {isEditMode ? 'Update Employment' : 'Create Employment'}
          </Button>
        </Flex>
      </form>
    </Box>
  );
};

export default EmploymentFormPage;
