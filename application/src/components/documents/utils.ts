export const formatFileSize = (bytes: number | undefined): string => {
  if (!bytes) return '-';
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
};

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
