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
} from 'ag-grid-community';
import { useNavigate } from 'react-router-dom';
import { employmentsApi, type EmploymentListDto } from '../../api';
import { useAgGridTheme } from '../../components/ui/use-ag-grid-theme';

ModuleRegistry.registerModules([AllCommunityModule]);

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

          try {
            const response = await employmentsApi.apiV1EmploymentsGet(
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
        field: 'displayId',
        headerName: 'ID',
        width: 150,
        sortable: true,
      },
      {
        field: 'employeeFullName',
        headerName: 'Employee Name',
        flex: 1,
        minWidth: 200,
        sortable: true,
      },
      {
        field: 'depEdId',
        headerName: 'DepEd ID',
        width: 180,
        sortable: true,
      },
      {
        field: 'positionTitle',
        headerName: 'Position',
        flex: 1,
        minWidth: 150,
        sortable: true,
      },
      {
        field: 'employmentStatus',
        headerName: 'Status',
        width: 130,
        sortable: true,
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
        cellRenderer: (params: ICellRendererParams<EmploymentListDto>) => {
          return (
            <Badge colorPalette={params.value ? 'green' : 'red'}>
              {params.value ? 'Yes' : 'No'}
            </Badge>
          );
        },
      },
      {
        headerName: 'Actions',
        width: 160,
        sortable: false,
        filter: false,
        cellRenderer: (params: ICellRendererParams<EmploymentListDto>) => {
          return (
            <Flex gap={2} align="center" h="100%">
              <Button
                size="xs"
                variant="outline"
                onClick={() =>
                  navigate(`/employments/${params.data?.displayId}`)
                }
              >
                View
              </Button>
              <Button
                size="xs"
                variant="outline"
                colorPalette="blue"
                onClick={() =>
                  navigate(`/employments/${params.data?.displayId}/edit`)
                }
              >
                Edit
              </Button>
            </Flex>
          );
        },
      },
    ],
    [navigate]
  );

  const defaultColDef: ColDef = useMemo(
    () => ({
      filter: false,
      floatingFilter: false,
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

          try {
            const response = await employmentsApi.apiV1EmploymentsGet(
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
