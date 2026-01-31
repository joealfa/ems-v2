import {
  Box,
  Flex,
  Heading,
  Text,
  VStack,
  SimpleGrid,
  Card,
} from '@chakra-ui/react';
import { GoogleLogin, type CredentialResponse } from '@react-oauth/google';
import { useNavigate, useLocation, NavLink } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { useEffect, useState } from 'react';
import { useColorMode } from '../components/ui/color-mode';
import { IconButton } from '@chakra-ui/react';

// Preview version of sidebar for background
const SidebarPreview = () => {
  const navItems = [
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
        {navItems.map((item) => (
          <NavLink key={item.to} to={item.to} style={{ width: '100%' }}>
            {({ isActive }) => (
              <Box
                px={4}
                py={3}
                borderRadius="md"
                bg={isActive ? 'colorPalette.subtle' : 'transparent'}
                color={isActive ? 'colorPalette.fg' : 'fg.muted'}
                display="flex"
                alignItems="center"
                gap={3}
                fontWeight={isActive ? 'semibold' : 'normal'}
              >
                <Text fontSize="lg">{item.icon}</Text>
                <Text>{item.label}</Text>
              </Box>
            )}
          </NavLink>
        ))}
      </VStack>
    </Box>
  );
};

// Preview version of header for background
const HeaderPreview = () => {
  const { colorMode, toggleColorMode } = useColorMode();

  return (
    <Box
      as="header"
      h="60px"
      bg="bg.panel"
      borderBottom="1px solid"
      borderColor="border.muted"
      px={6}
    >
      <Flex h="100%" align="center" justify="space-between">
        <Text fontSize="lg" fontWeight="semibold" color="fg">
          Employee Management System
        </Text>
        <Flex align="center" gap={4}>
          <IconButton
            aria-label="Toggle color mode"
            variant="ghost"
            size="sm"
            onClick={toggleColorMode}
          >
            {colorMode === 'light' ? '🌙' : '☀️'}
          </IconButton>
        </Flex>
      </Flex>
    </Box>
  );
};

// Preview stat card for dashboard background
const StatCardPreview = ({
  title,
  icon,
}: {
  title: string;
  value: string;
  icon: string;
}) => {
  return (
    <Card.Root>
      <Card.Body>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box>
            <Text fontSize="sm" color="fg.muted">
              {title}
            </Text>
            <Text fontSize="2xl" fontWeight="bold" color="fg.muted">
              --
            </Text>
          </Box>
          <Text fontSize="3xl">{icon}</Text>
        </Box>
      </Card.Body>
    </Card.Root>
  );
};

// Preview dashboard content for background
const DashboardPreview = () => {
  return (
    <Box>
      <Heading size="lg" mb={6} color="fg">
        Dashboard
      </Heading>
      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} gap={6}>
        <StatCardPreview title="Total Persons" value="--" icon="👤" />
        <StatCardPreview title="Active Employees" value="--" icon="💼" />
        <StatCardPreview title="Total Schools" value="--" icon="🏫" />
        <StatCardPreview title="Open Positions" value="--" icon="📋" />
      </SimpleGrid>
    </Box>
  );
};

const LoginPage = () => {
  const { login, isAuthenticated, isLoading } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [error, setError] = useState<string | null>(null);

  const from =
    (location.state as { from?: { pathname: string } })?.from?.pathname || '/';

  useEffect(() => {
    if (isAuthenticated && !isLoading) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, isLoading, navigate, from]);

  const handleGoogleSuccess = async (response: CredentialResponse) => {
    setError(null);
    if (response.credential) {
      try {
        await login(response.credential);
        navigate(from, { replace: true });
      } catch (err: unknown) {
        console.error('Login error:', err);
        // Extract error message from response if available
        if (err && typeof err === 'object' && 'response' in err) {
          const axiosError = err as {
            response?: { data?: { message?: string }; status?: number };
          };
          const message = axiosError.response?.data?.message;
          const status = axiosError.response?.status;
          if (status === 401) {
            setError('Invalid credentials. Please try again.');
          } else if (message) {
            setError(`Login failed: ${message}`);
          } else {
            setError('Login failed. Please try again.');
          }
        } else {
          setError('Login failed. Please try again.');
        }
      }
    } else {
      setError('No credential received from Google. Please try again.');
    }
  };

  const handleGoogleError = () => {
    console.error(
      'Google Sign-In error - check that your Google Client ID is configured correctly and localhost:5173 is in authorized origins'
    );
    setError('Google sign-in failed. Check console for details.');
  };

  if (isLoading) {
    return (
      <Flex minH="100vh" align="center" justify="center" bg="bg">
        <Text>Loading...</Text>
      </Flex>
    );
  }

  return (
    <Box position="relative" minH="100vh" overflow="hidden">
      {/* Background: Application Preview */}
      <Box
        position="absolute"
        inset={0}
        pointerEvents="none"
        userSelect="none"
        aria-hidden="true"
      >
        <Flex minH="100vh">
          <SidebarPreview />
          <Box flex={1} display="flex" flexDirection="column">
            <HeaderPreview />
            <Box as="main" flex={1} p={6} bg="bg" overflow="auto">
              <DashboardPreview />
            </Box>
          </Box>
        </Flex>
      </Box>

      {/* Backdrop Overlay */}
      <Box
        position="absolute"
        inset={0}
        bg="blackAlpha.600"
        backdropFilter="blur(4px)"
        zIndex={10}
      />

      {/* Login Modal */}
      <Flex
        position="relative"
        zIndex={20}
        minH="100vh"
        align="center"
        justify="center"
        p={4}
      >
        <Box
          p={8}
          maxW="md"
          w="full"
          bg="bg.panel"
          borderRadius="xl"
          boxShadow="2xl"
          border="1px solid"
          borderColor="border.muted"
        >
          <VStack gap={6}>
            <VStack gap={2}>
              <Heading size="xl" color="fg">
                Employee Management System
              </Heading>
              <Text color="fg.muted" textAlign="center">
                Sign in with your Google account to continue
              </Text>
            </VStack>

            {error && (
              <Box
                w="full"
                p={3}
                bg="red.50"
                borderRadius="md"
                border="1px solid"
                borderColor="red.200"
              >
                <Text color="red.600" fontSize="sm" textAlign="center">
                  {error}
                </Text>
              </Box>
            )}

            <Box>
              <GoogleLogin
                onSuccess={handleGoogleSuccess}
                onError={handleGoogleError}
                theme="outline"
                size="large"
                text="signin_with"
                shape="rectangular"
              />
            </Box>
          </VStack>
        </Box>
      </Flex>
    </Box>
  );
};

export default LoginPage;
