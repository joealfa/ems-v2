import { ApolloProvider as BaseApolloProvider } from '@apollo/client';
import type { ReactNode } from 'react';
import { apolloClient } from './client';

interface ApolloProviderProps {
  children: ReactNode;
}

export function ApolloProvider({ children }: ApolloProviderProps) {
  return (
    <BaseApolloProvider client={apolloClient}>{children}</BaseApolloProvider>
  );
}

export default ApolloProvider;
