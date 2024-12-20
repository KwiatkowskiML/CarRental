import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { AuthProvider } from './auth/AuthContext';
import { AuthGuard } from './auth/AuthGuard';
import { LoginPage } from './auth/LoginPage';
import { CarList } from './components/car/CarList';
import { CarDetails } from './components/car/CarDetails';
import { RentalConfirmationPage } from './components/rental/RentalConfirmationPage';
import { WorkerRentalsView } from './components/worker/WorkerRentalsView';
import { RentalDetails } from './components/rental/RentalDetails';
import RentalHistory from './components/rental/RentalHistory';
import NavBar from './components/layout/NavBar';

function App() {
  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

  if (!clientId) {
    console.error('Google Client ID not found in environment variables');
    return <div>Error: Google Client ID not configured</div>;
  }

  return (
    <GoogleOAuthProvider clientId={clientId}>
      <BrowserRouter>
        <AuthProvider>
          <NavBar />
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={
              <AuthGuard>
                <CarList />
              </AuthGuard>
            } />
            <Route path="/cars/:carId" element={
              <AuthGuard>
                <CarDetails />
              </AuthGuard>
            } />
            <Route path="/rental-confirm" element={
              <AuthGuard>
                <RentalConfirmationPage />
              </AuthGuard>
            } />
            <Route path="/rental/history" element={
              <AuthGuard>
                <RentalHistory />
              </AuthGuard>
            } />
            <Route path="/worker/rentals" element={
              <AuthGuard>
                <WorkerRentalsView />
              </AuthGuard>
            } />
            <Route path="/worker/rentals/:rentalId" element={
              <AuthGuard>
                <RentalDetails />
              </AuthGuard>
            } />
          </Routes>
        </AuthProvider>
      </BrowserRouter>
    </GoogleOAuthProvider>
  );
}

export default App;