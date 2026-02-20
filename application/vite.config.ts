import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';

const certPath = path.resolve(__dirname, 'certs/localhost.pem');
const keyPath = path.resolve(__dirname, 'certs/localhost-key.pem');
const certsExist = fs.existsSync(certPath) && fs.existsSync(keyPath);

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    preserveSymlinks: true,
  },
  server: {
    port: 5173,
    ...(certsExist && {
      https: {
        key: fs.readFileSync(keyPath),
        cert: fs.readFileSync(certPath),
      },
    }),
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
