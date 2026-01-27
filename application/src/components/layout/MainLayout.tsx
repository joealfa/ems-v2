import { useState } from 'react';
import { Box } from '@chakra-ui/react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Header from './Header';

const SIDEBAR_WIDTH = '250px';
const SIDEBAR_COLLAPSED_WIDTH = '60px';
const HEADER_HEIGHT = '60px';

const MainLayout = () => {
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);

  const toggleSidebar = () => {
    setSidebarCollapsed(prev => !prev);
  };

  const currentSidebarWidth = sidebarCollapsed
    ? SIDEBAR_COLLAPSED_WIDTH
    : SIDEBAR_WIDTH;

  return (
    <Box h="100vh" overflow="hidden">
      {/* Fixed Sidebar */}
      <Sidebar collapsed={sidebarCollapsed} onToggle={toggleSidebar} />

      {/* Main content area with fixed header */}
      <Box
        ml={currentSidebarWidth}
        h="100vh"
        display="flex"
        flexDirection="column"
        transition="margin-left 0.3s ease"
      >
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
