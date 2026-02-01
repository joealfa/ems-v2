/* eslint-disable */
import type { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';
export type Maybe<T> = T | null;
export type InputMaybe<T> = Maybe<T>;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
export type MakeEmpty<T extends { [key: string]: unknown }, K extends keyof T> = { [_ in K]?: never };
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: { input: string; output: string; }
  String: { input: string; output: string; }
  Boolean: { input: boolean; output: boolean; }
  Int: { input: number; output: number; }
  Float: { input: number; output: number; }
  /** The `DateTime` scalar represents an ISO-8601 compliant date time type. */
  DateTime: { input: string; output: string; }
  /** The `Decimal` scalar type represents a decimal floating-point number. */
  Decimal: { input: number; output: number; }
  /** The `LocalDate` scalar type represents a ISO date string, represented as UTF-8 character sequences YYYY-MM-DD. The scalar follows the specification defined in RFC3339 */
  LocalDate: { input: any; output: any; }
  /** The `Long` scalar type represents non-fractional signed whole 64-bit numeric values. Long can represent values between -(2^63) and 2^63 - 1. */
  Long: { input: number; output: number; }
  UUID: { input: string; output: string; }
  /** The `Upload` scalar type represents a file upload. */
  Upload: { input: File; output: File; }
};

export type AddressResponseDto = {
  __typename?: 'AddressResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  address1?: Maybe<Scalars['String']['output']>;
  address2?: Maybe<Scalars['String']['output']>;
  addressType: AddressType;
  barangay?: Maybe<Scalars['String']['output']>;
  city?: Maybe<Scalars['String']['output']>;
  country?: Maybe<Scalars['String']['output']>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  displayId: Scalars['Long']['output'];
  fullAddress?: Maybe<Scalars['String']['output']>;
  isActive: Scalars['Boolean']['output'];
  isCurrent: Scalars['Boolean']['output'];
  isPermanent: Scalars['Boolean']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  province?: Maybe<Scalars['String']['output']>;
  zipCode?: Maybe<Scalars['String']['output']>;
};

export type AddressType =
  | 'Business'
  | 'Home';

export type AppointmentStatus =
  | 'Original'
  | 'Promotion'
  | 'Reappointment'
  | 'Transfer';

export type AuthResponseDto = {
  __typename?: 'AuthResponseDto';
  accessToken?: Maybe<Scalars['String']['output']>;
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  expiresOn: Scalars['DateTime']['output'];
  refreshToken?: Maybe<Scalars['String']['output']>;
  user?: Maybe<UserDto>;
};

export type CivilStatus =
  | 'Married'
  | 'Other'
  | 'Separated'
  | 'Single'
  | 'SoloParent'
  | 'Widow';

export type ContactResponseDto = {
  __typename?: 'ContactResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  contactType: ContactType;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  displayId: Scalars['Long']['output'];
  email?: Maybe<Scalars['String']['output']>;
  fax?: Maybe<Scalars['String']['output']>;
  isActive: Scalars['Boolean']['output'];
  landLine?: Maybe<Scalars['String']['output']>;
  mobile?: Maybe<Scalars['String']['output']>;
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
};

export type ContactType =
  | 'Personal'
  | 'Work';

/** Input for creating a new address */
export type CreateAddressInput = {
  address1?: InputMaybe<Scalars['String']['input']>;
  address2?: InputMaybe<Scalars['String']['input']>;
  addressType?: InputMaybe<AddressType>;
  barangay?: InputMaybe<Scalars['String']['input']>;
  city?: InputMaybe<Scalars['String']['input']>;
  country?: InputMaybe<Scalars['String']['input']>;
  isCurrent?: InputMaybe<Scalars['Boolean']['input']>;
  isPermanent?: InputMaybe<Scalars['Boolean']['input']>;
  province?: InputMaybe<Scalars['String']['input']>;
  zipCode?: InputMaybe<Scalars['String']['input']>;
};

/** Input for creating a new contact */
export type CreateContactInput = {
  contactType?: InputMaybe<ContactType>;
  email?: InputMaybe<Scalars['String']['input']>;
  fax?: InputMaybe<Scalars['String']['input']>;
  landLine?: InputMaybe<Scalars['String']['input']>;
  mobile?: InputMaybe<Scalars['String']['input']>;
};

/** Input for creating a new employment */
export type CreateEmploymentInput = {
  appointmentStatus?: InputMaybe<AppointmentStatus>;
  dateOfOriginalAppointment?: InputMaybe<Scalars['LocalDate']['input']>;
  depEdId?: InputMaybe<Scalars['String']['input']>;
  eligibility?: InputMaybe<Eligibility>;
  employmentStatus?: InputMaybe<EmploymentStatus>;
  gsisId?: InputMaybe<Scalars['String']['input']>;
  itemDisplayId?: InputMaybe<Scalars['Long']['input']>;
  personDisplayId?: InputMaybe<Scalars['Long']['input']>;
  philHealthId?: InputMaybe<Scalars['String']['input']>;
  positionDisplayId?: InputMaybe<Scalars['Long']['input']>;
  psipopItemNumber?: InputMaybe<Scalars['String']['input']>;
  salaryGradeDisplayId?: InputMaybe<Scalars['Long']['input']>;
  schools?: InputMaybe<Array<CreateEmploymentSchoolInput>>;
  tinId?: InputMaybe<Scalars['String']['input']>;
};

/** Input for associating a school with an employment */
export type CreateEmploymentSchoolInput = {
  endDate?: InputMaybe<Scalars['LocalDate']['input']>;
  isCurrent?: InputMaybe<Scalars['Boolean']['input']>;
  schoolDisplayId?: InputMaybe<Scalars['Long']['input']>;
  startDate?: InputMaybe<Scalars['LocalDate']['input']>;
};

/** Input for creating a new item */
export type CreateItemInput = {
  description?: InputMaybe<Scalars['String']['input']>;
  itemName?: InputMaybe<Scalars['String']['input']>;
};

/** Input for creating a new person */
export type CreatePersonInput = {
  addresses?: InputMaybe<Array<CreateAddressInput>>;
  civilStatus?: InputMaybe<CivilStatus>;
  contacts?: InputMaybe<Array<CreateContactInput>>;
  dateOfBirth?: InputMaybe<Scalars['LocalDate']['input']>;
  firstName?: InputMaybe<Scalars['String']['input']>;
  gender?: InputMaybe<Gender>;
  lastName?: InputMaybe<Scalars['String']['input']>;
  middleName?: InputMaybe<Scalars['String']['input']>;
};

/** Input for creating a new position */
export type CreatePositionInput = {
  description?: InputMaybe<Scalars['String']['input']>;
  titleName?: InputMaybe<Scalars['String']['input']>;
};

/** Input for creating a new salary grade */
export type CreateSalaryGradeInput = {
  description?: InputMaybe<Scalars['String']['input']>;
  monthlySalary?: InputMaybe<Scalars['Decimal']['input']>;
  salaryGradeName?: InputMaybe<Scalars['String']['input']>;
  step?: InputMaybe<Scalars['Int']['input']>;
};

/** Input for creating a new school */
export type CreateSchoolInput = {
  addresses?: InputMaybe<Array<CreateAddressInput>>;
  contacts?: InputMaybe<Array<CreateContactInput>>;
  schoolName?: InputMaybe<Scalars['String']['input']>;
};

export type DashboardStatsDto = {
  __typename?: 'DashboardStatsDto';
  activeEmployments: Scalars['Int']['output'];
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  totalItems: Scalars['Int']['output'];
  totalPersons: Scalars['Int']['output'];
  totalPositions: Scalars['Int']['output'];
  totalSalaryGrades: Scalars['Int']['output'];
  totalSchools: Scalars['Int']['output'];
};

export type DocumentListDto = {
  __typename?: 'DocumentListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  contentType?: Maybe<Scalars['String']['output']>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  description?: Maybe<Scalars['String']['output']>;
  displayId: Scalars['Long']['output'];
  documentType: DocumentType;
  fileExtension?: Maybe<Scalars['String']['output']>;
  fileName?: Maybe<Scalars['String']['output']>;
  fileSizeBytes: Scalars['Long']['output'];
};

export type DocumentResponseDto = {
  __typename?: 'DocumentResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  blobUrl?: Maybe<Scalars['String']['output']>;
  contentType?: Maybe<Scalars['String']['output']>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  description?: Maybe<Scalars['String']['output']>;
  displayId: Scalars['Long']['output'];
  documentType: DocumentType;
  fileExtension?: Maybe<Scalars['String']['output']>;
  fileName?: Maybe<Scalars['String']['output']>;
  fileSizeBytes: Scalars['Long']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  personDisplayId: Scalars['Long']['output'];
};

export type DocumentType =
  | 'Excel'
  | 'ImageJpeg'
  | 'ImagePng'
  | 'Other'
  | 'Pdf'
  | 'PowerPoint'
  | 'Word';

export type Eligibility =
  | 'CivilServiceProfessional'
  | 'CivilServiceSubProfessional'
  | 'LET'
  | 'Other'
  | 'PBET';

export type EmploymentItemDto = {
  __typename?: 'EmploymentItemDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  displayId: Scalars['Long']['output'];
  itemName?: Maybe<Scalars['String']['output']>;
};

export type EmploymentListDto = {
  __typename?: 'EmploymentListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  depEdId?: Maybe<Scalars['String']['output']>;
  displayId: Scalars['Long']['output'];
  employeeFullName?: Maybe<Scalars['String']['output']>;
  employmentStatus: EmploymentStatus;
  isActive: Scalars['Boolean']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  positionTitle?: Maybe<Scalars['String']['output']>;
};

export type EmploymentPersonDto = {
  __typename?: 'EmploymentPersonDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  displayId: Scalars['Long']['output'];
  fullName?: Maybe<Scalars['String']['output']>;
};

export type EmploymentPositionDto = {
  __typename?: 'EmploymentPositionDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  displayId: Scalars['Long']['output'];
  titleName?: Maybe<Scalars['String']['output']>;
};

