import React, { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import WorkerRentalCard from './WorkerRentalCard';
import { WorkerSearchFilters } from './WorkerSearchFilters';
import { Pagination } from '../car/Pagination';

export function WorkerRentalsView() {
    const [rentals, setRentals] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const pageSize = 5;

    const [filters, setFilters] = useState({
        brand: '',
        model: '',
        status: ''
    });

    const [availableFilters, setAvailableFilters] = useState({
        brands: [],
        models: []
    });

    const { user } = useAuth();

    useEffect(() => {
        const fetchFilterOptions = async () => {
            try {
                const response = await fetch('/api/Cars', {
                    headers: {
                        'Authorization': `Bearer ${user.token}`
                    }
                });

                if (response.ok) {
                    const data = await response.json();
                    const cars = data.cars;

                    const allBrands = [...new Set(cars.map(car => car.brand))].sort();
                    const allModels = cars
                        .filter(car => !filters.brand || car.brand === filters.brand)
                        .map(car => car.model)
                        .sort();

                    setAvailableFilters({
                        brands: allBrands,
                        models: [...new Set(allModels)]
                    });
                }
            } catch (err) {
                console.error('Error fetching filter options:', err);
            }
        };

        fetchFilterOptions();
    }, [user.token, filters.brand]);

    const fetchRentals = async (page = currentPage) => {
        try {
            setLoading(true);
            const queryParams = new URLSearchParams({
                page: page.toString(),
                pageSize: pageSize.toString()
            });

            if (filters.brand) {
                queryParams.append('brand', filters.brand);
            }
            if (filters.model) {
                queryParams.append('model', filters.model);
            }
            if (filters.status) {
                let statusId;
                switch (filters.status.toLowerCase()) {
                    case 'confirmed': statusId = 1; break;
                    case 'pending': statusId = 2; break;
                    case 'completed': statusId = 3; break;
                }
                if (statusId) {
                    queryParams.append('rentalStatus', statusId.toString());
                }
            }

            const response = await fetch(`/api/Worker/rentals?${queryParams.toString()}`, {
                headers: {
                    'Authorization': `Bearer ${user.token}`
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch rentals');
            }

            const data = await response.json();
            setRentals(data.rentals);
            setTotalCount(data.totalCount);
            setTotalPages(data.totalPages);
            setCurrentPage(data.currentPage);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRentals(1);
    }, [filters]);

    const handleBrandChange = (brand) => {
        setFilters(prev => ({ ...prev, brand, model: '' }));
        setCurrentPage(1);
    };

    const handleModelChange = (model) => {
        setFilters(prev => ({ ...prev, model }));
        setCurrentPage(1);
    };

    const handleStatusChange = (e) => {
        setFilters(prev => ({ ...prev, status: e.target.value }));
        setCurrentPage(1);
    };

    const handlePageChange = (page) => {
        setCurrentPage(page);
        fetchRentals(page);
        window.scrollTo(0, 0);
    };

    return (
        <Page>
            <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '20px' }}>
                <div style={{ marginBottom: '24px' }}>
                    <h1 style={{ fontSize: '24px', fontWeight: 'bold', marginBottom: '16px' }}>
                        Rental Management
                    </h1>

                    <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                        <WorkerSearchFilters
                            filters={filters}
                            onBrandChange={handleBrandChange}
                            onModelChange={handleModelChange}
                            availableBrands={availableFilters.brands}
                            availableModels={availableFilters.models}
                        />

                        <select
                            value={filters.status}
                            onChange={handleStatusChange}
                            style={{
                                padding: '8px 12px',
                                borderRadius: '4px',
                                border: '1px solid #ddd',
                                minWidth: '150px'
                            }}
                        >
                            <option value="">All statuses</option>
                            <option value="confirmed">Confirmed</option>
                            <option value="pending">Pending Return</option>
                            <option value="completed">Completed</option>
                        </select>
                    </div>
                </div>

                {loading ? (
                    <div style={{ textAlign: 'center', padding: '48px 0' }}>Loading...</div>
                ) : rentals.length > 0 ? (
                    <>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                            {rentals.map((rental) => (
                                <WorkerRentalCard
                                    key={rental.rentalId}
                                    rental={rental}
                                    onStatusUpdate={() => fetchRentals(currentPage)}
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

                        <div style={{
                            textAlign: 'center',
                            marginTop: '16px',
                            color: '#666',
                            fontSize: '14px'
                        }}>
                            Showing {Math.min(pageSize * (currentPage - 1) + 1, totalCount)} to {Math.min(pageSize * currentPage, totalCount)} of {totalCount} rentals
                        </div>
                    </>
                ) : (
                    <div style={{ textAlign: 'center', padding: '48px 0', color: '#666' }}>
                        No rentals matching your search criteria.
                    </div>
                )}

                {error && (
                    <div style={{
                        marginTop: '20px',
                        padding: '12px',
                        backgroundColor: '#fee2e2',
                        color: '#dc2626',
                        borderRadius: '4px',
                        textAlign: 'center'
                    }}>
                        {error}
                    </div>
                )}
            </div>
        </Page>
    );
}