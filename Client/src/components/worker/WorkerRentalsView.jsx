import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import { Button } from '../ui/Button';

const RentalListItem = ({ rental }) => {
    const navigate = useNavigate();

    const handleButtonClick = (rentalId) => {
        navigate(`/worker/rentals/${rentalId}`);
    };

    return (
        <div className="py-4 border-b border-gray-200 flex justify-between items-center">
            <div className="space-y-1">
                <h3 className="text-lg font-medium">
                    {rental.offer.car.brand} {rental.offer.car.model}
                </h3>
                <div className="space-y-0.5 text-sm text-gray-600">
                    <p>ID wynajmu: {rental.rentalId}</p>
                    <p>Status wypożyczenia: {rental.status}</p>
                </div>
            </div>

            <Button
                onClick={() => handleButtonClick(rental.rentalId)}
                variant={rental.status === 'ready_for_return' ? "primary" : "secondary"}
            >
                {rental.status === 'ready_for_return' ? 'Przyjmij zwrot' : 'Zobacz szczegóły'}
            </Button>
        </div>
    );
};

export function WorkerRentalsView() {
    const [rentals, setRentals] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [filters, setFilters] = useState({
        status: 'all',
        searchTerm: '',
    });
    const { user } = useAuth();

    useEffect(() => {
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

        fetchRentals();
    }, [user.token]);

    const filteredRentals = rentals.filter(rental => {
        const matchesStatus = filters.status === 'all' || rental.status === filters.status;
        const searchLower = filters.searchTerm.toLowerCase();
        const matchesSearch = !filters.searchTerm ||
            `${rental.offer.car.brand} ${rental.offer.car.model}`.toLowerCase().includes(searchLower) ||
            rental.rentalId.toString().includes(searchLower);

        return matchesStatus && matchesSearch;
    });

    if (loading) return <div className="text-center py-12">Loading...</div>;
    if (error) return <div className="text-center py-12 text-red-600">Error: {error}</div>;

    return (
        <Page>
            <div className="max-w-3xl mx-auto space-y-6">
                <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                    <h1 className="text-2xl font-bold text-gray-900">Lista Wynajmów</h1>
                    <div className="flex flex-col sm:flex-row gap-4 w-full sm:w-auto">
                        <input
                            type="text"
                            placeholder="Szukaj po modelu lub ID..."
                            value={filters.searchTerm}
                            onChange={(e) => setFilters(prev => ({ ...prev, searchTerm: e.target.value }))}
                            className="px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-brown-500 w-full sm:w-64"
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

                <div className="bg-white rounded-lg shadow">
                    {filteredRentals.length > 0 ? (
                        <div className="divide-y divide-gray-200">
                            {filteredRentals.map((rental) => (
                                <RentalListItem key={rental.rentalId} rental={rental} />
                            ))}
                        </div>
                    ) : (
                        <div className="text-center py-12">
                            <p className="text-gray-500">Brak wynajmów spełniających kryteria wyszukiwania.</p>
                        </div>
                    )}
                </div>
            </div>
        </Page>
    );
}

export default WorkerRentalsView;