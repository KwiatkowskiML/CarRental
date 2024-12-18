import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';
import { CarPriceDialog } from './CarPriceDialog';

export function CarDetails() {
  const { carId } = useParams();
  const { user } = useAuth();
  const [car, setCar] = useState(null);
  const [currentUser, setCurrentUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [totalPrice, setTotalPrice] = useState(null);
  const [offerDetails, setOfferDetails] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // Fetch current user data
  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const response = await fetch('/api/User/current', {
          headers: {
            'Authorization': `Bearer ${user.token}`
          }
        });
        if (response.ok) {
          const userData = await response.json();
          setCurrentUser(userData);
        }
      } catch (error) {
        console.error('Error fetching user data:', error);
      }
    };

    if (user?.token) {
      fetchUserData();
    }
  }, [user]);

  useEffect(() => {
    fetch('/api/Cars', {
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

      console.log('Options:', options);
      console.log('Car ID:', carId);

      const response = await fetch('/api/Offers/get-offer', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${user.token}`
        },
        body: JSON.stringify({
          carId: parseInt(carId),
          userId: options.userId,
          startDate: options.startDate,
          endDate: options.endDate,
          insuranceId: options.insuranceId,
          hasGps: options.hasGps,
          hasChildSeat: options.hasChildSeat
        })
      });
  
      if (!response.ok) {
        throw new Error('Failed to get offer');
      }
  
      const data = await response.json();
      console.log('Offer response data:', data); // Debug log
      setTotalPrice(data.totalPrice)
      setOfferDetails(data);
      setIsDialogOpen(false);
    } catch (error) {
      console.error('Error getting offer:', error);
      setError('Failed to get offer. Please try again.');
    }
  };

  const handleRentMe = async () => {
    if (!offerDetails || !currentUser) return;
  
    setIsSubmitting(true);
    setError(null);
  
    try {
      const requestData = {
        offerId: offerDetails.offerId,
        userId: currentUser.userId
      };
      
      console.log('Offer object:', offerDetails); // Debug log
      console.log('Current user object:', currentUser); // Debug log
      console.log('Sending rental confirmation request:', requestData);
  
      const response = await fetch('/api/Rentals/send-confirmation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${user.token}`
        },
        body: JSON.stringify(requestData)
      });
  
      if (!response.ok) {
        const errorText = await response.text();
        console.error('Error response from server:', errorText);
        throw new Error(`Failed to initiate rental process: ${errorText}`);
      }
  
      // Show success message
      alert('Please check your email to confirm the rental.');
      navigate('/');
    } catch (error) {
      console.error('Error initiating rental:', error);
      setError('Failed to initiate rental. Please try again.');
    } finally {
      setIsSubmitting(false);
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
                  onClick={handleRentMe}
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