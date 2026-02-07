import { useQuery } from '@tanstack/react-query';
import { graphqlRequest } from '../graphql/graphql-client';
import { dashboardKeys } from '../graphql/query-keys';
import {
  GetDashboardStatsDocument,
  type GetDashboardStatsQuery,
} from '../graphql/generated/graphql';

export function useDashboardStats() {
  const query = useQuery({
    queryKey: dashboardKeys.stats(),
    queryFn: () =>
      graphqlRequest<GetDashboardStatsQuery, Record<string, unknown>>(
        GetDashboardStatsDocument
      ),
    staleTime: 1 * 60 * 1000,
  });

  return {
    stats: query.data?.dashboardStats,
    loading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
}
