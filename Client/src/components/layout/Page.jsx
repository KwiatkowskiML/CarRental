import React from 'react';

export function Page({ children }) {
  return (
    <div>
      <main style={{
        padding: '20px',
        maxWidth: '1200px',
        margin: '0 auto'
      }}>
        {children}
      </main>
    </div>
  );
}