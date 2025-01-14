import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

export const CustomerRoute = ({ children, allowUnauthorized = false }) => {
    const { user, isEmployee } = useAuth();

    if (isEmployee) {
        return <Navigate to="/worker/rentals" replace />;
    }

    if (!user && !allowUnauthorized) {
        return <Navigate to="/login" replace />;
    }

    return children;
};