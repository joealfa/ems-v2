/**
 * Converts enum values to readable labels.
 * Example: "FIRST_NAME" -> "First Name"
 * @param value - The enum value to format
 * @returns The formatted label
 */
export const formatEnumLabel = (value: string): string => {
  return value
    .split('_')
    .map((word) => word.charAt(0) + word.slice(1).toLowerCase())
    .join(' ');
};

/**
 * Formats a number as Philippine Peso currency.
 * Example: 1500 -> "₱1,500.00"
 * @param value - The number to format
 * @returns The formatted currency string
 */
export const formatCurrency = (value: number | undefined): string => {
  if (value === undefined || value === null) return '-';
  return new Intl.NumberFormat('en-PH', {
    style: 'currency',
    currency: 'PHP',
  }).format(value);
};

/**
 * Formats an address object into a single string.
 * Omits any undefined or null fields.
 * Example: { address1: "123 Main St", city: "Manila" } -> "123 Main St, Manila"
 * @param address - The address object to format
 * @returns The formatted address string
 */
export const formatAddress = (address: {
  address1?: string | null;
  address2?: string | null;
  barangay?: string | null;
  city?: string | null;
  province?: string | null;
  country?: string | null;
  zipCode?: string | null;
}): string => {
  const parts = [
    address.address1,
    address.address2,
    address.barangay,
    address.city,
    address.province,
    address.country,
    address.zipCode,
  ].filter(Boolean);
  return parts.join(', ') || '-';
};

/**
 * Formats a file size in bytes into a human-readable string.
 * Example: 2048 -> "2.0 KB"
 * @param bytes - The file size in bytes
 * @returns The formatted file size string
 */
export const formatFileSize = (bytes: number | undefined): string => {
  if (!bytes) return '-';
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
};
