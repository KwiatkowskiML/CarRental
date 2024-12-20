// WorkerRentalCard.jsx
import React, { useState } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';

function WorkerRentalCard({ rental, onStatusUpdate }) {
    const { user } = useAuth();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);

    const { offer } = rental;
    const { car, startDate, endDate, totalPrice, hasGps, hasChildSeat, insurance } = offer;
    const { brand, model, year, description, carProvider } = car;

    const handleAcceptReturn = async () => {
        setIsSubmitting(true);
        setError(null);

        try {
            const response = await fetch(`/api/Worker/rentals/${rental.rentalId}/accept-return`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${user.token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to accept return');
            }

            if (onStatusUpdate) {
                onStatusUpdate();
            }
        } catch (err) {
            console.error('Error accepting return:', err);
            setError('Failed to accept return. Please try again.');
        } finally {
            setIsSubmitting(false);
        }
    };

    const getStatusColor = (status) => {
        if (!status?.description) return '#6b7280'; // default gray for undefined/null status

        switch (status.description.toLowerCase()) {
            case 'confirmed':
                return '#22c55e'; // green
            case 'pending return':
                return '#eab308'; // yellow
            case 'completed':
                return '#6b7280'; // gray
            default:
                return '#6b7280';
        }
    };

    return (
        <div className="bg-white rounded-lg shadow p-6">
            <div className="flex justify-between items-start mb-4">
                <div>
                    <h3 className="text-xl font-semibold mb-2">
                        {brand} {model} ({year})
                    </h3>
                    <div
                        className="inline-block px-3 py-1 rounded-full text-sm"
                        style={{
                            backgroundColor: getStatusColor(rental.rentalStatus),
                            color: 'white'
                        }}
                    >
                        {rental.rentalStatus?.description || 'Unknown'}
                    </div>
                </div>
                {rental.status === 'ready_for_return' && (
                    <Button
                        variant="primary"
                        onClick={handleAcceptReturn}
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? 'Przetwarzanie...' : 'Przyjmij zwrot'}
                    </Button>
                )}
            </div>

            {description && (
                <p className="text-gray-600 mb-4">{description}</p>
            )}

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <DetailRow label="Data rozpoczęcia" value={new Date(startDate).toLocaleDateString()} />
                <DetailRow label="Data zakończenia" value={new Date(endDate).toLocaleDateString()} />
                <DetailRow label="Całkowita cena" value={`${totalPrice.toFixed(2)} PLN`} />
                <DetailRow label="GPS" value={hasGps ? 'Tak' : 'Nie'} />
                <DetailRow label="Fotelik" value={hasChildSeat ? 'Tak' : 'Nie'} />
                <DetailRow label="Ubezpieczenie" value={insurance?.name || 'Brak'} />
                <DetailRow label="Dostawca" value={carProvider.name} />
                <DetailRow label="Kontakt" value={carProvider.contactEmail} />
                <DetailRow label="ID wynajmu" value={rental.rentalId} />
            </div>

            {error && (
                <div className="mt-4 p-3 bg-red-50 text-red-700 rounded-md">
                    {error}
                </div>
            )}
        </div>
    );
}

const DetailRow = ({ label, value }) => (
    <div className="flex flex-col">
        <span className="text-sm text-gray-500">{label}</span>
        <span className="font-medium">{value}</span>
    </div>
);

export default WorkerRentalCard;