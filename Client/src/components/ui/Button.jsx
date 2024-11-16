import React from 'react';

const variants = {
  primary: {
    backgroundColor: '#8B4513',
    color: 'white',
    border: 'none',
  },
  secondary: {
    backgroundColor: 'white',
    color: 'black',
    border: '1px solid #ddd',
  }
};

export function Button({ children, variant = 'primary', disabled, onClick, ...props }) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      style={{
        padding: '12px 24px',
        borderRadius: '4px',
        cursor: disabled ? 'default' : 'pointer',
        opacity: disabled ? 0.5 : 1,
        ...variants[variant],
        ...props.style
      }}
      {...props}
    >
      {children}
    </button>
  );
}