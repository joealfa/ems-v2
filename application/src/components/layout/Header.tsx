import { Box, Flex, Text, IconButton, Image, Button } from '@chakra-ui/react';
import { Link, useLocation, useParams } from 'react-router-dom';
import { useColorMode, useColorModeValue } from '../ui/color-mode';
import { useAuth } from '../../hooks/useAuth';

// Icon Components
const HomeIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
    <path d="M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z" />
  </svg>
);

const UsersIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
    <circle cx="9" cy="7" r="4"></circle>
    <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
    <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
  </svg>
);

const UserIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
    <circle cx="12" cy="7" r="4"></circle>
  </svg>
);

const SchoolIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M22 10v6M2 10l10-5 10 5-10 5z"></path>
    <path d="M6 12v5c3 3 9 3 12 0v-5"></path>
  </svg>
);

const BriefcaseIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <rect x="2" y="7" width="20" height="14" rx="2" ry="2"></rect>
    <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"></path>
  </svg>
);

const AwardIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <circle cx="12" cy="8" r="7"></circle>
    <polyline points="8.21 13.89 7 23 12 20 17 23 15.79 13.88"></polyline>
  </svg>
);

const BoxIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
    <polyline points="3.27 6.96 12 12.01 20.73 6.96"></polyline>
    <line x1="12" y1="22.08" x2="12" y2="12"></line>
  </svg>
);

const FileTextIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
    <polyline points="14 2 14 8 20 8"></polyline>
    <line x1="16" y1="13" x2="8" y2="13"></line>
    <line x1="16" y1="17" x2="8" y2="17"></line>
    <polyline points="10 9 9 9 8 9"></polyline>
  </svg>
);

const ChevronRightIcon = () => (
  <svg
    width="14"
    height="14"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <polyline points="9 18 15 12 9 6"></polyline>
  </svg>
);

interface BreadcrumbItem {
  label: string;
  path: string;
  icon: React.ReactNode;
}

