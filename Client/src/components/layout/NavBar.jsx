import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';

function NavBar() {
  const { user, logout, isEmployee } = useAuth();

  return (
    <nav style={{
      padding: '1rem',
      backgroundColor: '#f8f9fa',
      borderBottom: '1px solid #dee2e6',
      marginBottom: '2rem'
    }}>
      <div style={{
        maxWidth: '1200px',
        margin: '0 auto',
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center'
      }}>
        <Link
          to="/"
          style={{
            fontWeight: 'bold',
            color: '#8B4513',
            fontSize: '1.5rem',
            textDecoration: 'none'
          }}
        >
          Car Rental
        </Link>

        <div style={{
          display: 'flex',
          alignItems: 'center',
          gap: '1rem'
        }}>
          {user ? (
            <>
              {isEmployee ? (
                <Link
                  to="/worker/rentals"
                  style={{
                    textDecoration: 'none',
                    color: '#333',
                    padding: '0.5rem',
                    borderRadius: '0.25rem',
                    ':hover': { backgroundColor: '#f0f0f0' }
                  }}
                >
                  Rentals
                </Link>
              ) : (
                <>
                  <Link
                    to="/"
                    style={{
                      textDecoration: 'none',
                      color: '#333',
                      padding: '0.5rem',
                      borderRadius: '0.25rem'
                    }}
                  >
                    Browse
                  </Link>
                  <Link
                    to="/rental/history"
                    style={{
                      textDecoration: 'none',
                      color: '#333',
                      padding: '0.5rem',
                      borderRadius: '0.25rem'
                    }}
                  >
                    Rental History
                  </Link>
                </>
              )}
              <span style={{ color: '#666' }}>Welcome!</span>
              <Button
                onClick={logout}
                style={{
                  backgroundColor: '#dc3545',
                  color: 'white',
                  border: 'none',
                  padding: '0.5rem 1rem',
                  borderRadius: '0.25rem',
                  cursor: 'pointer',
                  ':hover': { backgroundColor: '#c82333' }
                }}
              >
                Logout
              </Button>
            </>
          ) : (
            <Link to="/login">
              <Button
                style={{
                  backgroundColor: '#8B4513',
                  color: 'white',
                  border: 'none',
                  padding: '0.5rem 1rem',
                  borderRadius: '0.25rem',
                  cursor: 'pointer',
                  ':hover': { backgroundColor: '#704012' }
                }}
              >
                Login
              </Button>
            </Link>
          )}
        </div>
      </div>
    </nav>
  );
}

export default NavBar;