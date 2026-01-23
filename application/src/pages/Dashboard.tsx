import { Box, Heading, Text, SimpleGrid, Card } from '@chakra-ui/react';

interface StatCardProps {
  title: string;
  value: string;
  icon: string;
}

const StatCard = ({ title, value, icon }: StatCardProps) => {
  return (
    <Card.Root>
      <Card.Body>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box>
            <Text fontSize="sm" color="fg.muted">
              {title}
            </Text>
            <Text fontSize="2xl" fontWeight="bold">
              {value}
            </Text>
          </Box>
          <Text fontSize="3xl">{icon}</Text>
        </Box>
      </Card.Body>
    </Card.Root>
  );
};

const Dashboard = () => {
  return (
    <Box>
      <Heading size="lg" mb={6}>
        Dashboard
      </Heading>
      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} gap={6}>
        <StatCard title="Total Persons" value="--" icon="👤" />
        <StatCard title="Active Employments" value="--" icon="💼" />
        <StatCard title="Schools" value="--" icon="🏫" />
        <StatCard title="Positions" value="--" icon="📋" />
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
