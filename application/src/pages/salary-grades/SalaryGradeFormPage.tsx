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
  Textarea,
  Checkbox,
  NativeSelect,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  useSalaryGrade,
  useCreateSalaryGrade,
  useUpdateSalaryGrade,
} from '../../hooks/useSalaryGrades';
import type {
  CreateSalaryGradeInput,
  UpdateSalaryGradeInput,
} from '../../graphql/generated/graphql';
import { useToast } from '../../hooks';

interface SalaryGradeFormData {
  salaryGradeName: string;
  description: string;
  step: number;
  monthlySalary: number;
  isActive: boolean;
}

const initialFormData: SalaryGradeFormData = {
  salaryGradeName: '',
  description: '',
  step: 1,
  monthlySalary: 0,
  isActive: true,
};

const SalaryGradeFormPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();
  const isEditMode = displayId && displayId !== 'new';

  const { salaryGrade, loading: loadingSalaryGrade } = useSalaryGrade(
    isEditMode ? Number(displayId) : 0
  );
  const { createSalaryGrade, loading: creating } = useCreateSalaryGrade();
  const { updateSalaryGrade, loading: updating } = useUpdateSalaryGrade();

  const [formData, setFormData] =
    useState<SalaryGradeFormData>(initialFormData);
  const [error, setError] = useState<string | null>(null);
  const { showSuccess, showError } = useToast();

  const loading = loadingSalaryGrade;
  const saving = creating || updating;

  useEffect(() => {
    if (isEditMode && salaryGrade) {
      // eslint-disable-next-line react-hooks/set-state-in-effect -- Populating form with loaded data is a valid pattern
      setFormData({
        salaryGradeName: salaryGrade.salaryGradeName || '',
        description: salaryGrade.description || '',
        step: (salaryGrade.step as unknown as number) || 1,
        monthlySalary: (salaryGrade.monthlySalary as unknown as number) || 0,
        isActive: salaryGrade.isActive ?? true,
      });
    }
  }, [isEditMode, salaryGrade]);

  const handleChange = useCallback(
    (field: keyof SalaryGradeFormData, value: string | number | boolean) => {
      setFormData((prev) => ({ ...prev, [field]: value }));
    },
    []
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isEditMode) {
        const updateDto: UpdateSalaryGradeInput = {
          salaryGradeName: formData.salaryGradeName,
          description: formData.description || null,
          step: formData.step,
          monthlySalary: formData.monthlySalary,
          isActive: formData.isActive,
        };
        await updateSalaryGrade(Number(displayId), updateDto);
        showSuccess(
          'Salary Grade Updated',
          `${formData.salaryGradeName} has been updated successfully.`
        );
      } else {
        const createDto: CreateSalaryGradeInput = {
          salaryGradeName: formData.salaryGradeName,
          description: formData.description || null,
          step: formData.step,
          monthlySalary: formData.monthlySalary,
        };
        await createSalaryGrade(createDto);
        showSuccess(
          'Salary Grade Created',
          `${formData.salaryGradeName} has been added successfully.`
        );
      }
      navigate('/salary-grades');
    } catch (err) {
      console.error('Error saving salary grade:', err);
      const errorMessage = 'Failed to save salary grade';
      setError(errorMessage);
      showError('Operation Failed', errorMessage);
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
    <Box>
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">
          {isEditMode ? 'Edit Salary Grade' : 'Add New Salary Grade'}
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
        <Card.Root>
          <Card.Header>
            <Heading size="md">Salary Grade Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Field.Root required>
                <Field.Label>Salary Grade Name</Field.Label>
                <Input
                  value={formData.salaryGradeName}
                  onChange={(e) =>
                    handleChange('salaryGradeName', e.target.value)
                  }
                  placeholder="e.g., SG-1, SG-2"
                />
              </Field.Root>

              <Flex gap={4}>
                <Field.Root flex={1}>
                  <Field.Label>Step (1-8)</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={formData.step}
                      onChange={(e) =>
                        handleChange('step', Number(e.target.value))
                      }
                    >
                      {[1, 2, 3, 4, 5, 6, 7, 8].map((step) => (
                        <option key={step} value={step}>
                          Step {step}
                        </option>
                      ))}
                    </NativeSelect.Field>
                    <NativeSelect.Indicator />
                  </NativeSelect.Root>
                </Field.Root>

                <Field.Root flex={1}>
                  <Field.Label>Monthly Salary (PHP)</Field.Label>
                  <Input
                    type="number"
                    value={formData.monthlySalary}
                    onChange={(e) =>
                      handleChange('monthlySalary', Number(e.target.value))
                    }
                    placeholder="0.00"
                    min={0}
                    step="0.01"
                  />
                </Field.Root>
              </Flex>

              <Field.Root>
                <Field.Label>Description</Field.Label>
                <Textarea
                  value={formData.description}
                  onChange={(e) => handleChange('description', e.target.value)}
                  placeholder="Enter description (optional)"
                  rows={4}
                />
              </Field.Root>

              {isEditMode && (
                <Checkbox.Root
                  checked={formData.isActive}
                  onCheckedChange={(e) => handleChange('isActive', !!e.checked)}
                >
                  <Checkbox.HiddenInput />
                  <Checkbox.Control />
                  <Checkbox.Label>Active</Checkbox.Label>
                </Checkbox.Root>
              )}
            </Stack>
          </Card.Body>
        </Card.Root>

        <Flex justify="flex-end" mt={6} gap={4}>
          <Button variant="outline" onClick={() => navigate(-1)}>
            Cancel
          </Button>
          <Button type="submit" colorPalette="blue" loading={saving}>
            {isEditMode ? 'Update Salary Grade' : 'Create Salary Grade'}
          </Button>
        </Flex>
      </form>
    </Box>
  );
};

export default SalaryGradeFormPage;
