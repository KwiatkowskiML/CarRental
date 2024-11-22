import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from './AuthContext';
import { RegistrationForm } from './RegistrationForm';

export const LoginPage = () => {
  const { user, login } = useAuth();
  const navigate = useNavigate();
  const [googleData, setGoogleData] = useState(null);
  const [showRegistration, setShowRegistration] = useState(false);

  useEffect(() => {
    if (user) {
      navigate('/');
    }
  }, [user, navigate]);

  const handleGoogleSuccess = async (credentialResponse) => {
    try {
      const result = await login(credentialResponse.credential);
      if (result.needsRegistration) {
        setGoogleData({
          ...result.userData,
          token: credentialResponse.credential
        });
        setShowRegistration(true);
      } else if (result.token) {
        navigate('/');
      }
    } catch (error) {
      console.error('Login error:', error);
    }
  };

  const handleRegistrationComplete = async () => {
    const success = await login(googleData.token);
    if (success) {
      navigate('/');
    }
  };

  if (showRegistration) {
    return <RegistrationForm 
      googleData={googleData} 
      onRegistrationComplete={handleRegistrationComplete}
    />;
  }

  return (
    <div style={{ 
      display: 'flex', 
      justifyContent: 'center', 
      alignItems: 'center', 
      minHeight: '100vh',
      backgroundColor: '#f5f5f5'
    }}>
      <div style={{
        padding: '2rem',
        backgroundColor: 'white',
        borderRadius: '8px',
        boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)'
      }}>
        <h1 style={{ marginBottom: '1.5rem', textAlign: 'center' }}>
          Car Rental Login
        </h1>
        <GoogleLogin
          onSuccess={handleGoogleSuccess}
          onError={() => console.log('Login Failed')}
        />
      </div>
    </div>
  );
};