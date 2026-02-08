// Re-export all utility functions for easier imports
// Usage: import { formatCurrency, getActivityIcon } from '@/utils';

// Formatters
export {
  formatEnumLabel,
  formatCurrency,
  formatAddress,
  formatFileSize,
  formatTimestamp,
} from './formatters';

// Helpers
export {
  getDocumentTypeColor,
  getInitials,
  getActivityIcon,
} from './helper';

// Mappers
export {
  AppointmentStatusOptions,
  EmploymentStatusOptions,
  EligibilityOptions,
} from './mapper';

// Development authentication (only available in dev mode)
export { getDevToken, generateAndStoreDevToken } from './devAuth';
