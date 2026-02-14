import { GraphQLClient, type RequestDocument } from 'graphql-request';
import type { TypedDocumentNode } from '@graphql-typed-document-node/core';

// Resolve relative URLs (e.g. /graphql) to absolute URLs
// graphql-request internally uses new URL() which requires absolute URLs
const resolveUrl = (url: string): string => {
  if (url.startsWith('/')) {
    return `${window.location.origin}${url}`;
  }
  return url;
};

const GRAPHQL_URL = resolveUrl(
  import.meta.env.VITE_GRAPHQL_URL || 'https://localhost:5003/graphql'
);

export const graphqlClient = new GraphQLClient(GRAPHQL_URL, {
  credentials: 'include',
  headers: {
    'GraphQL-Preflight': '1',
  },
});

export const setAuthHeader = (_token: string | null) => {
  // No-op: auth is handled via HttpOnly cookies
};

export const graphqlRequest = <
  TData,
  TVariables extends Record<string, unknown> = Record<string, unknown>,
>(
  document: RequestDocument | TypedDocumentNode<TData, TVariables>,
  variables?: TVariables
): Promise<TData> => {
  // Auth is handled via HttpOnly cookies sent automatically with credentials: 'include'
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return graphqlClient.request<TData>(document as any, variables as any);
};
