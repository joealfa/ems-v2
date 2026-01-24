import { useMemo } from 'react';
import {
  themeQuartz,
  colorSchemeDarkBlue,
  colorSchemeLightCold,
} from 'ag-grid-community';
import { useColorMode } from './color-mode';

export const useAgGridTheme = () => {
  const { colorMode } = useColorMode();

  const theme = useMemo(() => {
    const baseTheme = themeQuartz.withParams({
      fontFamily:
        'system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif',
      fontSize: 14,
      headerFontSize: 14,
      headerFontWeight: 600,
      rowHeight: 44,
      headerHeight: 44,
      cellHorizontalPadding: 16,
      inputBorderRadius: 6,
      wrapperBorderRadius: 8,
      spacing: 8,
    });

    if (colorMode === 'dark') {
      return baseTheme.withPart(colorSchemeDarkBlue);
    }
    return baseTheme.withPart(colorSchemeLightCold);
  }, [colorMode]);

  return theme;
};
