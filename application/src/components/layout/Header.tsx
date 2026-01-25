import { Box, Flex, Text, IconButton, Image, Button } from '@chakra-ui/react';
import { useColorMode } from '../ui/color-mode';
import { useAuth } from '../../hooks/useAuth';

const Header = () => {
  const { colorMode, toggleColorMode } = useColorMode();
  const { user, logout, isAuthenticated } = useAuth();

  const handleLogout = async () => {
    await logout();
  };

  return (
    <Box
      as="header"
      h="60px"
      minH="60px"
      bg="bg.panel"
      borderBottom="1px solid"
      borderColor="border.muted"
      px={6}
      position="sticky"
      top={0}
      zIndex={10}
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
