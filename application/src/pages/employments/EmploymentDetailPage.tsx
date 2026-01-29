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
import { useEmployment, useDeleteEmployment } from '../../hooks/useEmployments';

// Enum display mappings
const AppointmentStatusDisplay: Record<number, string> = {
  1: 'Original',
  2: 'Promotion',
  3: 'Transfer',
  4: 'Reappointment',
};

const EmploymentStatusDisplay: Record<number, string> = {
  1: 'Regular',
  2: 'Permanent',
};

const EligibilityDisplay: Record<number, string> = {
  1: 'LET',
  2: 'CivilServiceProfessional',
  3: 'CivilServiceSubProfessional',
  4: 'Other',
};

const EmploymentDetailPage = () => {
  const navigate = useNavigate();
  const { displayId } = useParams<{ displayId: string }>();

  const { employment, loading, error } = useEmployment(Number(displayId));
  const { deleteEmployment, loading: deleting } = useDeleteEmployment();

  const formatCurrency = (value: number | undefined): string => {
    if (value === undefined || value === null) return '-';
    return new Intl.NumberFormat('en-PH', {
      style: 'currency',
      currency: 'PHP',
    }).format(value);
  };

  const handleDelete = async () => {
    if (
      !displayId ||
      !window.confirm('Are you sure you want to delete this employment record?')
    )
      return;

    try {
      await deleteEmployment(Number(displayId));
      navigate('/employments');
    } catch (err) {
      console.error('Error deleting employment:', err);
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
        <Text color="red.500">{error?.message || 'Employment not found'}</Text>
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
                  <Text fontWeight="medium">
                    {employment.displayId as unknown as number}
                  </Text>
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
                    {AppointmentStatusDisplay[
                      employment.appointmentStatus as unknown as number
                    ] || employment.appointmentStatus}
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
                      (employment.employmentStatus as unknown as number) === 1
                        ? 'green'
                        : 'blue'
                    }
                  >
                    {EmploymentStatusDisplay[
                      employment.employmentStatus as unknown as number
                    ] || employment.employmentStatus}
                  </Badge>
                </Box>
                <Box flex={1}>
                  <Text color="fg.muted" fontSize="sm">
                    Eligibility
                  </Text>
                  <Badge colorPalette="purple">
                    {(employment.eligibility as unknown as number) === 1
                      ? 'Civil Service Professional'
                      : (employment.eligibility as unknown as number) === 2
                        ? 'Civil Service Sub-Professional'
                        : EligibilityDisplay[
                            employment.eligibility as unknown as number
                          ] || employment.eligibility}
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
                    {formatCurrency(
                      employment.salaryGrade?.monthlySalary as unknown as number
                    )}
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
                {employment.schools.map((school, index) =>
                  school ? (
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
                  ) : null
                )}
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
