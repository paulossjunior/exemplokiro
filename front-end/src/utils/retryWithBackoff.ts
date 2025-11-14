import { AxiosError } from 'axios';

export interface RetryConfig {
  maxRetries?: number;
  initialDelay?: number;
  maxDelay?: number;
  backoffMultiplier?: number;
}

const DEFAULT_CONFIG: Required<RetryConfig> = {
  maxRetries: 3,
  initialDelay: 1000, // 1 second
  maxDelay: 10000, // 10 seconds
  backoffMultiplier: 2,
};

/**
 * Determines if an error is retryable (network errors, 5xx errors)
 */
function isRetryableError(error: unknown): boolean {
  if (error instanceof AxiosError) {
    // Network errors (no response)
    if (!error.response) {
      return true;
    }
    
    // Server errors (5xx)
    if (error.response.status >= 500) {
      return true;
    }
    
    // Timeout errors
    if (error.code === 'ECONNABORTED') {
      return true;
    }
  }
  
  return false;
}

/**
 * Calculate delay for exponential backoff
 */
function calculateDelay(attempt: number, config: Required<RetryConfig>): number {
  const delay = config.initialDelay * Math.pow(config.backoffMultiplier, attempt);
  return Math.min(delay, config.maxDelay);
}

/**
 * Sleep for specified milliseconds
 */
function sleep(ms: number): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms));
}

/**
 * Retry a function with exponential backoff
 */
export async function retryWithBackoff<T>(
  fn: () => Promise<T>,
  config: RetryConfig = {}
): Promise<T> {
  const finalConfig = { ...DEFAULT_CONFIG, ...config };
  let lastError: unknown;
  
  for (let attempt = 0; attempt <= finalConfig.maxRetries; attempt++) {
    try {
      const result = await fn();
      return result;
    } catch (error) {
      lastError = error;
      
      // Don't retry if error is not retryable
      if (!isRetryableError(error)) {
        throw error;
      }
      
      // Don't retry if we've exhausted all attempts
      if (attempt >= finalConfig.maxRetries) {
        throw error;
      }
      
      // Calculate delay and wait before retrying
      const delay = calculateDelay(attempt, finalConfig);
      
      if (import.meta.env.DEV) {
        console.log(`[Retry] Attempt ${attempt + 1}/${finalConfig.maxRetries} failed. Retrying in ${delay}ms...`);
      }
      
      await sleep(delay);
    }
  }
  
  throw lastError;
}
