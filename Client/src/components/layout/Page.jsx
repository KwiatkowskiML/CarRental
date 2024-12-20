import React from 'react';
import { Navbar } from './Navbar';

export function Page({ children }) {
  return (
    <div>
      <Navbar />
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