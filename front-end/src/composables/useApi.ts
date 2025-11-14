import { ref, type Ref } from 'vue';
import { AxiosError } from 'axios';
import type { ApiError } from '@/types/api';

export interface UseApiOptions {
  immediate?: boolean;
  onSuccess?: (data: any) => void;
  onError?: (error: ApiError) => void;
}

export function useApi<T, TArgs extends any[] = []>(
  apiCall: (...args: TArgs) => Promise<T>,
  options: UseApiOptions = {}
) {
  const data: Ref<T | null> = ref(null);
  const error: Ref<ApiError | null> = ref(null);
  const loading = ref(false);

  const execute = async (...args: TArgs) => {
    loading.value = true;
    error.value = null;

    try {
      const result = await apiCall(...args);
      data.value = result;
      
      if (options.onSuccess) {
        options.onSuccess(result);
      }
      
      return result;
    } catch (err) {
      const apiError = handleApiError(err);
      error.value = apiError;
      
      if (options.onError) {
        options.onError(apiError);
      }
      
      throw apiError;
    } finally {
      loading.value = false;
    }
  };

  if (options.immediate) {
    execute(...([] as unknown as TArgs));
  }

  return {
    data,
    error,
    loading,
    execute,
  };
}

function handleApiError(err: unknown): ApiError {
  if (err instanceof AxiosError) {
    return {
      message: err.response?.data?.message || err.message || 'An error occurred',
      errors: err.response?.data?.errors,
      statusCode: err.response?.status || 500,
    };
  }
  
  return {
    message: 'An unexpected error occurred',
    statusCode: 500,
  };
}
