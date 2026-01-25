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
  employmentsApi,
  personsApi,
  positionsApi,
  salaryGradesApi,
  itemsApi,
  type CreateEmploymentDto,
  type UpdateEmploymentDto,
  type EmploymentResponseDto,
  type PersonListDto,
  type PositionResponseDto,
  type SalaryGradeResponseDto,
  type ItemResponseDto,
  CreateEmploymentDtoAppointmentStatusEnum,
  CreateEmploymentDtoEmploymentStatusEnum,
  CreateEmploymentDtoEligibilityEnum,
} from '../../api';

interface EmploymentFormData {
  depEdId: string;
  psipopItemNumber: string;
  tinId: string;
  gsisId: string;
  philHealthId: string;
  dateOfOriginalAppointment: string;
  appointmentStatus: string;
  employmentStatus: string;
  eligibility: string;
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

  const [formData, setFormData] = useState<EmploymentFormData>(initialFormData);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [persons, setPersons] = useState<PersonListDto[]>([]);
  const [positions, setPositions] = useState<PositionResponseDto[]>([]);
  const [salaryGrades, setSalaryGrades] = useState<SalaryGradeResponseDto[]>(
    []
  );
  const [items, setItems] = useState<ItemResponseDto[]>([]);

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    loadLookupData();
    if (isEditMode) {
      loadEmployment();
    }
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadLookupData = async () => {
    try {
      const [personsRes, positionsRes, salaryGradesRes, itemsRes] =
        await Promise.all([
          personsApi.apiV1PersonsGet(
            undefined,
            undefined,
            undefined,
            undefined,
            1,
            1000
          ),
          positionsApi.apiV1PositionsGet(1, 1000),
          salaryGradesApi.apiV1SalarygradesGet(1, 1000),
          itemsApi.apiV1ItemsGet(1, 1000),
        ]);
      setPersons(personsRes.data.items || []);
      setPositions(positionsRes.data.items || []);
      setSalaryGrades(salaryGradesRes.data.items || []);
      setItems(itemsRes.data.items || []);
    } catch (err) {
      console.error('Error loading lookup data:', err);
    }
  };

