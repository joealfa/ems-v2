import { useState, useEffect, useRef, useCallback } from 'react';
import { Box, Input, Spinner, Text, Flex } from '@chakra-ui/react';
import { useItemsLazy } from '../hooks/useItems';
import type { ItemFieldsFragment } from '../graphql/generated/graphql';
import { useDebounce } from '../hooks/useDebounce';

interface ItemSearchSelectProps {
  value: number | '';
  onChange: (displayId: number | '', item: ItemFieldsFragment | null) => void;
  placeholder?: string;
  /** Pre-loaded item data (used when editing to show the current item) */
  initialItem?: { displayId: number; itemName: string | null } | null;
}

const ItemSearchSelect = ({
  value,
  onChange,
  placeholder = 'Search items by name...',
  initialItem,
}: ItemSearchSelectProps) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [results, setResults] = useState<ItemFieldsFragment[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [dropdownPosition, setDropdownPosition] = useState<'bottom' | 'top'>(
    'bottom'
  );
  const [selectedItem, setSelectedItem] = useState<{
    displayId: number;
    itemName: string | null;
  } | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const { fetchItems, loading } = useItemsLazy();

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

  // Set initial item when provided (for edit mode)
  useEffect(() => {
    if (initialItem && value && !selectedItem) {
      setSelectedItem(initialItem);
    }
  }, [initialItem, value, selectedItem]);

  // Load selected item details when value is set externally without initialItem
  useEffect(() => {
    if (value && !selectedItem && !initialItem) {
      loadItemByDisplayId(value as number);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  const loadItemByDisplayId = async (displayId: number) => {
    try {
      // Search for the specific item by its name pattern (displayId isn't directly searchable)
      const result = await fetchItems({
        variables: {
          searchTerm: String(displayId),
          pageNumber: 1,
          pageSize: 20,
        },
      });
      const items = result.data?.items?.items ?? [];
      const foundItem = items.find(
        (item) => item && item.displayId === displayId
      );
      if (foundItem) {
        setSelectedItem({
          displayId: foundItem.displayId as number,
          itemName: foundItem.itemName ?? null,
        });
      }
    } catch (err) {
      console.error('Error loading item:', err);
    }
  };

  // Search for items when debounced search term changes
  useEffect(() => {
    if (debouncedSearch.length >= 2) {
      searchItems(debouncedSearch);
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

  const searchItems = async (term: string) => {
    try {
      calculateDropdownPosition();
      const result = await fetchItems({
        variables: {
          searchTerm: term,
          pageNumber: 1,
          pageSize: 20,
        },
      });
      const items =
        result.data?.items?.items?.filter(
          (item): item is NonNullable<typeof item> => item !== null
        ) ?? [];
      setResults(items);
      // Only open dropdown after we have results or finished searching
      setIsOpen(true);
    } catch (err) {
      console.error('Error searching items:', err);
      setResults([]);
    }
  };

  const handleSelect = (item: ItemFieldsFragment) => {
    setSelectedItem({
      displayId: item.displayId as number,
      itemName: item.itemName ?? null,
    });
    setSearchTerm('');
    setIsOpen(false);
    onChange(item.displayId as number, item);
  };

  const handleClear = () => {
    setSelectedItem(null);
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
    if (selectedItem) {
      setSelectedItem(null);
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
      {selectedItem ? (
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
          <Text>{selectedItem.itemName}</Text>
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
          ref={inputRef}
          value={searchTerm}
          onChange={handleInputChange}
          onFocus={handleFocus}
          placeholder={placeholder}
          width="100%"
        />
      )}

      {isOpen && !selectedItem && (
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
            results.map((item) => (
              <Box
                key={item.displayId as React.Key}
                p={3}
                cursor="pointer"
                _hover={{ bg: 'bg.muted' }}
                onClick={() => handleSelect(item)}
                borderBottomWidth="1px"
                borderColor="border.muted"
              >
                <Text fontWeight="medium">{item.itemName}</Text>
                {item.description && (
                  <Text fontSize="sm" color="fg.muted">
                    {item.description}
                  </Text>
                )}
              </Box>
            ))
          ) : searchTerm.length >= 2 ? (
            <Box p={4} textAlign="center" color="fg.muted">
              No items found
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

export default ItemSearchSelect;
