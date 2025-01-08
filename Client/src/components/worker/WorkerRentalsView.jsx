import React, { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import WorkerRentalCard from './WorkerRentalCard';
import { SearchForm } from './SearchForm';

export function WorkerRentalsView() {
    const [rentals, setRentals] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [filters, setFilters] = useState({
        status: '',
        brand: '',
        model: ''
    });
    const { user } = useAuth();

    const fetchRentals = async (currentFilters) => {
        console.log('fetchRentals called with filters:', currentFilters);
        try {
            setLoading(true);
            const queryParams = new URLSearchParams();

            if (currentFilters.status) {
                let statusId;
                switch (currentFilters.status) {
                    case 'confirmed':
                        statusId = 1;
                        break;
                    case 'pending':
                        statusId = 2;
                        break;
                    case 'completed':
                        statusId = 3;
                        break;
                }
                if (statusId) {
                    queryParams.append('RentalStatus', statusId.toString());
                }
            }

            if (currentFilters.brand) {
                queryParams.append('Brand', currentFilters.brand);
            }

            if (currentFilters.model) {
                queryParams.append('Model', currentFilters.model);
            }

            console.log('API Request URL:', `/api/Worker/rentals?${queryParams.toString()}`);

            const response = await fetch(`/api/Worker/rentals?${queryParams.toString()}`, {
                headers: {
                    'Authorization': `Bearer ${user.token}`
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch rentals');
            }

            const data = await response.json();
            console.log('API Response data:', data);
            setRentals(data);
        } catch (err) {
            console.error('Error in fetchRentals:', err);
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        console.log('useEffect triggered with filters:', filters);
        fetchRentals(filters);
    }, [filters.status]);

    const handleSearch = (searchValues) => {
        console.log('handleSearch called with:', searchValues);
        const newFilters = {
            ...filters,
            brand: searchValues.brand,
            model: searchValues.model
        };
        console.log('Setting new filters:', newFilters);
        setFilters(newFilters);
        fetchRentals(newFilters);
    };

    const handleStatusChange = (e) => {
        console.log('Status changed to:', e.target.value);
        setFilters(prev => ({ ...prev, status: e.target.value }));
    };

    return (
        <Page>
            <div className="max-w-7xl mx-auto px-4">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900 mb-6">Lista Wynajmów</h1>

                    <div className="space-y-4">
                        <SearchForm onSearch={handleSearch} />

                        <select
                            value={filters.status}
                            onChange={handleStatusChange}
                            className="w-full sm:w-auto px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-brown-500"
                        >
                            <option value="">Wszystkie statusy</option>
                            <option value="confirmed">Confirmed</option>
                            <option value="pending">Pending Return</option>
                            <option value="completed">Completed</option>
                        </select>
                    </div>
                </div>

                {loading ? (
                    <div className="text-center py-12">
                        <div className="text-xl">Loading...</div>
                    </div>
                ) : rentals.length > 0 ? (
                    <div className="space-y-6">
                        {rentals.map((rental) => (
                            <WorkerRentalCard
                                key={rental.rentalId}
                                rental={rental}
                                onStatusUpdate={() => fetchRentals(filters)}
                            />
                        ))}
                    </div>
                ) : (
                    <div className="text-center py-12 bg-white rounded-lg shadow">
                        <p className="text-gray-500">Brak wynajmów spełniających kryteria wyszukiwania.</p>
                    </div>
                )}
            </div>
        </Page>
    );
}