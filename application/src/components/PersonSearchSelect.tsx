import { useState, useEffect, useRef } from 'react';
import { Box, Input, Spinner, Text, Flex } from '@chakra-ui/react';
import { personsApi, type PersonListDto } from '../api';
import { useDebounce } from '../hooks/useDebounce';

interface PersonSearchSelectProps {
  value: number | '';
  onChange: (displayId: number | '', person: PersonListDto | null) => void;
  placeholder?: string;
}

const PersonSearchSelect = ({
  value,
  onChange,
  placeholder = 'Search by name or ID...',
}: PersonSearchSelectProps) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [results, setResults] = useState<PersonListDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [selectedPerson, setSelectedPerson] = useState<PersonListDto | null>(
    null
  );
  const containerRef = useRef<HTMLDivElement>(null);

  const debouncedSearch = useDebounce(searchTerm, 300);

  // Load selected person details when value is set externally
  useEffect(() => {
    if (value && !selectedPerson) {
      loadPersonByDisplayId(value as number);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  const loadPersonByDisplayId = async (displayId: number) => {
    try {
      const response = await personsApi.apiV1PersonsGet(
        undefined,
        undefined,
        String(displayId),
        undefined,
        1,
        1
      );
      if (response.data.items && response.data.items.length > 0) {
        setSelectedPerson(response.data.items[0]);
      }
    } catch (err) {
      console.error('Error loading person:', err);
    }
  };

  // Search for persons when debounced search term changes
  useEffect(() => {
    if (debouncedSearch.length >= 2) {
      searchPersons(debouncedSearch);
    } else {
      setResults([]);
    }
  }, [debouncedSearch]);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const searchPersons = async (term: string) => {
    setLoading(true);
    try {
      // Check if the term looks like an ID (all digits)
      const isNumeric = /^\d+$/.test(term);

      const response = await personsApi.apiV1PersonsGet(
        undefined, // gender
        undefined, // civilStatus
        isNumeric ? term : undefined, // displayIdFilter - use if numeric
        undefined, // fullNameFilter
        1, // pageNumber
        20, // pageSize
        isNumeric ? undefined : term // searchTerm - use if not numeric (name search)
      );
      setResults(response.data.items || []);
    } catch (err) {
      console.error('Error searching persons:', err);
      setResults([]);
    } finally {
      setLoading(false);
    }
  };

  const handleSelect = (person: PersonListDto) => {
    setSelectedPerson(person);
    setSearchTerm('');
    setIsOpen(false);
    onChange(person.displayId!, person);
  };

  const handleClear = () => {
    setSelectedPerson(null);
    setSearchTerm('');
    setResults([]);
    onChange('', null);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
    setIsOpen(true);
    if (selectedPerson) {
      setSelectedPerson(null);
      onChange('', null);
    }
  };

  return (
    <Box position="relative" ref={containerRef}>
      {selectedPerson ? (
        <Flex
          align="center"
          justify="space-between"
          p={2}
          borderWidth="1px"
          borderRadius="md"
          borderColor="border.muted"
          bg="bg"
        >
          <Text>
            {selectedPerson.fullName} ({selectedPerson.displayId})
          </Text>
          <button
            type="button"
            onClick={handleClear}
            style={{
              padding: '0 8px',
              background: 'none',
              border: 'none',
              cursor: 'pointer',
              color: 'inherit',
              opacity: 0.6,
            }}
          >
            ✕
          </button>
        </Flex>
      ) : (
        <Input
          value={searchTerm}
          onChange={handleInputChange}
          onFocus={() => searchTerm.length >= 2 && setIsOpen(true)}
          placeholder={placeholder}
        />
      )}

      {isOpen && !selectedPerson && (
        <Box
          position="absolute"
          top="100%"
          left={0}
          right={0}
          mt={1}
          maxH="300px"
          overflowY="auto"
          bg="bg.panel"
          borderWidth="1px"
          borderColor="border.muted"
          borderRadius="md"
          zIndex={1000}
          boxShadow="lg"
        >
          {loading ? (
            <Flex justify="center" p={4}>
              <Spinner size="sm" />
            </Flex>
          ) : results.length > 0 ? (
            results.map(person => (
              <Box
                key={person.displayId}
                p={3}
                cursor="pointer"
                _hover={{ bg: 'bg.muted' }}
                onClick={() => handleSelect(person)}
                borderBottomWidth="1px"
                borderColor="border.muted"
              >
                <Text fontWeight="medium">{person.fullName}</Text>
                <Text fontSize="sm" color="fg.muted">
                  ID: {person.displayId}
                </Text>
              </Box>
            ))
          ) : searchTerm.length >= 2 ? (
            <Box p={4} textAlign="center" color="fg.muted">
              No persons found
            </Box>
          ) : (
            <Box p={4} textAlign="center" color="fg.muted">
              Type at least 2 characters to search
            </Box>
          )}
        </Box>
      )}
    </Box>
  );
};

export default PersonSearchSelect;
