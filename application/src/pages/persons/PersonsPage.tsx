import { useState, useCallback, useRef, useEffect, useMemo } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Input,
  Spinner,
  Center,
  Image,
  IconButton,
  Tooltip,
  Portal,
} from '@chakra-ui/react';
import { AgGridReact } from 'ag-grid-react';
import { useDebounce } from '../../hooks';
import {
  AllCommunityModule,
  ModuleRegistry,
  type ColDef,
  type GridReadyEvent,
  type IGetRowsParams,
  type ICellRendererParams,
  type IFloatingFilterParams,
} from 'ag-grid-community';
import { useNavigate } from 'react-router-dom';
import { usePersonsLazy } from '../../hooks/usePersons';
import type {
  PersonListDto,
  Gender,
  CivilStatus,
} from '../../graphql/generated/graphql';

// Display mapping for Gender using GraphQL generated enum
const GenderDisplay: Record<Gender, string> = {
  Male: 'Male',
  Female: 'Female',
};

// Display mapping for CivilStatus using GraphQL generated enum
const CivilStatusDisplay: Record<CivilStatus, string> = {
  Single: 'Single',
  Married: 'Married',
  SoloParent: 'Solo Parent',
  Widow: 'Widow',
  Separated: 'Separated',
  Other: 'Other',
};
import { useAgGridTheme } from '../../components/ui/use-ag-grid-theme';
import { EyeIcon, EditIcon } from '../../components/icons';

ModuleRegistry.registerModules([AllCommunityModule]);

// Profile image cell renderer using the URL from the list data directly
interface ProfileImageCellProps {
  fullName: string;
  imageUrl?: string | null;
  hasProfileImage?: boolean;
}

const ProfileImageCell = ({
  fullName,
  imageUrl,
  hasProfileImage,
}: ProfileImageCellProps) => {
  // Track which URL failed - error state automatically resets when URL changes
  const [failedImageUrl, setFailedImageUrl] = useState<string | null>(null);
  const imgError = failedImageUrl === imageUrl;

  const nameParts = fullName.split(' ');
  const initials =
    nameParts.length >= 2
      ? `${nameParts[0]?.[0] || ''}${nameParts[nameParts.length - 1]?.[0] || ''}`.toUpperCase()
      : (fullName[0] || '?').toUpperCase();

  // Only attempt to load image if hasProfileImage flag is true
  if (hasProfileImage && imageUrl && !imgError) {
    return (
      <Flex align="center" justify="center" h="100%">
        <Image
          src={imageUrl}
          alt="Profile"
          w="32px"
          h="32px"
          minW="32px"
          minH="32px"
          borderRadius="50%"
          objectFit="cover"
          onError={() => setFailedImageUrl(imageUrl)}
        />
      </Flex>
    );
  }

  return (
    <Flex align="center" justify="center" h="100%">
      <Center
        w="32px"
        h="32px"
        minW="32px"
        minH="32px"
        borderRadius="50%"
        bg="bg.muted"
        color="fg.muted"
        fontSize="xs"
        fontWeight="bold"
      >
        {initials}
      </Center>
    </Flex>
  );
};

// Custom floating filter component for dropdown selection
interface SelectFloatingFilterProps extends IFloatingFilterParams {
  values: string[];
}

const SelectFloatingFilter = (props: SelectFloatingFilterProps) => {
  const [currentValue, setCurrentValue] = useState<string>('');

  const onSelectChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const value = event.target.value;
    setCurrentValue(value);

    const colId = props.column.getColId();

    if (value === '') {
      props.api.setColumnFilterModel(colId, null).then(() => {
        props.api.onFilterChanged();
      });
    } else {
      props.api
        .setColumnFilterModel(colId, {
          filterType: 'text',
          type: 'equals',
          filter: value,
        })
        .then(() => {
          props.api.onFilterChanged();
        });
    }
  };

  return (
    <div
      style={{
        width: '100%',
        height: '100%',
        display: 'flex',
        alignItems: 'center',
      }}
    >
      <select
        value={currentValue}
        onChange={onSelectChange}
        style={{
          width: '100%',
          height: '32px',
          border: '1px solid var(--ag-input-border-color)',
          borderRadius: 'var(--ag-input-border-radius, 6px)',
          backgroundColor: 'var(--ag-background-color)',
          color: 'var(--ag-foreground-color)',
          fontSize: 'var(--ag-font-size)',
          outline: 'none',
          boxSizing: 'border-box',
        }}
      >
        <option value="">All</option>
        {props.values.map((value) => (
          <option key={value} value={value}>
            {value}
          </option>
        ))}
      </select>
    </div>
  );
};

