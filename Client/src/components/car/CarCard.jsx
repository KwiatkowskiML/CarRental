import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../ui/Button';
import { CarPriceDialog } from './CarPriceDialog';
import { useAuth } from '../../auth/AuthContext';

export function CarCard({ car }) {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [totalPrice, setTotalPrice] = useState(null);
  const [loading, setLoading] = useState(false);
  const { user } = useAuth();
  const navigate = useNavigate();

  const handleGetOffer = async (options) => {
    setLoading(true);
    try {
      const response = await fetch('/api/cars/get-offer', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${user.token}`
        },
        body: JSON.stringify({
          carId: car.carId,
          userId: 1, // TO DO
          startDate: options.startDate,
          endDate: options.endDate,
          insuranceId: options.insuranceId,
          hasGps: options.hasGps,
          hasChildSeat: options.hasChildSeat
        })
      });

      const data = await response.json();
      setTotalPrice(data.totalPrice);
      setIsDialogOpen(false);
    } catch (error) {
      console.error('Error getting offer:', error);
    } finally {
      setLoading(false);
    }
  };

  const handlePriceClick = () => {
    if (totalPrice) {
      navigate(`/cars/${car.carId}`, { 
        state: { totalPrice } 
      });
    }
  };

  return (
    <>
      <div style={{
        display: 'flex',
        backgroundColor: 'white',
        borderRadius: '8px',
        boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
        overflow: 'hidden',
        alignItems: 'center',
        padding: '16px',
        gap: '20px'
      }}>
        <img 
          src="" // TO DO
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
            {car.year} • {car.fuelType} • {car.power} HP • {car.location}
          </div>
          <p style={{ color: '#666', marginTop: '8px' }}>
            {car.description || '(opis)'}
          </p>
        </div>
        {totalPrice !== null ? (
          <div
            onClick={handlePriceClick}
            style={{
              padding: '12px 24px',
              borderRadius: '4px',
              border: '1px solid #8B4513',
              color: '#8B4513',
              fontSize: '18px',
              fontWeight: 'bold',
              cursor: 'pointer',
              transition: 'background-color 0.2s',
              ':hover': {
                backgroundColor: '#f8f4f1'
              }
            }}
          >
            {totalPrice.toFixed(2)} PLN
          </div>
        ) : (
          <Button 
            variant="primary"
            onClick={() => setIsDialogOpen(true)}
            disabled={loading}
          >
            {loading ? 'Wyliczanie...' : 'Zapytaj o cenę'}
          </Button>
        )}
      </div>

      <CarPriceDialog
        isOpen={isDialogOpen}
        onClose={() => setIsDialogOpen(false)}
        onSubmit={handleGetOffer}
      />
    </>
  );
}