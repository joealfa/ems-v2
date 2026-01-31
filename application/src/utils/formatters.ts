/**
 * Converts enum values to readable labels.
 * Example: "FIRST_NAME" -> "First Name"
 * @param value - The enum value to format
 * @returns The formatted label
 */
export const formatEnumLabel = (value: string): string => {
  return value
    .split('_')
    .map(word => word.charAt(0) + word.slice(1).toLowerCase())
    .join(' ');
};
