import { useNavigate } from 'react-router-dom';
import { Button } from '../ui/Button';

export function CarCard({ car }) {
  const navigate = useNavigate();

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
        src={car.images?.[0] ? car.images[0] : "/api/placeholder/400/320"}
        alt={`${car.brand} ${car.model}`}
        crossOrigin="anonymous"
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
          {car.year} • {car.fuelType} • {car.power} HP • {car.location}
        </div>
        <p style={{ color: '#666', marginTop: '8px' }}>
          {car.description || '(opis)'}
        </p>
      </div>
      <Button
        variant="primary"
        onClick={() => navigate(`/cars/${car.carId}`)}
      >
        View Details
      </Button>
    </div>
  );
}