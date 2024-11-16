import React from 'react';
import { Button } from '../ui/Button';

export function CarCard({ car }) {
  return (
    <div 
      style={{
        display: 'flex',
        backgroundColor: 'white',
        borderRadius: '8px',
        boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
        overflow: 'hidden',
        alignItems: 'center',
        padding: '16px',
        gap: '20px'
      }}
    >
      <img 
        src="/api/placeholder/300/200"
        alt={`${car.brand} ${car.model}`}
        style={{
          width: '300px',
          height: '200px',
          objectFit: 'cover',
          borderRadius: '4px'
        }}
      />
      <div style={{ flex: 1 }}>
        <h2 style={{ fontSize: '24px', marginBottom: '8px' }}>
          {car.brand} {car.model}
        </h2>
        <div style={{ fontSize: '14px', color: '#666', marginBottom: '4px' }}>
          {car.fuelType} • {car.year}
        </div>
        <p style={{ color: '#666', marginTop: '8px' }}>
          {car.description || '(opis)'}
        </p>
      </div>
      <Button variant="primary" onClick={() => {}}>
        Zapytaj o cenę
      </Button>
    </div>
  );
}