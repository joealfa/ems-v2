import {
  Portal,
  Toast,
  Toaster as ChakraToaster,
  Stack,
} from '@chakra-ui/react';
import { toaster } from '../../hooks/useToast';

/**
 * Global Toaster component that manages toast notifications.
 * Must be placed at the root of your app.
 * Automatically follows the current theme (light/dark mode).
 *
 * Color mapping:
 * - success: green
 * - error: red
 * - warning: orange
 * - info: blue
 * - loading: gray
 */
export const Toaster = () => {
  return (
    <Portal>
      <ChakraToaster toaster={toaster}>
        {(toast) => (
          <Toast.Root
            minWidth="sm"
            maxWidth="lg"
            shadow="lg"
            borderRadius="md"
            padding="4"
          >
            <Toast.Indicator />
            <Stack gap="1">
              {toast.title && (
                <Toast.Title fontWeight="semibold">{toast.title}</Toast.Title>
              )}
              {toast.description && (
                <Toast.Description fontSize="sm">
                  {toast.description}
                </Toast.Description>
              )}
            </Stack>
            <Toast.CloseTrigger top="2" insetEnd="2" />
          </Toast.Root>
        )}
      </ChakraToaster>
    </Portal>
  );
};
