import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';
import { CarPriceDialog } from './CarPriceDialog';

export function CarDetails() {
  const { carId } = useParams();
  const { user } = useAuth();
  const [car, setCar] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [totalPrice, setTotalPrice] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    fetch('/api/cars', {
      headers: {
        'Authorization': `Bearer ${user.token}`
      }
    })
      .then(res => res.json())
      .then(data => {
        const foundCar = data.find(car => car.carId === parseInt(carId));
        setCar(foundCar);
        setLoading(false);
      })
      .catch(error => {
        console.error('Error fetching cars:', error);
        setLoading(false);
      });
  }, [carId, user.token]);

  const handleGetOffer = async (options) => {
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
    }
  };

  if (loading) return <div>Loading...</div>;
  if (!car) return <div>Car not found</div>;

  return (
    <div style={{ 
      maxWidth: '1200px', 
      margin: '0 auto', 
      padding: '20px'
    }}>
      <div style={{ 
        display: 'flex', 
        alignItems: 'center', 
        marginBottom: '20px',
        gap: '10px'
      }}>
        <button
          onClick={() => navigate('/')}
          style={{
            background: 'none',
            border: 'none',
            cursor: 'pointer',
            display: 'flex',
            alignItems: 'center',
            gap: '5px',
            fontSize: '16px'
          }}
        >
          ← Oferty
        </button>
      </div>

      <div style={{
        display: 'grid',
        gridTemplateColumns: '1fr 1fr',
        gap: '40px',
        alignItems: 'start'
      }}>
        <img 
          src="" // TO DO
          alt={`${car.brand} ${car.model}`}
          style={{
            width: '100%',
            borderRadius: '8px',
            boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)'
          }}
        />

        <div>
          <h1 style={{ 
            fontSize: '32px', 
            marginBottom: '24px'
          }}>
            {car.brand} {car.model}
          </h1>

          <h2 style={{ 
            fontSize: '20px', 
            color: '#666',
            marginBottom: '16px'
          }}>
            Specyfikacja samochodu
          </h2>

          <div style={{
            display: 'grid',
            gap: '12px',
            fontSize: '16px',
            marginBottom: '32px'
          }}>
            <DetailRow label="Pojemność" value={`${car.engineCapacity} l`} />
            <DetailRow label="Moc" value={`${car.power} KM`} />
            <DetailRow label="Paliwo" value={car.fuelType} />
            <DetailRow label="Rok produkcji" value={car.year} />
            <DetailRow label="Lokalizacja" value={car.location} />
            <DetailRow label="Numer rejestracyjny" value={car.licensePlate} />
            <DetailRow label="Status" value={car.status} />
            {car.description && (
              <DetailRow label="Opis" value={car.description} />
            )}
            {car.carProvider && (
              <DetailRow 
                label="Dostawca" 
                value={`${car.carProvider.name} (${car.carProvider.contactEmail})`} 
              />
            )}
          </div>

          <div style={{ 
            display: 'flex',
            alignItems: 'center',
            gap: '16px'
          }}>
            {totalPrice ? (
              <>
                <div style={{
                  fontSize: '24px',
                  fontWeight: 'bold',
                  color: '#8B4513'
                }}>
                  {totalPrice.toFixed(2)} PLN
                </div>
                <Button 
                  variant="primary"
                  onClick={() => {}}  // TO DO
                >
                  Rent Me
                </Button>
              </>
            ) : (
              <Button 
                variant="primary"
                onClick={() => setIsDialogOpen(true)}
              >
                Zapytaj o cenę
              </Button>
            )}
          </div>
        </div>
      </div>

      <CarPriceDialog
        isOpen={isDialogOpen}
        onClose={() => setIsDialogOpen(false)}
        onSubmit={handleGetOffer}
      />
    </div>
  );
}

function DetailRow({ label, value }) {
  return (
    <div style={{ 
      display: 'flex',
      borderBottom: '1px solid #eee',
      paddingBottom: '8px'
    }}>
      <span style={{ 
        minWidth: '150px', 
        fontWeight: 'bold'
      }}>
        {label}:
      </span>
      <span>{value}</span>
    </div>
  );
}