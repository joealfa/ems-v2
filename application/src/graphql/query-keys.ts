export const personKeys = {
  all: ['persons'] as const,
  lists: () => [...personKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) =>
    [...personKeys.lists(), filters] as const,
  details: () => [...personKeys.all, 'detail'] as const,
  detail: (displayId: number) => [...personKeys.details(), displayId] as const,
} as const;

export const employmentKeys = {
  all: ['employments'] as const,
  lists: () => [...employmentKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) =>
    [...employmentKeys.lists(), filters] as const,
  details: () => [...employmentKeys.all, 'detail'] as const,
  detail: (displayId: number) =>
    [...employmentKeys.details(), displayId] as const,
} as const;

export const schoolKeys = {
  all: ['schools'] as const,
  lists: () => [...schoolKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) =>
    [...schoolKeys.lists(), filters] as const,
  details: () => [...schoolKeys.all, 'detail'] as const,
  detail: (displayId: number) => [...schoolKeys.details(), displayId] as const,
} as const;

export const positionKeys = {
  all: ['positions'] as const,
  lists: () => [...positionKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) =>
    [...positionKeys.lists(), filters] as const,
  details: () => [...positionKeys.all, 'detail'] as const,
  detail: (displayId: number) =>
    [...positionKeys.details(), displayId] as const,
} as const;

export const salaryGradeKeys = {
  all: ['salaryGrades'] as const,
  lists: () => [...salaryGradeKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) =>
    [...salaryGradeKeys.lists(), filters] as const,
  details: () => [...salaryGradeKeys.all, 'detail'] as const,
  detail: (displayId: number) =>
    [...salaryGradeKeys.details(), displayId] as const,
} as const;

export const itemKeys = {
  all: ['items'] as const,
  lists: () => [...itemKeys.all, 'list'] as const,
  list: (filters: Record<string, unknown>) =>
    [...itemKeys.lists(), filters] as const,
  details: () => [...itemKeys.all, 'detail'] as const,
  detail: (displayId: number) => [...itemKeys.details(), displayId] as const,
} as const;

export const documentKeys = {
  all: ['documents'] as const,
  lists: () => [...documentKeys.all, 'list'] as const,
  list: (personDisplayId: number, filters?: Record<string, unknown>) =>
    [...documentKeys.lists(), personDisplayId, filters] as const,
  details: () => [...documentKeys.all, 'detail'] as const,
  detail: (personDisplayId: number, documentDisplayId: number) =>
    [...documentKeys.details(), personDisplayId, documentDisplayId] as const,
  profileImage: (personDisplayId: number) =>
    [...documentKeys.all, 'profileImage', personDisplayId] as const,
} as const;

export const dashboardKeys = {
  all: ['dashboard'] as const,
  stats: () => [...dashboardKeys.all, 'stats'] as const,
} as const;

export const authKeys = {
  all: ['auth'] as const,
  currentUser: () => [...authKeys.all, 'currentUser'] as const,
} as const;
