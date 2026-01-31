import { Box, Flex, Text, IconButton, Image, Button } from '@chakra-ui/react';
import { Link, useLocation, useParams } from 'react-router-dom';
import { useColorMode, useColorModeValue } from '../ui/color-mode';
import { useAuth } from '../../hooks/useAuth';
import {
  AwardIcon,
  BoxIcon,
  BriefcaseIcon,
  ChevronRightIcon,
  FileTextIcon,
  HomeIcon,
  SchoolIcon,
  UsersIcon,
  UserIcon,
} from '../icons';

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
                        <ChevronRightIcon width={14} height={14} />
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
