import { Box, Flex, Text, IconButton } from '@chakra-ui/react';
import { useColorMode } from '../ui/color-mode';

const Header = () => {
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

export default Header;
