import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

export const EmployeeRoute = ({ children }) => {
    const { user, isEmployee } = useAuth();

    if (!user) {
        return <Navigate to="/login" replace />;
    }

    if (!isEmployee) {
        return <Navigate to="/" replace />;
    }

    return children;
};