import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Page } from '../layout/Page';
import { Button } from '../ui/Button';

const MOCK_RENTALS = [
    {
        rentalId: 123456,
        status: 'gotowy do zwrotu',
        offer: {
            car: {
                brand: 'Opel',
                model: 'Corsa E'
            }
        }
    },
    {
        rentalId: 234577,
        status: 'w trakcie',
        offer: {
            car: {
                brand: 'Polonez',
                model: 'Caro'
            }
        }
    },
    {
        rentalId: 345478,
        status: 'w trakcie',
        offer: {
            car: {
                brand: 'Skoda',
                model: 'Fabia'
            }
        }
    }
];

const RentalListItem = ({ rental }) => {
    const navigate = useNavigate();

    const handleButtonClick = (rentalId) => {
        // Zawsze przekierowujemy do szczegółów, niezależnie od statusu
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
                variant={rental.status === 'gotowy do zwrotu' ? "primary" : "secondary"}
                className={rental.status === 'gotowy do zwrotu' ?
                    "bg-brown-600 hover:bg-brown-700" :
                    "border border-brown-600 text-brown-600 hover:bg-brown-50"
                }
            >
                {rental.status === 'gotowy do zwrotu' ? 'Przyjmij zwrot' : 'Zobacz szczegóły'}
            </Button>
        </div>
    );
};

export function WorkerRentalsView() {
    const [filters, setFilters] = useState({
        status: 'all',
        searchTerm: '',
    });

    const filteredRentals = MOCK_RENTALS.filter(rental => {
        const matchesStatus = filters.status === 'all' || rental.status === filters.status;
        const searchLower = filters.searchTerm.toLowerCase();
        const matchesSearch = !filters.searchTerm ||
            `${rental.offer.car.brand} ${rental.offer.car.model}`.toLowerCase().includes(searchLower) ||
            rental.rentalId.toString().includes(searchLower);

        return matchesStatus && matchesSearch;
    });

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
                            <option value="w trakcie">W trakcie</option>
                            <option value="gotowy do zwrotu">Gotowe do zwrotu</option>
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