// Helper type for AG Grid filter model
interface FilterModel {
  [key: string]: {
    filterType: string;
    type?: string;
    filter?: string | number;
    values?: string[];
  };
}

// Helper function to extract filter values from AG Grid filter model
const extractFilters = (filterModel: FilterModel) => {
  const filters: {
    gender?: Gender;
    civilStatus?: CivilStatus;
    displayIdFilter?: string;
    fullNameFilter?: string;
  } = {};

  // Extract displayId filter (text filter)
  if (filterModel.displayId?.filter) {
    filters.displayIdFilter = String(filterModel.displayId.filter);
  }

  // Extract fullName filter (text filter)
  if (filterModel.fullName?.filter) {
    filters.fullNameFilter = String(filterModel.fullName.filter);
  }

  // Extract gender filter - convert display value to GraphQL enum
  if (filterModel.gender?.values && filterModel.gender.values.length > 0) {
    const displayValue = filterModel.gender.values[0];
    // Find the enum key by matching the display value
    const enumKey = Object.entries(GenderDisplay).find(
      ([, display]) => display === displayValue
    )?.[0] as Gender | undefined;
    if (enumKey) {
      filters.gender = enumKey;
    }
  } else if (filterModel.gender?.filter) {
    const displayValue = String(filterModel.gender.filter);
    // Find the enum key by matching the display value
    const enumKey = Object.entries(GenderDisplay).find(
      ([, display]) => display === displayValue
    )?.[0] as Gender | undefined;
    if (enumKey) {
      filters.gender = enumKey;
    }
  }

  // Extract civilStatus filter - convert display value to GraphQL enum
  if (
    filterModel.civilStatus?.values &&
    filterModel.civilStatus.values.length > 0
  ) {
    const displayValue = filterModel.civilStatus.values[0];
    // Find the enum key by matching the display value
    const enumKey = Object.entries(CivilStatusDisplay).find(
      ([, display]) => display === displayValue
    )?.[0] as CivilStatus | undefined;
    if (enumKey) {
      filters.civilStatus = enumKey;
    }
  } else if (filterModel.civilStatus?.filter) {
    const displayValue = String(filterModel.civilStatus.filter);
    // Find the enum key by matching the display value
    const enumKey = Object.entries(CivilStatusDisplay).find(
      ([, display]) => display === displayValue
    )?.[0] as CivilStatus | undefined;
    if (enumKey) {
      filters.civilStatus = enumKey;
    }
  }

  return filters;
};

