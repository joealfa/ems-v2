import { ChakraProvider } from '@chakra-ui/react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { system } from './theme';
import { ColorModeProvider } from './components/ui/color-mode';
import { MainLayout } from './components/layout';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/auth/ProtectedRoute';
import {
  Dashboard,
  PersonsPage,
  PersonFormPage,
  PersonDetailPage,
  SchoolsPage,
  SchoolFormPage,
  SchoolDetailPage,
  PositionsPage,
  PositionFormPage,
  PositionDetailPage,
  SalaryGradesPage,
  SalaryGradeFormPage,
  SalaryGradeDetailPage,
  ItemsPage,
  ItemFormPage,
  ItemDetailPage,
  EmploymentsPage,
  EmploymentFormPage,
  EmploymentDetailPage,
  LoginPage,
} from './pages';

const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID || '';

if (!googleClientId) {
  console.warn(
    'Warning: VITE_GOOGLE_CLIENT_ID is not configured. Google Sign-In will not work.'
  );
}

const App = () => {
  return (
    <GoogleOAuthProvider clientId={googleClientId}>
      <ChakraProvider value={system}>
        <ColorModeProvider>
          <BrowserRouter>
            <AuthProvider>
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
                  <Route path="positions/new" element={<PositionFormPage />} />
                  <Route
                    path="positions/:displayId"
                    element={<PositionDetailPage />}
                  />
                  <Route
                    path="positions/:displayId/edit"
                    element={<PositionFormPage />}
                  />

                  {/* Salary Grades Routes */}
                  <Route path="salary-grades" element={<SalaryGradesPage />} />
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
                  <Route path="items/:displayId" element={<ItemDetailPage />} />
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
            </AuthProvider>
          </BrowserRouter>
        </ColorModeProvider>
      </ChakraProvider>
    </GoogleOAuthProvider>
  );
};

export default App;
