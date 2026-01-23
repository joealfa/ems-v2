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
import { personsApi, type PersonListDto, API_BASE_URL } from '../../api';
import { useAgGridTheme } from '../../components/ui/use-ag-grid-theme';

ModuleRegistry.registerModules([AllCommunityModule]);

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

          try {
            const response = await personsApi.apiV1PersonsGet(
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
        field: 'displayId',
        headerName: 'ID',
        width: 150,
        sortable: true,
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
                  onError={(e) => {
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
      },
      {
        field: 'dateOfBirth',
        headerName: 'Date of Birth',
        width: 150,
        sortable: true,
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
      },
      {
        field: 'civilStatus',
        headerName: 'Civil Status',
        width: 140,
        sortable: true,
      },
      {
        headerName: 'Actions',
        width: 160,
        sortable: false,
        filter: false,
        cellRenderer: (params: ICellRendererParams<PersonListDto>) => {
          return (
            <Flex gap={2} align="center" h="100%">
              <Button
                size="xs"
                variant="outline"
                onClick={() => navigate(`/persons/${params.data?.displayId}`)}
              >
                View
              </Button>
              <Button
                size="xs"
                variant="outline"
                colorPalette="blue"
                onClick={() =>
                  navigate(`/persons/${params.data?.displayId}/edit`)
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
            const response = await personsApi.apiV1PersonsGet(
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
