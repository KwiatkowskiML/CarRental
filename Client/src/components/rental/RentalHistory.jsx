import React, { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import RentalCard from './RentalCard';

function RentalHistory() {
    const { user } = useAuth();
    const [rentals, setRentals] = useState([]);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    const fetchRentals = async () => {
        try {
            const userResponse = await fetch('/api/User/current', {
                headers: {
                    'Authorization': `Bearer ${user.token}`
                }
            });

            if (!userResponse.ok) {
                throw new Error('Failed to get user information');
            }

            const userData = await userResponse.json();

            const rentalsResponse = await fetch(`/api/User/11/rentals`, {
                headers: {
                    'Authorization': `Bearer ${user.token}`
                }
            });

            if (!rentalsResponse.ok) {
                throw new Error('Failed to fetch rentals');
            }

            const rentalsData = await rentalsResponse.json();
            setRentals(rentalsData);
        } catch (err) {
            console.error('Error fetching rentals:', err);
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchRentals();
    }, [user.token]);

    const handleStatusUpdate = () => {
        fetchRentals(); // Refresh the rentals list after status update
    };

    if (isLoading) {
        return <Page><div>Loading...</div></Page>;
    }

    if (error) {
        return <Page><div>Error: {error}</div></Page>;
    }

    return (
        <Page>
            <h1 style={{ marginBottom: '24px' }}>Rental History</h1>
            {rentals.length === 0 ? (
                <p>No rentals found.</p>
            ) : (
                rentals.map((rental) => (
                    <RentalCard
                        key={rental.rentalId}
                        rental={rental}
                        onStatusUpdate={handleStatusUpdate}
                    />
                ))
            )}
        </Page>
    );
}

export default RentalHistory;