import {
  Box,
  Heading,
  Text,
  SimpleGrid,
  Card,
  Spinner,
  Flex,
  Button,
  Grid,
  GridItem,
} from '@chakra-ui/react';
import { useDashboardStats } from '../hooks/useDashboard';
import { useRecentActivities } from '../hooks/useRecentActivities';
import { useState } from 'react';
import { formatTimestamp, getActivityIcon } from '../utils';

interface StatCardProps {
  title: string;
  value: string | number;
  icon: string;
  isLoading?: boolean;
}

const StatCard = ({ title, value, icon, isLoading }: StatCardProps) => {
  return (
    <Card.Root height="100%">
      <Card.Body>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box>
            <Text fontSize="sm" color="fg.muted">
              {title}
            </Text>
            {isLoading ? (
              <Spinner size="sm" mt={2} />
            ) : (
              <Text fontSize="2xl" fontWeight="bold">
                {value}
              </Text>
            )}
          </Box>
          <Text fontSize="3xl">{icon}</Text>
        </Box>
      </Card.Body>
    </Card.Root>
  );
};

const Dashboard = () => {
  const { stats, loading, error } = useDashboardStats();
  const {
    activities,
    isConnected,
    error: subscriptionError,
  } = useRecentActivities();

  // Dummy quotes data
  const quotes = [
    {
      text: 'The only way to do great work is to love what you do.',
      author: 'Steve Jobs',
    },
    {
      text: 'Success is not final, failure is not fatal: it is the courage to continue that counts.',
      author: 'Winston Churchill',
    },
    {
      text: 'The future belongs to those who believe in the beauty of their dreams.',
      author: 'Eleanor Roosevelt',
    },
    {
      text: 'Education is the most powerful weapon which you can use to change the world.',
      author: 'Nelson Mandela',
    },
    {
      text: 'Teaching is the one profession that creates all other professions.',
      author: 'Unknown',
    },
  ];

  const [currentQuoteIndex, setCurrentQuoteIndex] = useState(0);

  const handleNextQuote = () => {
    setCurrentQuoteIndex((prev) => (prev + 1) % quotes.length);
  };

  return (
    <Box>
      <Heading size="lg" mb={6}>
        Dashboard
      </Heading>

      {error && (
        <Box mb={4} p={4} bg="red.100" color="red.800" borderRadius="md">
          <Text>Failed to load dashboard statistics</Text>
        </Box>
      )}

      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} gap={6}>
        <StatCard
          title="Total Persons"
          value={(stats?.totalPersons as unknown as number) ?? '--'}
          icon="👤"
          isLoading={loading}
        />
        <StatCard
          title="Active Employments"
          value={(stats?.activeEmployments as unknown as number) ?? '--'}
          icon="💼"
          isLoading={loading}
        />
        <StatCard
          title="Schools"
          value={(stats?.totalSchools as unknown as number) ?? '--'}
          icon="🏫"
          isLoading={loading}
        />
        <StatCard
          title="Positions"
          value={(stats?.totalPositions as unknown as number) ?? '--'}
          icon="📋"
          isLoading={loading}
        />
      </SimpleGrid>

      <Flex gap={6} mt={6} justify="center" wrap="wrap">
        <Box
          flex={{
            base: '1 1 100%',
            md: '0 0 calc(50% - 12px)',
            lg: '0 0 calc(25% - 18px)',
          }}
          maxW="100%"
        >
          <StatCard
            title="Salary Grades"
            value={(stats?.totalSalaryGrades as unknown as number) ?? '--'}
            icon="💰"
            isLoading={loading}
          />
        </Box>
        <Box
          flex={{
            base: '1 1 100%',
            md: '0 0 calc(50% - 12px)',
            lg: '0 0 calc(25% - 18px)',
          }}
          maxW="100%"
        >
          <StatCard
            title="Items"
            value={(stats?.totalItems as unknown as number) ?? '--'}
            icon="📦"
            isLoading={loading}
          />
        </Box>
      </Flex>

      <Grid templateColumns={{ base: '1fr', lg: '1fr 1fr' }} gap={6} mt={8}>
        {/* Left Column - Recent Activities (Full Height, Scrollable) */}
        <GridItem rowSpan={{ base: 1, lg: 2 }}>
          <Card.Root height={{ base: 'auto', lg: '500px' }}>
            <Card.Header>
              <Flex justify="space-between" align="center">
                <Heading size="sm">📋 Recent Activities</Heading>
                {isConnected && (
                  <Box
                    width="10px"
                    height="10px"
                    borderRadius="full"
                    bg="green.500"
                    title="Connected"
                  />
                )}
              </Flex>
            </Card.Header>
            <Card.Body
              overflowY="auto"
              maxHeight={{ base: 'auto', lg: '420px' }}
            >
              {subscriptionError && (
                <Text fontSize="sm" color="red.500" mb={4}>
                  Error loading activities: {subscriptionError.message}
                </Text>
              )}

              {activities.length === 0 && !subscriptionError && (
                <Text fontSize="sm" color="fg.muted">
                  {isConnected
                    ? 'No recent activities yet. Activities will appear here as they occur.'
                    : 'Connecting to activity stream...'}
                </Text>
              )}

              <Box display="flex" flexDirection="column" gap={3}>
                {activities.map((activity) => (
                  <Box
                    key={activity.id}
                    pb={3}
                    borderBottom="1px"
                    borderColor="border"
                  >
                    <Flex justify="space-between" align="start" mb={1}>
                      <Text fontSize="sm" fontWeight="medium">
                        {getActivityIcon(activity.entityType)}{' '}
                        {activity.message}
                      </Text>
                    </Flex>
                    <Text fontSize="xs" color="fg.muted">
                      {formatTimestamp(activity.timestamp)}
                    </Text>
                  </Box>
                ))}
              </Box>
            </Card.Body>
          </Card.Root>
        </GridItem>

        {/* Right Column - Top - Quote of the Day */}
        <GridItem>
          <Card.Root>
            <Card.Header>
              <Flex justify="space-between" align="center">
                <Heading size="sm">💭 Quote of the Day</Heading>
                <Button size="sm" colorPalette="blue" onClick={handleNextQuote}>
                  Next
                </Button>
              </Flex>
            </Card.Header>
            <Card.Body>
              <Text fontStyle="italic" color="fg.muted" mb={2}>
                "{quotes[currentQuoteIndex].text}"
              </Text>
              <Text fontSize="sm" color="fg.muted" textAlign="right">
                — {quotes[currentQuoteIndex].author}
              </Text>
            </Card.Body>
          </Card.Root>
        </GridItem>

        {/* Right Column - Bottom - Birthdays This Month (Scrollable) */}
        <GridItem>
          <Card.Root height={{ base: 'auto', lg: '300px' }}>
            <Card.Header>
              <Heading size="sm">🎂 Birthdays This Month</Heading>
            </Card.Header>
            <Card.Body
              overflowY="auto"
              maxHeight={{ base: 'auto', lg: '220px' }}
            >
              <Box display="flex" flexDirection="column" gap={3}>
                <Box display="flex" alignItems="center" gap={3}>
                  <Box
                    width="40px"
                    height="40px"
                    borderRadius="full"
                    bg="blue.500"
                    display="flex"
                    alignItems="center"
                    justifyContent="center"
                    color="white"
                    fontWeight="bold"
                    flexShrink={0}
                  >
                    JD
                  </Box>
                  <Box flex="1">
                    <Text fontWeight="medium">John Doe</Text>
                    <Text fontSize="sm" color="fg.muted">
                      February 15
                    </Text>
                  </Box>
                </Box>
                <Box display="flex" alignItems="center" gap={3}>
                  <Box
                    width="40px"
                    height="40px"
                    borderRadius="full"
                    bg="purple.500"
                    display="flex"
                    alignItems="center"
                    justifyContent="center"
                    color="white"
                    fontWeight="bold"
                    flexShrink={0}
                  >
                    AS
                  </Box>
                  <Box flex="1">
                    <Text fontWeight="medium">Alice Smith</Text>
                    <Text fontSize="sm" color="fg.muted">
                      February 22
                    </Text>
                  </Box>
                </Box>
                <Box display="flex" alignItems="center" gap={3}>
                  <Box
                    width="40px"
                    height="40px"
                    borderRadius="full"
                    bg="green.500"
                    display="flex"
                    alignItems="center"
                    justifyContent="center"
                    color="white"
                    fontWeight="bold"
                    flexShrink={0}
                  >
                    MJ
                  </Box>
                  <Box flex="1">
                    <Text fontWeight="medium">Michael Johnson</Text>
                    <Text fontSize="sm" color="fg.muted">
                      February 28
                    </Text>
                  </Box>
                </Box>
                <Box display="flex" alignItems="center" gap={3}>
                  <Box
                    width="40px"
                    height="40px"
                    borderRadius="full"
                    bg="orange.500"
                    display="flex"
                    alignItems="center"
                    justifyContent="center"
                    color="white"
                    fontWeight="bold"
                    flexShrink={0}
                  >
                    EW
                  </Box>
                  <Box flex="1">
                    <Text fontWeight="medium">Emma Wilson</Text>
                    <Text fontSize="sm" color="fg.muted">
                      February 10
                    </Text>
                  </Box>
                </Box>
                <Box display="flex" alignItems="center" gap={3}>
                  <Box
                    width="40px"
                    height="40px"
                    borderRadius="full"
                    bg="red.500"
                    display="flex"
                    alignItems="center"
                    justifyContent="center"
                    color="white"
                    fontWeight="bold"
                    flexShrink={0}
                  >
                    DB
                  </Box>
                  <Box flex="1">
                    <Text fontWeight="medium">David Brown</Text>
                    <Text fontSize="sm" color="fg.muted">
                      February 5
                    </Text>
                  </Box>
                </Box>
              </Box>
            </Card.Body>
          </Card.Root>
        </GridItem>
      </Grid>

      <Box mt={8}>
        <Text color="fg.muted">
          Welcome to the Employee Management System. Use the sidebar to navigate
          between different sections.
        </Text>
      </Box>
    </Box>
  );
};

export default Dashboard;
