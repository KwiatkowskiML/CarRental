import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';

function NavBar() {
  const { user, logout, isEmployee } = useAuth();

  useEffect(() => {
  }, [user, isEmployee]);

  return (
    <nav style={{
      padding: '1rem',
      backgroundColor: '#f8f9fa',
      borderBottom: '1px solid #dee2e6',
      marginBottom: '2rem',
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center'
    }}>
      <div style={{ fontWeight: 'bold' }}>Car Rental</div>
      {user && (
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          {isEmployee ? (
            <>
              <Link to="/worker/rentals" style={{ textDecoration: 'none', color: 'black' }}>Rentals</Link>
            </>
          ) : (
            <>
              <Link to="/" style={{ textDecoration: 'none', color: 'black' }}>Browser</Link>
              <Link to="/rental/history" style={{ textDecoration: 'none', color: 'black' }}>Rental History</Link>
            </>
          )}
          <span>Welcome!</span>
          <button
            onClick={() => {
              logout();
            }}
            style={{
              padding: '0.5rem 1rem',
              backgroundColor: '#dc3545',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            Logout
          </button>
        </div>
      )}
    </nav>
  );
}

export default NavBar;