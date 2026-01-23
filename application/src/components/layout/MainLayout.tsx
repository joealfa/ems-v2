import { Box, Flex } from '@chakra-ui/react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Header from './Header';

const MainLayout = () => {
  return (
    <Flex minH="100vh">
      <Sidebar />
      <Box flex={1} display="flex" flexDirection="column">
        <Header />
        <Box as="main" flex={1} p={6} bg="bg" overflow="auto">
          <Outlet />
        </Box>
      </Box>
    </Flex>
  );
};

export default MainLayout;
