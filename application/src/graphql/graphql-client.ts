import { GraphQLClient, type RequestDocument } from 'graphql-request';
import type { TypedDocumentNode } from '@graphql-typed-document-node/core';

const GRAPHQL_URL =
  import.meta.env.VITE_GRAPHQL_URL || 'https://localhost:5003/graphql';

export const graphqlClient = new GraphQLClient(GRAPHQL_URL, {
  credentials: 'include',
  headers: {
    'GraphQL-Preflight': '1',
  },
});

export const setAuthHeader = (token: string | null) => {
  if (token) {
    graphqlClient.setHeader('authorization', `Bearer ${token}`);
  } else {
    graphqlClient.setHeader('authorization', '');
  }
};

export const graphqlRequest = <
  TData,
  TVariables extends Record<string, unknown> = Record<string, unknown>,
>(
  document: RequestDocument | TypedDocumentNode<TData, TVariables>,
  variables?: TVariables
): Promise<TData> => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    graphqlClient.setHeader('authorization', `Bearer ${token}`);
  }
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return graphqlClient.request<TData>(document as any, variables as any);
};
