import React, { useState } from 'react';
import { ReturnConfirmationDialog } from './ReturnConfirmationDialog';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';

function WorkerRentalCard({ rental, onStatusUpdate }) {
    const { user } = useAuth();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);
    const [isDialogOpen, setIsDialogOpen] = useState(false);

    const { offer } = rental;
    const { car, startDate, endDate, totalPrice, hasGps, hasChildSeat, insurance } = offer;
    const { brand, model, year, description, carProvider } = car;

    const handleAcceptReturn = async (returnData) => {
        setIsSubmitting(true);
        setError(null);

        try {
            const response = await fetch('/api/Worker/accept-return', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${user.token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    ...returnData,
                    rentalId: rental.rentalId
                })
            });

            if (!response.ok) {
                throw new Error('Failed to accept return');
            }

            setIsDialogOpen(false);
            onStatusUpdate();
        } catch (err) {
            setError(err.message);
        } finally {
            setIsSubmitting(false);
        }
    };

    const getStatusColor = (status) => {
        if (!status?.description) return '#6b7280';

        switch (status.description.toLowerCase()) {
            case 'confirmed':
                return '#22c55e';
            case 'pending return':
                return '#eab308';
            case 'completed':
                return '#6b7280';
            default:
                return '#6b7280';
        }
    };

    return (
        <div style={{
            border: '1px solid #ddd',
            borderRadius: '8px',
            padding: '16px',
            backgroundColor: 'white',
            boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                <div>
                    <h2 style={{ fontSize: '20px', marginBottom: '8px' }}>
                        {brand} {model} ({year})
                    </h2>
                    <div style={{
                        display: 'inline-block',
                        padding: '4px 8px',
                        borderRadius: '4px',
                        backgroundColor: getStatusColor(rental.rentalStatus),
                        color: 'white',
                        fontSize: '14px',
                        marginBottom: '12px'
                    }}>
                        {rental.rentalStatus?.description || 'Unknown'}
                    </div>
                </div>

                {rental.rentalStatus?.description === 'Pending return' && (
                    <Button
                        onClick={() => setIsDialogOpen(true)}
                        disabled={isSubmitting}
                    >
                        Accept Return
                    </Button>
                )}
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', marginTop: '16px' }}>
                <div>
                    <DetailRow label="Start Date" value={new Date(startDate).toLocaleDateString()} />
                    <DetailRow label="End Date" value={new Date(endDate).toLocaleDateString()} />
                    <DetailRow label="Total Price" value={`${totalPrice.toFixed(2)} PLN`} />
                </div>
                <div>
                    <DetailRow label="GPS" value={hasGps ? 'Yes' : 'No'} />
                    <DetailRow label="Child Seat" value={hasChildSeat ? 'Yes' : 'No'} />
                    <DetailRow label="Insurance" value={insurance ? insurance.name : 'None'} />
                </div>
                <div>
                    <DetailRow label="Provider" value={carProvider.name} />
                    <DetailRow label="Contact" value={carProvider.contactEmail} />
                    <DetailRow label="Rental ID" value={rental.rentalId} />
                </div>
            </div>

            <ReturnConfirmationDialog
                isOpen={isDialogOpen}
                onClose={() => setIsDialogOpen(false)}
                onConfirm={handleAcceptReturn}
                isSubmitting={isSubmitting}
            />

            {error && (
                <div style={{
                    marginTop: '12px',
                    padding: '8px',
                    backgroundColor: '#fee2e2',
                    color: '#dc2626',
                    borderRadius: '4px'
                }}>
                    {error}
                </div>
            )}
        </div>
    );
}

function DetailRow({ label, value }) {
    return (
        <div style={{ marginBottom: '8px' }}>
            <span style={{ fontWeight: 'bold', marginRight: '8px', color: '#666' }}>
                {label}:
            </span>
            <span>{value}</span>
        </div>
    );
}

export default WorkerRentalCard;