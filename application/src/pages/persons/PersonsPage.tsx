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
import {
  personsApi,
  type PersonListDto,
  API_BASE_URL,
  ApiV1PersonsGetGenderEnum,
  ApiV1PersonsGetCivilStatusEnum,
} from '../../api';
import { useAgGridTheme } from '../../components/ui/use-ag-grid-theme';

ModuleRegistry.registerModules([AllCommunityModule]);

// Icon components for action buttons
const EyeIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
    <circle cx="12" cy="12" r="3" />
  </svg>
);

const EditIcon = () => (
  <svg
    width="16"
    height="16"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
    <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
  </svg>
);

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
        {props.values.map(value => (
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
    gender?: ApiV1PersonsGetGenderEnum;
    civilStatus?: ApiV1PersonsGetCivilStatusEnum;
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

  // Extract gender filter (set filter or text filter)
  if (filterModel.gender?.values && filterModel.gender.values.length > 0) {
    // For set filter, take the first selected value
    filters.gender = filterModel.gender.values[0] as ApiV1PersonsGetGenderEnum;
  } else if (filterModel.gender?.filter) {
    filters.gender = filterModel.gender.filter as ApiV1PersonsGetGenderEnum;
  }

  // Extract civilStatus filter (set filter or text filter)
  if (
    filterModel.civilStatus?.values &&
    filterModel.civilStatus.values.length > 0
  ) {
    // For set filter, take the first selected value
    filters.civilStatus = filterModel.civilStatus
      .values[0] as ApiV1PersonsGetCivilStatusEnum;
  } else if (filterModel.civilStatus?.filter) {
    filters.civilStatus = filterModel.civilStatus
      .filter as ApiV1PersonsGetCivilStatusEnum;
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
            const response = await personsApi.apiV1PersonsGet(
              filters.gender,
              filters.civilStatus,
              filters.displayIdFilter,
              filters.fullNameFilter,
              pageNumber,
              pageSize,
              debouncedSearchTerm || undefined,
              sortBy,
              sortDescending
            );

            const data = response.data;
            const rowsThisPage = data.items || [];
            const lastRow = data.totalCount ?? -1;

            rowParams.successCallback(rowsThisPage, lastRow);
          } catch (error) {
            console.error('Error fetching persons:', error);
            rowParams.failCallback();
          }
        },
      });
    }
  }, [debouncedSearchTerm]);

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
          const displayId = params.data?.displayId;
          const nameParts = fullName.split(' ');
          const initials =
            nameParts.length >= 2
              ? `${nameParts[0]?.[0] || ''}${nameParts[nameParts.length - 1]?.[0] || ''}`.toUpperCase()
              : (fullName[0] || '?').toUpperCase();

          if (params.value && displayId) {
            // Use the streaming endpoint instead of the blob URL
            const imageUrl = `${API_BASE_URL}/api/v1/persons/${displayId}/documents/profile-image`;
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
                  onError={e => {
                    // Hide the image on error, showing fallback initials
                    (e.target as HTMLImageElement).style.display = 'none';
                  }}
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
        valueFormatter: params => {
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
            'SoloParent',
            'Widow',
            'Separated',
            'Other',
          ],
        },
        filterParams: {
          filterOptions: ['equals'],
          maxNumConditions: 1,
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
            const response = await personsApi.apiV1PersonsGet(
              filters.gender,
              filters.civilStatus,
              filters.displayIdFilter,
              filters.fullNameFilter,
              pageNumber,
              pageSize,
              debouncedSearchTerm || undefined,
              sortBy,
              sortDescending
            );

            const data = response.data;
            const rowsThisPage = data.items || [];
            const lastRow = data.totalCount ?? -1;

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
    [debouncedSearchTerm]
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
          onChange={e => setSearchTerm(e.target.value)}
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
