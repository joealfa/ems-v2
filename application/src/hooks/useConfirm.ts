import { useDisclosure } from '@chakra-ui/react';
import { useState, useCallback } from 'react';

interface ConfirmOptions {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmColorScheme?: string;
}

interface UseConfirmReturn {
  confirmDialog: {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
    title: string;
    message: string;
    confirmText: string;
    cancelText: string;
    confirmColorScheme: string;
  };
  confirm: (options: ConfirmOptions) => Promise<boolean>;
}

export const useConfirm = (): UseConfirmReturn => {
  const { open: isOpen, onOpen, onClose } = useDisclosure();
  const [resolvePromise, setResolvePromise] = useState<
    ((value: boolean) => void) | null
  >(null);
  const [confirmOptions, setConfirmOptions] = useState<
    Required<ConfirmOptions>
  >({
    title: 'Confirm Action',
    message: '',
    confirmText: 'Confirm',
    cancelText: 'Cancel',
    confirmColorScheme: 'red',
  });

  const confirm = useCallback(
    (options: ConfirmOptions): Promise<boolean> => {
      setConfirmOptions({
        title: options.title || 'Confirm Action',
        message: options.message,
        confirmText: options.confirmText || 'Confirm',
        cancelText: options.cancelText || 'Cancel',
        confirmColorScheme: options.confirmColorScheme || 'red',
      });

      return new Promise<boolean>((resolve) => {
        setResolvePromise(() => resolve);
        onOpen();
      });
    },
    [onOpen]
  );

  const handleClose = useCallback(() => {
    if (resolvePromise) {
      resolvePromise(false);
      setResolvePromise(null);
    }
    onClose();
  }, [resolvePromise, onClose]);

  const handleConfirm = useCallback(() => {
    if (resolvePromise) {
      resolvePromise(true);
      setResolvePromise(null);
    }
  }, [resolvePromise]);

  return {
    confirmDialog: {
      isOpen,
      onClose: handleClose,
      onConfirm: handleConfirm,
      ...confirmOptions,
    },
    confirm,
  };
};
