import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';

function NavBar() {
  const { user, logout, isEmployee } = useAuth();

  useEffect(() => {
    console.log('NavBar: User role status:', { isEmployee, hasUser: !!user });
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
              {console.log('Rendering employee navigation')}
              <Link to="/worker/rentals" style={{ textDecoration: 'none', color: 'black' }}>Rentals</Link>
            </>
          ) : (
            <>
              {console.log('Rendering customer navigation')}
              <Link to="/" style={{ textDecoration: 'none', color: 'black' }}>Browser</Link>
              <Link to="/rental/history" style={{ textDecoration: 'none', color: 'black' }}>Rental History</Link>
            </>
          )}
          <span>Welcome!</span>
          <button
            onClick={() => {
              console.log('Logout button clicked');
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