const PersonsPage = () => {
  const navigate = useNavigate();
  const gridRef = useRef<AgGridReact<PersonListDto>>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const debouncedSearchTerm = useDebounce(searchTerm, 300);
  const theme = useAgGridTheme();
  const { fetchPersons } = usePersonsLazy();

  useEffect(() => {
    if (gridRef.current?.api) {
      gridRef.current.api.setGridOption('datasource', {
        rowCount: undefined,
        getRows: async (rowParams: IGetRowsParams) => {
          const pageSize = 100;
          const pageNumber = Math.floor(rowParams.startRow / pageSize) + 1;

          const sortModel = rowParams.sortModel[0];
          const sortBy = sortModel?.colId;
          const sortDescending = sortModel?.sort === 'desc';

          // Extract filter values from the filter model
          const filters = extractFilters(rowParams.filterModel as FilterModel);

          try {
            const result = await fetchPersons({
              variables: {
                gender: filters.gender,
                civilStatus: filters.civilStatus,
                displayIdFilter: filters.displayIdFilter,
                fullNameFilter: filters.fullNameFilter,
                pageNumber,
                pageSize,
                searchTerm: debouncedSearchTerm || undefined,
                sortBy,
                sortDescending,
              },
            });

            const data = result.data?.persons;
            const rowsThisPage = data?.items || [];
            const lastRow = data?.totalCount ?? -1;

            rowParams.successCallback(rowsThisPage, lastRow);
          } catch (error) {
            console.error('Error fetching persons:', error);
            rowParams.failCallback();
          }
        },
      });
    }
  }, [debouncedSearchTerm, fetchPersons]);

  const columnDefs: ColDef<PersonListDto>[] = useMemo(
    () => [
      {
        headerName: 'Actions',
        width: 100,
        sortable: false,
        filter: false,
        cellRenderer: (params: ICellRendererParams<PersonListDto>) => {
          return (
            <Flex
              gap={1}
              align="center"
              h="100%"
              css={{
                '& button': {
                  padding: '5px !important',
                  minWidth: 'auto !important',
                  minHeight: 'auto !important',
                },
              }}
            >
              <Tooltip.Root
                positioning={{ placement: 'top' }}
                openDelay={300}
                closeDelay={0}
              >
                <Tooltip.Trigger asChild>
                  <IconButton
                    aria-label="View details"
                    size="xs"
                    variant="ghost"
                    onClick={() =>
                      navigate(`/persons/${params.data?.displayId}`)
                    }
                  >
                    <EyeIcon />
                  </IconButton>
                </Tooltip.Trigger>
                <Portal>
                  <Tooltip.Positioner>
                    <Tooltip.Content px={1} py={1}>
                      View details
                    </Tooltip.Content>
                  </Tooltip.Positioner>
                </Portal>
              </Tooltip.Root>
              <Tooltip.Root
                positioning={{ placement: 'top' }}
                openDelay={300}
                closeDelay={0}
              >
                <Tooltip.Trigger asChild>
                  <IconButton
                    aria-label="Edit person"
                    size="xs"
                    variant="ghost"
                    onClick={() =>
                      navigate(`/persons/${params.data?.displayId}/edit`)
                    }
                  >
                    <EditIcon />
                  </IconButton>
                </Tooltip.Trigger>
                <Portal>
                  <Tooltip.Positioner>
                    <Tooltip.Content px={1} py={1}>
                      Edit person
                    </Tooltip.Content>
                  </Tooltip.Positioner>
                </Portal>
              </Tooltip.Root>
            </Flex>
          );
        },
      },
      {
        field: 'displayId',
        headerName: 'ID',
        width: 150,
        sortable: true,
        filter: 'agTextColumnFilter',
        filterParams: {
          filterOptions: ['contains'],
          maxNumConditions: 1,
        },
      },
      {
        field: 'profileImageUrl',
        headerName: '',
        width: 60,
        sortable: false,
        filter: false,
        cellRenderer: (params: ICellRendererParams<PersonListDto>) => {
          const fullName = params.data?.fullName || '';
          if (!params.data) return null;

          return (
            <ProfileImageCell
              fullName={fullName}
              imageUrl={params.value}
              hasProfileImage={params.data.hasProfileImage}
            />
          );
        },
      },
      {
        field: 'fullName',
        headerName: 'Full Name',
        flex: 1,
        minWidth: 200,
        sortable: true,
        filter: 'agTextColumnFilter',
        filterParams: {
          filterOptions: ['contains'],
          maxNumConditions: 1,
        },
      },
      {
        field: 'dateOfBirth',
        headerName: 'Date of Birth',
        width: 150,
        sortable: true,
        filter: false,
        valueFormatter: (params) => {
          if (!params.value) return '';
          return new Date(params.value).toLocaleDateString();
        },
      },
      {
        field: 'gender',
        headerName: 'Gender',
        width: 120,
        sortable: true,
        filter: 'agTextColumnFilter',
        suppressFloatingFilterButton: true,
        suppressHeaderFilterButton: true,
        floatingFilterComponent: SelectFloatingFilter,
        floatingFilterComponentParams: {
          values: ['Male', 'Female'],
        },
        filterParams: {
          filterOptions: ['equals'],
          maxNumConditions: 1,
        },
        valueFormatter: (params) => {
          if (!params.value) return '';
          return GenderDisplay[params.value as Gender] || params.value;
        },
      },
      {
        field: 'civilStatus',
        headerName: 'Civil Status',
        width: 160,
        sortable: true,
        filter: 'agTextColumnFilter',
        suppressFloatingFilterButton: true,
        suppressHeaderFilterButton: true,
        floatingFilterComponent: SelectFloatingFilter,
        floatingFilterComponentParams: {
          values: [
            'Single',
            'Married',
            'Solo Parent',
            'Widow',
            'Separated',
            'Other',
          ],
        },
        filterParams: {
          filterOptions: ['equals'],
          maxNumConditions: 1,
        },
        valueFormatter: (params) => {
          if (!params.value) return '';
          return (
            CivilStatusDisplay[params.value as CivilStatus] || params.value
          );
        },
      },
    ],
    [navigate]
  );

  const defaultColDef: ColDef = useMemo(
    () => ({
      filter: false,
      floatingFilter: true,
      resizable: true,
    }),
    []
  );

  const onGridReady = useCallback(
    (params: GridReadyEvent) => {
      const dataSource = {
        rowCount: undefined,
        getRows: async (rowParams: IGetRowsParams) => {
          const pageSize = 100;
          const pageNumber = Math.floor(rowParams.startRow / pageSize) + 1;

          const sortModel = rowParams.sortModel[0];
          const sortBy = sortModel?.colId;
          const sortDescending = sortModel?.sort === 'desc';

          // Extract filter values from the filter model
          const filters = extractFilters(rowParams.filterModel as FilterModel);

          try {
            const result = await fetchPersons({
              variables: {
                gender: filters.gender,
                civilStatus: filters.civilStatus,
                displayIdFilter: filters.displayIdFilter,
                fullNameFilter: filters.fullNameFilter,
                pageNumber,
                pageSize,
                searchTerm: debouncedSearchTerm || undefined,
                sortBy,
                sortDescending,
              },
            });

            const data = result.data?.persons;
            const rowsThisPage = data?.items || [];
            const lastRow = data?.totalCount ?? -1;

            rowParams.successCallback(rowsThisPage, lastRow);
            setIsLoading(false);
          } catch (error) {
            console.error('Error fetching persons:', error);
            rowParams.failCallback();
            setIsLoading(false);
          }
        },
      };

      params.api.setGridOption('datasource', dataSource);
    },
    [debouncedSearchTerm, fetchPersons]
  );

  return (
    <Box h="100%">
      <Flex justify="space-between" align="center" mb={4}>
        <Heading size="lg">Persons</Heading>
        <Button colorPalette="blue" onClick={() => navigate('/persons/new')}>
          Add Person
        </Button>
      </Flex>

      <Flex gap={4} mb={4}>
        <Input
          placeholder="Search by name..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          maxW="300px"
        />
      </Flex>

      <Box h="calc(100% - 140px)" position="relative">
        {isLoading && (
          <Center
            position="absolute"
            top={0}
            left={0}
            right={0}
            bottom={0}
            bg="bg"
            zIndex={1}
          >
            <Spinner size="lg" />
          </Center>
        )}
        <Box opacity={isLoading ? 0 : 1} h="100%">
          <AgGridReact<PersonListDto>
            ref={gridRef}
            theme={theme}
            columnDefs={columnDefs}
            defaultColDef={defaultColDef}
            rowModelType="infinite"
            cacheBlockSize={100}
            cacheOverflowSize={2}
            maxConcurrentDatasourceRequests={1}
            infiniteInitialRowCount={20}
            maxBlocksInCache={10}
            onGridReady={onGridReady}
            pagination={true}
            paginationPageSize={20}
            paginationPageSizeSelector={[10, 20, 50, 100]}
            animateRows={false}
            suppressCellFocus={true}
            alwaysShowVerticalScroll={true}
          />
        </Box>
      </Box>
    </Box>
  );
};

export default PersonsPage;
