import type {
  AppointmentStatus,
  EmploymentStatus,
  Eligibility,
} from '../graphql/generated/graphql';

const appointmentStatusLabels: Record<AppointmentStatus, string> = {
  Original: 'New Hire',
  Promotion: 'Advancement',
  Reappointment: 'Renewal',
  Transfer: 'Lateral Move',
};

export const AppointmentStatusOptions = Object.keys(
  appointmentStatusLabels
) as AppointmentStatus[];

const EmploymentStatusLabels: Record<EmploymentStatus, string> = {
  Regular: 'Regular',
  Permanent: 'Permanent',
};

export const EmploymentStatusOptions = Object.keys(
  EmploymentStatusLabels
) as EmploymentStatus[];

const eligibilityLabels: Record<Eligibility, string> = {
  LET: 'LET',
  PBET: 'PBET',
  CivilServiceProfessional: 'Civil Service Professional',
  CivilServiceSubProfessional: 'Civil Service Sub-Professional',
  Other: 'Other',
};

export const EligibilityOptions = Object.keys(
  eligibilityLabels
) as Eligibility[];
