import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { AuthProvider, useAuth } from './auth/AuthContext';
import { AuthGuard } from './auth/AuthGuard';
import { LoginPage } from './auth/LoginPage';
import { CarList } from './components/car/CarList';
import { CarDetails } from './components/car/CarDetails';
import { RentalConfirmationPage } from './components/rental/RentalConfirmationPage';
import { WorkerRentalsView } from './components/worker/WorkerRentalsView';
import { RentalDetails } from './components/rental/RentalDetails';
import RentalHistory from './components/rental/RentalHistory';
import NavBar from './components/layout/NavBar';

// Move route guards outside the main App component
const CustomerRoute = ({ children }) => {
  const { isEmployee } = useAuth();
  console.log('CustomerRoute - isEmployee:', isEmployee);
  if (isEmployee) {
    console.log('Employee attempting to access customer route - redirecting to worker view');
    return <Navigate to="/worker/rentals" replace />;
  }
  return children;
};

const EmployeeRoute = ({ children }) => {
  const { isEmployee } = useAuth();
  console.log('EmployeeRoute - isEmployee:', isEmployee);
  if (!isEmployee) {
    console.log('Customer attempting to access employee route - redirecting to home');
    return <Navigate to="/" replace />;
  }
  return children;
};

function App() {
  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

  if (!clientId) {
    console.error('Google Client ID not found in environment variables');
    return <div>Error: Google Client ID not configured</div>;
  }

  console.log('App initialized with Google Client ID:', clientId ? 'Present' : 'Missing');

  return (
    <GoogleOAuthProvider clientId={clientId}>
      <BrowserRouter>
        <AuthProvider>
          <div>
            {console.log('Rendering App layout')}
            <NavBar />
            <Routes>
              <Route path="/login" element={<LoginPage />} />

              {/* Customer Routes */}
              <Route path="/" element={
                <AuthGuard>
                  <CustomerRoute>
                    {console.log('Rendering CarList route')}
                    <CarList />
                  </CustomerRoute>
                </AuthGuard>
              } />
              <Route path="/cars/:carId" element={
                <AuthGuard>
                  <CustomerRoute>
                    {console.log('Rendering CarDetails route')}
                    <CarDetails />
                  </CustomerRoute>
                </AuthGuard>
              } />
              <Route path="/rental-confirm" element={
                <AuthGuard>
                  <CustomerRoute>
                    {console.log('Rendering RentalConfirmation route')}
                    <RentalConfirmationPage />
                  </CustomerRoute>
                </AuthGuard>
              } />
              <Route path="/rental/history" element={
                <AuthGuard>
                  <CustomerRoute>
                    {console.log('Rendering RentalHistory route')}
                    <RentalHistory />
                  </CustomerRoute>
                </AuthGuard>
              } />

              {/* Employee Routes */}
              <Route path="/worker/rentals" element={
                <AuthGuard>
                  <EmployeeRoute>
                    {console.log('Rendering WorkerRentalsView route')}
                    <WorkerRentalsView />
                  </EmployeeRoute>
                </AuthGuard>
              } />
              <Route path="/worker/rentals/:rentalId" element={
                <AuthGuard>
                  <EmployeeRoute>
                    {console.log('Rendering RentalDetails (worker) route')}
                    <RentalDetails />
                  </EmployeeRoute>
                </AuthGuard>
              } />
            </Routes>
          </div>
        </AuthProvider>
      </BrowserRouter>
    </GoogleOAuthProvider>
  );
}

export default App;