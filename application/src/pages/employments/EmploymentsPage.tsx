import { useState, useCallback, useRef, useEffect, useMemo } from 'react';
import {
  Box,
  Heading,
  Button,
  Flex,
  Input,
  Badge,
  Spinner,
  Center,
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
  employmentsApi,
  type EmploymentListDto,
  ApiV1EmploymentsGetEmploymentStatusEnum,
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
    employmentStatus?: ApiV1EmploymentsGetEmploymentStatusEnum;
    isActive?: boolean;
    displayIdFilter?: string;
    employeeNameFilter?: string;
    depEdIdFilter?: string;
    positionFilter?: string;
  } = {};

  // Extract displayId filter (text filter)
  if (filterModel.displayId?.filter) {
    filters.displayIdFilter = String(filterModel.displayId.filter);
  }

  // Extract employeeFullName filter (text filter)
  if (filterModel.employeeFullName?.filter) {
    filters.employeeNameFilter = String(filterModel.employeeFullName.filter);
  }

  // Extract depEdId filter (text filter)
  if (filterModel.depEdId?.filter) {
    filters.depEdIdFilter = String(filterModel.depEdId.filter);
  }

  // Extract positionTitle filter (text filter)
  if (filterModel.positionTitle?.filter) {
    filters.positionFilter = String(filterModel.positionTitle.filter);
  }

  // Extract employmentStatus filter (set filter or text filter)
  if (
    filterModel.employmentStatus?.values &&
    filterModel.employmentStatus.values.length > 0
  ) {
    filters.employmentStatus = filterModel.employmentStatus
      .values[0] as ApiV1EmploymentsGetEmploymentStatusEnum;
  } else if (filterModel.employmentStatus?.filter) {
    filters.employmentStatus = String(
      filterModel.employmentStatus.filter
    ) as ApiV1EmploymentsGetEmploymentStatusEnum;
  }

  // Extract isActive filter (set filter or text filter)
  if (filterModel.isActive?.values && filterModel.isActive.values.length > 0) {
    filters.isActive = filterModel.isActive.values[0] === 'Yes';
  } else if (filterModel.isActive?.filter) {
    filters.isActive = filterModel.isActive.filter === 'Yes';
  }

  return filters;
};

const EmploymentsPage = () => {
  const navigate = useNavigate();
  const gridRef = useRef<AgGridReact<EmploymentListDto>>(null);
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
            const response = await employmentsApi.apiV1EmploymentsGet(
              filters.employmentStatus,
              filters.isActive,
              filters.displayIdFilter,
              filters.employeeNameFilter,
              filters.depEdIdFilter,
              filters.positionFilter,
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
            console.error('Error fetching employments:', error);
            rowParams.failCallback();
          }
        },
      });
    }
  }, [debouncedSearchTerm]);

  const columnDefs: ColDef<EmploymentListDto>[] = useMemo(
    () => [
      {
        headerName: 'Actions',
        width: 100,
        sortable: false,
        filter: false,
        cellRenderer: (params: ICellRendererParams<EmploymentListDto>) => {
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
                      navigate(`/employments/${params.data?.displayId}`)
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
                    aria-label="Edit employment"
                    size="xs"
                    variant="ghost"
                    onClick={() =>
                      navigate(`/employments/${params.data?.displayId}/edit`)
                    }
                  >
                    <EditIcon />
                  </IconButton>
                </Tooltip.Trigger>
                <Portal>
                  <Tooltip.Positioner>
                    <Tooltip.Content px={1} py={1}>
                      Edit employment
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
        field: 'employeeFullName',
        headerName: 'Employee Name',
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
        field: 'depEdId',
        headerName: 'DepEd ID',
        width: 180,
        sortable: true,
        filter: 'agTextColumnFilter',
        filterParams: {
          filterOptions: ['contains'],
          maxNumConditions: 1,
        },
      },
      {
        field: 'positionTitle',
        headerName: 'Position',
        flex: 1,
        minWidth: 150,
        sortable: true,
        filter: 'agTextColumnFilter',
        filterParams: {
          filterOptions: ['contains'],
          maxNumConditions: 1,
        },
      },
      {
        field: 'employmentStatus',
        headerName: 'Status',
        width: 130,
        sortable: true,
        filter: 'agTextColumnFilter',
        suppressFloatingFilterButton: true,
        suppressHeaderFilterButton: true,
        floatingFilterComponent: SelectFloatingFilter,
        floatingFilterComponentParams: {
          values: ['Regular', 'Permanent'],
        },
        filterParams: {
          filterOptions: ['equals'],
          maxNumConditions: 1,
        },
        cellRenderer: (params: ICellRendererParams<EmploymentListDto>) => {
          const value = params.value;
          return (
            <Badge colorPalette={value === 'Permanent' ? 'green' : 'blue'}>
              {value}
            </Badge>
          );
        },
      },
      {
        field: 'isActive',
        headerName: 'Active',
        width: 100,
        sortable: true,
        filter: 'agTextColumnFilter',
        suppressFloatingFilterButton: true,
        suppressHeaderFilterButton: true,
        floatingFilterComponent: SelectFloatingFilter,
        floatingFilterComponentParams: {
          values: ['Yes', 'No'],
        },
        filterParams: {
          filterOptions: ['equals'],
          maxNumConditions: 1,
        },
        cellRenderer: (params: ICellRendererParams<EmploymentListDto>) => {
          return (
            <Badge colorPalette={params.value ? 'green' : 'red'}>
              {params.value ? 'Yes' : 'No'}
            </Badge>
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
            const response = await employmentsApi.apiV1EmploymentsGet(
              filters.employmentStatus,
              filters.isActive,
              filters.displayIdFilter,
              filters.employeeNameFilter,
              filters.depEdIdFilter,
              filters.positionFilter,
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
            console.error('Error fetching employments:', error);
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
        <Heading size="lg">Employments</Heading>
        <Button
          colorPalette="blue"
          onClick={() => navigate('/employments/new')}
        >
          Add Employment
        </Button>
      </Flex>

      <Flex gap={4} mb={4}>
        <Input
          placeholder="Search by name or DepEd ID..."
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
          <AgGridReact<EmploymentListDto>
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

export default EmploymentsPage;
