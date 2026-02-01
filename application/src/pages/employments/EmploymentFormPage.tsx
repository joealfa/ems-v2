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
  Accordion,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  useEmployment,
  useCreateEmployment,
  useUpdateEmployment,
  useAddSchoolToEmployment,
  useRemoveSchoolFromEmployment,
} from '../../hooks/useEmployments';
import { usePositions } from '../../hooks/usePositions';
import { useSalaryGrades } from '../../hooks/useSalaryGrades';
import type {
  CreateEmploymentInput,
  UpdateEmploymentInput,
  PersonListFieldsFragment,
  AppointmentStatus,
  EmploymentStatus,
  Eligibility,
  CreateEmploymentSchoolInput,
} from '../../graphql/generated/graphql';
import PersonSearchSelect from '../../components/PersonSearchSelect';
import ItemSearchSelect from '../../components/ItemSearchSelect';
import SchoolSearchSelect from '../../components/SchoolSearchSelect';
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
  const {
    employment,
    loading: loadingEmployment,
    refetch: refetchEmployment,
  } = useEmployment(isEditMode ? Number(displayId) : 0);
  const { createEmployment, loading: creating } = useCreateEmployment();
  const { updateEmployment, loading: updating } = useUpdateEmployment();
  const { addSchoolToEmployment, loading: addingSchool } =
    useAddSchoolToEmployment();
  const { removeSchoolFromEmployment } = useRemoveSchoolFromEmployment();

  // Lookup data hooks
  const { positions, loading: loadingPositions } = usePositions({
    pageSize: 1000,
  });
  const { salaryGrades, loading: loadingSalaryGrades } = useSalaryGrades({
    pageSize: 1000,
  });

  const [formData, setFormData] = useState<EmploymentFormData>(initialFormData);
  const [error, setError] = useState<string | null>(null);
  const [selectedPerson, setSelectedPerson] =
    useState<PersonListFieldsFragment | null>(null);

  // School assignment form state
  interface SchoolFormData {
    schoolDisplayId: number | '';
    startDate: string;
    endDate: string | null;
    isCurrent: boolean;
  }
  const [schoolFormData, setSchoolFormData] = useState<SchoolFormData>({
    schoolDisplayId: '',
    startDate: '',
    endDate: null,
    isCurrent: true,
  });
  const [showAddSchoolForm, setShowAddSchoolForm] = useState(false);
  const [removingSchoolId, setRemovingSchoolId] = useState<number | null>(null);

  const loading = loadingEmployment || loadingPositions || loadingSalaryGrades;
  const saving = creating || updating;

  useEffect(() => {
    if (isEditMode && employment) {
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

  const handleAddSchool = async () => {
    if (
      !isEditMode ||
      !schoolFormData.schoolDisplayId ||
      !schoolFormData.startDate
    ) {
      return;
    }

    try {
      const input: CreateEmploymentSchoolInput = {
        schoolDisplayId: Number(schoolFormData.schoolDisplayId),
        startDate: schoolFormData.startDate,
        endDate: schoolFormData.endDate || null,
        isCurrent: schoolFormData.isCurrent,
      };
      await addSchoolToEmployment(Number(displayId), input);
      // Reset form and hide it
      setSchoolFormData({
        schoolDisplayId: '',
        startDate: '',
        endDate: null,
        isCurrent: true,
      });
      setShowAddSchoolForm(false);
      // Refetch employment to get updated schools
      await refetchEmployment();
    } catch (err) {
      console.error('Error adding school:', err);
      setError('Failed to add school assignment');
    }
  };

  const handleRemoveSchool = async (schoolAssignmentDisplayId: number) => {
    if (!isEditMode) return;

    setRemovingSchoolId(schoolAssignmentDisplayId);
    try {
      await removeSchoolFromEmployment(
        Number(displayId),
        schoolAssignmentDisplayId
      );
      await refetchEmployment();
    } catch (err) {
      console.error('Error removing school:', err);
      setError('Failed to remove school assignment');
    } finally {
      setRemovingSchoolId(null);
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
                  <ItemSearchSelect
                    value={formData.itemDisplayId}
                    onChange={(displayId) =>
                      handleChange('itemDisplayId', displayId)
                    }
                    initialItem={
                      isEditMode && employment?.item
                        ? {
                            displayId: employment.item.displayId as number,
                            itemName: employment.item.itemName ?? null,
                          }
                        : null
                    }
                    placeholder="Search items by name..."
                  />
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

          {/* School Assignments Section - Only show in edit mode */}
          {isEditMode && (
            <Card.Root>
              <Card.Header>
                <Flex justify="space-between" align="center">
                  <Heading size="md">School Assignments</Heading>
                  {!showAddSchoolForm && (
                    <Button
                      size="sm"
                      variant="outline"
                      colorPalette="blue"
                      onClick={() => setShowAddSchoolForm(true)}
                    >
                      Add School
                    </Button>
                  )}
                </Flex>
              </Card.Header>
              <Card.Body>
                {employment?.schools &&
                employment.schools.filter(Boolean).length > 0 ? (
                  <Accordion.Root
                    multiple
                    defaultValue={employment.schools
                      .filter(Boolean)
                      .map((s) => `school-${s!.displayId}`)}
                  >
                    <Stack gap={3}>
                      {employment.schools.filter(Boolean).map((school) => (
                        <Accordion.Item
                          key={school!.displayId}
                          value={`school-${school!.displayId}`}
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
                              {school!.schoolName}
                              {school!.isCurrent && ' (Current)'}
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
                                  onClick={() =>
                                    handleRemoveSchool(
                                      school!.displayId as number
                                    )
                                  }
                                  loading={
                                    removingSchoolId === school!.displayId
                                  }
                                >
                                  Remove
                                </Button>
                              </Flex>
                              <Stack gap={3}>
                                <Flex gap={6}>
                                  <Box>
                                    <Text fontSize="sm" color="fg.muted">
                                      Start Date
                                    </Text>
                                    <Text>
                                      {school!.startDate
                                        ? new Date(
                                            school!.startDate
                                          ).toLocaleDateString()
                                        : 'N/A'}
                                    </Text>
                                  </Box>
                                  <Box>
                                    <Text fontSize="sm" color="fg.muted">
                                      End Date
                                    </Text>
                                    <Text>
                                      {school!.endDate
                                        ? new Date(
                                            school!.endDate
                                          ).toLocaleDateString()
                                        : 'N/A'}
                                    </Text>
                                  </Box>
                                  <Box>
                                    <Text fontSize="sm" color="fg.muted">
                                      Current
                                    </Text>
                                    <Text>
                                      {school!.isCurrent ? 'Yes' : 'No'}
                                    </Text>
                                  </Box>
                                  <Box>
                                    <Text fontSize="sm" color="fg.muted">
                                      Active
                                    </Text>
                                    <Text>
                                      {school!.isActive ? 'Yes' : 'No'}
                                    </Text>
                                  </Box>
                                </Flex>
                              </Stack>
                            </Box>
                          </Accordion.ItemContent>
                        </Accordion.Item>
                      ))}
                    </Stack>
                  </Accordion.Root>
                ) : (
                  <Text color="fg.muted" fontSize="sm">
                    No school assignments. Click &quot;Add School&quot; to add
                    one.
                  </Text>
                )}

                {/* Add New School Assignment Form */}
                {showAddSchoolForm && (
                  <Accordion.Root
                    multiple
                    defaultValue={['new-school']}
                    mt={
                      employment?.schools &&
                      employment.schools.filter(Boolean).length > 0
                        ? 0
                        : undefined
                    }
                  >
                    <Accordion.Item
                      value="new-school"
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
                          New School Assignment
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
                              onClick={() => {
                                setShowAddSchoolForm(false);
                                setSchoolFormData({
                                  schoolDisplayId: '',
                                  startDate: '',
                                  endDate: null,
                                  isCurrent: true,
                                });
                              }}
                            >
                              Cancel
                            </Button>
                          </Flex>
                          <Stack gap={4}>
                            <Field.Root required>
                              <Field.Label>School</Field.Label>
                              <SchoolSearchSelect
                                value={schoolFormData.schoolDisplayId}
                                onChange={(displayId) =>
                                  setSchoolFormData((prev) => ({
                                    ...prev,
                                    schoolDisplayId: displayId,
                                  }))
                                }
                                placeholder="Search schools by name..."
                              />
                            </Field.Root>
                            <Flex gap={4}>
                              <Field.Root flex={1} required>
                                <Field.Label>Start Date</Field.Label>
                                <Input
                                  type="date"
                                  value={schoolFormData.startDate}
                                  onChange={(e) =>
                                    setSchoolFormData((prev) => ({
                                      ...prev,
                                      startDate: e.target.value,
                                    }))
                                  }
                                />
                              </Field.Root>
                              <Field.Root flex={1}>
                                <Field.Label>End Date</Field.Label>
                                <Input
                                  type="date"
                                  value={schoolFormData.endDate || ''}
                                  onChange={(e) =>
                                    setSchoolFormData((prev) => ({
                                      ...prev,
                                      endDate: e.target.value || null,
                                    }))
                                  }
                                />
                              </Field.Root>
                            </Flex>
                            <Checkbox.Root
                              checked={schoolFormData.isCurrent}
                              onCheckedChange={(e) =>
                                setSchoolFormData((prev) => ({
                                  ...prev,
                                  isCurrent: !!e.checked,
                                }))
                              }
                            >
                              <Checkbox.HiddenInput />
                              <Checkbox.Control />
                              <Checkbox.Label>
                                Current Assignment
                              </Checkbox.Label>
                            </Checkbox.Root>
                            <Button
                              colorPalette="blue"
                              onClick={handleAddSchool}
                              loading={addingSchool}
                              disabled={
                                !schoolFormData.schoolDisplayId ||
                                !schoolFormData.startDate
                              }
                              alignSelf="flex-start"
                            >
                              Save School
                            </Button>
                          </Stack>
                        </Box>
                      </Accordion.ItemContent>
                    </Accordion.Item>
                  </Accordion.Root>
                )}
              </Card.Body>
            </Card.Root>
          )}
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
