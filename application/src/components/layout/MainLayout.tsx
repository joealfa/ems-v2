import { Box } from '@chakra-ui/react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Header from './Header';

const SIDEBAR_WIDTH = '250px';
const HEADER_HEIGHT = '60px';

const MainLayout = () => {
  return (
    <Box h="100vh" overflow="hidden">
      {/* Fixed Sidebar */}
      <Sidebar />

      {/* Main content area with fixed header */}
      <Box ml={SIDEBAR_WIDTH} h="100vh" display="flex" flexDirection="column">
        {/* Fixed Header */}
        <Header />

        {/* Scrollable Main Content */}
        <Box
          as="main"
          flex={1}
          p={6}
          bg="bg"
          overflowY="auto"
          h={`calc(100vh - ${HEADER_HEIGHT})`}
        >
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
};

export default MainLayout;
