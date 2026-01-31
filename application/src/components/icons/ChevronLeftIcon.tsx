import React from 'react';
import type { IconProps } from './IconProps';

export const ChevronLeftIcon: React.FC<IconProps> = ({
  width = 16,
  height = 16,
  className,
}) => (
  <svg
    width={width}
    height={height}
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
    className={className}
  >
    <polyline points="15 18 9 12 15 6" />
  </svg>
);
