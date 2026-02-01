import { useState, useEffect, useRef, useCallback } from 'react';
import { Box, Input, Spinner, Text, Flex } from '@chakra-ui/react';
import { useSchoolsLazy } from '../hooks/useSchools';
import type { SchoolListFieldsFragment } from '../graphql/generated/graphql';
import { useDebounce } from '../hooks/useDebounce';

interface SchoolSearchSelectProps {
  value: number | '';
  onChange: (
    displayId: number | '',
    school: SchoolListFieldsFragment | null
  ) => void;
  placeholder?: string;
  /** Pre-loaded school data (used when editing to show the current school) */
  initialSchool?: { displayId: number; schoolName: string | null } | null;
}

const SchoolSearchSelect = ({
  value,
  onChange,
  placeholder = 'Search schools by name...',
  initialSchool,
}: SchoolSearchSelectProps) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [results, setResults] = useState<SchoolListFieldsFragment[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [dropdownPosition, setDropdownPosition] = useState<'bottom' | 'top'>(
    'bottom'
  );
  const [selectedSchool, setSelectedSchool] = useState<{
    displayId: number;
    schoolName: string | null;
  } | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const { fetchSchools, loading } = useSchoolsLazy();

  const debouncedSearch = useDebounce(searchTerm, 300);

  // Calculate dropdown position based on available space
  const calculateDropdownPosition = useCallback(() => {
    if (!containerRef.current) return;

    const rect = containerRef.current.getBoundingClientRect();
    const spaceBelow = window.innerHeight - rect.bottom;
    const spaceAbove = rect.top;
    const dropdownHeight = 300; // maxH of dropdown

    if (spaceBelow < dropdownHeight && spaceAbove > spaceBelow) {
      setDropdownPosition('top');
    } else {
      setDropdownPosition('bottom');
    }
  }, []);

  // Set initial school when provided (for edit mode)
  useEffect(() => {
    if (initialSchool && value && !selectedSchool) {
      setSelectedSchool(initialSchool);
    }
  }, [initialSchool, value, selectedSchool]);

  // Load selected school details when value is set externally without initialSchool
  useEffect(() => {
    if (value && !selectedSchool && !initialSchool) {
      loadSchoolByDisplayId(value as number);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  const loadSchoolByDisplayId = async (displayId: number) => {
    try {
      const result = await fetchSchools({
        variables: {
          searchTerm: String(displayId),
          pageNumber: 1,
          pageSize: 20,
        },
      });
      const schools = result.data?.schools?.items ?? [];
      const foundSchool = schools.find(
        (school) => school && school.displayId === displayId
      );
      if (foundSchool) {
        setSelectedSchool({
          displayId: foundSchool.displayId as number,
          schoolName: foundSchool.schoolName ?? null,
        });
      }
    } catch (err) {
      console.error('Error loading school:', err);
    }
  };

  // Search for schools when debounced search term changes
  useEffect(() => {
    if (debouncedSearch.length >= 2) {
      searchSchools(debouncedSearch);
    } else {
      setResults([]);
      setIsOpen(false);
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

  const searchSchools = async (term: string) => {
    try {
      calculateDropdownPosition();
      const result = await fetchSchools({
        variables: {
          searchTerm: term,
          pageNumber: 1,
          pageSize: 20,
        },
      });
      const schools =
        result.data?.schools?.items?.filter(
          (school): school is NonNullable<typeof school> => school !== null
        ) ?? [];
      setResults(schools);
      // Only open dropdown after we have results or finished searching
      setIsOpen(true);
    } catch (err) {
      console.error('Error searching schools:', err);
      setResults([]);
    }
  };

  const handleSelect = (school: SchoolListFieldsFragment) => {
    setSelectedSchool({
      displayId: school.displayId as number,
      schoolName: school.schoolName ?? null,
    });
    setSearchTerm('');
    setIsOpen(false);
    onChange(school.displayId as number, school);
  };

  const handleClear = () => {
    setSelectedSchool(null);
    setSearchTerm('');
    setResults([]);
    onChange('', null);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setSearchTerm(newValue);
    // Don't open dropdown immediately - wait for search results
    // This prevents flickering
    if (newValue.length < 2) {
      setIsOpen(false);
    }
    if (selectedSchool) {
      setSelectedSchool(null);
      onChange('', null);
    }
  };

  const handleFocus = () => {
    if (searchTerm.length >= 2 && results.length > 0) {
      calculateDropdownPosition();
      setIsOpen(true);
    }
  };

  return (
    <Box position="relative" ref={containerRef} width="100%">
      {selectedSchool ? (
        <Flex
          align="center"
          justify="space-between"
          p={2}
          borderWidth="1px"
          borderRadius="md"
          borderColor="border.muted"
          bg="bg"
          minH="40px"
          width="100%"
        >
          <Text>{selectedSchool.schoolName}</Text>
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
          onFocus={handleFocus}
          placeholder={placeholder}
          width="100%"
        />
      )}

      {isOpen && !selectedSchool && (
        <Box
          position="absolute"
          {...(dropdownPosition === 'top'
            ? { bottom: '100%', mb: 1 }
            : { top: '100%', mt: 1 })}
          left={0}
          right={0}
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
            results.map((school) => (
              <Box
                key={school.displayId as React.Key}
                p={3}
                cursor="pointer"
                _hover={{ bg: 'bg.muted' }}
                onClick={() => handleSelect(school)}
                borderBottomWidth="1px"
                borderColor="border.muted"
              >
                <Text fontWeight="medium">{school.schoolName}</Text>
              </Box>
            ))
          ) : searchTerm.length >= 2 ? (
            <Box p={4} textAlign="center" color="fg.muted">
              No schools found
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

export default SchoolSearchSelect;
