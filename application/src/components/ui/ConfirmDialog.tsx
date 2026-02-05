import { Button, Dialog } from '@chakra-ui/react';
import { useRef } from 'react';

interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmColorScheme?: string;
  isLoading?: boolean;
}

export const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  isOpen,
  onClose,
  onConfirm,
  title = 'Confirm Action',
  message,
  confirmText = 'Confirm',
  cancelText = 'Cancel',
  confirmColorScheme = 'red',
  isLoading = false,
}) => {
  const cancelRef = useRef<HTMLButtonElement>(null);

  const handleConfirm = () => {
    onConfirm();
    onClose();
  };

  return (
    <Dialog.Root
      open={isOpen}
      onOpenChange={(e) => !e.open && onClose()}
      initialFocusEl={() => cancelRef.current}
      placement="center"
      motionPreset="slide-in-bottom"
    >
      <Dialog.Backdrop bg="blackAlpha.300" backdropFilter="blur(10px)" />
      <Dialog.Positioner>
        <Dialog.Content>
          <Dialog.Header fontSize="lg" fontWeight="bold">
            {title}
          </Dialog.Header>

          <Dialog.Body>{message}</Dialog.Body>

          <Dialog.Footer>
            <Button ref={cancelRef} onClick={onClose} disabled={isLoading}>
              {cancelText}
            </Button>
            <Button
              colorPalette={confirmColorScheme}
              onClick={handleConfirm}
              ml={3}
              loading={isLoading}
            >
              {confirmText}
            </Button>
          </Dialog.Footer>

          <Dialog.CloseTrigger />
        </Dialog.Content>
      </Dialog.Positioner>
    </Dialog.Root>
  );
};
