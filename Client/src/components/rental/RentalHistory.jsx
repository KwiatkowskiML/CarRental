import React, { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import RentalCard from './RentalCard';

function RentalHistory() {
    const { user } = useAuth();
    const [rentals, setRentals] = useState([]);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true); // Changed from setLoading to setIsLoading

    useEffect(() => {
        const fetchRentals = async () => {
            try {
                // Get user ID from backend using the token
                const userResponse = await fetch('/api/User/current', {
                    headers: {
                        'Authorization': `Bearer ${user.token}`
                    }
                });

                if (!userResponse.ok) {
                    throw new Error('Failed to get user information');
                }

                const userData = await userResponse.json();

                // Note the proper route with case sensitivity
                const rentalsResponse = await fetch(`/api/User/11/rentals`, {
                    headers: {
                        'Authorization': `Bearer ${user.token}`
                    }
                });

                if (!rentalsResponse.ok) {
                    const errorText = await rentalsResponse.text();
                    console.error('Error fetching rentals:', errorText);
                    throw new Error('Failed to fetch rentals');
                }

                try {
                    const rentalsData = await rentalsResponse.json();
                    setRentals(rentalsData);
                    setIsLoading(false);
                } catch (error) {
                    console.error('Error parsing JSON:', error);
                    throw new Error('Failed to parse rentals response as JSON');
                }
            } catch (err) {
                console.error('Error fetching rentals:', err);
                setError(err.message);
                setIsLoading(false);
            }
        };

        fetchRentals();
    }, [user.token]);

    if (isLoading) {
        return <Page><div>Loading...</div></Page>;
    }

    if (error) {
        return <Page><div>Error: {error}</div></Page>;
    }

    return (
        <Page>
            <h1>Rental History</h1>
            {rentals.length === 0 ? (
                <p>No rentals found.</p>
            ) : (
                rentals.map((rental) => (
                    <RentalCard key={rental.rentalId} rental={rental} />
                ))
            )}
        </Page>
    );
}

export default RentalHistory;