export type EmploymentResponseDto = {
  __typename?: 'EmploymentResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  appointmentStatus: AppointmentStatus;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  dateOfOriginalAppointment?: Maybe<Scalars['DateTime']['output']>;
  depEdId?: Maybe<Scalars['String']['output']>;
  displayId: Scalars['Long']['output'];
  eligibility: Eligibility;
  employmentStatus: EmploymentStatus;
  gsisId?: Maybe<Scalars['String']['output']>;
  isActive: Scalars['Boolean']['output'];
  item?: Maybe<EmploymentItemDto>;
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  person?: Maybe<EmploymentPersonDto>;
  philHealthId?: Maybe<Scalars['String']['output']>;
  position?: Maybe<EmploymentPositionDto>;
  psipopItemNumber?: Maybe<Scalars['String']['output']>;
  salaryGrade?: Maybe<EmploymentSalaryGradeDto>;
  schools?: Maybe<Array<Maybe<EmploymentSchoolResponseDto>>>;
  tinId?: Maybe<Scalars['String']['output']>;
};

export type EmploymentSalaryGradeDto = {
  __typename?: 'EmploymentSalaryGradeDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  displayId: Scalars['Long']['output'];
  monthlySalary: Scalars['Decimal']['output'];
  salaryGradeName?: Maybe<Scalars['String']['output']>;
  step: Scalars['Int']['output'];
};

export type EmploymentSchoolResponseDto = {
  __typename?: 'EmploymentSchoolResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  displayId: Scalars['Long']['output'];
  endDate?: Maybe<Scalars['DateTime']['output']>;
  isActive: Scalars['Boolean']['output'];
  isCurrent: Scalars['Boolean']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  schoolDisplayId: Scalars['Long']['output'];
  schoolName?: Maybe<Scalars['String']['output']>;
  startDate?: Maybe<Scalars['DateTime']['output']>;
};

export type EmploymentStatus =
  | 'Permanent'
  | 'Regular';

export type Gender =
  | 'Female'
  | 'Male';

export type ItemResponseDto = {
  __typename?: 'ItemResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  description?: Maybe<Scalars['String']['output']>;
  displayId: Scalars['Long']['output'];
  isActive: Scalars['Boolean']['output'];
  itemName?: Maybe<Scalars['String']['output']>;
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
};

export type KeyValuePairOfStringAndObject = {
  __typename?: 'KeyValuePairOfStringAndObject';
  key: Scalars['String']['output'];
};

export type Mutation = {
  __typename?: 'Mutation';
  /** Add a school assignment to an employment */
  addSchoolToEmployment?: Maybe<EmploymentSchoolResponseDto>;
  /** Create a new employment */
  createEmployment?: Maybe<EmploymentResponseDto>;
  /** Create a new item */
  createItem?: Maybe<ItemResponseDto>;
  /** Create a new person */
  createPerson?: Maybe<PersonResponseDto>;
  /** Create a new position */
  createPosition?: Maybe<PositionResponseDto>;
  /** Create a new salary grade */
  createSalaryGrade?: Maybe<SalaryGradeResponseDto>;
  /** Create a new school */
  createSchool?: Maybe<SchoolResponseDto>;
  /** Delete a document */
  deleteDocument: Scalars['Boolean']['output'];
  /** Delete an employment */
  deleteEmployment: Scalars['Boolean']['output'];
  /** Delete an item */
  deleteItem: Scalars['Boolean']['output'];
  /** Delete a person */
  deletePerson: Scalars['Boolean']['output'];
  /** Delete a position */
  deletePosition: Scalars['Boolean']['output'];
  /** Delete a person's profile image */
  deleteProfileImage: Scalars['Boolean']['output'];
  /** Delete a salary grade */
  deleteSalaryGrade: Scalars['Boolean']['output'];
  /** Delete a school */
  deleteSchool: Scalars['Boolean']['output'];
  /** Login with Google ID token */
  googleLogin?: Maybe<AuthResponseDto>;
  /** Login with Google access token */
  googleTokenLogin?: Maybe<AuthResponseDto>;
  /** Logout and revoke tokens */
  logout: Scalars['Boolean']['output'];
  /** Refresh authentication token */
  refreshToken?: Maybe<AuthResponseDto>;
  /** Remove a school assignment from an employment */
  removeSchoolFromEmployment: Scalars['Boolean']['output'];
  /** Update document metadata */
  updateDocument?: Maybe<DocumentResponseDto>;
  /** Update an existing employment */
  updateEmployment?: Maybe<EmploymentResponseDto>;
  /** Update an existing item */
  updateItem?: Maybe<ItemResponseDto>;
  /** Update an existing person */
  updatePerson?: Maybe<PersonResponseDto>;
  /** Update an existing position */
  updatePosition?: Maybe<PositionResponseDto>;
  /** Update an existing salary grade */
  updateSalaryGrade?: Maybe<SalaryGradeResponseDto>;
  /** Update an existing school */
  updateSchool?: Maybe<SchoolResponseDto>;
};


export type MutationAddSchoolToEmploymentArgs = {
  employmentDisplayId: Scalars['Long']['input'];
  input: CreateEmploymentSchoolInput;
};


export type MutationCreateEmploymentArgs = {
  input: CreateEmploymentInput;
};


export type MutationCreateItemArgs = {
  input: CreateItemInput;
};


export type MutationCreatePersonArgs = {
  input: CreatePersonInput;
};


export type MutationCreatePositionArgs = {
  input: CreatePositionInput;
};


export type MutationCreateSalaryGradeArgs = {
  input: CreateSalaryGradeInput;
};


export type MutationCreateSchoolArgs = {
  input: CreateSchoolInput;
};


export type MutationDeleteDocumentArgs = {
  documentDisplayId: Scalars['Long']['input'];
  personDisplayId: Scalars['Long']['input'];
};


export type MutationDeleteEmploymentArgs = {
  displayId: Scalars['Long']['input'];
};


export type MutationDeleteItemArgs = {
  displayId: Scalars['Long']['input'];
};


export type MutationDeletePersonArgs = {
  displayId: Scalars['Long']['input'];
};


export type MutationDeletePositionArgs = {
  displayId: Scalars['Long']['input'];
};


export type MutationDeleteProfileImageArgs = {
  personDisplayId: Scalars['Long']['input'];
};


export type MutationDeleteSalaryGradeArgs = {
  displayId: Scalars['Long']['input'];
};


export type MutationDeleteSchoolArgs = {
  displayId: Scalars['Long']['input'];
};


export type MutationGoogleLoginArgs = {
  idToken: Scalars['String']['input'];
};


export type MutationGoogleTokenLoginArgs = {
  accessToken: Scalars['String']['input'];
};


export type MutationLogoutArgs = {
  refreshToken?: InputMaybe<Scalars['String']['input']>;
};


export type MutationRefreshTokenArgs = {
  refreshToken?: InputMaybe<Scalars['String']['input']>;
};


export type MutationRemoveSchoolFromEmploymentArgs = {
  employmentDisplayId: Scalars['Long']['input'];
  schoolAssignmentDisplayId: Scalars['Long']['input'];
};


export type MutationUpdateDocumentArgs = {
  description?: InputMaybe<Scalars['String']['input']>;
  documentDisplayId: Scalars['Long']['input'];
  personDisplayId: Scalars['Long']['input'];
};


export type MutationUpdateEmploymentArgs = {
  displayId: Scalars['Long']['input'];
  input: UpdateEmploymentInput;
};


export type MutationUpdateItemArgs = {
  displayId: Scalars['Long']['input'];
  input: UpdateItemInput;
};


export type MutationUpdatePersonArgs = {
  displayId: Scalars['Long']['input'];
  input: UpdatePersonInput;
};


export type MutationUpdatePositionArgs = {
  displayId: Scalars['Long']['input'];
  input: UpdatePositionInput;
};


export type MutationUpdateSalaryGradeArgs = {
  displayId: Scalars['Long']['input'];
  input: UpdateSalaryGradeInput;
};


export type MutationUpdateSchoolArgs = {
  displayId: Scalars['Long']['input'];
  input: UpdateSchoolInput;
};

export type PagedResultOfDocumentListDto = {
  __typename?: 'PagedResultOfDocumentListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  hasNextPage: Scalars['Boolean']['output'];
  hasPreviousPage: Scalars['Boolean']['output'];
  items?: Maybe<Array<Maybe<DocumentListDto>>>;
  pageNumber: Scalars['Int']['output'];
  pageSize: Scalars['Int']['output'];
  totalCount: Scalars['Int']['output'];
  totalPages: Scalars['Int']['output'];
};

export type PagedResultOfEmploymentListDto = {
  __typename?: 'PagedResultOfEmploymentListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  hasNextPage: Scalars['Boolean']['output'];
  hasPreviousPage: Scalars['Boolean']['output'];
  items?: Maybe<Array<Maybe<EmploymentListDto>>>;
  pageNumber: Scalars['Int']['output'];
  pageSize: Scalars['Int']['output'];
  totalCount: Scalars['Int']['output'];
  totalPages: Scalars['Int']['output'];
};

export type PagedResultOfItemResponseDto = {
  __typename?: 'PagedResultOfItemResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  hasNextPage: Scalars['Boolean']['output'];
  hasPreviousPage: Scalars['Boolean']['output'];
  items?: Maybe<Array<Maybe<ItemResponseDto>>>;
  pageNumber: Scalars['Int']['output'];
  pageSize: Scalars['Int']['output'];
  totalCount: Scalars['Int']['output'];
  totalPages: Scalars['Int']['output'];
};

export type PagedResultOfPersonListDto = {
  __typename?: 'PagedResultOfPersonListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  hasNextPage: Scalars['Boolean']['output'];
  hasPreviousPage: Scalars['Boolean']['output'];
  items?: Maybe<Array<Maybe<PersonListDto>>>;
  pageNumber: Scalars['Int']['output'];
  pageSize: Scalars['Int']['output'];
  totalCount: Scalars['Int']['output'];
  totalPages: Scalars['Int']['output'];
};

