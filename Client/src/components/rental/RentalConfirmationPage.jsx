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

    validateToken(token);
    confirmRental()
  }, [searchParams, user]);

  const validateToken = async (token) => {
    try {
      console.log('Starting token validation');
      console.log('Raw token from URL:', token);
      console.log('Encoded token:', encodeURIComponent(token));
  
      // Try to decode the token first to see what we're working with
      const decodedToken = decodeURIComponent(token);
      console.log('Decoded token:', decodedToken);
  
      // Split and examine token parts
      const parts = decodedToken.split('_');
      console.log('Token parts:', parts);
      console.log('Number of parts:', parts.length);
  
      const response = await fetch(`/api/RentalConfirmation/validate?token=${encodeURIComponent(token)}`, {
        headers: {
          'Authorization': `Bearer ${user.token}`
        }
      });
  
      console.log('Response status:', response.status);
      const responseText = await response.text();
      console.log('Response text:', responseText);
  
      if (!response.ok) {
        throw new Error(responseText);
      }
  
      const data = JSON.parse(responseText);
      console.log('Parsed response data:', data);
  
      setRentalDetails(data);
      setStatus('success');
    } catch (error) {
      console.error('Validation error details:', {
        message: error.message,
        stack: error.stack,
        name: error.name
      });
      setError(error.message);
      setStatus('error');
    }
  };

  const confirmRental = async () => {
    try {
      setStatus('loading');
      const token = searchParams.get('token');
      
      const response = await fetch(`/api/RentalConfirmation/confirm?token=${encodeURIComponent(token)}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${user.token}`
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText);
      }

      const data = await response.json();
      setRentalDetails(data);
      setStatus('success');
    } catch (error) {
      console.error('Confirmation error:', error);
      setError(error.message);
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
        Verifying your rental confirmation...
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
      <Button variant="primary" onClick={() => {}}>   {/*TODO*/}
        View My Rentals
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

function DetailRow({ label, value }) {
  return (
    <div style={{ 
      display: 'flex',
      padding: '12px 16px',
      backgroundColor: '#f8f9fa',
      borderRadius: '4px',
      alignItems: 'center'
    }}>
      <span style={{ 
        fontWeight: 'bold',
        minWidth: '120px',
        color: '#4b5563'
      }}>
        {label}:
      </span>
      <span style={{ color: '#1f2937' }}>{value}</span>
    </div>
  );
}

function formatDate(dateString) {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}