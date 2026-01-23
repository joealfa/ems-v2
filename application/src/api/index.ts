import {
  PersonsApi,
  SchoolsApi,
  EmploymentsApi,
  PositionsApi,
  SalaryGradesApi,
  ItemsApi,
  DocumentsApi,
} from './generated';
import { apiConfiguration, axiosInstance } from './config';

// Export configured API instances
export const personsApi = new PersonsApi(
  apiConfiguration,
  undefined,
  axiosInstance
);
export const schoolsApi = new SchoolsApi(
  apiConfiguration,
  undefined,
  axiosInstance
);
export const employmentsApi = new EmploymentsApi(
  apiConfiguration,
  undefined,
  axiosInstance
);
export const positionsApi = new PositionsApi(
  apiConfiguration,
  undefined,
  axiosInstance
);
export const salaryGradesApi = new SalaryGradesApi(
  apiConfiguration,
  undefined,
  axiosInstance
);
export const itemsApi = new ItemsApi(
  apiConfiguration,
  undefined,
  axiosInstance
);
export const documentsApi = new DocumentsApi(
  apiConfiguration,
  undefined,
  axiosInstance
);

// Re-export types for convenience
export * from './generated/models';
export { API_BASE_URL } from './config';
