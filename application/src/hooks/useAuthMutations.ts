import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { graphqlRequest } from '../graphql/graphql-client';
import { setAuthHeader } from '../graphql/graphql-client';
import { authKeys } from '../graphql/query-keys';
import {
  GoogleLoginDocument,
  GoogleTokenLoginDocument,
  RefreshTokenDocument,
  LogoutDocument,
  GetCurrentUserDocument,
  type GoogleLoginMutation,
  type GoogleLoginMutationVariables,
  type GoogleTokenLoginMutation,
  type GoogleTokenLoginMutationVariables,
  type RefreshTokenMutation,
  type RefreshTokenMutationVariables,
  type LogoutMutation,
  type LogoutMutationVariables,
  type GetCurrentUserQuery,
} from '../graphql/generated/graphql';

export function useGoogleLogin() {
  const mutation = useMutation({
    mutationFn: (variables: GoogleLoginMutationVariables) =>
      graphqlRequest<GoogleLoginMutation, GoogleLoginMutationVariables>(
        GoogleLoginDocument,
        variables
      ),
    onSuccess: (data) => {
      if (data.googleLogin?.accessToken) {
        localStorage.setItem('accessToken', data.googleLogin.accessToken);
        setAuthHeader(data.googleLogin.accessToken);
      }
    },
  });

  const handleLogin = async (idToken: string) => {
    const result = await mutation.mutateAsync({ idToken });
    return result.googleLogin;
  };

  return {
    login: handleLogin,
    authResponse: mutation.data?.googleLogin,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useGoogleTokenLogin() {
  const mutation = useMutation({
    mutationFn: (variables: GoogleTokenLoginMutationVariables) =>
      graphqlRequest<
        GoogleTokenLoginMutation,
        GoogleTokenLoginMutationVariables
      >(GoogleTokenLoginDocument, variables),
    onSuccess: (data) => {
      if (data.googleTokenLogin?.accessToken) {
        localStorage.setItem('accessToken', data.googleTokenLogin.accessToken);
        setAuthHeader(data.googleTokenLogin.accessToken);
      }
    },
  });

  const handleLogin = async (accessToken: string) => {
    const result = await mutation.mutateAsync({ accessToken });
    return result.googleTokenLogin;
  };

  return {
    login: handleLogin,
    authResponse: mutation.data?.googleTokenLogin,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useRefreshToken() {
  const mutation = useMutation({
    mutationFn: (variables: RefreshTokenMutationVariables) =>
      graphqlRequest<RefreshTokenMutation, RefreshTokenMutationVariables>(
        RefreshTokenDocument,
        variables
      ),
  });

  const handleRefresh = async (refreshToken?: string) => {
    const result = await mutation.mutateAsync({ refreshToken });
    return result.refreshToken;
  };

  return {
    refreshToken: handleRefresh,
    authResponse: mutation.data?.refreshToken,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useLogout() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (variables: LogoutMutationVariables) =>
      graphqlRequest<LogoutMutation, LogoutMutationVariables>(
        LogoutDocument,
        variables
      ),
    onSuccess: (data) => {
      if (data.logout) {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('tokenExpiry');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        setAuthHeader(null);
        queryClient.clear();
      }
    },
  });

  const handleLogout = async (refreshToken?: string) => {
    const result = await mutation.mutateAsync({ refreshToken });
    return result.logout ?? false;
  };

  return {
    logout: handleLogout,
    loading: mutation.isPending,
    error: mutation.error,
  };
}

export function useCurrentUser() {
  const token = localStorage.getItem('accessToken');

  const query = useQuery({
    queryKey: authKeys.currentUser(),
    queryFn: () =>
      graphqlRequest<GetCurrentUserQuery, Record<string, unknown>>(
        GetCurrentUserDocument
      ),
    enabled: !!token,
    staleTime: Infinity,
    gcTime: Infinity,
  });

  return {
    user: query.data?.currentUser,
    loading: query.isPending,
    error: query.error,
    refetch: query.refetch,
  };
}
