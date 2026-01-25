import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    preserveSymlinks: true,
  },
  server: {
    port: 5173,
    fs: {
      strict: false,
      allow: ['..'],
    },
  },
  preview: {
    port: 5173,
  },
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          // React core
          'vendor-react': ['react', 'react-dom', 'react-router-dom'],
          // Chakra UI and Emotion
          'vendor-chakra': ['@chakra-ui/react', '@emotion/react'],
          // AG Grid
          'vendor-ag-grid': ['ag-grid-react'],
          // Other vendors
          'vendor-misc': ['axios', '@react-oauth/google'],
        },
      },
    },
  },
});
