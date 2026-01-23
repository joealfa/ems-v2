import { useState, useEffect } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Card,
  Stack,
  Text,
  Spinner,
  Badge,
} from '@chakra-ui/react';
import { useNavigate, useParams } from 'react-router-dom';
import { salaryGradesApi, type SalaryGradeResponseDto } from '../../api';

const SalaryGradeDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();

  const [salaryGrade, setSalaryGrade] = useState<SalaryGradeResponseDto | null>(
    null
  );
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  const formatCurrency = (value: number | undefined): string => {
    if (value === undefined || value === null) return '-';
    return new Intl.NumberFormat('en-PH', {
      style: 'currency',
      currency: 'PHP',
    }).format(value);
  };

  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    loadSalaryGrade();
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadSalaryGrade = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await salaryGradesApi.apiV1SalaryGradesDisplayIdGet(
        Number(displayId)
      );
      setSalaryGrade(response.data);
    } catch (err) {
      console.error('Error loading salary grade:', err);
      setError('Failed to load salary grade data');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete this salary grade?')
    )
      return;
    setDeleting(true);
    try {
      await salaryGradesApi.apiV1SalaryGradesDisplayIdDelete(Number(displayId));
      navigate('/salary-grades');
    } catch (err) {
      console.error('Error deleting salary grade:', err);
      setError('Failed to delete salary grade');
    } finally {
      setDeleting(false);
    }
  };

  if (loading) {
    return (
      <Flex justify="center" align="center" h="100%">
        <Spinner size="lg" />
      </Flex>
    );
  }

  if (error || !salaryGrade) {
    return (
      <Box>
        <Text color="red.500">{error || 'Salary grade not found'}</Text>
        <Button mt={4} onClick={() => navigate('/salary-grades')}>
          Back to Salary Grades
        </Button>
      </Box>
    );
  }

  return (
    <Box maxW="800px">
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">{salaryGrade.salaryGradeName}</Heading>
        <Flex gap={2}>
          <Button variant="outline" onClick={() => navigate('/salary-grades')}>
            Back
          </Button>
          <Button
            colorPalette="blue"
            onClick={() => navigate(`/salary-grades/${displayId}/edit`)}
          >
            Edit
          </Button>
          <Button
            colorPalette="red"
            variant="outline"
            onClick={handleDelete}
            loading={deleting}
          >
            Delete
          </Button>
        </Flex>
      </Flex>

      <Stack gap={6}>
        <Card.Root>
          <Card.Header>
            <Heading size="md">Salary Grade Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Display ID
                  </Text>
                  <Text fontWeight="medium">{salaryGrade.displayId}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Salary Grade Name
                  </Text>
                  <Text fontWeight="medium">{salaryGrade.salaryGradeName}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Status
                  </Text>
                  <Badge colorPalette={salaryGrade.isActive ? 'green' : 'red'}>
                    {salaryGrade.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </Box>
              </Flex>

              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Step
                  </Text>
                  <Text fontWeight="medium">{salaryGrade.step || '-'}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Monthly Salary
                  </Text>
                  <Text fontWeight="medium" fontSize="lg" color="green.600">
                    {formatCurrency(salaryGrade.monthlySalary)}
                  </Text>
                </Box>
                <Box flex={1}></Box>
              </Flex>

              {salaryGrade.description && (
                <Box>
                  <Text color="fg.muted" fontSize="sm">
                    Description
                  </Text>
                  <Text fontWeight="medium">{salaryGrade.description}</Text>
                </Box>
              )}
            </Stack>
          </Card.Body>
        </Card.Root>

        <Card.Root>
          <Card.Header>
            <Heading size="md">Audit Information</Heading>
          </Card.Header>
          <Card.Body>
            <Flex gap={8}>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created On
                </Text>
                <Text fontWeight="medium">
                  {salaryGrade.createdOn
                    ? new Date(salaryGrade.createdOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created By
                </Text>
                <Text fontWeight="medium">{salaryGrade.createdBy || '-'}</Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified On
                </Text>
                <Text fontWeight="medium">
                  {salaryGrade.modifiedOn
                    ? new Date(salaryGrade.modifiedOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified By
                </Text>
                <Text fontWeight="medium">{salaryGrade.modifiedBy || '-'}</Text>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>
      </Stack>
    </Box>
  );
};

export default SalaryGradeDetailPage;