export type PagedResultOfPositionResponseDto = {
  __typename?: 'PagedResultOfPositionResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  hasNextPage: Scalars['Boolean']['output'];
  hasPreviousPage: Scalars['Boolean']['output'];
  items?: Maybe<Array<Maybe<PositionResponseDto>>>;
  pageNumber: Scalars['Int']['output'];
  pageSize: Scalars['Int']['output'];
  totalCount: Scalars['Int']['output'];
  totalPages: Scalars['Int']['output'];
};

export type PagedResultOfSalaryGradeResponseDto = {
  __typename?: 'PagedResultOfSalaryGradeResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  hasNextPage: Scalars['Boolean']['output'];
  hasPreviousPage: Scalars['Boolean']['output'];
  items?: Maybe<Array<Maybe<SalaryGradeResponseDto>>>;
  pageNumber: Scalars['Int']['output'];
  pageSize: Scalars['Int']['output'];
  totalCount: Scalars['Int']['output'];
  totalPages: Scalars['Int']['output'];
};

export type PagedResultOfSchoolListDto = {
  __typename?: 'PagedResultOfSchoolListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  hasNextPage: Scalars['Boolean']['output'];
  hasPreviousPage: Scalars['Boolean']['output'];
  items?: Maybe<Array<Maybe<SchoolListDto>>>;
  pageNumber: Scalars['Int']['output'];
  pageSize: Scalars['Int']['output'];
  totalCount: Scalars['Int']['output'];
  totalPages: Scalars['Int']['output'];
};

export type PersonListDto = {
  __typename?: 'PersonListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  civilStatus: CivilStatus;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  dateOfBirth: Scalars['DateTime']['output'];
  displayId: Scalars['Long']['output'];
  fullName?: Maybe<Scalars['String']['output']>;
  gender: Gender;
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  profileImageUrl?: Maybe<Scalars['String']['output']>;
};

export type PersonResponseDto = {
  __typename?: 'PersonResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  addresses?: Maybe<Array<Maybe<AddressResponseDto>>>;
  civilStatus: CivilStatus;
  contacts?: Maybe<Array<Maybe<ContactResponseDto>>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  dateOfBirth: Scalars['DateTime']['output'];
  displayId: Scalars['Long']['output'];
  firstName?: Maybe<Scalars['String']['output']>;
  fullName?: Maybe<Scalars['String']['output']>;
  gender: Gender;
  lastName?: Maybe<Scalars['String']['output']>;
  middleName?: Maybe<Scalars['String']['output']>;
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  profileImageUrl?: Maybe<Scalars['String']['output']>;
};

export type PositionResponseDto = {
  __typename?: 'PositionResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  description?: Maybe<Scalars['String']['output']>;
  displayId: Scalars['Long']['output'];
  isActive: Scalars['Boolean']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  titleName?: Maybe<Scalars['String']['output']>;
};

export type Query = {
  __typename?: 'Query';
  /** Get current authenticated user */
  currentUser?: Maybe<UserDto>;
  /** Get dashboard statistics */
  dashboardStats?: Maybe<DashboardStatsDto>;
  /** Get a single document by display ID */
  document?: Maybe<DocumentResponseDto>;
  /** Get a single employment by display ID */
  employment?: Maybe<EmploymentResponseDto>;
  /** Get paginated list of employments with optional filtering */
  employments?: Maybe<PagedResultOfEmploymentListDto>;
  /** Get a single item by display ID */
  item?: Maybe<ItemResponseDto>;
  /** Get paginated list of items with optional filtering */
  items?: Maybe<PagedResultOfItemResponseDto>;
  /** Get a single person by display ID */
  person?: Maybe<PersonResponseDto>;
  /** Get paginated list of documents for a person */
  personDocuments?: Maybe<PagedResultOfDocumentListDto>;
  /** Get paginated list of persons with optional filtering */
  persons?: Maybe<PagedResultOfPersonListDto>;
  /** Get a single position by display ID */
  position?: Maybe<PositionResponseDto>;
  /** Get paginated list of positions with optional filtering */
  positions?: Maybe<PagedResultOfPositionResponseDto>;
  /** Get a single salary grade by display ID */
  salaryGrade?: Maybe<SalaryGradeResponseDto>;
  /** Get paginated list of salary grades with optional filtering */
  salaryGrades?: Maybe<PagedResultOfSalaryGradeResponseDto>;
  /** Get a single school by display ID */
  school?: Maybe<SchoolResponseDto>;
  /** Get paginated list of schools with optional filtering */
  schools?: Maybe<PagedResultOfSchoolListDto>;
};


export type QueryDocumentArgs = {
  documentDisplayId: Scalars['Long']['input'];
  personDisplayId: Scalars['Long']['input'];
};


export type QueryEmploymentArgs = {
  displayId: Scalars['Long']['input'];
};


export type QueryEmploymentsArgs = {
  depEdIdFilter?: InputMaybe<Scalars['String']['input']>;
  displayIdFilter?: InputMaybe<Scalars['String']['input']>;
  employeeNameFilter?: InputMaybe<Scalars['String']['input']>;
  employmentStatus?: InputMaybe<Scalars['String']['input']>;
  isActive?: InputMaybe<Scalars['Boolean']['input']>;
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  positionFilter?: InputMaybe<Scalars['String']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
};


export type QueryItemArgs = {
  displayId: Scalars['Long']['input'];
};


export type QueryItemsArgs = {
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
};


export type QueryPersonArgs = {
  displayId: Scalars['Long']['input'];
};


export type QueryPersonDocumentsArgs = {
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  personDisplayId: Scalars['Long']['input'];
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
};


export type QueryPersonsArgs = {
  civilStatus?: InputMaybe<Scalars['String']['input']>;
  displayIdFilter?: InputMaybe<Scalars['String']['input']>;
  fullNameFilter?: InputMaybe<Scalars['String']['input']>;
  gender?: InputMaybe<Scalars['String']['input']>;
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
};


export type QueryPositionArgs = {
  displayId: Scalars['Long']['input'];
};


export type QueryPositionsArgs = {
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
};


export type QuerySalaryGradeArgs = {
  displayId: Scalars['Long']['input'];
};


export type QuerySalaryGradesArgs = {
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
};


export type QuerySchoolArgs = {
  displayId: Scalars['Long']['input'];
};


export type QuerySchoolsArgs = {
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
};

export type SalaryGradeResponseDto = {
  __typename?: 'SalaryGradeResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  description?: Maybe<Scalars['String']['output']>;
  displayId: Scalars['Long']['output'];
  isActive: Scalars['Boolean']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  monthlySalary: Scalars['Decimal']['output'];
  salaryGradeName?: Maybe<Scalars['String']['output']>;
  step: Scalars['Int']['output'];
};

export type SchoolListDto = {
  __typename?: 'SchoolListDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  displayId: Scalars['Long']['output'];
  isActive: Scalars['Boolean']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  schoolName?: Maybe<Scalars['String']['output']>;
};

export type SchoolResponseDto = {
  __typename?: 'SchoolResponseDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  addresses?: Maybe<Array<Maybe<AddressResponseDto>>>;
  contacts?: Maybe<Array<Maybe<ContactResponseDto>>>;
  createdBy?: Maybe<Scalars['String']['output']>;
  createdOn: Scalars['DateTime']['output'];
  displayId: Scalars['Long']['output'];
  isActive: Scalars['Boolean']['output'];
  modifiedBy?: Maybe<Scalars['String']['output']>;
  modifiedOn?: Maybe<Scalars['DateTime']['output']>;
  schoolName?: Maybe<Scalars['String']['output']>;
};

/** Input for updating an existing employment */
export type UpdateEmploymentInput = {
  appointmentStatus?: InputMaybe<AppointmentStatus>;
  dateOfOriginalAppointment?: InputMaybe<Scalars['LocalDate']['input']>;
  depEdId?: InputMaybe<Scalars['String']['input']>;
  eligibility?: InputMaybe<Eligibility>;
  employmentStatus?: InputMaybe<EmploymentStatus>;
  gsisId?: InputMaybe<Scalars['String']['input']>;
  isActive?: InputMaybe<Scalars['Boolean']['input']>;
  itemDisplayId?: InputMaybe<Scalars['Long']['input']>;
  philHealthId?: InputMaybe<Scalars['String']['input']>;
  positionDisplayId?: InputMaybe<Scalars['Long']['input']>;
  psipopItemNumber?: InputMaybe<Scalars['String']['input']>;
  salaryGradeDisplayId?: InputMaybe<Scalars['Long']['input']>;
  tinId?: InputMaybe<Scalars['String']['input']>;
};

/** Input for updating an existing item */
export type UpdateItemInput = {
  description?: InputMaybe<Scalars['String']['input']>;
  isActive?: InputMaybe<Scalars['Boolean']['input']>;
  itemName?: InputMaybe<Scalars['String']['input']>;
};

/** Input for updating an existing person */
export type UpdatePersonInput = {
  civilStatus?: InputMaybe<CivilStatus>;
  dateOfBirth?: InputMaybe<Scalars['LocalDate']['input']>;
  firstName?: InputMaybe<Scalars['String']['input']>;
  gender?: InputMaybe<Gender>;
  lastName?: InputMaybe<Scalars['String']['input']>;
  middleName?: InputMaybe<Scalars['String']['input']>;
};

/** Input for updating an existing position */
export type UpdatePositionInput = {
  description?: InputMaybe<Scalars['String']['input']>;
  isActive?: InputMaybe<Scalars['Boolean']['input']>;
  titleName?: InputMaybe<Scalars['String']['input']>;
};

/** Input for updating an existing salary grade */
export type UpdateSalaryGradeInput = {
  description?: InputMaybe<Scalars['String']['input']>;
  isActive?: InputMaybe<Scalars['Boolean']['input']>;
  monthlySalary?: InputMaybe<Scalars['Decimal']['input']>;
  salaryGradeName?: InputMaybe<Scalars['String']['input']>;
  step?: InputMaybe<Scalars['Int']['input']>;
};

/** Input for updating an existing school */
export type UpdateSchoolInput = {
  addresses?: InputMaybe<Array<UpsertAddressInput>>;
  contacts?: InputMaybe<Array<UpsertContactInput>>;
  isActive?: InputMaybe<Scalars['Boolean']['input']>;
  schoolName?: InputMaybe<Scalars['String']['input']>;
};

