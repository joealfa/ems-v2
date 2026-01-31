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
