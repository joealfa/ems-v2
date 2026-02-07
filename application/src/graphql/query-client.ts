import {
  QueryClient,
  QueryCache,
  MutationCache,
  type DefaultOptions,
} from '@tanstack/react-query';
import { handleGlobalError } from './error-handler';

const defaultOptions: DefaultOptions = {
  queries: {
    staleTime: 2 * 60 * 1000,
    gcTime: 5 * 60 * 1000,
    refetchOnWindowFocus: true,
    refetchOnMount: true,
    retry: 1,
    retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 10000),
  },
  mutations: {
    retry: false,
  },
};

export const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error) => {
      handleGlobalError(error);
    },
  }),
  mutationCache: new MutationCache({
    onError: (error) => {
      handleGlobalError(error);
    },
  }),
  defaultOptions,
});
