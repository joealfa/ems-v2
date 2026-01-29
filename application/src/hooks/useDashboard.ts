import { useQuery } from '@apollo/client';
import { GetDashboardStatsDocument } from '../graphql/generated/graphql';

/**
 * Hook to fetch dashboard statistics
 */
export function useDashboardStats() {
  const { data, loading, error, refetch } = useQuery(
    GetDashboardStatsDocument,
    {
      fetchPolicy: 'cache-and-network',
    }
  );

  return {
    stats: data?.dashboardStats,
    loading,
    error,
    refetch,
  };
}
