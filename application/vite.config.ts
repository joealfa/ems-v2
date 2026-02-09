import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    preserveSymlinks: true,
  },
  server: {
    port: 5173,
    https: {
      key: fs.readFileSync(path.resolve(__dirname, 'certs/localhost-key.pem')),
      cert: fs.readFileSync(path.resolve(__dirname, 'certs/localhost.pem')),
    },
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
          'vendor-misc': ['@react-oauth/google'],
        },
      },
    },
  },
});
