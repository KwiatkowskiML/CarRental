// WorkerRentalsView.jsx
import React, { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import WorkerRentalCard from './WorkerRentalCard';

export function WorkerRentalsView() {
    const [rentals, setRentals] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [filters, setFilters] = useState({
        status: 'all',
        searchTerm: '',
    });
    const { user } = useAuth();

    const fetchRentals = async () => {
        try {
            const response = await fetch('/api/Worker/rentals', {
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
    }, [user.token]);

    const handleStatusUpdate = () => {
        fetchRentals();
    };

    const filteredRentals = rentals.filter(rental => {
        const matchesStatus = filters.status === 'all' ||
            rental.rentalStatus?.description?.toLowerCase() === filters.status.toLowerCase();
        const searchLower = filters.searchTerm.toLowerCase();
        const matchesSearch = !filters.searchTerm ||
            `${rental.offer.car.brand} ${rental.offer.car.model}`.toLowerCase().includes(searchLower) ||
            rental.rentalId.toString().includes(searchLower);

        return matchesStatus && matchesSearch;
    });

    if (loading) return (
        <div className="flex justify-center items-center min-h-screen">
            <div className="text-xl">Loading...</div>
        </div>
    );

    if (error) return (
        <div className="flex justify-center items-center min-h-screen">
            <div className="text-xl text-red-600">Error: {error}</div>
        </div>
    );

    return (
        <Page>
            <div className="max-w-7xl mx-auto px-4">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900 mb-6">Lista Wynajmów</h1>

                    <div className="flex flex-col sm:flex-row gap-4">
                        <input
                            type="text"
                            placeholder="Szukaj po modelu lub ID..."
                            value={filters.searchTerm}
                            onChange={(e) => setFilters(prev => ({ ...prev, searchTerm: e.target.value }))}
                            className="flex-1 px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-brown-500"
                        />
                        <select
                            value={filters.status}
                            onChange={(e) => setFilters(prev => ({ ...prev, status: e.target.value }))}
                            className="px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-brown-500"
                        >
                            <option value="all">Wszystkie statusy</option>
                            <option value="active">W trakcie</option>
                            <option value="ready_for_return">Gotowe do zwrotu</option>
                            <option value="completed">Zakończone</option>
                        </select>
                    </div>
                </div>

                {filteredRentals.length > 0 ? (
                    <div className="space-y-6">
                        {filteredRentals.map((rental) => (
                            <WorkerRentalCard
                                key={rental.rentalId}
                                rental={rental}
                                onStatusUpdate={handleStatusUpdate}
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

export default WorkerRentalsView;