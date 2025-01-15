import { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';
import { CarPriceDialog } from './CarPriceDialog';

export function CarDetails() {
  const navigate = useNavigate();
  const location = useLocation();
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

  // Fetch current user data
  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const response = await fetch('/api/User/current', {
          method: 'GET',
          headers: {
            'Authorization': `Bearer ${user.token}`,
            'Cache-Control': 'no-cache, no-store, must-revalidate',
            'Pragma': 'no-cache'
          },
          cache: 'no-store'
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
    fetch('/api/Cars')
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
  }, [carId]);

  const handleCheckAvailability = () => {
    if (!user) {
      const redirectUrl = location.pathname;
      localStorage.setItem('redirectAfterLogin', redirectUrl);
      setTimeout(() => navigate('/login'), 0);
      return;
    }
    setIsDialogOpen(true);
  };

  const handleGetOffer = async (options) => {
    setError(null);
    try {
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
        const errorText = await response.text();
        if (errorText.includes('Car is not available for the selected dates')) {
          throw new Error('The car is not available for the selected dates. Please choose different dates.');
        }
        throw new Error(errorText || 'Failed to get offer');
      }

      const data = await response.json();
      setTotalPrice(data.totalPrice);
      setOfferDetails(data);
      setIsDialogOpen(false);
    } catch (error) {
      console.error('Error getting offer:', error);
      setError(error.message);
      return Promise.reject(error);
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
        throw new Error(errorText || 'Failed to initiate rental process');
      }

      alert('Please check your email to confirm the rental.');
      navigate('/');
    } catch (error) {
      console.error('Error initiating rental:', error);
      setError(error.message || 'Failed to initiate rental. Please try again.');
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
          ← Back to Offers
        </button>
      </div>

      <div style={{
        display: 'grid',
        gridTemplateColumns: '1fr 1fr',
        gap: '40px',
        alignItems: 'start'
      }}>
        <img 
          src={car.images?.[0] ? car.images[0] : ""}
          crossOrigin="anonymous"
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

          <div style={{
            display: 'grid',
            gap: '12px',
            fontSize: '16px',
            marginBottom: '32px'
          }}>
            <DetailRow label="Engine Capacity" value={`${car.engineCapacity} l`} />
            <DetailRow label="Power" value={`${car.power} HP`} />
            <DetailRow label="Fuel Type" value={car.fuelType} />
            <DetailRow label="Year" value={car.year} />
            <DetailRow label="Location" value={car.location} />
            <DetailRow label="License Plate" value={car.licensePlate} />
            <DetailRow label="Status" value={car.status} />
            {car.description && (
              <DetailRow label="Description" value={car.description} />
            )}
            {car.carProvider && (
              <DetailRow
                label="Provider"
                value={`${car.carProvider.name} (${car.carProvider.contactEmail})`}
              />
            )}
          </div>

          {error && (
            <div style={{
              padding: '12px',
              marginBottom: '20px',
              backgroundColor: '#fee2e2',
              color: '#dc2626',
              borderRadius: '4px',
              fontSize: '14px'
            }}>
              {error}
            </div>
          )}

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
                  ${totalPrice.toFixed(2)}
                </div>
                <Button
                  variant="primary"
                  onClick={handleRentMe}
                  disabled={isSubmitting}
                >
                  {isSubmitting ? 'Processing...' : 'Rent Now'}
                </Button>
              </>
            ) : (
              <Button
                variant="primary"
                onClick={handleCheckAvailability}
              >
                Check Availability & Price
              </Button>
            )}
          </div>
        </div>
      </div>

      <CarPriceDialog
        isOpen={isDialogOpen && user}
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