/** Input for upserting an address (create or update) */
export type UpsertAddressInput = {
  address1?: InputMaybe<Scalars['String']['input']>;
  address2?: InputMaybe<Scalars['String']['input']>;
  addressType?: InputMaybe<AddressType>;
  barangay?: InputMaybe<Scalars['String']['input']>;
  city?: InputMaybe<Scalars['String']['input']>;
  country?: InputMaybe<Scalars['String']['input']>;
  displayId?: InputMaybe<Scalars['Long']['input']>;
  isCurrent?: InputMaybe<Scalars['Boolean']['input']>;
  isPermanent?: InputMaybe<Scalars['Boolean']['input']>;
  province?: InputMaybe<Scalars['String']['input']>;
  zipCode?: InputMaybe<Scalars['String']['input']>;
};

/** Input for upserting a contact (create or update) */
export type UpsertContactInput = {
  contactType?: InputMaybe<ContactType>;
  displayId?: InputMaybe<Scalars['Long']['input']>;
  email?: InputMaybe<Scalars['String']['input']>;
  fax?: InputMaybe<Scalars['String']['input']>;
  landLine?: InputMaybe<Scalars['String']['input']>;
  mobile?: InputMaybe<Scalars['String']['input']>;
};

export type UserDto = {
  __typename?: 'UserDto';
  additionalProperties?: Maybe<Array<KeyValuePairOfStringAndObject>>;
  email?: Maybe<Scalars['String']['output']>;
  firstName?: Maybe<Scalars['String']['output']>;
  id: Scalars['UUID']['output'];
  lastName?: Maybe<Scalars['String']['output']>;
  profilePictureUrl?: Maybe<Scalars['String']['output']>;
  role?: Maybe<Scalars['String']['output']>;
};

export type AuthResponseFieldsFragment = { __typename?: 'AuthResponseDto', accessToken?: string | null, expiresOn: string, refreshToken?: string | null, user?: { __typename?: 'UserDto', id: string, email?: string | null, firstName?: string | null, lastName?: string | null, profilePictureUrl?: string | null, role?: string | null } | null };

export type UserFieldsFragment = { __typename?: 'UserDto', id: string, email?: string | null, firstName?: string | null, lastName?: string | null, profilePictureUrl?: string | null, role?: string | null };

export type GetCurrentUserQueryVariables = Exact<{ [key: string]: never; }>;


export type GetCurrentUserQuery = { __typename?: 'Query', currentUser?: { __typename?: 'UserDto', id: string, email?: string | null, firstName?: string | null, lastName?: string | null, profilePictureUrl?: string | null, role?: string | null } | null };

export type GoogleLoginMutationVariables = Exact<{
  idToken: Scalars['String']['input'];
}>;


export type GoogleLoginMutation = { __typename?: 'Mutation', googleLogin?: { __typename?: 'AuthResponseDto', accessToken?: string | null, expiresOn: string, refreshToken?: string | null, user?: { __typename?: 'UserDto', id: string, email?: string | null, firstName?: string | null, lastName?: string | null, profilePictureUrl?: string | null, role?: string | null } | null } | null };

export type GoogleTokenLoginMutationVariables = Exact<{
  accessToken: Scalars['String']['input'];
}>;


export type GoogleTokenLoginMutation = { __typename?: 'Mutation', googleTokenLogin?: { __typename?: 'AuthResponseDto', accessToken?: string | null, expiresOn: string, refreshToken?: string | null, user?: { __typename?: 'UserDto', id: string, email?: string | null, firstName?: string | null, lastName?: string | null, profilePictureUrl?: string | null, role?: string | null } | null } | null };

export type RefreshTokenMutationVariables = Exact<{
  refreshToken?: InputMaybe<Scalars['String']['input']>;
}>;


export type RefreshTokenMutation = { __typename?: 'Mutation', refreshToken?: { __typename?: 'AuthResponseDto', accessToken?: string | null, expiresOn: string, refreshToken?: string | null, user?: { __typename?: 'UserDto', id: string, email?: string | null, firstName?: string | null, lastName?: string | null, profilePictureUrl?: string | null, role?: string | null } | null } | null };

export type LogoutMutationVariables = Exact<{
  refreshToken?: InputMaybe<Scalars['String']['input']>;
}>;


export type LogoutMutation = { __typename?: 'Mutation', logout: boolean };

export type GetDashboardStatsQueryVariables = Exact<{ [key: string]: never; }>;


export type GetDashboardStatsQuery = { __typename?: 'Query', dashboardStats?: { __typename?: 'DashboardStatsDto', totalPersons: number, totalSchools: number, totalPositions: number, totalSalaryGrades: number, totalItems: number, activeEmployments: number } | null };

export type DocumentListFieldsFragment = { __typename?: 'DocumentListDto', displayId: number, fileName?: string | null, fileExtension?: string | null, contentType?: string | null, fileSizeBytes: number, documentType: DocumentType, description?: string | null, createdOn: string, createdBy?: string | null };

export type DocumentDetailFieldsFragment = { __typename?: 'DocumentResponseDto', displayId: number, personDisplayId: number, blobUrl?: string | null, fileName?: string | null, fileExtension?: string | null, contentType?: string | null, fileSizeBytes: number, documentType: DocumentType, description?: string | null, createdOn: string, createdBy?: string | null, modifiedOn?: string | null, modifiedBy?: string | null };

export type GetPersonDocumentsQueryVariables = Exact<{
  personDisplayId: Scalars['Long']['input'];
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
}>;


export type GetPersonDocumentsQuery = { __typename?: 'Query', personDocuments?: { __typename?: 'PagedResultOfDocumentListDto', totalCount: number, pageNumber: number, pageSize: number, totalPages: number, hasPreviousPage: boolean, hasNextPage: boolean, items?: Array<{ __typename?: 'DocumentListDto', displayId: number, fileName?: string | null, fileExtension?: string | null, contentType?: string | null, fileSizeBytes: number, documentType: DocumentType, description?: string | null, createdOn: string, createdBy?: string | null } | null> | null } | null };

export type GetDocumentQueryVariables = Exact<{
  personDisplayId: Scalars['Long']['input'];
  documentDisplayId: Scalars['Long']['input'];
}>;


export type GetDocumentQuery = { __typename?: 'Query', document?: { __typename?: 'DocumentResponseDto', displayId: number, personDisplayId: number, blobUrl?: string | null, fileName?: string | null, fileExtension?: string | null, contentType?: string | null, fileSizeBytes: number, documentType: DocumentType, description?: string | null, createdOn: string, createdBy?: string | null, modifiedOn?: string | null, modifiedBy?: string | null } | null };

export type UpdateDocumentMutationVariables = Exact<{
  personDisplayId: Scalars['Long']['input'];
  documentDisplayId: Scalars['Long']['input'];
  description?: InputMaybe<Scalars['String']['input']>;
}>;


export type UpdateDocumentMutation = { __typename?: 'Mutation', updateDocument?: { __typename?: 'DocumentResponseDto', displayId: number, personDisplayId: number, blobUrl?: string | null, fileName?: string | null, fileExtension?: string | null, contentType?: string | null, fileSizeBytes: number, documentType: DocumentType, description?: string | null, createdOn: string, createdBy?: string | null, modifiedOn?: string | null, modifiedBy?: string | null } | null };

export type DeleteDocumentMutationVariables = Exact<{
  personDisplayId: Scalars['Long']['input'];
  documentDisplayId: Scalars['Long']['input'];
}>;


export type DeleteDocumentMutation = { __typename?: 'Mutation', deleteDocument: boolean };

export type DeleteProfileImageMutationVariables = Exact<{
  personDisplayId: Scalars['Long']['input'];
}>;


export type DeleteProfileImageMutation = { __typename?: 'Mutation', deleteProfileImage: boolean };

export type EmploymentListFieldsFragment = { __typename?: 'EmploymentListDto', displayId: number, employmentStatus: EmploymentStatus, depEdId?: string | null, employeeFullName?: string | null, positionTitle?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null };

