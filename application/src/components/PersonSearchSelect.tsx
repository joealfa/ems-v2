import { useState, useEffect, useRef } from 'react';
import { Box, Input, Spinner, Text, Flex } from '@chakra-ui/react';
import { usePersonsLazy } from '../hooks/usePersons';
import type { PersonListFieldsFragment } from '../graphql/generated/graphql';
import { useDebounce } from '../hooks/useDebounce';

interface PersonSearchSelectProps {
  value: number | '';
  onChange: (
    displayId: number | '',
    person: PersonListFieldsFragment | null
  ) => void;
  placeholder?: string;
}

const PersonSearchSelect = ({
  value,
  onChange,
  placeholder = 'Search by name or ID...',
}: PersonSearchSelectProps) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [results, setResults] = useState<PersonListFieldsFragment[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [selectedPerson, setSelectedPerson] =
    useState<PersonListFieldsFragment | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const { fetchPersons, loading } = usePersonsLazy();

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
      const result = await fetchPersons({
        variables: {
          displayIdFilter: String(displayId),
          pageNumber: 1,
          pageSize: 1,
        },
      });
      if (result.data?.persons?.items && result.data.persons.items.length > 0) {
        setSelectedPerson(result.data.persons.items[0]);
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
    // eslint-disable-next-line react-hooks/exhaustive-deps
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
    try {
      // Check if the term looks like an ID (all digits)
      const isNumeric = /^\d+$/.test(term);

      const result = await fetchPersons({
        variables: {
          displayIdFilter: isNumeric ? term : undefined,
          searchTerm: isNumeric ? undefined : term,
          pageNumber: 1,
          pageSize: 20,
        },
      });
      setResults(
        result.data?.persons?.items?.filter(
          (item): item is NonNullable<typeof item> => item !== null
        ) || []
      );
    } catch (err) {
      console.error('Error searching persons:', err);
      setResults([]);
    }
  };

  const handleSelect = (person: PersonListFieldsFragment) => {
    setSelectedPerson(person);
    setSearchTerm('');
    setIsOpen(false);
    onChange(person.displayId as number, person);
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
            {selectedPerson.fullName} ({selectedPerson.displayId as number})
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
            results.map((person) => (
              <Box
                key={person.displayId as React.Key}
                p={3}
                cursor="pointer"
                _hover={{ bg: 'bg.muted' }}
                onClick={() => handleSelect(person)}
                borderBottomWidth="1px"
                borderColor="border.muted"
              >
                <Text fontWeight="medium">{person.fullName}</Text>
                <Text fontSize="sm" color="fg.muted">
                  ID: {person.displayId as number}
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
