import { useState, useEffect } from 'react';
import {
  Box,
  Heading,
  Text,
  SimpleGrid,
  Card,
  Spinner,
} from '@chakra-ui/react';
import { reportsApi, type DashboardStatsDto } from '../api';

interface StatCardProps {
  title: string;
  value: string | number;
  icon: string;
  isLoading?: boolean;
}

const StatCard = ({ title, value, icon, isLoading }: StatCardProps) => {
  return (
    <Card.Root>
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
  const [stats, setStats] = useState<DashboardStatsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const response = await reportsApi.apiV1ReportsDashboardGet();
        setStats(response.data);
      } catch (err) {
        console.error('Error fetching dashboard stats:', err);
        setError('Failed to load dashboard statistics');
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  return (
    <Box>
      <Heading size="lg" mb={6}>
        Dashboard
      </Heading>

      {error && (
        <Box mb={4} p={4} bg="red.100" color="red.800" borderRadius="md">
          <Text>{error}</Text>
        </Box>
      )}

      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} gap={6}>
        <StatCard
          title="Total Persons"
          value={stats?.totalPersons ?? '--'}
          icon="👤"
          isLoading={loading}
        />
        <StatCard
          title="Active Employments"
          value={stats?.activeEmployments ?? '--'}
          icon="💼"
          isLoading={loading}
        />
        <StatCard
          title="Schools"
          value={stats?.totalSchools ?? '--'}
          icon="🏫"
          isLoading={loading}
        />
        <StatCard
          title="Positions"
          value={stats?.totalPositions ?? '--'}
          icon="📋"
          isLoading={loading}
        />
      </SimpleGrid>

      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} gap={6} mt={6}>
        <StatCard
          title="Salary Grades"
          value={stats?.totalSalaryGrades ?? '--'}
          icon="💰"
          isLoading={loading}
        />
        <StatCard
          title="Items"
          value={stats?.totalItems ?? '--'}
          icon="📦"
          isLoading={loading}
        />
      </SimpleGrid>

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
