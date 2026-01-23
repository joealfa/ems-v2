import { createSystem, defaultConfig, defineConfig } from '@chakra-ui/react';

const config = defineConfig({
  theme: {
    tokens: {
      colors: {
        brand: {
          50: { value: '#e6f2ff' },
          100: { value: '#b3d9ff' },
          200: { value: '#80bfff' },
          300: { value: '#4da6ff' },
          400: { value: '#1a8cff' },
          500: { value: '#0073e6' },
          600: { value: '#005bb3' },
          700: { value: '#004280' },
          800: { value: '#002a4d' },
          900: { value: '#00111a' },
        },
      },
    },
    semanticTokens: {
      colors: {
        bg: {
          DEFAULT: {
            value: { _light: '{colors.gray.100}', _dark: '{colors.gray.800}' },
          },
          muted: {
            value: { _light: '{colors.gray.200}', _dark: '{colors.gray.700}' },
          },
          subtle: {
            value: { _light: '{colors.gray.50}', _dark: '{colors.gray.900}' },
          },
          panel: {
            value: { _light: '{colors.white}', _dark: '{colors.gray.800}' },
          },
        },
        border: {
          DEFAULT: {
            value: { _light: '{colors.gray.300}', _dark: '{colors.gray.600}' },
          },
          muted: {
            value: { _light: '{colors.gray.200}', _dark: '{colors.gray.700}' },
          },
        },
      },
    },
  },
});

export const system = createSystem(defaultConfig, config);
