export const getDocumentTypeColor = (type: string | undefined): string => {
  switch (type) {
    case 'Pdf':
      return 'red';
    case 'Word':
      return 'blue';
    case 'Excel':
      return 'green';
    case 'PowerPoint':
      return 'orange';
    case 'ImageJpeg':
    case 'ImagePng':
      return 'purple';
    default:
      return 'gray';
  }
};

export const getInitials = (
  firstName?: string | null | undefined,
  lastName?: string | null | undefined
): string => {
  if (!firstName && !lastName) return '?';

  const firstInitial = firstName?.charAt(0).toUpperCase() || '';
  const lastInitial = lastName?.charAt(0).toUpperCase() || '';

  return `${firstInitial}${lastInitial}`.trim() || '?';
};
