import { lazy, Suspense } from 'react';
import { ChakraProvider, Spinner, Center } from '@chakra-ui/react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { system } from './theme';
import { ColorModeProvider } from './components/ui/color-mode';
import { MainLayout } from './components/layout';
import { AuthProvider } from './contexts/AuthContext';
import { ApolloProvider } from './graphql';
import ProtectedRoute from './components/auth/ProtectedRoute';

// Lazy load page components for code splitting
const Dashboard = lazy(() => import('./pages/Dashboard'));
const LoginPage = lazy(() => import('./pages/LoginPage'));

// Persons
const PersonsPage = lazy(() => import('./pages/persons/PersonsPage'));
const PersonFormPage = lazy(() => import('./pages/persons/PersonFormPage'));
const PersonDetailPage = lazy(() => import('./pages/persons/PersonDetailPage'));

// Schools
const SchoolsPage = lazy(() => import('./pages/schools/SchoolsPage'));
const SchoolFormPage = lazy(() => import('./pages/schools/SchoolFormPage'));
const SchoolDetailPage = lazy(() => import('./pages/schools/SchoolDetailPage'));

// Positions
const PositionsPage = lazy(() => import('./pages/positions/PositionsPage'));
const PositionFormPage = lazy(
  () => import('./pages/positions/PositionFormPage')
);
const PositionDetailPage = lazy(
  () => import('./pages/positions/PositionDetailPage')
);

// Salary Grades
const SalaryGradesPage = lazy(
  () => import('./pages/salary-grades/SalaryGradesPage')
);
const SalaryGradeFormPage = lazy(
  () => import('./pages/salary-grades/SalaryGradeFormPage')
);
const SalaryGradeDetailPage = lazy(
  () => import('./pages/salary-grades/SalaryGradeDetailPage')
);

// Items
const ItemsPage = lazy(() => import('./pages/items/ItemsPage'));
const ItemFormPage = lazy(() => import('./pages/items/ItemFormPage'));
const ItemDetailPage = lazy(() => import('./pages/items/ItemDetailPage'));

// Employments
const EmploymentsPage = lazy(
  () => import('./pages/employments/EmploymentsPage')
);
const EmploymentFormPage = lazy(
  () => import('./pages/employments/EmploymentFormPage')
);
const EmploymentDetailPage = lazy(
  () => import('./pages/employments/EmploymentDetailPage')
);

// Loading fallback component
const PageLoader = () => (
  <Center h="100vh">
    <Spinner size="xl" />
  </Center>
);

const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID || '';

if (!googleClientId) {
  console.warn(
    'Warning: VITE_GOOGLE_CLIENT_ID is not configured. Google Sign-In will not work.'
  );
}

const App = () => {
  return (
    <GoogleOAuthProvider clientId={googleClientId}>
      <ApolloProvider>
        <ChakraProvider value={system}>
          <ColorModeProvider>
            <BrowserRouter>
              <AuthProvider>
                <Suspense fallback={<PageLoader />}>
                  <Routes>
                    <Route path="/login" element={<LoginPage />} />
                    <Route
                      path="/"
                      element={
                        <ProtectedRoute>
                          <MainLayout />
                        </ProtectedRoute>
                      }
                    >
                      <Route index element={<Dashboard />} />

                      {/* Persons Routes */}
                      <Route path="persons" element={<PersonsPage />} />
                      <Route path="persons/new" element={<PersonFormPage />} />
                      <Route
                        path="persons/:displayId"
                        element={<PersonDetailPage />}
                      />
                      <Route
                        path="persons/:displayId/edit"
                        element={<PersonFormPage />}
                      />

                      {/* Schools Routes */}
                      <Route path="schools" element={<SchoolsPage />} />
                      <Route path="schools/new" element={<SchoolFormPage />} />
                      <Route
                        path="schools/:displayId"
                        element={<SchoolDetailPage />}
                      />
                      <Route
                        path="schools/:displayId/edit"
                        element={<SchoolFormPage />}
                      />

                      {/* Positions Routes */}
                      <Route path="positions" element={<PositionsPage />} />
                      <Route
                        path="positions/new"
                        element={<PositionFormPage />}
                      />
                      <Route
                        path="positions/:displayId"
                        element={<PositionDetailPage />}
                      />
                      <Route
                        path="positions/:displayId/edit"
                        element={<PositionFormPage />}
                      />

                      {/* Salary Grades Routes */}
                      <Route
                        path="salary-grades"
                        element={<SalaryGradesPage />}
                      />
                      <Route
                        path="salary-grades/new"
                        element={<SalaryGradeFormPage />}
                      />
                      <Route
                        path="salary-grades/:displayId"
                        element={<SalaryGradeDetailPage />}
                      />
                      <Route
                        path="salary-grades/:displayId/edit"
                        element={<SalaryGradeFormPage />}
                      />

                      {/* Items Routes */}
                      <Route path="items" element={<ItemsPage />} />
                      <Route path="items/new" element={<ItemFormPage />} />
                      <Route
                        path="items/:displayId"
                        element={<ItemDetailPage />}
                      />
                      <Route
                        path="items/:displayId/edit"
                        element={<ItemFormPage />}
                      />

                      {/* Employments Routes */}
                      <Route path="employments" element={<EmploymentsPage />} />
                      <Route
                        path="employments/new"
                        element={<EmploymentFormPage />}
                      />
                      <Route
                        path="employments/:displayId"
                        element={<EmploymentDetailPage />}
                      />
                      <Route
                        path="employments/:displayId/edit"
                        element={<EmploymentFormPage />}
                      />
                    </Route>
                  </Routes>
                </Suspense>
              </AuthProvider>
            </BrowserRouter>
          </ColorModeProvider>
        </ChakraProvider>
      </ApolloProvider>
    </GoogleOAuthProvider>
  );
};

export default App;
