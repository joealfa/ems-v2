import { ChakraProvider } from '@chakra-ui/react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { system } from './theme';
import { ColorModeProvider } from './components/ui/color-mode';
import { MainLayout } from './components/layout';
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
} from './pages';

const App = () => {
  return (
    <ChakraProvider value={system}>
      <ColorModeProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<MainLayout />}>
              <Route index element={<Dashboard />} />

              {/* Persons Routes */}
              <Route path="persons" element={<PersonsPage />} />
              <Route path="persons/new" element={<PersonFormPage />} />
              <Route path="persons/:displayId" element={<PersonDetailPage />} />
              <Route
                path="persons/:displayId/edit"
                element={<PersonFormPage />}
              />

              {/* Schools Routes */}
              <Route path="schools" element={<SchoolsPage />} />
              <Route path="schools/new" element={<SchoolFormPage />} />
              <Route path="schools/:displayId" element={<SchoolDetailPage />} />
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
              <Route path="items/:displayId/edit" element={<ItemFormPage />} />

              {/* Employments Routes */}
              <Route path="employments" element={<EmploymentsPage />} />
              <Route path="employments/new" element={<EmploymentFormPage />} />
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
        </BrowserRouter>
      </ColorModeProvider>
    </ChakraProvider>
  );
};

export default App;
