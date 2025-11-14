import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios';
import type { ApiError } from '@/types/api';
import { useNetworkStore } from '@/stores/networkStore';
import { retryWithBackoff } from '@/utils/retryWithBackoff';

// Create Axios instance with base configuration
const httpClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Flag to track if we're currently retrying
let isRetrying = false;

// Request interceptor - attach authentication token and check network status
httpClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Check network status before making request
    const networkStore = useNetworkStore();
    
    // If offline and not retrying, reject immediately
    if (networkStore.isOffline && !isRetrying) {
      const offlineError = new Error('Network is offline');
      offlineError.name = 'NetworkOfflineError';
      return Promise.reject(offlineError);
    }
    
    // Get token from localStorage
    const token = localStorage.getItem('auth_token');
    
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Log requests in development mode
    if (import.meta.env.DEV && import.meta.env.VITE_ENABLE_API_LOGGING !== 'false') {
      console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, {
        params: config.params,
        data: config.data,
        headers: config.headers,
      });
    }
    
    return config;
  },
  (error) => {
    if (import.meta.env.DEV) {
      console.error('[API Request Error]', error);
    }
    return Promise.reject(error);
  }
);

// Response interceptor - handle errors, logging, and network status
httpClient.interceptors.response.use(
  (response) => {
    // Record successful fetch
    const networkStore = useNetworkStore();
    networkStore.recordSuccessfulFetch();
    
    // Ensure we're marked as online
    if (networkStore.isOffline) {
      networkStore.setOnline(true);
    }
    
    // Log responses in development mode
    if (import.meta.env.DEV && import.meta.env.VITE_ENABLE_API_LOGGING !== 'false') {
      console.log(`[API Response] ${response.config.method?.toUpperCase()} ${response.config.url}`, {
        status: response.status,
        data: response.data,
      });
    }
    
    return response;
  },
  async (error: AxiosError) => {
    const networkStore = useNetworkStore();
    
    // Log errors in development mode
    if (import.meta.env.DEV) {
      console.error('[API Error]', {
        url: error.config?.url,
        method: error.config?.method,
        status: error.response?.status,
        data: error.response?.data,
        message: error.message,
      });
    }
    
    // Detect network errors (no response from server)
    if (!error.response) {
      networkStore.recordFailedRequest();
      
      // Mark as offline if we have multiple failed requests
      if (networkStore.failedRequestCount >= 2) {
        networkStore.setOnline(false);
      }
    }
    
    // Handle authentication errors (401)
    if (error.response?.status === 401) {
      // Clear token from localStorage
      localStorage.removeItem('auth_token');
      
      // Redirect to login page with return URL
      const currentPath = window.location.pathname;
      if (currentPath !== '/login') {
        window.location.href = `/login?redirect=${encodeURIComponent(currentPath)}`;
      }
    }
    
    // Handle server errors (500+)
    if (error.response?.status && error.response.status >= 500) {
      console.error('Server error occurred:', error.response.data);
      networkStore.recordFailedRequest();
    }
    
    return Promise.reject(error);
  }
);

/**
 * Transform Axios error to ApiError type
 */
export function handleApiError(err: unknown): ApiError {
  if (axios.isAxiosError(err)) {
    const axiosError = err as AxiosError<any>;
    
    return {
      message: axiosError.response?.data?.message || axiosError.message || 'An error occurred',
      errors: axiosError.response?.data?.errors,
      statusCode: axiosError.response?.status || 500,
    };
  }
  
  // Handle network offline error
  if (err instanceof Error && err.name === 'NetworkOfflineError') {
    return {
      message: 'Network is offline. Please check your internet connection.',
      statusCode: 0,
    };
  }
  
  return {
    message: err instanceof Error ? err.message : 'An unexpected error occurred',
    statusCode: 500,
  };
}

/**
 * Make an HTTP request with automatic retry on network errors
 * @param requestFn Function that makes the HTTP request
 * @param enableRetry Whether to enable retry logic (default: true)
 */
export async function httpClientWithRetry<T>(
  requestFn: () => Promise<T>,
  enableRetry: boolean = true
): Promise<T> {
  if (!enableRetry) {
    return requestFn();
  }
  
  isRetrying = true;
  try {
    const result = await retryWithBackoff(requestFn, {
      maxRetries: 3,
      initialDelay: 1000,
      maxDelay: 10000,
      backoffMultiplier: 2,
    });
    return result;
  } finally {
    isRetrying = false;
  }
}

export default httpClient;
