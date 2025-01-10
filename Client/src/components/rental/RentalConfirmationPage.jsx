import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import { Button } from '../ui/Button';

export function RentalConfirmationPage() {
  const [searchParams] = useSearchParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  
  const [status, setStatus] = useState('loading');
  const [error, setError] = useState(null);
  const [rentalDetails, setRentalDetails] = useState(null);

  useEffect(() => {
    const token = searchParams.get('token');
    if (!token || !user) {
      navigate('/login');
      return;
    }

    // Add a cleanup function
    let isSubscribed = true;

    const validateAndConfirm = async (token) => {
      try {
        // First just validate the token
        const validateResponse = await fetch(`/api/Rentals/validate-token?token=${encodeURIComponent(token)}`, {
          headers: {
            'Authorization': `Bearer ${user.token}`
          }
        });

        if (!validateResponse.ok) {
          const errorText = await validateResponse.text();
          throw new Error(errorText);
        }

        const validationData = await validateResponse.json();

        // Check if component is still mounted before proceeding
        if (!isSubscribed) return;

        // If validation successful and rental doesn't exist yet, try to confirm
        try {
          const confirmResponse = await fetch(`/api/Rentals/confirm?token=${encodeURIComponent(token)}`, {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${user.token}`
            }
          });

          // If we get a 409, it means the rental was already confirmed
          if (confirmResponse.status === 409) {
            setRentalDetails(validationData);
            setStatus('success');
            return;
          }

          if (!confirmResponse.ok) {
            const errorText = await confirmResponse.text();
            throw new Error(errorText);
          }

          // Check if component is still mounted before updating state
          if (!isSubscribed) return;

          const data = await confirmResponse.json();
          setRentalDetails(data);
          setStatus('success');
        } catch (confirmError) {
          if (!isSubscribed) return;
          // If we get here with a 409, it's not really an error
          if (confirmError.message?.includes('already confirmed')) {
            setRentalDetails(validationData);
            setStatus('success');
            return;
          }
          throw confirmError;
        }
      } catch (error) {
        if (!isSubscribed) return;
        console.error('Error processing rental:', error);
        setError(error.message || 'An unexpected error occurred');
        setStatus('error');
      }
    };

    validateAndConfirm(token);

    // Cleanup function to handle unmounting
    return () => {
      isSubscribed = false;
    };
  }, [searchParams, user]);

  const validateAndConfirm = async (token) => {
    try {
      // First just validate the token
      const validateResponse = await fetch(`/api/Rentals/validate-token?token=${encodeURIComponent(token)}`, {
        headers: {
          'Authorization': `Bearer ${user.token}`
        }
      });

      if (!validateResponse.ok) {
        const errorText = await validateResponse.text();
        throw new Error(errorText);
      }

      const validationData = await validateResponse.json();

      // If validation successful and rental doesn't exist yet, try to confirm
      try {
        const confirmResponse = await fetch(`/api/Rentals/confirm?token=${encodeURIComponent(token)}`, {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${user.token}`
          }
        });

        // If we get a 409, it means the rental was already confirmed
        if (confirmResponse.status === 409) {
          setRentalDetails(validationData);
          setStatus('success');
          return;
        }

        if (!confirmResponse.ok) {
          const errorText = await confirmResponse.text();
          throw new Error(errorText);
        }

        const data = await confirmResponse.json();
        setRentalDetails(data);
        setStatus('success');
      } catch (confirmError) {
        // If we get here with a 409, it's not really an error
        if (confirmError.message?.includes('already confirmed')) {
          setRentalDetails(validationData);
          setStatus('success');
          return;
        }
        throw confirmError;
      }
    } catch (error) {
      console.error('Error processing rental:', error);
      setError(error.message || 'An unexpected error occurred');
      setStatus('error');
    }
  };

  const renderLoadingState = () => (
    <div style={{ 
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '60vh',
      gap: '20px'
    }}>
      <h2 style={{ fontSize: '24px', fontWeight: 'bold' }}>
        Processing your rental confirmation...
      </h2>
      <div style={{
        width: '48px',
        height: '48px',
        border: '4px solid #f3f3f3',
        borderTop: '4px solid #8B4513',
        borderRadius: '50%',
        animation: 'spin 1s linear infinite',
      }} />
      <style>
        {`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}
      </style>
    </div>
  );

  const renderSuccessState = () => (
    <div style={{ 
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '60vh',
      gap: '20px',
      textAlign: 'center'
    }}>
      <div style={{
        width: '64px',
        height: '64px',
        borderRadius: '50%',
        backgroundColor: '#22c55e',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '20px'
      }}>
        <svg 
          width="32" 
          height="32" 
          viewBox="0 0 24 24" 
          fill="none" 
          stroke="white" 
          strokeWidth="3"
          strokeLinecap="round" 
          strokeLinejoin="round"
        >
          <polyline points="20 6 9 17 4 12"></polyline>
        </svg>
      </div>
      <h2 style={{ 
        fontSize: '28px',
        fontWeight: 'bold',
        color: '#22c55e',
        marginBottom: '16px'
      }}>
        Rental Confirmed!
      </h2>
      <p style={{ 
        fontSize: '16px',
        color: '#666',
        marginBottom: '24px',
        maxWidth: '400px'
      }}>
        Your rental has been successfully confirmed. You can view all the details in your rentals section.
      </p>
      <Button variant="primary" onClick={() => navigate('/')}>
        Return to Home
      </Button>
    </div>
  );

  const renderErrorState = () => (
    <div style={{ 
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '60vh',
      gap: '20px'
    }}>
      <h2 style={{ 
        fontSize: '24px', 
        fontWeight: 'bold',
        color: '#dc2626' 
      }}>
        Confirmation Error
      </h2>
      <p style={{ marginBottom: '20px', color: '#666' }}>
        {error}
      </p>
      <Button variant="primary" onClick={() => navigate('/')}>
        Return to Home
      </Button>
    </div>
  );

  return (
    <Page>
      {status === 'loading' && renderLoadingState()}
      {status === 'error' && renderErrorState()}
      {status === 'success' && renderSuccessState()}
    </Page>
  );
}