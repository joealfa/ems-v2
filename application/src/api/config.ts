import axios from 'axios';
import { Configuration } from './generated';

// Base URL for the API
export const API_BASE_URL = 'http://localhost:5062';

// Create axios instance with default configuration
export const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// API configuration for generated clients
export const apiConfiguration = new Configuration({
  basePath: API_BASE_URL,
});

// Request interceptor for adding auth tokens (if needed in the future)
axiosInstance.interceptors.request.use(
  config => {
    // Add auth token here if needed
    // const token = localStorage.getItem('token');
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`;
    // }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Response interceptor for handling errors
axiosInstance.interceptors.response.use(
  response => response,
  error => {
    // Handle common errors here
    if (error.response?.status === 401) {
      // Handle unauthorized
      console.error('Unauthorized request');
    }
    return Promise.reject(error);
  }
);