  const loadEmployment = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await employmentsApi.apiV1EmploymentsDisplayIdGet(
        Number(displayId)
      );
      const employment: EmploymentResponseDto = response.data;
      setFormData({
        depEdId: employment.depEdId || '',
        psipopItemNumber: employment.psipopItemNumber || '',
        tinId: employment.tinId || '',
        gsisId: employment.gsisId || '',
        philHealthId: employment.philHealthId || '',
        dateOfOriginalAppointment:
          employment.dateOfOriginalAppointment?.split('T')[0] || '',
        appointmentStatus: employment.appointmentStatus || 'Original',
        employmentStatus: employment.employmentStatus || 'Regular',
        eligibility: employment.eligibility || 'LET',
        personDisplayId: employment.person?.displayId || '',
        positionDisplayId: employment.position?.displayId || '',
        salaryGradeDisplayId: employment.salaryGrade?.displayId || '',
        itemDisplayId: employment.item?.displayId || '',
        isActive: employment.isActive ?? true,
      });
    } catch (err) {
      console.error('Error loading employment:', err);
      setError('Failed to load employment data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (
    field: keyof EmploymentFormData,
    value: string | number | boolean
  ) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdateEmploymentDto = {
          depEdId: formData.depEdId || null,
          psipopItemNumber: formData.psipopItemNumber || null,
          tinId: formData.tinId || null,
          gsisId: formData.gsisId || null,
          philHealthId: formData.philHealthId || null,
          dateOfOriginalAppointment: formData.dateOfOriginalAppointment || null,
          appointmentStatus:
            formData.appointmentStatus as UpdateEmploymentDto['appointmentStatus'],
          employmentStatus:
            formData.employmentStatus as UpdateEmploymentDto['employmentStatus'],
          eligibility:
            formData.eligibility as UpdateEmploymentDto['eligibility'],
          positionDisplayId: Number(formData.positionDisplayId),
          salaryGradeDisplayId: Number(formData.salaryGradeDisplayId),
          itemDisplayId: Number(formData.itemDisplayId),
          isActive: formData.isActive,
        };
        await employmentsApi.apiV1EmploymentsDisplayIdPut(
          Number(displayId),
          updateDto
        );
      } else {
        const createDto: CreateEmploymentDto = {
          depEdId: formData.depEdId || null,
          psipopItemNumber: formData.psipopItemNumber || null,
          tinId: formData.tinId || null,
          gsisId: formData.gsisId || null,
          philHealthId: formData.philHealthId || null,
          dateOfOriginalAppointment: formData.dateOfOriginalAppointment || null,
          appointmentStatus:
            formData.appointmentStatus as CreateEmploymentDto['appointmentStatus'],
          employmentStatus:
            formData.employmentStatus as CreateEmploymentDto['employmentStatus'],
          eligibility:
            formData.eligibility as CreateEmploymentDto['eligibility'],
          personDisplayId: Number(formData.personDisplayId),
          positionDisplayId: Number(formData.positionDisplayId),
          salaryGradeDisplayId: Number(formData.salaryGradeDisplayId),
          itemDisplayId: Number(formData.itemDisplayId),
        };
        await employmentsApi.apiV1EmploymentsPost(createDto);
      }
      navigate('/employments');
    } catch (err) {
      console.error('Error saving employment:', err);
      setError('Failed to save employment');
    } finally {
      setSaving(false);
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
        <Button variant="outline" onClick={() => navigate('/employments')}>
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
                <Field.Root required>
                  <Field.Label>Select Person</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={formData.personDisplayId}
                      onChange={e =>
                        handleChange('personDisplayId', e.target.value)
                      }
                    >
                      <option value="">-- Select a Person --</option>
                      {persons.map(person => (
                        <option key={person.displayId} value={person.displayId}>
                          {person.fullName} ({person.displayId})
                        </option>
                      ))}
                    </NativeSelect.Field>
                    <NativeSelect.Indicator />
                  </NativeSelect.Root>
                </Field.Root>
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
                      onChange={e => handleChange('depEdId', e.target.value)}
                      placeholder="Enter DepEd ID"
                    />
                  </Field.Root>
                  <Field.Root flex={1}>
                    <Field.Label>PSIPOP Item Number</Field.Label>
                    <Input
                      value={formData.psipopItemNumber}
                      onChange={e =>
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
                      onChange={e => handleChange('tinId', e.target.value)}
                      placeholder="Enter TIN"
                    />
                  </Field.Root>
                  <Field.Root flex={1}>
                    <Field.Label>GSIS ID</Field.Label>
                    <Input
                      value={formData.gsisId}
                      onChange={e => handleChange('gsisId', e.target.value)}
                      placeholder="Enter GSIS ID"
                    />
                  </Field.Root>
                  <Field.Root flex={1}>
                    <Field.Label>PhilHealth ID</Field.Label>
                    <Input
                      value={formData.philHealthId}
                      onChange={e =>
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
                      onChange={e =>
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
                        onChange={e =>
                          handleChange('appointmentStatus', e.target.value)
                        }
                      >
                        {Object.values(
                          CreateEmploymentDtoAppointmentStatusEnum
                        ).map(status => (
                          <option key={status} value={status}>
                            {status}
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
                        onChange={e =>
                          handleChange('employmentStatus', e.target.value)
                        }
                      >
                        {Object.values(
                          CreateEmploymentDtoEmploymentStatusEnum
                        ).map(status => (
                          <option key={status} value={status}>
                            {status}
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
                        onChange={e =>
                          handleChange('eligibility', e.target.value)
                        }
                      >
                        {Object.values(CreateEmploymentDtoEligibilityEnum).map(
                          elig => (
                            <option key={elig} value={elig}>
                              {elig === 'CivilServiceProfessional'
                                ? 'Civil Service Professional'
                                : elig === 'CivilServiceSubProfessional'
                                  ? 'Civil Service Sub-Professional'
                                  : elig}
                            </option>
                          )
                        )}
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
                        onChange={e =>
                          handleChange('positionDisplayId', e.target.value)
                        }
                      >
                        <option value="">-- Select Position --</option>
                        {positions.map(pos => (
                          <option key={pos.displayId} value={pos.displayId}>
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
                        onChange={e =>
                          handleChange('salaryGradeDisplayId', e.target.value)
                        }
                      >
                        <option value="">-- Select Salary Grade --</option>
                        {salaryGrades.map(sg => (
                          <option key={sg.displayId} value={sg.displayId}>
                            {sg.salaryGradeName} - Step {sg.step}
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
                      onChange={e =>
                        handleChange('itemDisplayId', e.target.value)
                      }
                    >
                      <option value="">-- Select Item --</option>
                      {items.map(item => (
                        <option key={item.displayId} value={item.displayId}>
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
        </Stack>

        <Flex justify="flex-end" mt={6} gap={4}>
          <Button variant="outline" onClick={() => navigate('/employments')}>
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
