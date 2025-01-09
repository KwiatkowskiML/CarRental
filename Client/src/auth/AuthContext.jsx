import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isEmployee, setIsEmployee] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    console.log('AuthProvider useEffect - Checking stored token');
    const token = localStorage.getItem('token');
    if (token) {
      console.log('Found stored token, setting user and checking role');
      setUser({ token });
      checkUserRole(token);
    } else {
      console.log('No stored token found');
    }
    setLoading(false);
  }, []);

  const checkUserRole = async (token) => {
    console.log('Checking user role with token:', token);
    try {
      console.log('Fetching current user data...');
      const userResponse = await fetch('/api/User/current', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!userResponse.ok) {
        console.error('Failed to get user data:', userResponse.status);
        throw new Error('Failed to get user');
      }
      const userData = await userResponse.json();
      console.log('User data received:', userData);

      try {
        console.log('Checking if user is customer with userId:', userData.userId);
        const customerResponse = await fetch(`/api/Customer/id?userId=${userData.userId}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!customerResponse.ok) {
          console.log('User is an employee (no customer record found)');
          setIsEmployee(true);
          // Przekieruj tylko jeśli użytkownik jest na stronie głównej lub logowania
          if (location.pathname === '/' || location.pathname === '/login') {
            navigate('/worker/rentals');
          }
        } else {
          console.log('User is a customer');
          setIsEmployee(false);
          // Przekieruj tylko jeśli użytkownik jest na stronie głównej lub logowania
          if (location.pathname === '/' || location.pathname === '/login') {
            navigate('/');
          }
        }
      } catch (error) {
        console.log('Error checking customer status, assuming employee:', error);
        setIsEmployee(true);
        if (location.pathname === '/' || location.pathname === '/login') {
          navigate('/worker/rentals');
        }
      }
    } catch (error) {
      console.error('Error in checkUserRole:', error);
    }
  };

  const login = async (googleToken) => {
    console.log('Processing login with Google token');
    try {
      const response = await fetch('/api/Auth/google', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ token: googleToken })
      });

      const data = await response.json();
      console.log('Login response:', data);

      if (response.status === 404 && data.needsRegistration) {
        console.log('New user needs registration');
        return {
          needsRegistration: true,
          userData: data.userData
        };
      }

      if (!response.ok) {
        console.error('Login failed:', response.status);
        throw new Error('Login failed');
      }

      console.log('Login successful, storing token and checking role');
      localStorage.setItem('token', data.token);
      setUser({ token: data.token });
      await checkUserRole(data.token);
      return { token: data.token };
    } catch (error) {
      console.error('Login error:', error);
      return { error: true };
    }
  };

  const logout = () => {
    console.log('Logging out user');
    localStorage.removeItem('token');
    setUser(null);
    setIsEmployee(false);
    navigate('/login');
  };

  if (loading) {
    console.log('Auth provider still loading');
    return <div>Loading...</div>;
  }

  return (
    <AuthContext.Provider value={{ user, login, logout, isEmployee }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    console.error('useAuth must be used within an AuthProvider');
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};