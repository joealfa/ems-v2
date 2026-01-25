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
import { employmentsApi, type EmploymentResponseDto } from '../../api';

const EmploymentDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();

  const [employment, setEmployment] = useState<EmploymentResponseDto | null>(
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
    loadEmployment();
  }, [displayId]);
  /* eslint-enable react-hooks/exhaustive-deps */

  const loadEmployment = async () => {
    if (!displayId) return;
    setLoading(true);
    try {
      const response = await employmentsApi.apiV1EmploymentsDisplayIdGet(
        Number(displayId)
      );
      setEmployment(response.data);
    } catch (err) {
      console.error('Error loading employment:', err);
      setError('Failed to load employment data');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete this employment record?')
    )
      return;
    setDeleting(true);
    try {
      await employmentsApi.apiV1EmploymentsDisplayIdDelete(Number(displayId));
      navigate('/employments');
    } catch (err) {
      console.error('Error deleting employment:', err);
      setError('Failed to delete employment');
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

  if (error || !employment) {
    return (
      <Box>
        <Text color="red.500">{error || 'Employment not found'}</Text>
        <Button mt={4} onClick={() => navigate('/employments')}>
          Back to Employments
        </Button>
      </Box>
    );
  }

  return (
    <Box maxW="900px">
      <Flex justify="space-between" align="center" mb={6}>
        <Box>
          <Heading size="lg">
            {employment.person?.fullName || 'Employment Record'}
          </Heading>
          <Text color="fg.muted">{employment.position?.titleName}</Text>
        </Box>
        <Flex gap={2}>
          <Button variant="outline" onClick={() => navigate('/employments')}>
            Back
          </Button>
          <Button
            colorPalette="blue"
            onClick={() =>
              navigate(`/employments/${displayId}/edit`, {
                state: { fromView: true },
              })
            }
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
            <Heading size="md">Employee Information</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Display ID
                  </Text>
                  <Text fontWeight="medium">{employment.displayId}</Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Employee Name
                  </Text>
                  <Text fontWeight="medium">
                    {employment.person?.fullName || '-'}
                  </Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Status
                  </Text>
                  <Badge colorPalette={employment.isActive ? 'green' : 'red'}>
                    {employment.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </Box>
              </Flex>
            </Stack>
          </Card.Body>
        </Card.Root>

        <Card.Root>
          <Card.Header>
            <Heading size="md">Government IDs</Heading>
          </Card.Header>
          <Card.Body>
            <Flex gap={8} wrap="wrap">
              <Box minW="150px">
                <Text color="fg.muted" fontSize="sm">
                  DepEd ID
                </Text>
                <Text fontWeight="medium">{employment.depEdId || '-'}</Text>
              </Box>
              <Box minW="150px">
                <Text color="fg.muted" fontSize="sm">
                  PSIPOP Item Number
                </Text>
                <Text fontWeight="medium">
                  {employment.psipopItemNumber || '-'}
                </Text>
              </Box>
              <Box minW="150px">
                <Text color="fg.muted" fontSize="sm">
                  TIN
                </Text>
                <Text fontWeight="medium">{employment.tinId || '-'}</Text>
              </Box>
              <Box minW="150px">
                <Text color="fg.muted" fontSize="sm">
                  GSIS ID
                </Text>
                <Text fontWeight="medium">{employment.gsisId || '-'}</Text>
              </Box>
              <Box minW="150px">
                <Text color="fg.muted" fontSize="sm">
                  PhilHealth ID
                </Text>
                <Text fontWeight="medium">
                  {employment.philHealthId || '-'}
                </Text>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>

        <Card.Root>
          <Card.Header>
            <Heading size="md">Employment Details</Heading>
          </Card.Header>
          <Card.Body>
            <Stack gap={4}>
              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Date of Original Appointment
                  </Text>
                  <Text fontWeight="medium">
                    {employment.dateOfOriginalAppointment
                      ? new Date(
                          employment.dateOfOriginalAppointment
                        ).toLocaleDateString()
                      : '-'}
                  </Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Appointment Status
                  </Text>
                  <Badge colorPalette="blue">
                    {employment.appointmentStatus}
                  </Badge>
                </Box>
              </Flex>

              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Employment Status
                  </Text>
                  <Badge
                    colorPalette={
                      employment.employmentStatus === 'Permanent'
                        ? 'green'
                        : 'blue'
                    }
                  >
                    {employment.employmentStatus}
                  </Badge>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Eligibility
                  </Text>
                  <Badge colorPalette="purple">
                    {employment.eligibility === 'CivilServiceProfessional'
                      ? 'Civil Service Professional'
                      : employment.eligibility === 'CivilServiceSubProfessional'
                        ? 'Civil Service Sub-Professional'
                        : employment.eligibility}
                  </Badge>
                </Box>
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
              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Position
                  </Text>
                  <Text fontWeight="medium">
                    {employment.position?.titleName || '-'}
                  </Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Plantilla Item
                  </Text>
                  <Text fontWeight="medium">
                    {employment.item?.itemName || '-'}
                  </Text>
                </Box>
              </Flex>

              <Flex gap={8}>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Salary Grade
                  </Text>
                  <Text fontWeight="medium">
                    {employment.salaryGrade?.salaryGradeName || '-'}
                    {employment.salaryGrade?.step &&
                      ` - Step ${employment.salaryGrade.step}`}
                  </Text>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Monthly Salary
                  </Text>
                  <Text fontWeight="medium" fontSize="lg" color="green.600">
                    {formatCurrency(employment.salaryGrade?.monthlySalary)}
                  </Text>
                </Box>
              </Flex>
            </Stack>
          </Card.Body>
        </Card.Root>

        {employment.schools && employment.schools.length > 0 && (
          <Card.Root>
            <Card.Header>
              <Heading size="md">School Assignments</Heading>
            </Card.Header>
            <Card.Body>
              <Stack gap={4}>
                {employment.schools.map((school, index) => (
                  <Box key={index} p={4} borderWidth={1} borderRadius="md">
                    <Flex justify="space-between" align="center">
                      <Box>
                        <Text fontWeight="medium">{school.schoolName}</Text>
                        <Text fontSize="sm" color="fg.muted">
                          {school.startDate &&
                            `From: ${new Date(school.startDate).toLocaleDateString()}`}
                          {school.endDate &&
                            ` To: ${new Date(school.endDate).toLocaleDateString()}`}
                        </Text>
                      </Box>
                      <Flex gap={2}>
                        {school.isCurrent && (
                          <Badge colorPalette="green">Current</Badge>
                        )}
                        <Badge
                          colorPalette={school.isActive ? 'green' : 'gray'}
                        >
                          {school.isActive ? 'Active' : 'Inactive'}
                        </Badge>
                      </Flex>
                    </Flex>
                  </Box>
                ))}
              </Stack>
            </Card.Body>
          </Card.Root>
        )}

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
                  {employment.createdOn
                    ? new Date(employment.createdOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Created By
                </Text>
                <Text fontWeight="medium">{employment.createdBy || '-'}</Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified On
                </Text>
                <Text fontWeight="medium">
                  {employment.modifiedOn
                    ? new Date(employment.modifiedOn).toLocaleString()
                    : '-'}
                </Text>
              </Box>
              <Box flex={1}>
                <Text color="fg.muted" fontSize="sm">
                  Modified By
                </Text>
                <Text fontWeight="medium">{employment.modifiedBy || '-'}</Text>
              </Box>
            </Flex>
          </Card.Body>
        </Card.Root>
      </Stack>
    </Box>
  );
};

export default EmploymentDetailPage;
