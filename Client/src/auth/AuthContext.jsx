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
    const token = localStorage.getItem('token');
    const redirectUrl = localStorage.getItem('redirectAfterLogin');

    if (token) {
      setUser({ token });
      if (redirectUrl) {
        localStorage.removeItem('redirectAfterLogin');
        navigate(redirectUrl);
      } else {
        checkUserRole(token);
      }
    }
    setLoading(false);
  }, []);

  const checkUserRole = async (token) => {
    if (!token) return;

    try {
      const userResponse = await fetch('/api/User/current', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!userResponse.ok) {
        if (userResponse.status === 401) {
          localStorage.removeItem('token');
          setUser(null);
          setIsEmployee(false);
          return;
        }
        throw new Error('Failed to get user');
      }

      const userData = await userResponse.json();

      try {
        const customerResponse = await fetch(`/api/Customer/id?userId=${userData.userId}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        setIsEmployee(!customerResponse.ok);
        if (location.pathname === '/') {
          navigate(customerResponse.ok ? '/' : '/worker/rentals');
        }
      } catch (error) {
        setIsEmployee(true);
        if (location.pathname === '/') {
          navigate('/worker/rentals');
        }
      }
    } catch (error) {
      console.error('Error in checkUserRole:', error);
    }
  };

  const login = async (googleToken) => {
    try {
      const redirectUrl = localStorage.getItem('redirectAfterLogin');
      const response = await fetch('/api/Auth/google', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ token: googleToken })
      });

      const data = await response.json();

      if (response.status === 404 && data.needsRegistration) {
        return {
          needsRegistration: true,
          userData: data.userData,
        };
      }

      localStorage.setItem('token', data.token);
      setUser({ token: data.token });

      const userResponse = await fetch('/api/User/current', {
        headers: {
          'Authorization': `Bearer ${data.token}`
        }
      });
      const userData = await userResponse.json();

      try {
        const customerResponse = await fetch(`/api/Customer/id?userId=${userData.userId}`, {
          headers: {
            'Authorization': `Bearer ${data.token}`
          }
        });

        const isEmployee = !customerResponse.ok;
        setIsEmployee(isEmployee);

        if (isEmployee) {
          navigate('/worker/rentals');
        } else if (redirectUrl) {
          navigate(redirectUrl);
          localStorage.removeItem('redirectAfterLogin');
        } else {
          navigate('/');
        }

      } catch (error) {
        setIsEmployee(true);
        navigate('/worker/rentals');
      }

      return { token: data.token };
    } catch (error) {
      console.error('Login error:', error);
      return { error: true };
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
    setIsEmployee(false);
    navigate('/login');
  };

  if (loading) {
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