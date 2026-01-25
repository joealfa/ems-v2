import { Flex, Text, Box } from '@chakra-ui/react';
import { Link, useLocation, useParams } from 'react-router-dom';
import { useColorModeValue } from '../ui/color-mode';

interface BreadcrumbItem {
  label: string;
  path: string;
}

const ChevronRightIcon = () => (
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
    <polyline points="9 18 15 12 9 6"></polyline>
  </svg>
);

const Breadcrumb = () => {
  const location = useLocation();
  const params = useParams();
  const textColor = useColorModeValue('gray.600', 'gray.400');
  const activeColor = useColorModeValue('gray.900', 'gray.100');
  const hoverColor = useColorModeValue('blue.600', 'blue.400');
  const separatorColor = useColorModeValue('gray.400', 'gray.500');

  const generateBreadcrumbs = (): BreadcrumbItem[] => {
    const pathSegments = location.pathname.split('/').filter(Boolean);
    const breadcrumbs: BreadcrumbItem[] = [{ label: 'Dashboard', path: '/' }];

    if (pathSegments.length === 0) {
      return breadcrumbs;
    }

    // Map route segments to human-readable names
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

    // Add the main resource route
    breadcrumbs.push({
      label: displayName,
      path: `/${firstSegment}`,
    });

    // Handle different scenarios
    if (pathSegments.length === 2) {
      const secondSegment = pathSegments[1];

      if (secondSegment === 'new') {
        // /resource/new -> Add
        breadcrumbs.push({
          label: 'Add',
          path: location.pathname,
        });
      } else if (params.displayId) {
        // /resource/:displayId -> View
        breadcrumbs.push({
          label: 'View',
          path: location.pathname,
        });
      }
    } else if (pathSegments.length === 3) {
      // /resource/:displayId/edit
      const displayId = pathSegments[1];

      // Add View breadcrumb
      breadcrumbs.push({
        label: 'View',
        path: `/${firstSegment}/${displayId}`,
      });

      // Add Edit breadcrumb
      breadcrumbs.push({
        label: 'Edit',
        path: location.pathname,
      });
    }

    return breadcrumbs;
  };

  const breadcrumbs = generateBreadcrumbs();

  // Don't show breadcrumbs on dashboard
  if (breadcrumbs.length === 1) {
    return null;
  }

  return (
    <Flex
      align="center"
      gap={2}
      py={3}
      px={6}
      bg="bg.panel"
      borderBottom="1px solid"
      borderColor="border.muted"
    >
      {breadcrumbs.map((crumb, index) => {
        const isLast = index === breadcrumbs.length - 1;

        return (
          <Flex key={crumb.path} align="center" gap={2}>
            {isLast ? (
              <Text fontSize="sm" fontWeight="medium" color={activeColor}>
                {crumb.label}
              </Text>
            ) : (
              <>
                <Link to={crumb.path}>
                  <Text
                    fontSize="sm"
                    color={textColor}
                    _hover={{
                      color: hoverColor,
                      textDecoration: 'underline',
                    }}
                    transition="color 0.2s"
                  >
                    {crumb.label}
                  </Text>
                </Link>
                <Box color={separatorColor} display="flex" alignItems="center">
                  <ChevronRightIcon />
                </Box>
              </>
            )}
          </Flex>
        );
      })}
    </Flex>
  );
};

export default Breadcrumb;
