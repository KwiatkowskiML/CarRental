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
  if (isEmployee) {
    return <Navigate to="/worker/rentals" replace />;
  }
  return children;
};

const EmployeeRoute = ({ children }) => {
  const { isEmployee } = useAuth();
  if (!isEmployee) {
    return <Navigate to="/" replace />;
  }
  return children;
};

function App() {
  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

  return (
    <GoogleOAuthProvider clientId={clientId}>
      <BrowserRouter>
        <AuthProvider>
          <div>
            <NavBar />
            <Routes>
              <Route path="/login" element={<LoginPage />} />

              {/* Public route - accessible to unauthorized users and customers */}
              <Route path="/" element={
                <CustomerRoute allowUnauthorized>
                  <CarList />
                </CustomerRoute>
              } />
              <Route path="/cars/:carId" element={
                <CustomerRoute allowUnauthorized>
                  <CarDetails />
                </CustomerRoute>
              } />

              {/* Customer Routes - require auth */}
              <Route path="/rental-confirm" element={
                <AuthGuard>
                  <CustomerRoute>
                    <RentalConfirmationPage />
                  </CustomerRoute>
                </AuthGuard>
              } />
              <Route path="/rental/history" element={
                <AuthGuard>
                  <CustomerRoute>
                    <RentalHistory />
                  </CustomerRoute>
                </AuthGuard>
              } />

              {/* Employee Routes */}
              <Route path="/worker/rentals" element={
                <AuthGuard>
                  <EmployeeRoute>
                    <WorkerRentalsView />
                  </EmployeeRoute>
                </AuthGuard>
              } />
              <Route path="/worker/rentals/:rentalId" element={
                <AuthGuard>
                  <EmployeeRoute>
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