const Header = () => {
  const location = useLocation();
  const params = useParams();
  const { colorMode, toggleColorMode } = useColorMode();
  const { user, logout, isAuthenticated } = useAuth();
  const textColor = useColorModeValue('gray.600', 'gray.400');
  const activeColor = useColorModeValue('blue.600', 'blue.400');
  const separatorColor = useColorModeValue('gray.400', 'gray.500');

  const handleLogout = async () => {
    await logout();
  };

  const getIconForRoute = (route: string): React.ReactNode => {
    const iconMap: Record<string, React.ReactNode> = {
      persons: <UsersIcon />,
      schools: <SchoolIcon />,
      positions: <BriefcaseIcon />,
      'salary-grades': <AwardIcon />,
      items: <BoxIcon />,
      employments: <FileTextIcon />,
    };
    return iconMap[route] || <UserIcon />;
  };

  const generateBreadcrumbs = (): BreadcrumbItem[] => {
    const pathSegments = location.pathname.split('/').filter(Boolean);
    const breadcrumbs: BreadcrumbItem[] = [
      { label: 'Dashboard', path: '/', icon: <HomeIcon /> },
    ];

    if (pathSegments.length === 0) {
      return breadcrumbs;
    }

    const routeNameMap: Record<string, string> = {
      persons: 'Persons',
      schools: 'Schools',
      positions: 'Positions',
      'salary-grades': 'Salary Grades',
      items: 'Items',
      employments: 'Employments',
    };

    const firstSegment = pathSegments[0];
    const displayName = routeNameMap[firstSegment] || firstSegment;

    breadcrumbs.push({
      label: displayName,
      path: `/${firstSegment}`,
      icon: getIconForRoute(firstSegment),
    });

    if (pathSegments.length === 2) {
      const secondSegment = pathSegments[1];

      if (secondSegment === 'new') {
        breadcrumbs.push({
          label: 'Add',
          path: location.pathname,
          icon: <UserIcon />,
        });
      } else if (params.displayId) {
        breadcrumbs.push({
          label: 'View',
          path: location.pathname,
          icon: <UserIcon />,
        });
      }
    } else if (pathSegments.length === 3) {
      const displayId = pathSegments[1];
      const thirdSegment = pathSegments[2];

      if (thirdSegment === 'edit') {
        // Check if coming from view page via location state
        const fromView = (location.state as { fromView?: boolean })?.fromView;

        if (fromView) {
          // Add View breadcrumb before Edit
          breadcrumbs.push({
            label: 'View',
            path: `/${firstSegment}/${displayId}`,
            icon: <UserIcon />,
          });
        }

        breadcrumbs.push({
          label: 'Edit',
          path: location.pathname,
          icon: <UserIcon />,
        });
      }
    }

    return breadcrumbs;
  };

  const breadcrumbs = generateBreadcrumbs();

  return (
    <Box
      as="header"
      bg="bg.panel"
      borderBottom="1px solid"
      borderColor="border.muted"
      px={6}
      py={2}
      position="sticky"
      top={0}
      zIndex={10}
    >
      <Flex h="60px" align="center" justify="space-between">
        <Box as="span">
          <Text fontSize="lg" fontWeight="semibold" color="fg">
            Employee Management System
          </Text>
          {/* Breadcrumb */}
          <Flex align="center" gap={2} pt={1}>
            {breadcrumbs.map((crumb, index) => {
              const isLast = index === breadcrumbs.length - 1;

              return (
                <Flex key={crumb.path} align="center" gap={2}>
                  {isLast ? (
                    <Flex align="center" gap={1.5}>
                      <Box
                        color={activeColor}
                        display="flex"
                        alignItems="center"
                      >
                        {crumb.icon}
                      </Box>
                      <Text
                        fontSize="xs"
                        fontWeight="medium"
                        color={activeColor}
                      >
                        {crumb.label}
                      </Text>
                    </Flex>
                  ) : (
                    <>
                      <Link to={crumb.path}>
                        <Flex
                          align="center"
                          gap={1.5}
                          _hover={{
                            color: activeColor,
                          }}
                          transition="color 0.2s"
                          color={textColor}
                        >
                          <Box display="flex" alignItems="center">
                            {crumb.icon}
                          </Box>
                          <Text fontSize="xs">{crumb.label}</Text>
                        </Flex>
                      </Link>
                      <Box
                        color={separatorColor}
                        display="flex"
                        alignItems="center"
                      >
                        <ChevronRightIcon />
                      </Box>
                    </>
                  )}
                </Flex>
              );
            })}
          </Flex>
        </Box>
        <Flex align="center" gap={4}>
          <IconButton
            aria-label="Toggle color mode"
            variant="ghost"
            size="sm"
            onClick={toggleColorMode}
          >
            {colorMode === 'light' ? '🌙' : '☀️'}
          </IconButton>

          {isAuthenticated && user && (
            <Flex align="center" gap={3}>
              <Flex align="center" gap={2}>
                {user.profilePictureUrl ? (
                  <Image
                    src={user.profilePictureUrl}
                    alt={`${user.firstName} ${user.lastName}`}
                    boxSize="32px"
                    borderRadius="full"
                    referrerPolicy="no-referrer"
                  />
                ) : (
                  <Box
                    boxSize="32px"
                    borderRadius="full"
                    bg="blue.500"
                    display="flex"
                    alignItems="center"
                    justifyContent="center"
                    color="white"
                    fontWeight="bold"
                    fontSize="sm"
                  >
                    {user.firstName?.charAt(0)}
                    {user.lastName?.charAt(0)}
                  </Box>
                )}
                <Text
                  fontSize="sm"
                  color="fg"
                  display={{ base: 'none', md: 'block' }}
                >
                  {user.firstName} {user.lastName}
                </Text>
              </Flex>
              <Button size="sm" variant="ghost" onClick={handleLogout}>
                Logout
              </Button>
            </Flex>
          )}
        </Flex>
      </Flex>
    </Box>
  );
};

export default Header;
