import React, { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import WorkerRentalCard from './WorkerRentalCard';
import { WorkerSearchFilters } from './WorkerSearchFilters';

export function WorkerRentalsView() {
    const [rentals, setRentals] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
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
                    const cars = await response.json();
                    setAvailableFilters({
                        brands: [...new Set(cars.map(car => car.brand))].sort(),
                        models: [...new Set(cars
                            .filter(car => !filters.brand || car.brand === filters.brand)
                            .map(car => car.model))
                        ].sort()
                    });
                }
            } catch (err) {
                console.error('Error fetching filter options:', err);
            }
        };

        fetchFilterOptions();
    }, [user.token, filters.brand]);

    const fetchRentals = async () => {
        try {
            setLoading(true);
            const queryParams = new URLSearchParams();

            if (filters.brand) {
                queryParams.append('brand', filters.brand);
            }
            if (filters.model) {
                queryParams.append('model', filters.model);
            }
            if (filters.status) {
                let statusId;
                switch (filters.status) {
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
            setRentals(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRentals();
    }, [filters]);

    const handleBrandChange = (brand) => {
        setFilters(prev => ({ ...prev, brand, model: '' }));
    };

    const handleModelChange = (model) => {
        setFilters(prev => ({ ...prev, model }));
    };

    const handleStatusChange = (e) => {
        setFilters(prev => ({ ...prev, status: e.target.value }));
    };

    return (
        <Page>
            <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '20px' }}>
                <div style={{ marginBottom: '24px' }}>
                    <h1 style={{ fontSize: '24px', fontWeight: 'bold', marginBottom: '16px' }}>
                        Lista Wynajmów
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
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                        {rentals.map((rental) => (
                            <WorkerRentalCard
                                key={rental.rentalId}
                                rental={rental}
                                onStatusUpdate={fetchRentals}
                            />
                        ))}
                    </div>
                ) : (
                    <div style={{ textAlign: 'center', padding: '48px 0' }}>
                        <p>No rentals matching your search criteria.</p>
                    </div>
                )}
            </div>
        </Page>
    );
}