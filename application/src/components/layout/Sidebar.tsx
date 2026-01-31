import { Box, VStack, Text, IconButton } from '@chakra-ui/react';
import { NavLink } from 'react-router-dom';
import { ChevronLeftIcon, ChevronRightIcon } from '../icons';

interface NavItemProps {
  to: string;
  label: string;
  icon: string;
  collapsed?: boolean;
}

const NavItem = ({ to, label, icon, collapsed }: NavItemProps) => {
  return (
    <NavLink
      to={to}
      style={{ width: '100%' }}
      title={collapsed ? label : undefined}
    >
      {({ isActive }) => (
        <Box
          px={collapsed ? 0 : 4}
          py={3}
          borderRadius="md"
          bg={isActive ? 'colorPalette.subtle' : 'transparent'}
          color={isActive ? 'colorPalette.fg' : 'fg.muted'}
          _hover={{ bg: 'bg.muted' }}
          cursor="pointer"
          display="flex"
          alignItems="center"
          justifyContent={collapsed ? 'center' : 'flex-start'}
          gap={3}
          fontWeight={isActive ? 'semibold' : 'normal'}
        >
          <Text fontSize="lg">{icon}</Text>
          {!collapsed && <Text>{label}</Text>}
        </Box>
      )}
    </NavLink>
  );
};

interface SidebarProps {
  collapsed: boolean;
  onToggle: () => void;
}

const Sidebar = ({ collapsed, onToggle }: SidebarProps) => {
  const navItems: Omit<NavItemProps, 'collapsed'>[] = [
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
      w={collapsed ? '60px' : '250px'}
      h="100vh"
      position="fixed"
      top={0}
      left={0}
      bg="bg.panel"
      borderRight="1px solid"
      borderColor="border.muted"
      p={collapsed ? 2 : 4}
      overflowY="auto"
      overflowX="hidden"
      transition="width 0.3s ease, padding 0.3s ease"
    >
      <Box
        mb={8}
        display="flex"
        alignItems="center"
        justifyContent="space-between"
      >
        {!collapsed && (
          <Box>
            <Text fontSize="xl" fontWeight="bold" color="fg">
              EMS
            </Text>
            <Text fontSize="sm" color="fg.muted">
              Employee Management
            </Text>
          </Box>
        )}
        <IconButton
          aria-label={collapsed ? 'Expand sidebar' : 'Collapse sidebar'}
          onClick={onToggle}
          variant="ghost"
          size="sm"
          color="fg.muted"
          _hover={{ bg: 'bg.muted', color: 'fg' }}
        >
          {collapsed ? (
            <ChevronRightIcon width={20} height={20} />
          ) : (
            <ChevronLeftIcon width={20} height={20} />
          )}
        </IconButton>
      </Box>
      <VStack gap={1} align="stretch" colorPalette="blue">
        {navItems.map((item) => (
          <NavItem key={item.to} {...item} collapsed={collapsed} />
        ))}
      </VStack>
    </Box>
  );
};

export default Sidebar;
