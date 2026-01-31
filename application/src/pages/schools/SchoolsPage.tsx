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
} from 'ag-grid-community';
import { useNavigate } from 'react-router-dom';
import { useSchoolsLazy } from '../../hooks/useSchools';
import type { SchoolListDto } from '../../graphql/generated/graphql';
import { useAgGridTheme } from '../../components/ui/use-ag-grid-theme';
import { EyeIcon, EditIcon } from '../../components/icons';

ModuleRegistry.registerModules([AllCommunityModule]);

const SchoolsPage = () => {
  const navigate = useNavigate();
  const gridRef = useRef<AgGridReact<SchoolListDto>>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const debouncedSearchTerm = useDebounce(searchTerm, 300);
  const theme = useAgGridTheme();
  const { fetchSchools } = useSchoolsLazy();

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
            const result = await fetchSchools({
              variables: {
                pageNumber,
                pageSize,
                searchTerm: debouncedSearchTerm || undefined,
                sortBy,
                sortDescending,
              },
            });

            const data = result.data?.schools;
            const rowsThisPage = data?.items || [];
            const lastRow = data?.totalCount ?? -1;

            rowParams.successCallback(rowsThisPage, lastRow);
          } catch (error) {
            console.error('Error fetching schools:', error);
            rowParams.failCallback();
          }
        },
      });
    }
  }, [debouncedSearchTerm, fetchSchools]);

  const columnDefs: ColDef<SchoolListDto>[] = useMemo(
    () => [
      {
        headerName: 'Actions',
        width: 100,
        sortable: false,
        filter: false,
        cellRenderer: (params: ICellRendererParams<SchoolListDto>) => {
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
                      navigate(`/schools/${params.data?.displayId}`)
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
                    aria-label="Edit school"
                    size="xs"
                    variant="ghost"
                    onClick={() =>
                      navigate(`/schools/${params.data?.displayId}/edit`)
                    }
                  >
                    <EditIcon />
                  </IconButton>
                </Tooltip.Trigger>
                <Portal>
                  <Tooltip.Positioner>
                    <Tooltip.Content px={1} py={1}>
                      Edit school
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
      },
      {
        field: 'schoolName',
        headerName: 'School Name',
        flex: 1,
        minWidth: 250,
        sortable: true,
      },
      {
        field: 'isActive',
        headerName: 'Status',
        width: 120,
        sortable: true,
        cellRenderer: (params: ICellRendererParams<SchoolListDto>) => {
          return (
            <Badge colorPalette={params.value ? 'green' : 'red'}>
              {params.value ? 'Active' : 'Inactive'}
            </Badge>
          );
        },
      },
      {
        field: 'createdOn',
        headerName: 'Created On',
        width: 180,
        sortable: true,
        valueFormatter: params => {
          if (!params.value) return '';
          return new Date(params.value).toLocaleDateString();
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
            const result = await fetchSchools({
              variables: {
                pageNumber,
                pageSize,
                searchTerm: debouncedSearchTerm || undefined,
                sortBy,
                sortDescending,
              },
            });

            const data = result.data?.schools;
            const rowsThisPage = data?.items || [];
            const lastRow = data?.totalCount ?? -1;

            rowParams.successCallback(rowsThisPage, lastRow);
            setIsLoading(false);
          } catch (error) {
            console.error('Error fetching schools:', error);
            rowParams.failCallback();
            setIsLoading(false);
          }
        },
      };

      params.api.setGridOption('datasource', dataSource);
    },
    [debouncedSearchTerm, fetchSchools]
  );

  return (
    <Box h="100%">
      <Flex justify="space-between" align="center" mb={4}>
        <Heading size="lg">Schools</Heading>
        <Button colorPalette="blue" onClick={() => navigate('/schools/new')}>
          Add School
        </Button>
      </Flex>

      <Flex gap={4} mb={4}>
        <Input
          placeholder="Search by school name..."
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
          <AgGridReact<SchoolListDto>
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

export default SchoolsPage;
