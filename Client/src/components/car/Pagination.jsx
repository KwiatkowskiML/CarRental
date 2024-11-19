import React from 'react';
import { Button } from '../ui/Button';

export function Pagination({ currentPage, totalPages, onPageChange }) {
  return (
    <div style={{
      display: 'flex',
      justifyContent: 'center',
      gap: '10px',
      marginTop: '20px',
      padding: '20px'
    }}>
      <Button 
        variant="secondary"
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
      >
        Previous
      </Button>
      
      {[...Array(totalPages)].map((_, index) => (
        <Button
          key={index + 1}
          variant={currentPage === index + 1 ? "primary" : "secondary"}
          onClick={() => onPageChange(index + 1)}
        >
          {index + 1}
        </Button>
      ))}
      
      <Button
        variant="secondary"
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages}
      >
        Next
      </Button>
    </div>
  );
}