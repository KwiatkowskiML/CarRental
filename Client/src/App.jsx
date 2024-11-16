import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { AuthProvider } from './auth/AuthContext';
import { AuthGuard } from './auth/AuthGuard';
import { LoginPage } from './auth/LoginPage';
import { CarList } from './components/CarList';

function App() {
  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
  
  if (!clientId) {
    console.error('Google Client ID not found in environment variables');
    return <div>Error: Google Client ID not configured</div>;
  }

  return (
    <GoogleOAuthProvider clientId={clientId}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={
              <AuthGuard>
                <CarList />
              </AuthGuard>
            } />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </GoogleOAuthProvider>
  );
}

export default App;