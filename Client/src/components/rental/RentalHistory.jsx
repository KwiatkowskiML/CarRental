import React, { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import RentalCard from './RentalCard';
import { Pagination } from '../car/Pagination';

function RentalHistory() {
    const { user } = useAuth();
    const [rentals, setRentals] = useState([]);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [customerId, setCustomerId] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const pageSize = 5;

    const fetchRentals = async (page) => {
        try {
            const userResponse = await fetch('/api/User/current', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${user.token}`,
                    'Cache-Control': 'no-cache, no-store, must-revalidate',
                    'Pragma': 'no-cache'
                },
                cache: 'no-store'
            });

            if (!userResponse.ok) {
                throw new Error('Failed to get user information');
            }

            const userData = await userResponse.json();
            const customerIdResponse = await fetch(`/api/Customer/id?userId=${userData.userId}`, {
                headers: {
                    'Authorization': `Bearer ${user.token}`
                }
            });
            const customerData = await customerIdResponse.json();
            setCustomerId(customerData.customerId);

            const rentalsResponse = await fetch(
                `/api/Customer/${customerData.customerId}/rentals?page=${page}&pageSize=${pageSize}`,
                {
                    headers: {
                        'Authorization': `Bearer ${user.token}`
                    }
                }
            );

            if (!rentalsResponse.ok) {
                throw new Error('Failed to fetch rentals');
            }

            const rentalsData = await rentalsResponse.json();
            setRentals(rentalsData.rentals);
            setTotalPages(rentalsData.totalPages);
        } catch (err) {
            console.error('Error fetching rentals:', err);
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchRentals(currentPage);
    }, [user.token, currentPage]);

    const handlePageChange = (page) => {
        setCurrentPage(page);
        window.scrollTo(0, 0);
    };

    const handleStatusUpdate = () => {
        fetchRentals(currentPage);
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
                <>
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '20px', marginBottom: '20px' }}>
                        {rentals.map((rental) => (
                            <RentalCard
                                key={rental.rentalId}
                                rental={rental}
                                customerId={customerId}
                                onStatusUpdate={handleStatusUpdate}
                            />
                        ))}
                    </div>

                    {totalPages > 1 && (
                        <Pagination
                            currentPage={currentPage}
                            totalPages={totalPages}
                            onPageChange={handlePageChange}
                        />
                    )}

                    {rentals.length > 0 && (
                        <div style={{
                            textAlign: 'center',
                            marginTop: '16px',
                            color: '#666',
                            fontSize: '14px'
                        }}>
                            Showing {Math.min(pageSize * (currentPage - 1) + 1, pageSize * totalPages)} to {Math.min(pageSize * currentPage, pageSize * totalPages - 1)} of {pageSize * totalPages - 1} rentals
                        </div>
                    )}
                </>
            )}
        </Page>
    );
}

export default RentalHistory;