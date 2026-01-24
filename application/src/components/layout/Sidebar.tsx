import { Box, VStack, Text } from '@chakra-ui/react';
import { NavLink } from 'react-router-dom';

interface NavItemProps {
  to: string;
  label: string;
  icon: string;
}

const NavItem = ({ to, label, icon }: NavItemProps) => {
  return (
    <NavLink to={to} style={{ width: '100%' }}>
      {({ isActive }) => (
        <Box
          px={4}
          py={3}
          borderRadius="md"
          bg={isActive ? 'colorPalette.subtle' : 'transparent'}
          color={isActive ? 'colorPalette.fg' : 'fg.muted'}
          _hover={{ bg: 'bg.muted' }}
          cursor="pointer"
          display="flex"
          alignItems="center"
          gap={3}
          fontWeight={isActive ? 'semibold' : 'normal'}
        >
          <Text fontSize="lg">{icon}</Text>
          <Text>{label}</Text>
        </Box>
      )}
    </NavLink>
  );
};

const Sidebar = () => {
  const navItems: NavItemProps[] = [
    { to: '/', label: 'Dashboard', icon: '📊' },
    { to: '/employments', label: 'Employments', icon: '💼' },
    { to: '/schools', label: 'Schools', icon: '🏫' },
    { to: '/positions', label: 'Positions', icon: '📋' },
    { to: '/salary-grades', label: 'Salary Grades', icon: '💰' },
    { to: '/items', label: 'Items', icon: '📦' },
    { to: '/persons', label: 'Persons', icon: '👤' },
  ];

  return (
    <Box
      as="nav"
      w="250px"
      minH="100vh"
      bg="bg.panel"
      borderRight="1px solid"
      borderColor="border.muted"
      p={4}
    >
      <Box mb={8}>
        <Text fontSize="xl" fontWeight="bold" color="fg">
          EMS
        </Text>
        <Text fontSize="sm" color="fg.muted">
          Employee Management
        </Text>
      </Box>
      <VStack gap={1} align="stretch" colorPalette="blue">
        {navItems.map(item => (
          <NavItem key={item.to} {...item} />
        ))}
      </VStack>
    </Box>
  );
};

export default Sidebar;