export type EmploymentDetailFieldsFragment = { __typename?: 'EmploymentResponseDto', displayId: number, employmentStatus: EmploymentStatus, appointmentStatus: AppointmentStatus, eligibility: Eligibility, dateOfOriginalAppointment?: string | null, psipopItemNumber?: string | null, depEdId?: string | null, gsisId?: string | null, philHealthId?: string | null, tinId?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, person?: { __typename?: 'EmploymentPersonDto', displayId: number, fullName?: string | null } | null, position?: { __typename?: 'EmploymentPositionDto', displayId: number, titleName?: string | null } | null, salaryGrade?: { __typename?: 'EmploymentSalaryGradeDto', displayId: number, salaryGradeName?: string | null, step: number, monthlySalary: number } | null, item?: { __typename?: 'EmploymentItemDto', displayId: number, itemName?: string | null } | null, schools?: Array<{ __typename?: 'EmploymentSchoolResponseDto', displayId: number, startDate?: string | null, endDate?: string | null, isCurrent: boolean, isActive: boolean, schoolDisplayId: number, schoolName?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null };

export type GetEmploymentsQueryVariables = Exact<{
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  displayIdFilter?: InputMaybe<Scalars['String']['input']>;
  employeeNameFilter?: InputMaybe<Scalars['String']['input']>;
  positionFilter?: InputMaybe<Scalars['String']['input']>;
  depEdIdFilter?: InputMaybe<Scalars['String']['input']>;
  employmentStatus?: InputMaybe<Scalars['String']['input']>;
  isActive?: InputMaybe<Scalars['Boolean']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
}>;


export type GetEmploymentsQuery = { __typename?: 'Query', employments?: { __typename?: 'PagedResultOfEmploymentListDto', totalCount: number, pageNumber: number, pageSize: number, totalPages: number, hasPreviousPage: boolean, hasNextPage: boolean, items?: Array<{ __typename?: 'EmploymentListDto', displayId: number, employmentStatus: EmploymentStatus, depEdId?: string | null, employeeFullName?: string | null, positionTitle?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type GetEmploymentQueryVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type GetEmploymentQuery = { __typename?: 'Query', employment?: { __typename?: 'EmploymentResponseDto', displayId: number, employmentStatus: EmploymentStatus, appointmentStatus: AppointmentStatus, eligibility: Eligibility, dateOfOriginalAppointment?: string | null, psipopItemNumber?: string | null, depEdId?: string | null, gsisId?: string | null, philHealthId?: string | null, tinId?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, person?: { __typename?: 'EmploymentPersonDto', displayId: number, fullName?: string | null } | null, position?: { __typename?: 'EmploymentPositionDto', displayId: number, titleName?: string | null } | null, salaryGrade?: { __typename?: 'EmploymentSalaryGradeDto', displayId: number, salaryGradeName?: string | null, step: number, monthlySalary: number } | null, item?: { __typename?: 'EmploymentItemDto', displayId: number, itemName?: string | null } | null, schools?: Array<{ __typename?: 'EmploymentSchoolResponseDto', displayId: number, startDate?: string | null, endDate?: string | null, isCurrent: boolean, isActive: boolean, schoolDisplayId: number, schoolName?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type CreateEmploymentMutationVariables = Exact<{
  input: CreateEmploymentInput;
}>;


export type CreateEmploymentMutation = { __typename?: 'Mutation', createEmployment?: { __typename?: 'EmploymentResponseDto', displayId: number, employmentStatus: EmploymentStatus, appointmentStatus: AppointmentStatus, eligibility: Eligibility, dateOfOriginalAppointment?: string | null, psipopItemNumber?: string | null, depEdId?: string | null, gsisId?: string | null, philHealthId?: string | null, tinId?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, person?: { __typename?: 'EmploymentPersonDto', displayId: number, fullName?: string | null } | null, position?: { __typename?: 'EmploymentPositionDto', displayId: number, titleName?: string | null } | null, salaryGrade?: { __typename?: 'EmploymentSalaryGradeDto', displayId: number, salaryGradeName?: string | null, step: number, monthlySalary: number } | null, item?: { __typename?: 'EmploymentItemDto', displayId: number, itemName?: string | null } | null, schools?: Array<{ __typename?: 'EmploymentSchoolResponseDto', displayId: number, startDate?: string | null, endDate?: string | null, isCurrent: boolean, isActive: boolean, schoolDisplayId: number, schoolName?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type UpdateEmploymentMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
  input: UpdateEmploymentInput;
}>;


export type UpdateEmploymentMutation = { __typename?: 'Mutation', updateEmployment?: { __typename?: 'EmploymentResponseDto', displayId: number, employmentStatus: EmploymentStatus, appointmentStatus: AppointmentStatus, eligibility: Eligibility, dateOfOriginalAppointment?: string | null, psipopItemNumber?: string | null, depEdId?: string | null, gsisId?: string | null, philHealthId?: string | null, tinId?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, person?: { __typename?: 'EmploymentPersonDto', displayId: number, fullName?: string | null } | null, position?: { __typename?: 'EmploymentPositionDto', displayId: number, titleName?: string | null } | null, salaryGrade?: { __typename?: 'EmploymentSalaryGradeDto', displayId: number, salaryGradeName?: string | null, step: number, monthlySalary: number } | null, item?: { __typename?: 'EmploymentItemDto', displayId: number, itemName?: string | null } | null, schools?: Array<{ __typename?: 'EmploymentSchoolResponseDto', displayId: number, startDate?: string | null, endDate?: string | null, isCurrent: boolean, isActive: boolean, schoolDisplayId: number, schoolName?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type DeleteEmploymentMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type DeleteEmploymentMutation = { __typename?: 'Mutation', deleteEmployment: boolean };

export type EmploymentSchoolFieldsFragment = { __typename?: 'EmploymentSchoolResponseDto', displayId: number, startDate?: string | null, endDate?: string | null, isCurrent: boolean, isActive: boolean, schoolDisplayId: number, schoolName?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null };

export type AddSchoolToEmploymentMutationVariables = Exact<{
  employmentDisplayId: Scalars['Long']['input'];
  input: CreateEmploymentSchoolInput;
}>;


export type AddSchoolToEmploymentMutation = { __typename?: 'Mutation', addSchoolToEmployment?: { __typename?: 'EmploymentSchoolResponseDto', displayId: number, startDate?: string | null, endDate?: string | null, isCurrent: boolean, isActive: boolean, schoolDisplayId: number, schoolName?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type RemoveSchoolFromEmploymentMutationVariables = Exact<{
  employmentDisplayId: Scalars['Long']['input'];
  schoolAssignmentDisplayId: Scalars['Long']['input'];
}>;


export type RemoveSchoolFromEmploymentMutation = { __typename?: 'Mutation', removeSchoolFromEmployment: boolean };

export type ItemFieldsFragment = { __typename?: 'ItemResponseDto', displayId: number, itemName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null };

export type GetItemsQueryVariables = Exact<{
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
}>;


export type GetItemsQuery = { __typename?: 'Query', items?: { __typename?: 'PagedResultOfItemResponseDto', totalCount: number, pageNumber: number, pageSize: number, totalPages: number, hasPreviousPage: boolean, hasNextPage: boolean, items?: Array<{ __typename?: 'ItemResponseDto', displayId: number, itemName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type GetItemQueryVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type GetItemQuery = { __typename?: 'Query', item?: { __typename?: 'ItemResponseDto', displayId: number, itemName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type CreateItemMutationVariables = Exact<{
  input: CreateItemInput;
}>;


export type CreateItemMutation = { __typename?: 'Mutation', createItem?: { __typename?: 'ItemResponseDto', displayId: number, itemName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type UpdateItemMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
  input: UpdateItemInput;
}>;


export type UpdateItemMutation = { __typename?: 'Mutation', updateItem?: { __typename?: 'ItemResponseDto', displayId: number, itemName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type DeleteItemMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type DeleteItemMutation = { __typename?: 'Mutation', deleteItem: boolean };

export type PersonListFieldsFragment = { __typename?: 'PersonListDto', displayId: number, fullName?: string | null, gender: Gender, civilStatus: CivilStatus, dateOfBirth: string, profileImageUrl?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null };

export type PersonDetailFieldsFragment = { __typename?: 'PersonResponseDto', displayId: number, firstName?: string | null, middleName?: string | null, lastName?: string | null, fullName?: string | null, gender: Gender, civilStatus: CivilStatus, dateOfBirth: string, profileImageUrl?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null } | null> | null };

export type GetPersonsQueryVariables = Exact<{
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  fullNameFilter?: InputMaybe<Scalars['String']['input']>;
  displayIdFilter?: InputMaybe<Scalars['String']['input']>;
  gender?: InputMaybe<Scalars['String']['input']>;
  civilStatus?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
}>;


export type GetPersonsQuery = { __typename?: 'Query', persons?: { __typename?: 'PagedResultOfPersonListDto', totalCount: number, pageNumber: number, pageSize: number, totalPages: number, hasPreviousPage: boolean, hasNextPage: boolean, items?: Array<{ __typename?: 'PersonListDto', displayId: number, fullName?: string | null, gender: Gender, civilStatus: CivilStatus, dateOfBirth: string, profileImageUrl?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type GetPersonQueryVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type GetPersonQuery = { __typename?: 'Query', person?: { __typename?: 'PersonResponseDto', displayId: number, firstName?: string | null, middleName?: string | null, lastName?: string | null, fullName?: string | null, gender: Gender, civilStatus: CivilStatus, dateOfBirth: string, profileImageUrl?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null } | null> | null } | null };

export type CreatePersonMutationVariables = Exact<{
  input: CreatePersonInput;
}>;


export type CreatePersonMutation = { __typename?: 'Mutation', createPerson?: { __typename?: 'PersonResponseDto', displayId: number, firstName?: string | null, middleName?: string | null, lastName?: string | null, fullName?: string | null, gender: Gender, civilStatus: CivilStatus, dateOfBirth: string, profileImageUrl?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null } | null> | null } | null };

export type UpdatePersonMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
  input: UpdatePersonInput;
}>;


export type UpdatePersonMutation = { __typename?: 'Mutation', updatePerson?: { __typename?: 'PersonResponseDto', displayId: number, firstName?: string | null, middleName?: string | null, lastName?: string | null, fullName?: string | null, gender: Gender, civilStatus: CivilStatus, dateOfBirth: string, profileImageUrl?: string | null, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null } | null> | null } | null };

export type DeletePersonMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type DeletePersonMutation = { __typename?: 'Mutation', deletePerson: boolean };

export type PositionFieldsFragment = { __typename?: 'PositionResponseDto', displayId: number, titleName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null };

export type GetPositionsQueryVariables = Exact<{
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
}>;


export type GetPositionsQuery = { __typename?: 'Query', positions?: { __typename?: 'PagedResultOfPositionResponseDto', totalCount: number, pageNumber: number, pageSize: number, totalPages: number, hasPreviousPage: boolean, hasNextPage: boolean, items?: Array<{ __typename?: 'PositionResponseDto', displayId: number, titleName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type GetPositionQueryVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type GetPositionQuery = { __typename?: 'Query', position?: { __typename?: 'PositionResponseDto', displayId: number, titleName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type CreatePositionMutationVariables = Exact<{
  input: CreatePositionInput;
}>;


export type CreatePositionMutation = { __typename?: 'Mutation', createPosition?: { __typename?: 'PositionResponseDto', displayId: number, titleName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type UpdatePositionMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
  input: UpdatePositionInput;
}>;


export type UpdatePositionMutation = { __typename?: 'Mutation', updatePosition?: { __typename?: 'PositionResponseDto', displayId: number, titleName?: string | null, description?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type DeletePositionMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type DeletePositionMutation = { __typename?: 'Mutation', deletePosition: boolean };

export type SalaryGradeFieldsFragment = { __typename?: 'SalaryGradeResponseDto', displayId: number, salaryGradeName?: string | null, description?: string | null, step: number, monthlySalary: number, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null };

export type GetSalaryGradesQueryVariables = Exact<{
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
}>;


export type GetSalaryGradesQuery = { __typename?: 'Query', salaryGrades?: { __typename?: 'PagedResultOfSalaryGradeResponseDto', totalCount: number, pageNumber: number, pageSize: number, totalPages: number, hasPreviousPage: boolean, hasNextPage: boolean, items?: Array<{ __typename?: 'SalaryGradeResponseDto', displayId: number, salaryGradeName?: string | null, description?: string | null, step: number, monthlySalary: number, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type GetSalaryGradeQueryVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type GetSalaryGradeQuery = { __typename?: 'Query', salaryGrade?: { __typename?: 'SalaryGradeResponseDto', displayId: number, salaryGradeName?: string | null, description?: string | null, step: number, monthlySalary: number, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type CreateSalaryGradeMutationVariables = Exact<{
  input: CreateSalaryGradeInput;
}>;


export type CreateSalaryGradeMutation = { __typename?: 'Mutation', createSalaryGrade?: { __typename?: 'SalaryGradeResponseDto', displayId: number, salaryGradeName?: string | null, description?: string | null, step: number, monthlySalary: number, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type UpdateSalaryGradeMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
  input: UpdateSalaryGradeInput;
}>;


export type UpdateSalaryGradeMutation = { __typename?: 'Mutation', updateSalaryGrade?: { __typename?: 'SalaryGradeResponseDto', displayId: number, salaryGradeName?: string | null, description?: string | null, step: number, monthlySalary: number, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null };

export type DeleteSalaryGradeMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type DeleteSalaryGradeMutation = { __typename?: 'Mutation', deleteSalaryGrade: boolean };

export type SchoolListFieldsFragment = { __typename?: 'SchoolListDto', displayId: number, schoolName?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null };

export type SchoolDetailFieldsFragment = { __typename?: 'SchoolResponseDto', displayId: number, schoolName?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null };

export type GetSchoolsQueryVariables = Exact<{
  pageNumber?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  searchTerm?: InputMaybe<Scalars['String']['input']>;
  sortBy?: InputMaybe<Scalars['String']['input']>;
  sortDescending?: InputMaybe<Scalars['Boolean']['input']>;
}>;


export type GetSchoolsQuery = { __typename?: 'Query', schools?: { __typename?: 'PagedResultOfSchoolListDto', totalCount: number, pageNumber: number, pageSize: number, totalPages: number, hasPreviousPage: boolean, hasNextPage: boolean, items?: Array<{ __typename?: 'SchoolListDto', displayId: number, schoolName?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type GetSchoolQueryVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type GetSchoolQuery = { __typename?: 'Query', school?: { __typename?: 'SchoolResponseDto', displayId: number, schoolName?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type CreateSchoolMutationVariables = Exact<{
  input: CreateSchoolInput;
}>;


export type CreateSchoolMutation = { __typename?: 'Mutation', createSchool?: { __typename?: 'SchoolResponseDto', displayId: number, schoolName?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type UpdateSchoolMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
  input: UpdateSchoolInput;
}>;


export type UpdateSchoolMutation = { __typename?: 'Mutation', updateSchool?: { __typename?: 'SchoolResponseDto', displayId: number, schoolName?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null, addresses?: Array<{ __typename?: 'AddressResponseDto', displayId: number, address1?: string | null, address2?: string | null, barangay?: string | null, city?: string | null, province?: string | null, country?: string | null, zipCode?: string | null, addressType: AddressType, isCurrent: boolean, isPermanent: boolean, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null, contacts?: Array<{ __typename?: 'ContactResponseDto', displayId: number, contactType: ContactType, email?: string | null, mobile?: string | null, landLine?: string | null, fax?: string | null, isActive: boolean, createdBy?: string | null, createdOn: string, modifiedBy?: string | null, modifiedOn?: string | null } | null> | null } | null };

export type DeleteSchoolMutationVariables = Exact<{
  displayId: Scalars['Long']['input'];
}>;


export type DeleteSchoolMutation = { __typename?: 'Mutation', deleteSchool: boolean };

export const AuthResponseFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"AuthResponseFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"AuthResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"accessToken"}},{"kind":"Field","name":{"kind":"Name","value":"expiresOn"}},{"kind":"Field","name":{"kind":"Name","value":"refreshToken"}},{"kind":"Field","name":{"kind":"Name","value":"user"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"profilePictureUrl"}},{"kind":"Field","name":{"kind":"Name","value":"role"}}]}}]}}]} as unknown as DocumentNode<AuthResponseFieldsFragment, unknown>;
export const UserFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"UserFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"UserDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"profilePictureUrl"}},{"kind":"Field","name":{"kind":"Name","value":"role"}}]}}]} as unknown as DocumentNode<UserFieldsFragment, unknown>;
export const DocumentListFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"DocumentListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"DocumentListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fileName"}},{"kind":"Field","name":{"kind":"Name","value":"fileExtension"}},{"kind":"Field","name":{"kind":"Name","value":"contentType"}},{"kind":"Field","name":{"kind":"Name","value":"fileSizeBytes"}},{"kind":"Field","name":{"kind":"Name","value":"documentType"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}}]}}]} as unknown as DocumentNode<DocumentListFieldsFragment, unknown>;
export const DocumentDetailFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"DocumentDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"DocumentResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"personDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"blobUrl"}},{"kind":"Field","name":{"kind":"Name","value":"fileName"}},{"kind":"Field","name":{"kind":"Name","value":"fileExtension"}},{"kind":"Field","name":{"kind":"Name","value":"contentType"}},{"kind":"Field","name":{"kind":"Name","value":"fileSizeBytes"}},{"kind":"Field","name":{"kind":"Name","value":"documentType"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}}]}}]} as unknown as DocumentNode<DocumentDetailFieldsFragment, unknown>;
export const EmploymentListFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"employmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"depEdId"}},{"kind":"Field","name":{"kind":"Name","value":"employeeFullName"}},{"kind":"Field","name":{"kind":"Name","value":"positionTitle"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<EmploymentListFieldsFragment, unknown>;
export const EmploymentDetailFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"employmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"appointmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"eligibility"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfOriginalAppointment"}},{"kind":"Field","name":{"kind":"Name","value":"psipopItemNumber"}},{"kind":"Field","name":{"kind":"Name","value":"depEdId"}},{"kind":"Field","name":{"kind":"Name","value":"gsisId"}},{"kind":"Field","name":{"kind":"Name","value":"philHealthId"}},{"kind":"Field","name":{"kind":"Name","value":"tinId"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"person"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"position"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"salaryGrade"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}}]}},{"kind":"Field","name":{"kind":"Name","value":"item"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"schools"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"startDate"}},{"kind":"Field","name":{"kind":"Name","value":"endDate"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"schoolDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<EmploymentDetailFieldsFragment, unknown>;
export const EmploymentSchoolFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentSchoolFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentSchoolResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"startDate"}},{"kind":"Field","name":{"kind":"Name","value":"endDate"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"schoolDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<EmploymentSchoolFieldsFragment, unknown>;
export const ItemFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"ItemFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"ItemResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<ItemFieldsFragment, unknown>;
export const PersonListFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PersonListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PersonListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}},{"kind":"Field","name":{"kind":"Name","value":"gender"}},{"kind":"Field","name":{"kind":"Name","value":"civilStatus"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfBirth"}},{"kind":"Field","name":{"kind":"Name","value":"profileImageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<PersonListFieldsFragment, unknown>;
export const PersonDetailFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PersonDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PersonResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"middleName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}},{"kind":"Field","name":{"kind":"Name","value":"gender"}},{"kind":"Field","name":{"kind":"Name","value":"civilStatus"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfBirth"}},{"kind":"Field","name":{"kind":"Name","value":"profileImageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}}]}}]}}]} as unknown as DocumentNode<PersonDetailFieldsFragment, unknown>;
export const PositionFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PositionFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PositionResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<PositionFieldsFragment, unknown>;
export const SalaryGradeFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SalaryGradeFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SalaryGradeResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<SalaryGradeFieldsFragment, unknown>;
export const SchoolListFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SchoolListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SchoolListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<SchoolListFieldsFragment, unknown>;
export const SchoolDetailFieldsFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SchoolDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SchoolResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<SchoolDetailFieldsFragment, unknown>;
export const GetCurrentUserDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetCurrentUser"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"currentUser"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"UserFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"UserFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"UserDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"profilePictureUrl"}},{"kind":"Field","name":{"kind":"Name","value":"role"}}]}}]} as unknown as DocumentNode<GetCurrentUserQuery, GetCurrentUserQueryVariables>;
export const GoogleLoginDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"GoogleLogin"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"idToken"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"googleLogin"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"idToken"},"value":{"kind":"Variable","name":{"kind":"Name","value":"idToken"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"AuthResponseFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"AuthResponseFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"AuthResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"accessToken"}},{"kind":"Field","name":{"kind":"Name","value":"expiresOn"}},{"kind":"Field","name":{"kind":"Name","value":"refreshToken"}},{"kind":"Field","name":{"kind":"Name","value":"user"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"profilePictureUrl"}},{"kind":"Field","name":{"kind":"Name","value":"role"}}]}}]}}]} as unknown as DocumentNode<GoogleLoginMutation, GoogleLoginMutationVariables>;
export const GoogleTokenLoginDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"GoogleTokenLogin"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"accessToken"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"googleTokenLogin"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"accessToken"},"value":{"kind":"Variable","name":{"kind":"Name","value":"accessToken"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"AuthResponseFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"AuthResponseFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"AuthResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"accessToken"}},{"kind":"Field","name":{"kind":"Name","value":"expiresOn"}},{"kind":"Field","name":{"kind":"Name","value":"refreshToken"}},{"kind":"Field","name":{"kind":"Name","value":"user"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"profilePictureUrl"}},{"kind":"Field","name":{"kind":"Name","value":"role"}}]}}]}}]} as unknown as DocumentNode<GoogleTokenLoginMutation, GoogleTokenLoginMutationVariables>;
export const RefreshTokenDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"RefreshToken"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"refreshToken"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"refreshToken"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"refreshToken"},"value":{"kind":"Variable","name":{"kind":"Name","value":"refreshToken"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"AuthResponseFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"AuthResponseFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"AuthResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"accessToken"}},{"kind":"Field","name":{"kind":"Name","value":"expiresOn"}},{"kind":"Field","name":{"kind":"Name","value":"refreshToken"}},{"kind":"Field","name":{"kind":"Name","value":"user"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"profilePictureUrl"}},{"kind":"Field","name":{"kind":"Name","value":"role"}}]}}]}}]} as unknown as DocumentNode<RefreshTokenMutation, RefreshTokenMutationVariables>;
export const LogoutDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"Logout"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"refreshToken"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"logout"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"refreshToken"},"value":{"kind":"Variable","name":{"kind":"Name","value":"refreshToken"}}}]}]}}]} as unknown as DocumentNode<LogoutMutation, LogoutMutationVariables>;
export const GetDashboardStatsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetDashboardStats"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"dashboardStats"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"totalPersons"}},{"kind":"Field","name":{"kind":"Name","value":"totalSchools"}},{"kind":"Field","name":{"kind":"Name","value":"totalPositions"}},{"kind":"Field","name":{"kind":"Name","value":"totalSalaryGrades"}},{"kind":"Field","name":{"kind":"Name","value":"totalItems"}},{"kind":"Field","name":{"kind":"Name","value":"activeEmployments"}}]}}]}}]} as unknown as DocumentNode<GetDashboardStatsQuery, GetDashboardStatsQueryVariables>;
export const GetPersonDocumentsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetPersonDocuments"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"personDocuments"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"personDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageNumber"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"Argument","name":{"kind":"Name","value":"searchTerm"},"value":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortBy"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortDescending"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"DocumentListFields"}}]}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"pageNumber"}},{"kind":"Field","name":{"kind":"Name","value":"pageSize"}},{"kind":"Field","name":{"kind":"Name","value":"totalPages"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"DocumentListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"DocumentListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fileName"}},{"kind":"Field","name":{"kind":"Name","value":"fileExtension"}},{"kind":"Field","name":{"kind":"Name","value":"contentType"}},{"kind":"Field","name":{"kind":"Name","value":"fileSizeBytes"}},{"kind":"Field","name":{"kind":"Name","value":"documentType"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}}]}}]} as unknown as DocumentNode<GetPersonDocumentsQuery, GetPersonDocumentsQueryVariables>;
export const GetDocumentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetDocument"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"documentDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"document"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"personDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"documentDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"documentDisplayId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"DocumentDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"DocumentDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"DocumentResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"personDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"blobUrl"}},{"kind":"Field","name":{"kind":"Name","value":"fileName"}},{"kind":"Field","name":{"kind":"Name","value":"fileExtension"}},{"kind":"Field","name":{"kind":"Name","value":"contentType"}},{"kind":"Field","name":{"kind":"Name","value":"fileSizeBytes"}},{"kind":"Field","name":{"kind":"Name","value":"documentType"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}}]}}]} as unknown as DocumentNode<GetDocumentQuery, GetDocumentQueryVariables>;
export const UpdateDocumentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"UpdateDocument"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"documentDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"description"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"updateDocument"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"personDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"documentDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"documentDisplayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"description"},"value":{"kind":"Variable","name":{"kind":"Name","value":"description"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"DocumentDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"DocumentDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"DocumentResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"personDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"blobUrl"}},{"kind":"Field","name":{"kind":"Name","value":"fileName"}},{"kind":"Field","name":{"kind":"Name","value":"fileExtension"}},{"kind":"Field","name":{"kind":"Name","value":"contentType"}},{"kind":"Field","name":{"kind":"Name","value":"fileSizeBytes"}},{"kind":"Field","name":{"kind":"Name","value":"documentType"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}}]}}]} as unknown as DocumentNode<UpdateDocumentMutation, UpdateDocumentMutationVariables>;
export const DeleteDocumentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeleteDocument"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"documentDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deleteDocument"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"personDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"documentDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"documentDisplayId"}}}]}]}}]} as unknown as DocumentNode<DeleteDocumentMutation, DeleteDocumentMutationVariables>;
export const DeleteProfileImageDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeleteProfileImage"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deleteProfileImage"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"personDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"personDisplayId"}}}]}]}}]} as unknown as DocumentNode<DeleteProfileImageMutation, DeleteProfileImageMutationVariables>;
export const GetEmploymentsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetEmployments"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayIdFilter"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"employeeNameFilter"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"positionFilter"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"depEdIdFilter"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"employmentStatus"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"isActive"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"employments"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"pageNumber"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"Argument","name":{"kind":"Name","value":"searchTerm"},"value":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}}},{"kind":"Argument","name":{"kind":"Name","value":"displayIdFilter"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayIdFilter"}}},{"kind":"Argument","name":{"kind":"Name","value":"employeeNameFilter"},"value":{"kind":"Variable","name":{"kind":"Name","value":"employeeNameFilter"}}},{"kind":"Argument","name":{"kind":"Name","value":"positionFilter"},"value":{"kind":"Variable","name":{"kind":"Name","value":"positionFilter"}}},{"kind":"Argument","name":{"kind":"Name","value":"depEdIdFilter"},"value":{"kind":"Variable","name":{"kind":"Name","value":"depEdIdFilter"}}},{"kind":"Argument","name":{"kind":"Name","value":"employmentStatus"},"value":{"kind":"Variable","name":{"kind":"Name","value":"employmentStatus"}}},{"kind":"Argument","name":{"kind":"Name","value":"isActive"},"value":{"kind":"Variable","name":{"kind":"Name","value":"isActive"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortBy"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortDescending"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"EmploymentListFields"}}]}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"pageNumber"}},{"kind":"Field","name":{"kind":"Name","value":"pageSize"}},{"kind":"Field","name":{"kind":"Name","value":"totalPages"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"employmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"depEdId"}},{"kind":"Field","name":{"kind":"Name","value":"employeeFullName"}},{"kind":"Field","name":{"kind":"Name","value":"positionTitle"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetEmploymentsQuery, GetEmploymentsQueryVariables>;
export const GetEmploymentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetEmployment"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"employment"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"EmploymentDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"employmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"appointmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"eligibility"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfOriginalAppointment"}},{"kind":"Field","name":{"kind":"Name","value":"psipopItemNumber"}},{"kind":"Field","name":{"kind":"Name","value":"depEdId"}},{"kind":"Field","name":{"kind":"Name","value":"gsisId"}},{"kind":"Field","name":{"kind":"Name","value":"philHealthId"}},{"kind":"Field","name":{"kind":"Name","value":"tinId"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"person"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"position"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"salaryGrade"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}}]}},{"kind":"Field","name":{"kind":"Name","value":"item"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"schools"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"startDate"}},{"kind":"Field","name":{"kind":"Name","value":"endDate"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"schoolDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<GetEmploymentQuery, GetEmploymentQueryVariables>;
export const CreateEmploymentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"CreateEmployment"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"CreateEmploymentInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"createEmployment"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"EmploymentDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"employmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"appointmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"eligibility"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfOriginalAppointment"}},{"kind":"Field","name":{"kind":"Name","value":"psipopItemNumber"}},{"kind":"Field","name":{"kind":"Name","value":"depEdId"}},{"kind":"Field","name":{"kind":"Name","value":"gsisId"}},{"kind":"Field","name":{"kind":"Name","value":"philHealthId"}},{"kind":"Field","name":{"kind":"Name","value":"tinId"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"person"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"position"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"salaryGrade"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}}]}},{"kind":"Field","name":{"kind":"Name","value":"item"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"schools"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"startDate"}},{"kind":"Field","name":{"kind":"Name","value":"endDate"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"schoolDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<CreateEmploymentMutation, CreateEmploymentMutationVariables>;
export const UpdateEmploymentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"UpdateEmployment"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"UpdateEmploymentInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"updateEmployment"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"EmploymentDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"employmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"appointmentStatus"}},{"kind":"Field","name":{"kind":"Name","value":"eligibility"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfOriginalAppointment"}},{"kind":"Field","name":{"kind":"Name","value":"psipopItemNumber"}},{"kind":"Field","name":{"kind":"Name","value":"depEdId"}},{"kind":"Field","name":{"kind":"Name","value":"gsisId"}},{"kind":"Field","name":{"kind":"Name","value":"philHealthId"}},{"kind":"Field","name":{"kind":"Name","value":"tinId"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"person"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"position"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"salaryGrade"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}}]}},{"kind":"Field","name":{"kind":"Name","value":"item"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}}]}},{"kind":"Field","name":{"kind":"Name","value":"schools"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"startDate"}},{"kind":"Field","name":{"kind":"Name","value":"endDate"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"schoolDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<UpdateEmploymentMutation, UpdateEmploymentMutationVariables>;
export const DeleteEmploymentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeleteEmployment"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deleteEmployment"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}]}]}}]} as unknown as DocumentNode<DeleteEmploymentMutation, DeleteEmploymentMutationVariables>;
export const AddSchoolToEmploymentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"AddSchoolToEmployment"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"employmentDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"CreateEmploymentSchoolInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"addSchoolToEmployment"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"employmentDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"employmentDisplayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"EmploymentSchoolFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"EmploymentSchoolFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"EmploymentSchoolResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"startDate"}},{"kind":"Field","name":{"kind":"Name","value":"endDate"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"schoolDisplayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<AddSchoolToEmploymentMutation, AddSchoolToEmploymentMutationVariables>;
export const RemoveSchoolFromEmploymentDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"RemoveSchoolFromEmployment"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"employmentDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"schoolAssignmentDisplayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"removeSchoolFromEmployment"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"employmentDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"employmentDisplayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"schoolAssignmentDisplayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"schoolAssignmentDisplayId"}}}]}]}}]} as unknown as DocumentNode<RemoveSchoolFromEmploymentMutation, RemoveSchoolFromEmploymentMutationVariables>;
export const GetItemsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetItems"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"pageNumber"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"Argument","name":{"kind":"Name","value":"searchTerm"},"value":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortBy"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortDescending"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"ItemFields"}}]}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"pageNumber"}},{"kind":"Field","name":{"kind":"Name","value":"pageSize"}},{"kind":"Field","name":{"kind":"Name","value":"totalPages"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"ItemFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"ItemResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetItemsQuery, GetItemsQueryVariables>;
export const GetItemDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetItem"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"item"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"ItemFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"ItemFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"ItemResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetItemQuery, GetItemQueryVariables>;
export const CreateItemDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"CreateItem"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"CreateItemInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"createItem"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"ItemFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"ItemFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"ItemResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<CreateItemMutation, CreateItemMutationVariables>;
export const UpdateItemDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"UpdateItem"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"UpdateItemInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"updateItem"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"ItemFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"ItemFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"ItemResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"itemName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<UpdateItemMutation, UpdateItemMutationVariables>;
export const DeleteItemDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeleteItem"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deleteItem"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}]}]}}]} as unknown as DocumentNode<DeleteItemMutation, DeleteItemMutationVariables>;
export const GetPersonsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetPersons"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"fullNameFilter"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayIdFilter"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"gender"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"civilStatus"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"persons"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"pageNumber"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"Argument","name":{"kind":"Name","value":"searchTerm"},"value":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}}},{"kind":"Argument","name":{"kind":"Name","value":"fullNameFilter"},"value":{"kind":"Variable","name":{"kind":"Name","value":"fullNameFilter"}}},{"kind":"Argument","name":{"kind":"Name","value":"displayIdFilter"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayIdFilter"}}},{"kind":"Argument","name":{"kind":"Name","value":"gender"},"value":{"kind":"Variable","name":{"kind":"Name","value":"gender"}}},{"kind":"Argument","name":{"kind":"Name","value":"civilStatus"},"value":{"kind":"Variable","name":{"kind":"Name","value":"civilStatus"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortBy"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortDescending"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PersonListFields"}}]}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"pageNumber"}},{"kind":"Field","name":{"kind":"Name","value":"pageSize"}},{"kind":"Field","name":{"kind":"Name","value":"totalPages"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PersonListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PersonListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}},{"kind":"Field","name":{"kind":"Name","value":"gender"}},{"kind":"Field","name":{"kind":"Name","value":"civilStatus"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfBirth"}},{"kind":"Field","name":{"kind":"Name","value":"profileImageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetPersonsQuery, GetPersonsQueryVariables>;
export const GetPersonDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetPerson"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"person"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PersonDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PersonDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PersonResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"middleName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}},{"kind":"Field","name":{"kind":"Name","value":"gender"}},{"kind":"Field","name":{"kind":"Name","value":"civilStatus"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfBirth"}},{"kind":"Field","name":{"kind":"Name","value":"profileImageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}}]}}]}}]} as unknown as DocumentNode<GetPersonQuery, GetPersonQueryVariables>;
export const CreatePersonDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"CreatePerson"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"CreatePersonInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"createPerson"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PersonDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PersonDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PersonResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"middleName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}},{"kind":"Field","name":{"kind":"Name","value":"gender"}},{"kind":"Field","name":{"kind":"Name","value":"civilStatus"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfBirth"}},{"kind":"Field","name":{"kind":"Name","value":"profileImageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}}]}}]}}]} as unknown as DocumentNode<CreatePersonMutation, CreatePersonMutationVariables>;
export const UpdatePersonDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"UpdatePerson"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"UpdatePersonInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"updatePerson"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PersonDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PersonDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PersonResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"firstName"}},{"kind":"Field","name":{"kind":"Name","value":"middleName"}},{"kind":"Field","name":{"kind":"Name","value":"lastName"}},{"kind":"Field","name":{"kind":"Name","value":"fullName"}},{"kind":"Field","name":{"kind":"Name","value":"gender"}},{"kind":"Field","name":{"kind":"Name","value":"civilStatus"}},{"kind":"Field","name":{"kind":"Name","value":"dateOfBirth"}},{"kind":"Field","name":{"kind":"Name","value":"profileImageUrl"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}}]}}]}}]} as unknown as DocumentNode<UpdatePersonMutation, UpdatePersonMutationVariables>;
export const DeletePersonDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeletePerson"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deletePerson"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}]}]}}]} as unknown as DocumentNode<DeletePersonMutation, DeletePersonMutationVariables>;
export const GetPositionsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetPositions"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"positions"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"pageNumber"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"Argument","name":{"kind":"Name","value":"searchTerm"},"value":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortBy"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortDescending"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PositionFields"}}]}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"pageNumber"}},{"kind":"Field","name":{"kind":"Name","value":"pageSize"}},{"kind":"Field","name":{"kind":"Name","value":"totalPages"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PositionFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PositionResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetPositionsQuery, GetPositionsQueryVariables>;
export const GetPositionDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetPosition"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"position"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PositionFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PositionFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PositionResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetPositionQuery, GetPositionQueryVariables>;
export const CreatePositionDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"CreatePosition"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"CreatePositionInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"createPosition"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PositionFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PositionFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PositionResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<CreatePositionMutation, CreatePositionMutationVariables>;
export const UpdatePositionDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"UpdatePosition"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"UpdatePositionInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"updatePosition"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"PositionFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"PositionFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"PositionResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"titleName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<UpdatePositionMutation, UpdatePositionMutationVariables>;
export const DeletePositionDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeletePosition"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deletePosition"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}]}]}}]} as unknown as DocumentNode<DeletePositionMutation, DeletePositionMutationVariables>;
export const GetSalaryGradesDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetSalaryGrades"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"salaryGrades"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"pageNumber"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"Argument","name":{"kind":"Name","value":"searchTerm"},"value":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortBy"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortDescending"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SalaryGradeFields"}}]}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"pageNumber"}},{"kind":"Field","name":{"kind":"Name","value":"pageSize"}},{"kind":"Field","name":{"kind":"Name","value":"totalPages"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SalaryGradeFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SalaryGradeResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetSalaryGradesQuery, GetSalaryGradesQueryVariables>;
export const GetSalaryGradeDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetSalaryGrade"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"salaryGrade"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SalaryGradeFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SalaryGradeFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SalaryGradeResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetSalaryGradeQuery, GetSalaryGradeQueryVariables>;
export const CreateSalaryGradeDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"CreateSalaryGrade"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"CreateSalaryGradeInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"createSalaryGrade"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SalaryGradeFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SalaryGradeFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SalaryGradeResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<CreateSalaryGradeMutation, CreateSalaryGradeMutationVariables>;
export const UpdateSalaryGradeDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"UpdateSalaryGrade"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"UpdateSalaryGradeInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"updateSalaryGrade"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SalaryGradeFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SalaryGradeFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SalaryGradeResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"salaryGradeName"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"step"}},{"kind":"Field","name":{"kind":"Name","value":"monthlySalary"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<UpdateSalaryGradeMutation, UpdateSalaryGradeMutationVariables>;
export const DeleteSalaryGradeDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeleteSalaryGrade"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deleteSalaryGrade"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}]}]}}]} as unknown as DocumentNode<DeleteSalaryGradeMutation, DeleteSalaryGradeMutationVariables>;
export const GetSchoolsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetSchools"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Boolean"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"schools"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"pageNumber"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageNumber"}}},{"kind":"Argument","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"Argument","name":{"kind":"Name","value":"searchTerm"},"value":{"kind":"Variable","name":{"kind":"Name","value":"searchTerm"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortBy"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortBy"}}},{"kind":"Argument","name":{"kind":"Name","value":"sortDescending"},"value":{"kind":"Variable","name":{"kind":"Name","value":"sortDescending"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SchoolListFields"}}]}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"pageNumber"}},{"kind":"Field","name":{"kind":"Name","value":"pageSize"}},{"kind":"Field","name":{"kind":"Name","value":"totalPages"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SchoolListFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SchoolListDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]} as unknown as DocumentNode<GetSchoolsQuery, GetSchoolsQueryVariables>;
export const GetSchoolDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetSchool"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"school"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SchoolDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SchoolDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SchoolResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<GetSchoolQuery, GetSchoolQueryVariables>;
export const CreateSchoolDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"CreateSchool"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"CreateSchoolInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"createSchool"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SchoolDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SchoolDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SchoolResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<CreateSchoolMutation, CreateSchoolMutationVariables>;
export const UpdateSchoolDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"UpdateSchool"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"input"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"UpdateSchoolInput"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"updateSchool"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}},{"kind":"Argument","name":{"kind":"Name","value":"input"},"value":{"kind":"Variable","name":{"kind":"Name","value":"input"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"FragmentSpread","name":{"kind":"Name","value":"SchoolDetailFields"}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"SchoolDetailFields"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"SchoolResponseDto"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"schoolName"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}},{"kind":"Field","name":{"kind":"Name","value":"addresses"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"address1"}},{"kind":"Field","name":{"kind":"Name","value":"address2"}},{"kind":"Field","name":{"kind":"Name","value":"barangay"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"province"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"zipCode"}},{"kind":"Field","name":{"kind":"Name","value":"addressType"}},{"kind":"Field","name":{"kind":"Name","value":"isCurrent"}},{"kind":"Field","name":{"kind":"Name","value":"isPermanent"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}},{"kind":"Field","name":{"kind":"Name","value":"contacts"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"displayId"}},{"kind":"Field","name":{"kind":"Name","value":"contactType"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"mobile"}},{"kind":"Field","name":{"kind":"Name","value":"landLine"}},{"kind":"Field","name":{"kind":"Name","value":"fax"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"createdBy"}},{"kind":"Field","name":{"kind":"Name","value":"createdOn"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedBy"}},{"kind":"Field","name":{"kind":"Name","value":"modifiedOn"}}]}}]}}]} as unknown as DocumentNode<UpdateSchoolMutation, UpdateSchoolMutationVariables>;
export const DeleteSchoolDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"DeleteSchool"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Long"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"deleteSchool"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"displayId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"displayId"}}}]}]}}]} as unknown as DocumentNode<DeleteSchoolMutation, DeleteSchoolMutationVariables>;