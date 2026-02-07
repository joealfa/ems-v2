import { useState, useEffect, useCallback } from 'react';
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
  UpsertEmploymentSchoolInput,
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
import { useToast } from '../../hooks';

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

  const [formData, setFormData] = useState<EmploymentFormData>(initialFormData);
  const [error, setError] = useState<string | null>(null);
  const { showSuccess, showError } = useToast();
  const [selectedPerson, setSelectedPerson] =
    useState<PersonListFieldsFragment | null>(null);

  // School assignment state - tracks all schools locally for batch update
  interface SchoolAssignmentData {
    displayId?: number; // Present for existing schools, undefined for new
    schoolDisplayId: number | '';
    schoolName: string;
    startDate: string;
    endDate: string | null;
    isCurrent: boolean;
    isActive: boolean;
  }
  const [schools, setSchools] = useState<SchoolAssignmentData[]>([]);

  // Accordion state - track which items are expanded
  const [expandedSchools, setExpandedSchools] = useState<string[]>([]);

  const loading = loadingEmployment || loadingPositions || loadingSalaryGrades;
  const saving = creating || updating;

  /* eslint-disable react-hooks/set-state-in-effect */
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

      // Load existing schools into local state
      if (employment.schools && employment.schools.length > 0) {
        setSchools(
          employment.schools
            .filter((s) => s !== null)
            .map((s) => ({
              displayId: s.displayId as number,
              schoolDisplayId: s.schoolDisplayId as number,
              schoolName: s.schoolName || '',
              startDate: s.startDate?.split('T')[0] || '',
              endDate: s.endDate?.split('T')[0] || null,
              isCurrent: s.isCurrent ?? false,
              isActive: s.isActive ?? true,
            }))
        );
      } else {
        setSchools([]);
      }
    }
  }, [isEditMode, employment]);
  /* eslint-enable react-hooks/set-state-in-effect */

  const handleChange = useCallback(
    (field: keyof EmploymentFormData, value: string | number | boolean) => {
      setFormData((prev) => ({ ...prev, [field]: value }));
    },
    []
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isEditMode) {
        // Build school upsert DTOs with displayId for edit mode
        const schoolDtos: UpsertEmploymentSchoolInput[] = schools
          .filter((s) => s.schoolDisplayId && s.startDate)
          .map((s) => ({
            displayId: s.displayId,
            schoolDisplayId: Number(s.schoolDisplayId),
            startDate: s.startDate,
            endDate: s.endDate || null,
            isCurrent: s.isCurrent,
          }));

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
          schools: schoolDtos,
        };
        await updateEmployment(Number(displayId), updateDto);
        showSuccess(
          'Employment Updated',
          `Employment record has been updated successfully.`
        );
      } else {
        // Build school create DTOs for create mode
        const schoolDtos: CreateEmploymentSchoolInput[] = schools
          .filter((s) => s.schoolDisplayId && s.startDate)
          .map((s) => ({
            schoolDisplayId: Number(s.schoolDisplayId),
            startDate: s.startDate,
            endDate: s.endDate || null,
            isCurrent: s.isCurrent,
          }));

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
          schools: schoolDtos.length > 0 ? schoolDtos : undefined,
        };
        await createEmployment(createDto);
        showSuccess(
          'Employment Created',
          `Employment record has been added successfully.`
        );
      }
      navigate('/employments');
    } catch (err) {
      console.error('Error saving employment:', err);
      const errorMessage = 'Failed to save employment';
      setError(errorMessage);
      showError('Operation Failed', errorMessage);
    }
  };

  // School handlers - inline editing like addresses
  const addSchool = () => {
    setSchools((prev) => {
      const newIndex = prev.length;
      setExpandedSchools([`school-${newIndex}`]); // Expand only the new item
      return [
        ...prev,
        {
          displayId: undefined,
          schoolDisplayId: '',
          schoolName: '',
          startDate: '',
          endDate: null,
          isCurrent: true,
          isActive: true,
        },
      ];
    });
  };

  const removeSchool = (index: number) => {
    setSchools((prev) => prev.filter((_, i) => i !== index));
    setExpandedSchools((prev) =>
      prev.filter((key) => key !== `school-${index}`)
    );
  };

  const updateSchool = useCallback(
    (
      index: number,
      field: keyof SchoolAssignmentData,
      value: string | number | boolean | null
    ) => {
      setSchools((prev) =>
        prev.map((school, i) =>
          i === index ? { ...school, [field]: value } : school
        )
      );
    },
    []
  );

  // Helper to get school summary for accordion header
  const getSchoolSummary = (school: SchoolAssignmentData, index: number) => {
    if (!school.schoolName) return `School ${index + 1} (New)`;
    return `${school.schoolName}${school.isCurrent ? ' (Current)' : ''}`;
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

      <form onSubmit={handleSubmit} noValidate>
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

          {/* School Assignments Section */}
          <Card.Root>
            <Card.Header>
              <Flex justify="space-between" align="center">
                <Heading size="md">School Assignments</Heading>
                <Button
                  size="sm"
                  variant="outline"
                  colorPalette="blue"
                  onClick={addSchool}
                >
                  + Add School
                </Button>
              </Flex>
            </Card.Header>
            <Card.Body>
              {schools.length === 0 ? (
                <Text color="fg.muted" fontSize="sm">
                  No school assignments. Click &quot;+ Add School&quot; to add
                  one.
                </Text>
              ) : (
                <Accordion.Root
                  multiple
                  value={expandedSchools}
                  onValueChange={(details) => setExpandedSchools(details.value)}
                >
                  <Stack gap={3}>
                    {schools.map((school, index) => (
                      <Accordion.Item
                        key={index}
                        value={`school-${index}`}
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
                            {getSchoolSummary(school, index)}
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
                                onClick={() => removeSchool(index)}
                              >
                                Remove
                              </Button>
                            </Flex>
                            <Stack gap={4}>
                              <Field.Root>
                                <Field.Label>School</Field.Label>
                                <SchoolSearchSelect
                                  value={school.schoolDisplayId}
                                  onChange={(displayId, selectedSchool) => {
                                    updateSchool(
                                      index,
                                      'schoolDisplayId',
                                      displayId || ''
                                    );
                                    updateSchool(
                                      index,
                                      'schoolName',
                                      selectedSchool?.schoolName || ''
                                    );
                                  }}
                                  placeholder="Search schools by name..."
                                />
                              </Field.Root>
                              <Flex gap={4}>
                                <Field.Root flex={1}>
                                  <Field.Label>Start Date</Field.Label>
                                  <Input
                                    type="date"
                                    value={school.startDate}
                                    onChange={(e) =>
                                      updateSchool(
                                        index,
                                        'startDate',
                                        e.target.value
                                      )
                                    }
                                  />
                                </Field.Root>
                                <Field.Root flex={1}>
                                  <Field.Label>End Date</Field.Label>
                                  <Input
                                    type="date"
                                    value={school.endDate || ''}
                                    onChange={(e) =>
                                      updateSchool(
                                        index,
                                        'endDate',
                                        e.target.value || null
                                      )
                                    }
                                  />
                                </Field.Root>
                              </Flex>
                              <Checkbox.Root
                                checked={school.isCurrent}
                                onCheckedChange={(e) =>
                                  updateSchool(index, 'isCurrent', !!e.checked)
                                }
                              >
                                <Checkbox.HiddenInput />
                                <Checkbox.Control />
                                <Checkbox.Label>
                                  Current Assignment
                                </Checkbox.Label>
                              </Checkbox.Root>
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
