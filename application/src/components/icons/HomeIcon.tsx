import React from 'react';
import type { IconProps } from './IconProps';

export const HomeIcon: React.FC<IconProps> = ({
  width = 16,
  height = 16,
  className,
}) => (
  <svg
    width={width}
    height={height}
    viewBox="0 0 24 24"
    fill="currentColor"
    className={className}
  >
    <path d="M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z" />
  </svg>
);
